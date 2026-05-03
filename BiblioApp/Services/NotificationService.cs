using System.Text;
using System.Text.Json;

namespace BiblioApp.Services
{
    // ──────────────────────────────────────────────────────────────────────────
    // Contract
    // ──────────────────────────────────────────────────────────────────────────

    public interface INotificationService
    {
        /// <summary>Send a raw email via the n8n SMTP node.</summary>
        Task SendEmailAsync(string toEmail, string subject, string message);

        /// <summary>Send a raw SMS via the n8n SMS node.</summary>
        Task SendSmsAsync(string phoneNumber, string message);

        /// <summary>Notify the borrower when they borrow a book.</summary>
        Task SendBorrowNotificationAsync(string userEmail, string bookTitle, DateTime dueDate);

        /// <summary>Notify the borrower when their book is overdue.</summary>
        Task SendOverdueNotificationAsync(string userEmail, string bookTitle, int daysOverdue);

    /// <summary>Remind the borrower to return a book before the due date.</summary>
    Task SendReturnReminderAsync(string userEmail, string bookTitle, DateTime dueDate);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Options  (bound from appsettings.json → "Notifications" section)
    // ──────────────────────────────────────────────────────────────────────────

    public sealed class NotificationOptions
    {
        public const string SectionName = "Notifications";

        /// <summary>Base URL of the n8n instance, e.g. http://n8n:5678</summary>
        public string N8nBaseUrl { get; set; } = "http://n8n:5678";

        /// <summary>Unified webhook path registered in n8n.</summary>
        public string WebhookPath { get; set; } = "/webhook-test/biblio-notify";

        /// <summary>HTTP timeout in seconds.</summary>
        public int TimeoutSeconds { get; set; } = 10;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Implementation
    // ──────────────────────────────────────────────────────────────────────────

    public sealed class NotificationService : INotificationService
    {
        private readonly HttpClient _http;
        private readonly NotificationOptions _opts;
        private readonly ILogger<NotificationService> _logger;

        private static readonly JsonSerializerOptions _jsonOpts =
            new(JsonSerializerDefaults.Web);

        public NotificationService(
            HttpClient http,
            ILogger<NotificationService> logger,
            IConfiguration configuration)
        {
            _http   = http;
            _logger = logger;
            _opts   = configuration
                          .GetSection(NotificationOptions.SectionName)
                          .Get<NotificationOptions>()
                      ?? new NotificationOptions();
        }

        // ── Low-level helper ───────────────────────────────────────────────────

        /// <summary>
        /// POST a JSON payload to the unified n8n webhook.
        /// Never throws — errors are logged so the user-facing request
        /// is never interrupted by a notification failure.
        /// </summary>
        private async Task PostToN8nAsync(object payload)
        {
            // Build the full URL from config so it is easy to override per-environment
            var url = $"{_opts.N8nBaseUrl.TrimEnd('/')}{_opts.WebhookPath}";
            try
            {
                using var cts = new CancellationTokenSource(
                    TimeSpan.FromSeconds(_opts.TimeoutSeconds));

                var json    = JsonSerializer.Serialize(payload, _jsonOpts);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Dispatching notification to n8n → {Url}", url);
                var response = await _http.PostAsync(url, content, cts.Token);

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("n8n acknowledged notification — HTTP {Status}",
                        (int)response.StatusCode);
                else
                {
                    var body = await response.Content.ReadAsStringAsync(cts.Token);
                    _logger.LogWarning("n8n returned {Status} for {Url}. Body: {Body}",
                        (int)response.StatusCode, url, body);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("n8n webhook timed out after {Timeout}s — {Url}",
                    _opts.TimeoutSeconds, url);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error reaching n8n webhook {Url}", url);
            }
        }

        // ── Public API ─────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public Task SendEmailAsync(string toEmail, string subject, string message)
        {
            _logger.LogInformation("Sending email to {Email} — {Subject}", toEmail, subject);

            return PostToN8nAsync(new
            {
                email     = toEmail,
                subject,
                message,
                timestamp = DateTime.UtcNow
            });
        }

        /// <inheritdoc/>
        public Task SendSmsAsync(string phoneNumber, string message)
        {
            _logger.LogInformation("Sending SMS to {Phone} — {Message}", phoneNumber, message);

            return PostToN8nAsync(new
            {
                phone     = phoneNumber,
                message,
                timestamp = DateTime.UtcNow
            });
        }

        /// <inheritdoc/>
                public Task SendBorrowNotificationAsync(
                        string userEmail, string bookTitle, DateTime dueDate)
        {
            var daysLeft = (int)(dueDate - DateTime.UtcNow).TotalDays;

                        var body = $"""
                                Bonjour,

                                Vous avez emprunté le livre suivant avec succès :

                                    📗 Titre       : {bookTitle}
                                    📅 À retourner : {dueDate:dd/MM/yyyy}
                                    🕐 Jours restants : {daysLeft}

                                Merci de le retourner avant la date d'échéance.

                                Cordialement,
                                L'équipe BiblioApp
                                """;

            return PostToN8nAsync(new
            {
                email     = userEmail,
                subject   = "📚 Confirmation d'emprunt",
                message   = body,
                timestamp = DateTime.UtcNow
            });
        }

        /// <inheritdoc/>
                public Task SendOverdueNotificationAsync(
                        string userEmail, string bookTitle, int daysOverdue)
        {
                        var body = $"""
                                Bonjour,

                                Le livre suivant est en retard :

                                    📕 Titre          : {bookTitle}
                                    ⚠️  Jours de retard : {daysOverdue}

                                Merci de le retourner immédiatement pour éviter des pénalités supplémentaires.

                                Cordialement,
                                L'équipe BiblioApp
                                """;

            return PostToN8nAsync(new
            {
                email     = userEmail,
                subject   = "⚠️ Livre en retard",
                message   = body,
                timestamp = DateTime.UtcNow
            });
        }

        /// <inheritdoc/>
        public Task SendReturnReminderAsync(
            string userEmail, string bookTitle, DateTime dueDate)
        {
            var daysLeft = (int)(dueDate - DateTime.UtcNow).TotalDays;
            var urgent   = daysLeft <= 1;

            var subject = urgent
                ? "⏰ URGENT : Retour du livre aujourd'hui"
                : $"📖 Rappel de retour — {daysLeft} jour(s) restant(s)";

            var body = $"""
                Bonjour,

                Ceci est un rappel{(urgent ? " urgent" : "")} concernant votre emprunt :

                  📘 Titre           : {bookTitle}
                  📅 Date d'échéance : {dueDate:dd/MM/yyyy}
                  🕐 Jours restants  : {daysLeft}

                {(urgent ? "Merci de retourner le livre AUJOURD'HUI pour éviter des pénalités." : "Merci de retourner le livre à temps.")}

                Cordialement,
                L'équipe BiblioApp
                """;

            return PostToN8nAsync(new
            {
                email     = userEmail,
                subject,
                message   = body,
                timestamp = DateTime.UtcNow
            });
        }
    }
}

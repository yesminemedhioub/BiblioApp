using BiblioApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BiblioApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _notificationService.SendEmailAsync(request.Email, request.Subject, request.Message);
                return Ok(new { message = "Email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                return StatusCode(500, new { error = "Failed to send email" });
            }
        }

        [HttpPost("send-sms")]
        public async Task<IActionResult> SendSms([FromBody] SendSmsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _notificationService.SendSmsAsync(request.PhoneNumber, request.Message);
                return Ok(new { message = "SMS sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending SMS: {ex.Message}");
                return StatusCode(500, new { error = "Failed to send SMS" });
            }
        }

        [HttpPost("borrow-notification")]
        public async Task<IActionResult> SendBorrowNotification([FromBody] BorrowNotificationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _notificationService.SendBorrowNotificationAsync(request.UserEmail, request.BookTitle, request.DueDate);
                return Ok(new { message = "Borrow notification sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending borrow notification: {ex.Message}");
                return StatusCode(500, new { error = "Failed to send notification" });
            }
        }

        [HttpPost("overdue-notification")]
        public async Task<IActionResult> SendOverdueNotification([FromBody] OverdueNotificationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _notificationService.SendOverdueNotificationAsync(request.UserEmail, request.BookTitle, request.DaysOverdue);
                return Ok(new { message = "Overdue notification sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending overdue notification: {ex.Message}");
                return StatusCode(500, new { error = "Failed to send notification" });
            }
        }

        [HttpPost("return-reminder")]
        public async Task<IActionResult> SendReturnReminder([FromBody] ReturnReminderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _notificationService.SendReturnReminderAsync(request.UserEmail, request.BookTitle, request.DueDate);
                return Ok(new { message = "Return reminder sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending return reminder: {ex.Message}");
                return StatusCode(500, new { error = "Failed to send reminder" });
            }
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health()
        {
            return Ok(new { status = "Notification service is running" });
        }
    }

    public class SendEmailRequest
    {
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }

    public class SendSmsRequest
    {
        public string? PhoneNumber { get; set; }
        public string? Message { get; set; }
    }

    public class BorrowNotificationRequest
    {
        public string? UserEmail { get; set; }
        public string? BookTitle { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class OverdueNotificationRequest
    {
        public string? UserEmail { get; set; }
        public string? BookTitle { get; set; }
        public int DaysOverdue { get; set; }
    }

    public class ReturnReminderRequest
    {
        public string? UserEmail { get; set; }
        public string? BookTitle { get; set; }
        public DateTime DueDate { get; set; }
    }
}

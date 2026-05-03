using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BiblioApp.Models;

public class Emprunt
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Nom de l'emprunteur")]
    public string NomEmprunteur { get; set; } = string.Empty;

    // ── NEW: email address so we can send notifications ──────────────────────
    [Required]
    [EmailAddress]
    [Display(Name = "Email de l'emprunteur")]
    public string EmailEmprunteur { get; set; } = string.Empty;

    [Display(Name = "Date d'emprunt")]
    public DateTime DateEmprunt { get; set; } = DateTime.Now;

    // ── NEW: explicit due date chosen by the librarian ───────────────────────
    [Required]
    [Display(Name = "Date d'échéance")]
    public DateTime DateEcheance { get; set; } = DateTime.Now.AddDays(14);

    [Display(Name = "Date de retour")]
    public DateTime? DateRetour { get; set; }   // null = not yet returned

    // Foreign key
    public int LivreId { get; set; }

    // Navigation
    [ValidateNever]
    public Livre? Livre { get; set; }
}

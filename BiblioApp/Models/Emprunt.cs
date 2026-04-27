using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BiblioApp.Models;

public class Emprunt
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Nom de l'emprunteur")]
    public string NomEmprunteur { get; set; } = string.Empty;

    [Display(Name = "Date d'emprunt")]
    public DateTime DateEmprunt { get; set; } = DateTime.Now;

    [Display(Name = "Date de retour")]
    public DateTime? DateRetour { get; set; }   // nullable = pas encore rendu

    // Clé étrangère
    public int LivreId { get; set; }

    // Navigation
    [ValidateNever]
    public Livre? Livre { get; set; }
}
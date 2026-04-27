using System.ComponentModel.DataAnnotations;

namespace BiblioApp.Models;

public class Livre
{
    public int Id { get; set; }

    [Required]
    public string Titre { get; set; } = string.Empty;

    [Required]
    [StringLength(13, MinimumLength = 13, ErrorMessage = "L'ISBN doit contenir exactement 13 chiffres")]
    public string ISBN { get; set; } = string.Empty;

    [Range(1000, 2100, ErrorMessage = "Année invalide")]
    [Display(Name = "Année de publication")]
    public int AnneePublication { get; set; }

    // Clé étrangère
    public int AuteurId { get; set; }

    // Navigation
    public Auteur? Auteur { get; set; }
    public ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();

    // Propriété calculée
    public bool EstDisponible => Emprunts == null || !Emprunts.Any(e => e.DateRetour == null);
}
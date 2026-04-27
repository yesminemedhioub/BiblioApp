using System.ComponentModel.DataAnnotations;

namespace BiblioApp.Models;

public class Auteur
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [Display(Name = "Date de naissance")]
    public DateTime DateNaissance { get; set; }

    // Propriété calculée (pas en BDD)
    public string NomComplet => $"{Prenom} {Nom}";

    // Navigation
    public ICollection<Livre> Livres { get; set; } = new List<Livre>();
}
using Microsoft.EntityFrameworkCore;
using BiblioApp.Models;

namespace BiblioApp.Data;

public class BiblioContext : DbContext
{
    public BiblioContext(DbContextOptions<BiblioContext> options) : base(options) { }

    public DbSet<Auteur> Auteurs { get; set; }
    public DbSet<Livre> Livres { get; set; }
    public DbSet<Emprunt> Emprunts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Auteur → Livres (Restrict = ne supprime pas les livres si on supprime l'auteur)
        modelBuilder.Entity<Livre>()
            .HasOne(l => l.Auteur)
            .WithMany(a => a.Livres)
            .HasForeignKey(l => l.AuteurId)
            .OnDelete(DeleteBehavior.Restrict);

        // Livre → Emprunts (Cascade = supprime les emprunts si on supprime le livre)
        modelBuilder.Entity<Emprunt>()
            .HasOne(e => e.Livre)
            .WithMany(l => l.Emprunts)
            .HasForeignKey(e => e.LivreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
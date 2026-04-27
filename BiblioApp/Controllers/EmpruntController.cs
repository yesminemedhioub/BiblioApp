using BiblioApp.Data;
using BiblioApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiblioApp.Controllers;

public class EmpruntController : Controller
{
    private readonly BiblioContext _context;

    public EmpruntController(BiblioContext context) { _context = context; }

    // Uniquement les emprunts en cours (non rendus)
    public async Task<IActionResult> Index()
    {
        var emprunts = await _context.Emprunts
            .Where(e => e.DateRetour == null)
            .Include(e => e.Livre)
                .ThenInclude(l => l!.Auteur)
            .ToListAsync();
        return View(emprunts);
    }

    public async Task<IActionResult> Create()
    {
        // Seulement les livres disponibles
        var livresDisponibles = await _context.Livres
            .Include(l => l.Emprunts)
            .Where(l => !l.Emprunts.Any(e => e.DateRetour == null))
            .ToListAsync();

        ViewBag.Livres = new SelectList(livresDisponibles, "Id", "Titre");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Emprunt emprunt)
    {
        if (ModelState.IsValid)
        {
            emprunt.DateEmprunt = DateTime.Now;
            _context.Add(emprunt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        var livresDisponibles = await _context.Livres
            .Include(l => l.Emprunts)
            .Where(l => !l.Emprunts.Any(e => e.DateRetour == null))
            .ToListAsync();
        ViewBag.Livres = new SelectList(livresDisponibles, "Id", "Titre");
        return View(emprunt);
    }

    // POST: Marquer comme retourné
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Retourner(int id)
    {
        var emprunt = await _context.Emprunts.FindAsync(id);
        if (emprunt == null) return NotFound();

        emprunt.DateRetour = DateTime.Now;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
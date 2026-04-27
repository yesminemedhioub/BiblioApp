using BiblioApp.Data;
using BiblioApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiblioApp.Controllers;

public class AuteurController : Controller
{
    private readonly BiblioContext _context;

    public AuteurController(BiblioContext context)
    {
        _context = context;
    }

    // GET: Liste des auteurs avec nb de livres
    public async Task<IActionResult> Index()
    {
        var auteurs = await _context.Auteurs
            .Include(a => a.Livres)
            .ToListAsync();
        return View(auteurs);
    }

    // GET: Formulaire de création
    public IActionResult Create() => View();

    // POST: Création
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Auteur auteur)
    {
        if (ModelState.IsValid)
        {
            _context.Add(auteur);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(auteur);
    }

    // GET: Formulaire de modification
    public async Task<IActionResult> Edit(int id)
    {
        var auteur = await _context.Auteurs.FindAsync(id);
        if (auteur == null) return NotFound();
        return View(auteur);
    }

    // POST: Modification
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Auteur auteur)
    {
        if (id != auteur.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(auteur);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(auteur);
    }

    // GET: Confirmation de suppression
    public async Task<IActionResult> Delete(int id)
    {
        var auteur = await _context.Auteurs
            .Include(a => a.Livres)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (auteur == null) return NotFound();
        return View(auteur);
    }

    // POST: Suppression
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var auteur = await _context.Auteurs.FindAsync(id);
        if (auteur != null) _context.Auteurs.Remove(auteur);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
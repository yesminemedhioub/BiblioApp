using BiblioApp.Data;
using BiblioApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BiblioApp.Controllers;

public class LivreController : Controller
{
    private readonly BiblioContext _context;

    public LivreController(BiblioContext context) { _context = context; }

    public async Task<IActionResult> Index()
    {
        var livres = await _context.Livres
            .Include(l => l.Auteur)
            .Include(l => l.Emprunts)
            .ToListAsync();
        return View(livres);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Auteurs = new SelectList(
            await _context.Auteurs.ToListAsync(), "Id", "NomComplet");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Livre livre)
    {
        if (ModelState.IsValid)
        {
            _context.Add(livre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Auteurs = new SelectList(
            await _context.Auteurs.ToListAsync(), "Id", "NomComplet");
        return View(livre);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var livre = await _context.Livres.FindAsync(id);
        if (livre == null) return NotFound();
        ViewBag.Auteurs = new SelectList(
            await _context.Auteurs.ToListAsync(), "Id", "NomComplet", livre.AuteurId);
        return View(livre);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Livre livre)
    {
        if (id != livre.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(livre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Auteurs = new SelectList(
            await _context.Auteurs.ToListAsync(), "Id", "NomComplet", livre.AuteurId);
        return View(livre);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var livre = await _context.Livres
            .Include(l => l.Auteur)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (livre == null) return NotFound();
        return View(livre);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var livre = await _context.Livres.FindAsync(id);
        if (livre != null) _context.Livres.Remove(livre);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
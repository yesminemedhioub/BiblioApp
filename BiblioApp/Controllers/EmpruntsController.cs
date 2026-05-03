using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiblioApp.Data;
using BiblioApp.Models;
using BiblioApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace BiblioApp.Controllers
{
    [Authorize]
    public class EmpruntsController : Controller
    {
        private readonly BiblioContext _context;
        private readonly INotificationService _notifications;

        public EmpruntsController(BiblioContext context, INotificationService notifications)
        {
            _context       = context;
            _notifications = notifications;
        }

        // GET: Emprunts
        public IActionResult Index(string? search, int page = 1)
        {
            int pageSize = 5;

            var query = _context.Emprunts
                .Where(e => e.DateRetour == null)
                .Include(e => e.Livre)
                    .ThenInclude(l => l!.Auteur)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e =>
                    e.NomEmprunteur.Contains(search) ||
                    e.Livre!.Titre.Contains(search)
                );
            }

            var emprunts = query
                .OrderByDescending(e => e.DateEmprunt)
                .ToList()
                .ToPagedList(page, pageSize);

            ViewBag.Search = search;
            return View(emprunts);
        }

        // GET: Emprunts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (emprunt == null) return NotFound();

            return View(emprunt);
        }

        // GET: Emprunts/Create
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public IActionResult Create()
        {
            var livresDisponibles = _context.Livres
                .Include(l => l.Emprunts)
                .Where(l => !l.Emprunts.Any(e => e.DateRetour == null))
                .ToList();

            ViewData["LivreId"] = new SelectList(livresDisponibles, "Id", "Titre");
            return View();
        }

        // POST: Emprunts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> Create(
            [Bind("Id,NomEmprunteur,EmailEmprunteur,DateEmprunt,DateEcheance,LivreId")] Emprunt emprunt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emprunt);
                await _context.SaveChangesAsync();

                // ── Fire borrow notification (fire-and-forget, never throws) ──
                var livre = await _context.Livres.FindAsync(emprunt.LivreId);
                _ = _notifications.SendBorrowNotificationAsync(
                    emprunt.EmailEmprunteur,
                    livre?.Titre ?? "—",
                    emprunt.DateEcheance);

                return RedirectToAction(nameof(Index));
            }

            var livresDisponibles = _context.Livres
                .Include(l => l.Emprunts)
                .Where(l => !l.Emprunts.Any(e => e.DateRetour == null))
                .ToList();
            ViewData["LivreId"] = new SelectList(livresDisponibles, "Id", "Titre");
            return View(emprunt);
        }

        // GET: Emprunts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt == null) return NotFound();

            ViewData["LivreId"] = new SelectList(_context.Livres, "Id", "ISBN", emprunt.LivreId);
            return View(emprunt);
        }

        // POST: Emprunts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,NomEmprunteur,EmailEmprunteur,DateEmprunt,DateEcheance,DateRetour,LivreId")] Emprunt emprunt)
        {
            if (id != emprunt.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emprunt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpruntExists(emprunt.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LivreId"] = new SelectList(_context.Livres, "Id", "ISBN", emprunt.LivreId);
            return View(emprunt);
        }

        // GET: Emprunts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (emprunt == null) return NotFound();

            return View(emprunt);
        }

        // POST: Emprunts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt != null) _context.Emprunts.Remove(emprunt);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpruntExists(int id) =>
            _context.Emprunts.Any(e => e.Id == id);

        // POST: Emprunts/Retourner
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> Retourner(int id)
        {
            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprunt == null) return NotFound();

            emprunt.DateRetour = DateTime.Now;
            await _context.SaveChangesAsync();

            // ── Send return confirmation email ─────────────────────────────────
            _ = _notifications.SendEmailAsync(
                emprunt.EmailEmprunteur,
                "✅ Retour confirmé",
                $"""
                Bonjour {emprunt.NomEmprunteur},

                Le retour du livre « {emprunt.Livre?.Titre} » a bien été enregistré le {DateTime.Now:dd/MM/yyyy}.

                Merci et à bientôt !

                L'équipe BiblioApp
                """);

            return RedirectToAction(nameof(Index));
        }
    }
}

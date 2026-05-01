using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiblioApp.Data;
using BiblioApp.Models;
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

        public EmpruntsController(BiblioContext context)
        {
            _context = context;
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
            if (id == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emprunt == null)
            {
                return NotFound();
            }

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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> Create([Bind("Id,NomEmprunteur,DateEmprunt,DateRetour,LivreId")] Emprunt emprunt)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emprunt);
                await _context.SaveChangesAsync();
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
            if (id == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt == null)
            {
                return NotFound();
            }
            ViewData["LivreId"] = new SelectList(_context.Livres, "Id", "ISBN", emprunt.LivreId);
            return View(emprunt);
        }

        // POST: Emprunts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomEmprunteur,DateEmprunt,DateRetour,LivreId")] Emprunt emprunt)
        {
            if (id != emprunt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emprunt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpruntExists(emprunt.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LivreId"] = new SelectList(_context.Livres, "Id", "ISBN", emprunt.LivreId);
            return View(emprunt);
        }

        // GET: Emprunts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emprunt == null)
            {
                return NotFound();
            }

            return View(emprunt);
        }

        // POST: Emprunts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt != null)
            {
                _context.Emprunts.Remove(emprunt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpruntExists(int id)
        {
            return _context.Emprunts.Any(e => e.Id == id);
        }

        // POST: Emprunts/Retourner
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> Retourner(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt == null) return NotFound();

            emprunt.DateRetour = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

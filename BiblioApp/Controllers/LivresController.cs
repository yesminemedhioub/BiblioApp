using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BiblioApp.Data;
using BiblioApp.Models;
using X.PagedList;
using X.PagedList.Extensions;



namespace BiblioApp.Controllers
{
    [Authorize]
    public class LivresController : Controller
    {
        private readonly BiblioContext _context;

        public LivresController(BiblioContext context)
        {
            _context = context;
        }

        // GET: Livres
        public IActionResult Index(string? search, int page = 1)
        {
            int pageSize = 5;

            var query = _context.Livres
                .Include(l => l.Auteur)
                .Include(l => l.Emprunts)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l =>
                    l.Titre.Contains(search) ||
                    l.ISBN.Contains(search) ||
                    l.Auteur!.Nom.Contains(search) ||
                    l.Auteur!.Prenom.Contains(search)
                );
            }

            var livres = query
                .OrderBy(l => l.Titre)
                .ToList()
                .ToPagedList(page, pageSize);

            ViewBag.Search = search;
            return View(livres);
        }

        // GET: Livres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .Include(l => l.Auteur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        // GET: Livres/Create
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public IActionResult Create()
        {
            ViewData["AuteurId"] = new SelectList(_context.Auteurs, "Id", "Nom");
            return View();
        }

        // POST: Livres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> Create([Bind("Id,Titre,ISBN,AnneePublication,AuteurId")] Livre livre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(livre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuteurId"] = new SelectList(_context.Auteurs, "Id", "Nom", livre.AuteurId);
            return View(livre);
        }

        // GET: Livres/Edit/5
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres.FindAsync(id);
            if (livre == null)
            {
                return NotFound();
            }
            ViewData["AuteurId"] = new SelectList(_context.Auteurs, "Id", "Nom", livre.AuteurId);
            return View(livre);
        }

        // POST: Livres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliothecaire")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,ISBN,AnneePublication,AuteurId")] Livre livre)
        {
            if (id != livre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(livre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivreExists(livre.Id))
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
            ViewData["AuteurId"] = new SelectList(_context.Auteurs, "Id", "Nom", livre.AuteurId);
            return View(livre);
        }

        // GET: Livres/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .Include(l => l.Auteur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        // POST: Livres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre != null)
            {
                _context.Livres.Remove(livre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LivreExists(int id)
        {
            return _context.Livres.Any(e => e.Id == id);
        }
    }
}

using BiblioApp.Data;
using BiblioApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiblioApp.Controllers;

public class HomeController : Controller
{
    private readonly BiblioContext _context;

    public HomeController(BiblioContext context) { _context = context; }

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel
        {
            NombreLivres = await _context.Livres.CountAsync(),
            NombreAuteurs = await _context.Auteurs.CountAsync(),
            EmpruntsEnCours = await _context.Emprunts
                                    .CountAsync(e => e.DateRetour == null),
            DerniersEmprunts = await _context.Emprunts
                                    .Include(e => e.Livre)
                                    .OrderByDescending(e => e.DateEmprunt)
                                    .Take(5)
                                    .ToListAsync()
        };
        return View(vm);
    }
}
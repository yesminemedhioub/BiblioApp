using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BiblioApp.Data;
using BiblioApp.Models;

namespace BiblioApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuteursController : ControllerBase
    {
        private readonly BiblioContext _context;

        public AuteursController(BiblioContext context)
        {
            _context = context;
        }

        // GET: api/Auteurs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Auteur>>> GetAuteurs()
        {
            return await _context.Auteurs.ToListAsync();
        }

        // GET: api/Auteurs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Auteur>> GetAuteur(int id)
        {
            var auteur = await _context.Auteurs.FindAsync(id);

            if (auteur == null)
            {
                return NotFound();
            }

            return auteur;
        }

        // PUT: api/Auteurs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuteur(int id, Auteur auteur)
        {
            if (id != auteur.Id)
            {
                return BadRequest();
            }

            _context.Entry(auteur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuteurExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Auteurs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Auteur>> PostAuteur(Auteur auteur)
        {
            _context.Auteurs.Add(auteur);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuteur", new { id = auteur.Id }, auteur);
        }

        // DELETE: api/Auteurs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuteur(int id)
        {
            var auteur = await _context.Auteurs.FindAsync(id);
            if (auteur == null)
            {
                return NotFound();
            }

            _context.Auteurs.Remove(auteur);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuteurExists(int id)
        {
            return _context.Auteurs.Any(e => e.Id == id);
        }
    }
}

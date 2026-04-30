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
    public class LivresController : ControllerBase
    {
        private readonly BiblioContext _context;

        public LivresController(BiblioContext context)
        {
            _context = context;
        }

        // GET: api/Livres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Livre>>> GetLivres()
        {
            return await _context.Livres.ToListAsync();
        }

        // GET: api/Livres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Livre>> GetLivre(int id)
        {
            var livre = await _context.Livres.FindAsync(id);

            if (livre == null)
            {
                return NotFound();
            }

            return livre;
        }

        // PUT: api/Livres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLivre(int id, Livre livre)
        {
            if (id != livre.Id)
            {
                return BadRequest();
            }

            _context.Entry(livre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LivreExists(id))
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

        // POST: api/Livres
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Livre>> PostLivre(Livre livre)
        {
            _context.Livres.Add(livre);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLivre", new { id = livre.Id }, livre);
        }

        // DELETE: api/Livres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivre(int id)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre == null)
            {
                return NotFound();
            }

            _context.Livres.Remove(livre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LivreExists(int id)
        {
            return _context.Livres.Any(e => e.Id == id);
        }
    }
}

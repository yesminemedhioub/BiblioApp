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
    public class EmpruntsController : ControllerBase
    {
        private readonly BiblioContext _context;

        public EmpruntsController(BiblioContext context)
        {
            _context = context;
        }

        // GET: api/Emprunts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Emprunt>>> GetEmprunts()
        {
            return await _context.Emprunts.ToListAsync();
        }

        // GET: api/Emprunts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Emprunt>> GetEmprunt(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);

            if (emprunt == null)
            {
                return NotFound();
            }

            return emprunt;
        }

        // PUT: api/Emprunts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmprunt(int id, Emprunt emprunt)
        {
            if (id != emprunt.Id)
            {
                return BadRequest();
            }

            _context.Entry(emprunt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpruntExists(id))
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

        // POST: api/Emprunts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Emprunt>> PostEmprunt(Emprunt emprunt)
        {
            _context.Emprunts.Add(emprunt);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmprunt", new { id = emprunt.Id }, emprunt);
        }

        // DELETE: api/Emprunts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmprunt(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt == null)
            {
                return NotFound();
            }

            _context.Emprunts.Remove(emprunt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmpruntExists(int id)
        {
            return _context.Emprunts.Any(e => e.Id == id);
        }
    }
}

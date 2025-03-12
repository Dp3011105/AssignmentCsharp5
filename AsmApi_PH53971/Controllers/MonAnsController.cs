using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsmApi_PH53971.Data;
using AsmApi_PH53971.Models;

namespace AsmApi_PH53971.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonAnsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MonAnsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MonAn>>> GetMonAns()
        {
            return await _context.MonAns.ToListAsync();
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<MonAn>> GetMonAn(int id)
        {
            var monAn = await _context.MonAns.FindAsync(id);

            if (monAn == null)
            {
                return NotFound();
            }

            return monAn;
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMonAn(int id, MonAn monAn)
        {
            if (id != monAn.MaMonAn)
            {
                return BadRequest();
            }

            _context.Entry(monAn).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonAnExists(id))
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

       
        [HttpPost]
        public async Task<ActionResult<MonAn>> PostMonAn(MonAn monAn)
        {
            _context.MonAns.Add(monAn);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMonAn", new { id = monAn.MaMonAn }, monAn);
        }

      

        private bool MonAnExists(int id)
        {
            return _context.MonAns.Any(e => e.MaMonAn == id);
        }
    }
}

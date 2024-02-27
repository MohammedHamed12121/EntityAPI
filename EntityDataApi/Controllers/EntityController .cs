using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityDataApi.Data;
using EntityDataApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityDataApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EntityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // [HttpGet]
        // public ActionResult<IEnumerable<Entity>> GetEntities()
        // {
        //     return Ok(_context.Entities.ToList());
        // }
        
        [HttpGet]
        public ActionResult<IEnumerable<Entity>> GetEntities([FromQuery] string? search = null, [FromQuery] string? gender = null)
        {
            IQueryable<Entity> query = _context.Entities;

            // Apply search filter if search query parameter is provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Id!.Contains(search) ||
                                          e.Names!.Any(n => n.FirstName!.Contains(search) || n.Surname!.Contains(search)));
            }

            // Apply gender filter if gender query parameter is provided
            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(e => e.Gender == gender);
            }

            return query.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Entity> GetEntity(string id)
        {
            var entity = _context.Entities.Find(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        [HttpPost]
        public ActionResult<Entity> PostEntity(Entity entity)
        {
            _context.Entities.Add(entity);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public IActionResult PutEntity(string id, Entity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }

            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEntity(string id)
        {
            var entity = _context.Entities.Find(id);
            if (entity == null)
            {
                return NotFound();
            }

            _context.Entities.Remove(entity);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
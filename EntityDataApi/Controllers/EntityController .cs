using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        [HttpGet]
        // TODO: move those to an object
        public ActionResult<IEnumerable<Entity>> GetEntities(
                                                    [FromQuery] string? search = null,
                                                    [FromQuery] string? gender = null,
                                                    [FromQuery] string? country = null,
                                                    [FromQuery] string? addressLine = null)
        {
            IQueryable<Entity> query = _context.Entities
                                                .Include(e => e.Addresses) 
                                                .Include(e => e.Names)
                                                .Include(e => e.Dates);

            // Apply search filter if search provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Id.Contains(search) ||
                                          e.Names.Any(n => n.FirstName.Contains(search) || n.Surname.Contains(search)) ||
                                          e.Addresses.Any(a => a.Country.Contains(search) || a.AddressLine.Contains(search)));
            }

            // Apply gender filter if gender is provided
            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(e => e.Gender == gender);
            }

            // Apply address country filter if country provided
            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(e => e.Addresses.Any(a => a.Country.Contains(country)));
            }

            // Apply address line filter if addressLine provided
            if (!string.IsNullOrEmpty(addressLine))
            {
                query = query.Where(e => e.Addresses.Any(a => a.AddressLine.Contains(addressLine)));
            }
            return Ok(query.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<Entity> GetEntity(string id)
        {
            var entity = _context.Entities
                .Include(e => e.Addresses)
                .Include(e => e.Dates)
                .Include(e => e.Names)
                .FirstOrDefault(e => e.Id == id);
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
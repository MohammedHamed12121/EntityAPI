using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                                                    [FromQuery] string? addressLine = null,
                                                    [FromQuery] DateTime? startDate = null,
                                                    [FromQuery] DateTime? endDate = null,
                                                    [FromQuery] string sortBy = "Id",
                                                    [FromQuery] string sortDirection = "asc",
                                                    [FromQuery] int page = 1,
                                                    [FromQuery] int pageSize = 10)
        {
            IQueryable<Entity> query = _context.Entities
                                                .Include(e => e.Addresses)
                                                .Include(e => e.Names)
                                                .Include(e => e.Dates);

            // search filter if search is provided
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Id.Contains(search) ||
                                          e.Names.Any(n => n.FirstName.Contains(search) || n.Surname.Contains(search)) ||
                                          e.Addresses.Any(a => a.Country.Contains(search) || a.AddressLine.Contains(search)));
            }

            // gender filter if gender is provided
            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(e => e.Gender == gender);
            }

            // address country filter if country is provided
            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(e => e.Addresses.Any(a => a.Country.Contains(country)));
            }

            // address line filter if addressLine is provided
            if (!string.IsNullOrEmpty(addressLine))
            {
                query = query.Where(e => e.Addresses.Any(a => a.AddressLine.Contains(addressLine)));
            }
            if (startDate != null && endDate != null)
            {
                DateTime start = startDate.Value.Date;
                DateTime end = endDate.Value.Date
                                            .AddDays(1)
                                            .AddTicks(-1); // Set the end date to 23:59:59.999 because end data is inclusive

                query = query.Where(e => e.Dates.Any(d => d.DateValue >= start && d.DateValue <= end));
            }

            // sorting
            var propertyInfo = typeof(Entity).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                query = sortDirection.ToLower() == "desc" ?
                    query.OrderByDescending(x => EF.Property<object>(x, sortBy)) :
                    query.OrderBy(x => EF.Property<object>(x, sortBy));
            }


            // Add pagination and paganition meta date to the response
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var entities = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var metadata = new
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = totalPages
            };

            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(metadata));

            return Ok(query);
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
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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EntityDataApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EntityController> _logger;
        private const int MaxRetryAttempts = 3;
        private const int DelayBetweenRetriesMilliseconds = 1000;

        public EntityController(ApplicationDbContext context, ILogger<EntityController> logger)
        {
            _context = context;
            _logger = logger;
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
                
                query = query.Where(e => e.Id.ToString() == search ||
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
        public ActionResult<Entity> GetEntity(int id)
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
        public async Task<ActionResult<Entity>> CreateEntity(Entity entity)
        {
            int retryCount = 0;

            while (retryCount < MaxRetryAttempts)
            {
                try
                {
                    _context.Entities.Add(entity);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, entity);
                }
                catch (DbUpdateException ex) when (IsTransientError(ex) && retryCount < MaxRetryAttempts)
                {
                    retryCount++;
                    _logger.LogWarning($"Transient error occurred while saving entity. Retrying attempt {retryCount} after {DelayBetweenRetriesMilliseconds} milliseconds.");
                    await Task.Delay(DelayBetweenRetriesMilliseconds);
                }
            }

            _logger.LogError($"Failed to save entity after {MaxRetryAttempts} retry attempts.");
            return StatusCode(500, "Failed to save entity after retry attempts.");
        }

        private bool IsTransientError(DbUpdateException ex)
        {
            // Check if the exception is a transient error
            return ex.InnerException is SqlException sqlException && (sqlException.Number == -2 || sqlException.Number == 1205);
        }

        [HttpPut("{id}")]
        public IActionResult PutEntity(int id, Entity entity)
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
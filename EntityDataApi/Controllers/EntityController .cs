using System;
using System.Collections.Generic;
using System.Linq;
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
                                                    [FromQuery] string? addressLine = null)
        {
            IQueryable<Entity> query = _context.Entities
                                                .Include(e => e.Addresses)
                                                .Include(e => e.Names)
                                                .Include(e => e.Dates);

            // Apply search filter if search provided
            if (!string.IsNullOrEmpty(search))
            {
                
                query = query.Where(e => e.Id.ToString() == search ||
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
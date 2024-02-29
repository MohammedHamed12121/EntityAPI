using EntityDataApi.Data;
using EntityDataApi.Helpers;
using EntityDataApi.IRepositories;
using EntityDataApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EntityDataApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityController : ControllerBase
    {
       private readonly IEntitiesRepository _entityRepo;
        private readonly ILogger<EntityController> _logger;
        private readonly RetryHelper _retryHelper;

        public EntityController(ILogger<EntityController> logger, IEntitiesRepository entityRepo, RetryHelper retryHelper)
        {
            _logger = logger;
            _entityRepo = entityRepo;
            _retryHelper = retryHelper;
        }

        // TODO: Has error
        #region GetAll
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entity>>> GetEntities([FromQuery] SearchFilterPagnationParameters parameters)
        {
            try
            {
                var query = await _entityRepo.GetAllEntites(parameters);

                // Add pagination meta date to the response
                var totalCount = query.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize);
                var metadata = new
                {
                    TotalCount = totalCount,
                    PageSize = parameters.PageSize,
                    CurrentPage = parameters.Page,
                    TotalPages = totalPages
                };
                Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(metadata));

                return Ok(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving entities.");
                return StatusCode(500, "An error occurred while retrieving entities.");
            }
        }
        #endregion

        #region GetById
        [HttpGet("{id}")]
        public async Task<ActionResult<Entity>> GetEntity(int id)
        {
            try
            {
                var entity = await _entityRepo.GetEntityAsync(id);
                if (entity == null)
                {
                    return NotFound();
                }
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the entity.");
                return StatusCode(500, "An error occurred while retrieving the entity.");
            }
        }
        #endregion

        #region  Post
        [HttpPost]
        public async Task<ActionResult<Entity>> CreateEntity(Entity entity)
        {
            try
            {
                // create the entity with retry and back off 
                var newEntity = await _retryHelper.RetryWithBackoffAsync(async () =>
                {
                    return await _entityRepo.AddEntityAsync(entity);
                });

                return CreatedAtAction(nameof(GetEntity), new { id = newEntity.Id }, newEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the entity.");
                return StatusCode(500, "An error occurred while creating the entity.");
            }
        }
        #endregion

        #region put
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntity(int id, Entity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest("Entity ID in the request body does not match the ID in the URL.");
            }

            try
            {
                var existingEntity = await _entityRepo.GetEntityAsync(id);
                if (existingEntity == null)
                {
                    return NotFound("Entity not found.");
                }

                var newEntity = await _retryHelper.RetryWithBackoffAsync(async () =>
                {
                    return await _entityRepo.UpdateEntityAsync(entity);
                });

                return CreatedAtAction(nameof(GetEntity), new { id = newEntity.Id }, newEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the entity.");
                return StatusCode(500, "An error occurred while updating the entity.");
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntity(int id)
        {
            try
            { 
                var result = await _retryHelper.RetryWithBackoffAsync(async () =>
                {
                    return await _entityRepo.DeleteEntityAsync(id);
                });
                if (!result)
                {
                    return NotFound("Entity not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the entity.");
                return StatusCode(500, "An error occurred while deleting the entity.");
            }
        }
        #endregion
    }
}
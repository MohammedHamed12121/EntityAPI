using EntityDataApi.Data;
using EntityDataApi.IRepositories;
using EntityDataApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EntityDataApi.Repositories
{
    public class EntitiesRepository : IEntitiesRepository
    {
        private readonly ApplicationDbContext _context;
        public EntitiesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Entity>> GetAllEntites(SearchFilterPagnationParameters parameters)
        {
            IQueryable<Entity> query = _context.Entities
                                                .Include(e => e.Addresses)
                                                .Include(e => e.Names)
                                                .Include(e => e.Dates);

            // search filter if search is provided
            if (!string.IsNullOrEmpty(parameters.Search))
            {
                query = query.Where(e => e.Names.Any(n => n.FirstName.Contains(parameters.Search) || 
                                                          n.Surname.Contains(parameters.Search)) ||
                                                          e.Addresses.Any(a => a.Country.Contains(parameters.Search) || 
                                                                               a.AddressLine.Contains(parameters.Search)));
            }

            // gender filter if gender is provided
            if (!string.IsNullOrEmpty(parameters.Gender))
            {
                query = query.Where(e => e.Gender == parameters.Gender);
            }

            // address country filter if country is provided
            if (!string.IsNullOrEmpty(parameters.Country))
            {
                query = query.Where(e => e.Addresses.Any(a => a.Country.Contains(parameters.Country)));
            }

            // address line filter if addressLine is provided
            if (!string.IsNullOrEmpty(parameters.AddressLine))
            {
                query = query.Where(e => e.Addresses.Any(a => a.AddressLine.Contains(parameters.AddressLine)));
            }

            if (parameters.StartDate != null && parameters.EndDate != null)
            {
                DateTime start = parameters.StartDate.Value.Date;
                DateTime end = parameters.EndDate.Value.Date
                                            .AddDays(1)
                                            .AddTicks(-1); // Set the end date to 23:59:59.999 because end data is inclusive

                query = query.Where(e => e.Dates.Any(d => d.DateValue >= start && d.DateValue <= end));
            }

            // sorting
            var propertyInfo = typeof(Entity).GetProperty(parameters.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                query = parameters.SortDirection.ToLower() == "desc" ?
                    query.OrderByDescending(x => EF.Property<object>(x, parameters.SortBy)) :
                    query.OrderBy(x => EF.Property<object>(x, parameters.SortBy));
            }


            // Add pagination and paganition meta date to the response
            var entities = await query.Skip((parameters.Page - 1) * parameters.PageSize)
                                      .Take(parameters.PageSize)
                                      .AsNoTracking()
                                      .ToListAsync();
            return entities;
        }

        public async Task<Entity> GetEntityAsync(int id)
        {
            return await _context.Entities
                                 .Include(e => e.Addresses)
                                 .Include(e => e.Dates)
                                 .Include(e => e.Names)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Entity> AddEntityAsync(Entity entity)
        {
            _context.Entities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Entity> UpdateEntityAsync(Entity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteEntityAsync(int entityId)
        {
            var entity = await _context.Entities.FindAsync(entityId);
            if (entity == null)
            {
                return false;
            }

            _context.Entities.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
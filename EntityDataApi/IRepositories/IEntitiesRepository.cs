using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityDataApi.Data;
using EntityDataApi.Models;

namespace EntityDataApi.IRepositories
{
    public interface IEntitiesRepository
    {
        Task<IList<Entity>> GetAllEntites(SearchFilterPagnationParameters parameters);
        Task<Entity> GetEntityAsync(int id);
        Task<Entity> AddEntityAsync(Entity entity);
        Task<Entity> UpdateEntityAsync(Entity entity);
        Task<bool> DeleteEntityAsync(int entityId);

    }                                        
}                                            
                                             
                                             
                                             
                                             
                                             
                                             
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nello_Data.Interfaces
{
    public interface ITableStorageRepository<T> where T : class
    {
        string TABLE_NAME { get; set; }
        CloudTable TABLE { get; set; }

        IEnumerable<T> GetEntities();
        Task<T> GetEntityAsync(string partitionKey, string rowKey);
        IEnumerable<T> QueryEntities(string query);
        Task<bool> AddEntityAsync(T obj);
        Task<int> AddEntitiesAsync(IEnumerable<T> objs);
        Task<bool> UpdateEntityAsync(T obj);
        Task<bool> DeleteEntityAsync(string partitionKey, string rowKey);
        Task<bool> DeleteEntityAsync(T obj);
    }
}
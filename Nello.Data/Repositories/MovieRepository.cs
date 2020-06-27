using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Nello_Data.Interfaces;
using Nello_Data.Models;

namespace Nello_Data.Repositories
{
    public class MovieRepository : ITableStorageRepository<MovieModel>
    {
        public MovieRepository()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Config.conn_string);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            TABLE_NAME = Config.MOVIE_TABLE_NAME;
            TABLE = tableClient.GetTableReference(TABLE_NAME);
            TABLE.CreateIfNotExists();
        }

        public MovieRepository(string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Config.conn_string);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            TABLE_NAME = tableName;
            TABLE = tableClient.GetTableReference(TABLE_NAME);
            TABLE.CreateIfNotExists();
        }

        public async Task DeleteTableAsync()
        {
            await TABLE.DeleteIfExistsAsync();
        }

        public string TABLE_NAME { get; set; }
        public CloudTable TABLE { get; set; }


        public async Task<int> AddEntitiesAsync(IEnumerable<MovieModel> objs)
        {
            var totalAdded = 0;
            foreach (var obj in objs)
            {
                if (await this.AddEntityAsync(obj))
                {
                    totalAdded++;
                }
            }
            return totalAdded;
        }

        public async Task<bool> AddEntityAsync(MovieModel obj)
        {
            var insertOperation = TableOperation.Insert(obj);
            var result = await TABLE.ExecuteAsync(insertOperation);
            return result.HttpStatusCode.Equals(204);
        }

        public async Task<bool> DeleteEntityAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<MovieModel>(partitionKey, rowKey);
            var executionResult = await TABLE.ExecuteAsync(retrieveOperation);
            var deleteOperation = TableOperation.Delete(executionResult.Result as MovieModel);
            return ((await TABLE.ExecuteAsync(deleteOperation)).HttpStatusCode == 204);
        }

        public async Task<bool> DeleteEntityAsync(MovieModel obj)
        {
            var deleteOperation = TableOperation.Delete(obj);
            return ((await TABLE.ExecuteAsync(deleteOperation)).HttpStatusCode == 204);
        }

        public IEnumerable<MovieModel> GetEntities()
        {
            var result = TABLE.ExecuteQuery(new TableQuery<MovieModel>()).ToList();
            return result;
        }

        public async Task<MovieModel> GetEntityAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<MovieModel>(partitionKey, rowKey);
            var executionResult = await TABLE.ExecuteAsync(retrieveOperation);
            return executionResult.Result as MovieModel;
        }

        public IEnumerable<MovieModel> QueryEntities(int minRating, int maxRating, int maxRuntime)
        {
            var result = from entity in TABLE.CreateQuery<MovieModel>() where 
                           entity.Rating > minRating
                            && entity.Rating < maxRating
                            && entity.RunTime < maxRuntime
                         select entity;
            return result.ToList<MovieModel>();
        }

        public async Task<bool> UpdateEntityAsync(MovieModel obj)
        {
            var retrieveOperation = TableOperation.Retrieve<MovieModel>(obj.PartitionKey, obj.RowKey);
            await TABLE.ExecuteAsync(retrieveOperation);
            var updateOperation = TableOperation.Replace(obj);
            var result = await TABLE.ExecuteAsync(updateOperation);
            return result.HttpStatusCode.Equals(204);
        }

        public IEnumerable<MovieModel> QueryEntities(string query)
        {
            throw new NotImplementedException();
        }
    }
}

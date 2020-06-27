using Microsoft.Azure.Cosmos.Table;
using Nello_Data.Interfaces;
using Nello_Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nello_Data.Repositories
{
    public class MovieGenredataRepository : ITableStorageRepository<MovieGenredataModel>
    {
        public string TABLE_NAME { get; set; }
        public CloudTable TABLE { get; set; }

        public MovieGenredataRepository()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Config.conn_string);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            TABLE_NAME = Config.CATAGORYTABLE_NAME;
            TABLE = tableClient.GetTableReference(TABLE_NAME);
            TABLE.CreateIfNotExists();
        }

        public MovieGenredataRepository(string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Config.conn_string);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            TABLE_NAME = tableName;
            TABLE = tableClient.GetTableReference(TABLE_NAME);
            TABLE.CreateIfNotExists();
        }

        public async Task DeleteTableAsync()
        {
            await TABLE.DeleteAsync();
        }

        public async Task<int> AddEntitiesAsync(IEnumerable<MovieGenredataModel> objs)
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

        public async Task<bool> AddEntityAsync(MovieGenredataModel obj)
        {
            var insertOperation = TableOperation.Insert(obj);
            var result = await TABLE.ExecuteAsync(insertOperation);
            return result.HttpStatusCode.Equals(204);
        }

        public async Task<bool> DeleteEntityAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<MovieGenredataModel>(partitionKey, rowKey);
            var executionResult = await TABLE.ExecuteAsync(retrieveOperation);
            var deleteOperation = TableOperation.Delete(executionResult.Result as MovieGenredataModel);
            return ((await TABLE.ExecuteAsync(deleteOperation)).HttpStatusCode == 204);
        }

        public async Task<bool> DeleteEntityAsync(MovieGenredataModel obj)
        {
            var deleteOperation = TableOperation.Delete(obj);
            return ((await TABLE.ExecuteAsync(deleteOperation)).HttpStatusCode == 204);
        }

        public IEnumerable<MovieGenredataModel> GetEntities()
        {
            var result = TABLE.ExecuteQuery(new TableQuery<MovieGenredataModel>()).ToList();
            return result;
        }

        public async Task<MovieGenredataModel> GetEntityAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<MovieGenredataModel>(partitionKey, rowKey);
            var executionResult = await TABLE.ExecuteAsync(retrieveOperation);
            return executionResult.Result as MovieGenredataModel;
        }

        public IEnumerable<MovieGenredataModel> QueryEntities(string query)
        {
            var result = from entity in TABLE.CreateQuery<MovieGenredataModel>()
                         select entity;
            return result.ToList<MovieGenredataModel>();
        }

        public async Task<bool> UpdateEntityAsync(MovieGenredataModel obj)
        {
            var retrieveOperation = TableOperation.Retrieve<MovieGenredataModel>(obj.PartitionKey, obj.RowKey);
            await TABLE.ExecuteAsync(retrieveOperation);
            var updateOperation = TableOperation.InsertOrReplace(obj);
            var result = await TABLE.ExecuteAsync(updateOperation);
            return result.HttpStatusCode.Equals(204);
        }
    }
}

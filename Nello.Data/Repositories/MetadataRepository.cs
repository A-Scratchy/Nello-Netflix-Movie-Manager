using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Nello_Data.Interfaces;
using Nello_Data.Models;

namespace Nello_Data.Repositories
{
    public class MetadataRepository : ITableStorageRepository<MetadataModel>
    {
        public MetadataRepository()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Config.conn_string);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            TABLE_NAME = Config.META_TABLE_NAME;
            TABLE = tableClient.GetTableReference(TABLE_NAME);
            TABLE.CreateIfNotExists();
        }

        public MetadataRepository(string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Config.conn_string);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            TABLE_NAME = tableName;
            TABLE = tableClient.GetTableReference(TABLE_NAME);
            TABLE.CreateIfNotExists();
        }

        public string TABLE_NAME { get; set; }
        public CloudTable TABLE { get; set; }

        public async Task DeleteTableAsync()
        {
            await TABLE.DeleteAsync();
        }

        public async Task<int> AddEntitiesAsync(IEnumerable<MetadataModel> objs)
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

        public async Task<bool> AddEntityAsync(MetadataModel obj)
        {
            var insertOperation = TableOperation.Insert(obj);
            var result = await TABLE.ExecuteAsync(insertOperation);
            return result.HttpStatusCode.Equals(204);
        }

        public async Task<bool> DeleteEntityAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<MetadataModel>(partitionKey, rowKey);
            var executionResult = await TABLE.ExecuteAsync(retrieveOperation);
            var deleteOperation = TableOperation.Delete(executionResult.Result as MetadataModel);
            return ((await TABLE.ExecuteAsync(deleteOperation)).HttpStatusCode == 204);
        }

        public async Task<bool> DeleteEntityAsync(MetadataModel obj)
        {
            var deleteOperation = TableOperation.Delete(obj);
            return ((await TABLE.ExecuteAsync(deleteOperation)).HttpStatusCode == 204);
        }

        public IEnumerable<MetadataModel> GetEntities()
        {
            var result = TABLE.ExecuteQuery(new TableQuery<MetadataModel>()).ToList();
            return result;
        }

        public async Task<MetadataModel> GetEntityAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<MetadataModel>(partitionKey, rowKey);
            var executionResult = await TABLE.ExecuteAsync(retrieveOperation);
            return executionResult.Result as MetadataModel;
        }

        public IEnumerable<MetadataModel> QueryEntities(string query)
        {
            var result = from entity in TABLE.CreateQuery<MetadataModel>()
                         select entity;
            return result.ToList<MetadataModel>();
        }

        public async Task<bool> UpdateEntityAsync(MetadataModel obj)
        {
            var retrieveOperation = TableOperation.Retrieve<MetadataModel>(obj.PartitionKey, obj.RowKey);
            await TABLE.ExecuteAsync(retrieveOperation);
            var updateOperation = TableOperation.InsertOrReplace(obj);
            var result = await TABLE.ExecuteAsync(updateOperation);
            return result.HttpStatusCode.Equals(204);
        }
    }
}

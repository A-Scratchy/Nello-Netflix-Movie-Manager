using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Nello_Data.Interfaces;
using Nello_Data.Models;
using Newtonsoft.Json;

namespace Nello_Data.Repositories
{
    public class CatalogRepository : ITableStorageRepository<CatalogModel>
    {
        public CatalogRepository()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Config.conn_string);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            TABLE_NAME = Config.GENREDATATABLE_NAME;
            TABLE = tableClient.GetTableReference(TABLE_NAME);
            TABLE.CreateIfNotExists();
        }

        public CatalogRepository(string tableName)
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

        public async Task<int> AddEntitiesAsync(IEnumerable<CatalogModel> objs)
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

        public async Task<bool> AddEntityAsync(CatalogModel obj)
        {
            var insertOperation = TableOperation.Insert(obj);
            var result = await TABLE.ExecuteAsync(insertOperation);
            return result.HttpStatusCode.Equals(204);
        }

        public async Task<bool> DeleteEntityAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<CatalogModel>(partitionKey, rowKey);
            var executionResult = await TABLE.ExecuteAsync(retrieveOperation);
            var deleteOperation = TableOperation.Delete(executionResult.Result as CatalogModel);
            return ((await TABLE.ExecuteAsync(deleteOperation)).HttpStatusCode == 204);
        }

        public async Task<bool> DeleteEntityAsync(CatalogModel obj)
        {
            var deleteOperation = TableOperation.Delete(obj);
            return ((await TABLE.ExecuteAsync(deleteOperation)).HttpStatusCode == 204);
        }

        public IEnumerable<CatalogModel> GetEntities()
        {
            var result = TABLE.ExecuteQuery(new TableQuery<CatalogModel>()).ToList();
            return result;
        }

        public async Task<CatalogModel> GetEntityAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<CatalogModel>(partitionKey, rowKey);
            var executionResult = await TABLE.ExecuteAsync(retrieveOperation);
            return executionResult.Result as CatalogModel;
        }

        public IEnumerable<CatalogModel> QueryEntities(string query)
        {
            var result = from entity in TABLE.CreateQuery<CatalogModel>()
                         select entity;
            return result.ToList<CatalogModel>();
        }

        public async Task<bool> UpdateEntityAsync(CatalogModel obj)
        {
            var retrieveOperation = TableOperation.Retrieve<CatalogModel>(obj.PartitionKey, obj.RowKey);
            await TABLE.ExecuteAsync(retrieveOperation);
            var updateOperation = TableOperation.InsertOrReplace(obj);
            var result = await TABLE.ExecuteAsync(updateOperation);
            return result.HttpStatusCode.Equals(204);
        }
    }
}

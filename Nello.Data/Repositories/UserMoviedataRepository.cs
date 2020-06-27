using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Nello_Data.Interfaces;
using Nello_Data.Models;


namespace Nello_Data.Repositories
{
    public class UserMoviedataRepository : ITableStorageRepository<UserMoviedataModel>
    {
        public UserMoviedataRepository()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Config.conn_string);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            TABLE_NAME = Config.USERDATA_TABLE_NAME;
            TABLE = tableClient.GetTableReference(TABLE_NAME);
            TABLE.CreateIfNotExists();
        }

        public UserMoviedataRepository(string tableName)
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

        public async Task<int> AddEntitiesAsync(IEnumerable<UserMoviedataModel> objs)
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

        public async Task<bool> AddEntityAsync(UserMoviedataModel obj)
        {
            var insertOperation = TableOperation.Insert(obj);
            var result = await TABLE.ExecuteAsync(insertOperation);
            return result.HttpStatusCode.Equals(204);
        }

        public async Task<bool> DeleteEntityAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<UserMoviedataModel>(partitionKey, rowKey);
            var executionResult = await TABLE.ExecuteAsync(retrieveOperation);
            var deleteOperation = TableOperation.Delete(executionResult.Result as UserMoviedataModel);
            return ((await TABLE.ExecuteAsync(deleteOperation)).HttpStatusCode == 204);
        }

        public async Task<bool> DeleteEntityAsync(UserMoviedataModel obj)
        {
            var deleteOperation = TableOperation.Delete(obj);
            return ((await TABLE.ExecuteAsync(deleteOperation)).HttpStatusCode == 204);
        }

        public IEnumerable<UserMoviedataModel> GetEntities()
        {
            var result = TABLE.ExecuteQuery(new TableQuery<UserMoviedataModel>()).ToList();
            return result;
        }

        public async Task<UserMoviedataModel> GetEntityAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<UserMoviedataModel>(partitionKey, rowKey);
            var executionResult = await TABLE.ExecuteAsync(retrieveOperation);
            return executionResult.Result as UserMoviedataModel;
        }

        public IEnumerable<UserMoviedataModel> QueryEntities(string query)
        {
            var result = from entity in TABLE.CreateQuery<UserMoviedataModel>()
                         select entity;
            return result.ToList<UserMoviedataModel>();
        }

        public async Task<bool> UpdateEntityAsync(UserMoviedataModel obj)
        {
            var retrieveOperation = TableOperation.Retrieve<UserMoviedataModel>(obj.PartitionKey, obj.RowKey);
            await TABLE.ExecuteAsync(retrieveOperation);
            var updateOperation = TableOperation.InsertOrReplace(obj);
            var result = await TABLE.ExecuteAsync(updateOperation);
            return result.HttpStatusCode.Equals(204);
        }
    }
}

using MongoDB.Bson;
using MongoDB.Driver;

using Nello.Data.Interfaces;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Nello.Data.Repos
{
    public class MongoDBRepo : IMongoDBRepo
    {
        private readonly IMongoDatabase db;

        public MongoDBRepo()
        {
            var client = new MongoClient();      // connectionstring goes in here
            db = client.GetDatabase("Nello");
        }

        public DeleteResult Delete<T>(string table, string id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("id", id);
            return collection.DeleteOne(filter);
        }

        public IList<T> GetAll<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
        }

        public T GetById<T>(string table, string id)
        {
            try
            {
                var collection = db.GetCollection<T>(table);
                var filter = Builders<T>.Filter.Eq("_id", id);
                return collection.Find(filter).First();
            }
            catch (InvalidOperationException)
            {
                //Debug.WriteLine(id + " Not found");
                return default;
            }
       }

        public bool Insert<T>(string table, T record)
        {

            try
            {
                var collection = db.GetCollection<T>(table);
                 collection.InsertOne(record);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("InsertOne threw the following exception: " + e);
            }
            return false;
        }

        public void InsertMany<T>(string table, IEnumerable<T> records)
        {
            if (records.Count() > 0)
            {
                try
                {
                    var collection = db.GetCollection<T>(table);
                    collection.InsertMany(records);
                }
                catch (MongoBulkWriteException)
                {

                    Debug.WriteLine("Insert Many: Duplicate Id detected");
                }
            }
            else
            {
                Debug.WriteLine("no records to insert");
            }
        }

        // e.g SearchFor(hamster => hamster.Name == "Bar")
        public IList<T> SearchFor<T>(string table, Expression<Func<T, bool>> predicate)
        {
            var collection = db.GetCollection<T>(table);
            return collection
                .AsQueryable<T>()
                    .Where(predicate)
                        .ToList();
        }

        public IList<T> Query<T>(string table, FilterDefinition<T> filters)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find<T>(filters).ToList();
        }

        public bool Upsert<T>(string table, string id, T record)
        {
            var collection = db.GetCollection<T>(table);

            var result = collection.ReplaceOne(
                new BsonDocument("_id", id),
                record,
                new ReplaceOptions { IsUpsert = true });

            return result.IsAcknowledged;
        }

    }
}

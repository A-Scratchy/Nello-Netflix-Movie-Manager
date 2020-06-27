using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nello.Data.Interfaces
{
    public interface IMongoDBRepo
    {
        DeleteResult Delete<T>(string table, string id);
        IList<T> GetAll<T>(string table);
        T GetById<T>(string table, string id);
        bool Insert<T>(string table, T record);
        void InsertMany<T>(string table, IEnumerable<T> records);
        IList<T> SearchFor<T>(string table, Expression<Func<T, bool>> predicate);
        bool Upsert<T>(string table, string id, T record);
        IList<T> Query<T>(string table, FilterDefinition<T> filters);
    }
}
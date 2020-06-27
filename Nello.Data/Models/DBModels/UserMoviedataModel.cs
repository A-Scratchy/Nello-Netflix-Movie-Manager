using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Nello.Data.Models.DBModels
{
    public class UserMoviedataModel
    {

        public UserMoviedataModel() { }

        public UserMoviedataModel(string movieId, int userId, bool seen)
        {
            UserId = userId;
            Id = movieId;
            Seen = seen;
            UserCatalogs = new List<string>();
        }

        [BsonId]
        [BsonRequired]
        public string Id { get; set; }

        public int UserId { get; set; }

        public bool Seen { get; set; }

        public ICollection<string> UserCatalogs { get; set; }
    }
}

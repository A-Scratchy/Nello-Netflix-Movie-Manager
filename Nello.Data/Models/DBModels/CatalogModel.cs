using MongoDB.Bson.Serialization.Attributes;
using Nello.Data.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Nello.Data.Models.DBModels
{
    public class CatalogModel
    {
        public CatalogModel()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            Movies = new List<string>();
        }

        public CatalogModel(int ownerUserId, string name, string privacyLevel)
        {
            Id = Guid.NewGuid().ToString();
            this.DateCreated = DateTime.Now;
            Movies = new List<string>();
            Name = name;
            OwnerUserId = ownerUserId;
            PrivacyLevel = (PrivacyLevels)Enum.Parse(typeof(PrivacyLevels), privacyLevel, true);
            DateCreated = DateTime.Now;
        }

        [BsonId]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("movies")]
        public ICollection<string> Movies { get; set; }

        [JsonProperty("privacyLevel")]
        public PrivacyLevels PrivacyLevel { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public DateTime? DateCreated { get; set; }

        [JsonRequired]
        [JsonProperty("ownerUserId")]
        public int OwnerUserId { get; set; }

        public IEnumerable<int> ReadOnlySubscribers { get; set; }

        public IEnumerable<int> WriteSubscribers { get; set; }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }
}

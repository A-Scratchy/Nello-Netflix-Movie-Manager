using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nello_Data.Models
{
    public class CatalogModel : TableEntity
    {
        public CatalogModel(int ownerUserId, string privacyLevel, string name)
        {
            Id = Guid.NewGuid();
            this.DateCreated = DateTime.Now;
            PartitionKey = ownerUserId.ToString();
            PrivacyLevel = privacyLevel;
            RowKey = Id.ToString();
            Movies = new MovieIdCollection();
            Name = name;
            OwnerUserId = ownerUserId;
        }

        public CatalogModel()
        {
        }

        public Guid Id { get; set; }

        public MovieIdCollection Movies { get; set; }

        public string MoviesJson
        {
            get { return JsonConvert.SerializeObject(Movies); }
            set { Movies = JsonConvert.DeserializeObject<MovieIdCollection>(value); }
        }

        public string PrivacyLevel { get; set; }

        public string Name { get; set; }

        public DateTime? DateCreated { get; set; }

        public int OwnerUserId { get; set; }
    }
}

using System;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Text;

namespace Nello_Data.Models
{
    public class MetadataModel : TableEntity
    {
        public DateTime? LastUpdatedMoviesDB { get; set; }
    }
}

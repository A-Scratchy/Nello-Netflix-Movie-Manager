using Nello.Data.Enums;
using Nello.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nello.Data.Domain
{
    public class FilterModel
    {
        public bool IncludeSeen { get; set; } = true;
        public double MinRating { get; set; } = 0;
        public int MaxRuntime { get; set; } = 50000;
        public ICollection<Genres> Genres { get; set; } = new List<Genres>();
        public IEnumerable<int> Catalogs { get; set; } = new List<int>();
        public string Keyword { get; set; } = "";
    }

}

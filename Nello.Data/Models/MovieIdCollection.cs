
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nello_Data.Models
{
    public class MovieIdCollection : Dictionary<string, string>
    {

        public MovieIdCollection()
        {

        }

        public ICollection MovieId
        {
            get
            {
                return Keys;
            }
        }

        public ICollection MovieYear
        {
            get
            {
                return Values;
            }
        }

        public ICollection RowKey
        {
            get
            {
                return Keys;
            }
        }

        public ICollection PartitionKey
        {
            get
            {
                return Values;
            }
        }

        public void AddMovieId(int id, int year)
        {
            Add(id.ToString(), year.ToString());
        }

        public bool ContainsMovieId(int id)
        {
            return (ContainsKey(id.ToString()));
        }

        public void RemoveMovieId(int id)
        {
            Remove(id.ToString());
        }

    }
}

using System.Collections.Generic;

namespace StarWarsDatabankApi.Models
{
    public class Movie
    {
        public long id { get; set; }
        public string name { get; set; }
        public string director { get; set; }
        public string rating { get; set; }
        public string runtime { get; set; }
        public IEnumerable<string> writers { get; set; }
        public IEnumerable<string> stars { get; set; }
        public long released { get; set; }
        public IEnumerable<string> tags { get; set; }
    }
}
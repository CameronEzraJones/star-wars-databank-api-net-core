using System.Collections.Generic;

namespace StarWarsDatabankApi.Models
{
    public class TvShow
    {
        public long id { get; set; }
        public string name { get; set; }
        public int seasons { get; set; }
        public IEnumerable<string> creators { get; set; }
        public int number_episodes { get; set; }
        public string rating { get; set; }
        public string runtime { get; set; }
        public int show_start { get; set; }
        public int show_end { get; set; }
        public IEnumerable<string> stars { get; set; }
        public IEnumerable<string> tags { get; set; }
    }
}
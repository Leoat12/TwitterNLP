using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterNLP
{
    class Location
    {
        public long TweetId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string PlaceName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TwitterNLP
{
    public class Tweet
    {
        public long Id { get; set; }
        public long CreatedById { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Language { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        public string toJson(){
            return JsonConvert.SerializeObject(this);
        }
    }
}
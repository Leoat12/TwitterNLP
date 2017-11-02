using System.Collections.Generic;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Tweetinvi.Streaming.Parameters;

namespace TwitterNLP{
    public class Properties{
        public string consumerKey {get; set;}
        public string consumerSecret { get; set; }
        public string userAccessToken {get; set;}
        public string userAcessSecret { get; set; }
        public List<string> profilesToSearch {get; set;}
        public Coordinates boundingBoxBottomLeft {get; set;}
        public Coordinates boundingBoxTopRight { get; set; }
        public StreamFilterLevel filterLevel {get; set;}
        public int jsonCache { get; set; }
        public int timeLimit {get; set;}
        public int tweetCountLimit { get; set; }
        public string dbCommunityString {get; set;}
    }
}
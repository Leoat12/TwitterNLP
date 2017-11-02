using System.Collections.Generic;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Tweetinvi.Streaming.Parameters;
using Newtonsoft.Json;

namespace TwitterNLP{
    public class Properties{
        public string consumerKey { get; set; }
        public string consumerSecret { get; set; }
        public string userAccessToken { get; set; }
        public string userAcessSecret { get; set; }
        public List<string> profilesToSearch { get; set; }
        public Coordinates boundingBoxBottomLeft { get; set; }
        public Coordinates boundingBoxTopRight { get; set; }
        public StreamFilterLevel filterLevel { get; set; }
        public int jsonCache { get; set; }
        public int timeLimit { get; set; }
        public int tweetCountLimit { get; set; }
        public string dbCommunityString { get; set; }
        public bool autoMode { get; set;}
        public bool useSearch{ get; set;}
        
        public Properties(){}
        public Properties(string path){
            string json = System.IO.File.ReadAllText(path);
            Properties props = JsonConvert.DeserializeObject<Properties>(json);
            consumerKey = props.consumerKey;
            consumerSecret = props.consumerSecret;
            userAccessToken = props.userAccessToken;
            userAcessSecret = props.userAcessSecret;
            profilesToSearch = props.profilesToSearch;
            boundingBoxBottomLeft = props.boundingBoxBottomLeft;
            boundingBoxTopRight = props.boundingBoxTopRight;
            filterLevel = props.filterLevel;
            jsonCache = props.jsonCache;
            timeLimit = props.timeLimit;
            tweetCountLimit = props.tweetCountLimit;
            dbCommunityString = props.dbCommunityString;
            autoMode = props.autoMode;
            useSearch = props.useSearch;
        }
    }
}
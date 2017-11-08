using System.Collections.Generic;
using System;
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
        public Coordinates boundingBoxBottomLeft { get; set; }
        public Coordinates boundingBoxTopRight { get; set; }
        public StreamFilterLevel filterLevel { get; set; }
        public int jsonCache { get; set; }
        public int timeLimit { get; set; }
        public int tweetCountLimit { get; set; }
        public string dbCommunityString { get; set; }
        public int dbInsertSleepTime{ get; set; }
        
        public Properties(){}
        public Properties(string path){
            string json = System.IO.File.ReadAllText(path);
            Properties props = JsonConvert.DeserializeObject<Properties>(json);
            consumerKey = props.consumerKey;
            consumerSecret = props.consumerSecret;
            userAccessToken = props.userAccessToken;
            userAcessSecret = props.userAcessSecret;
            boundingBoxBottomLeft = props.boundingBoxBottomLeft;
            boundingBoxTopRight = props.boundingBoxTopRight;
            filterLevel = props.filterLevel;
            jsonCache = props.jsonCache;
            timeLimit = props.timeLimit;
            tweetCountLimit = props.tweetCountLimit;
            dbCommunityString = props.dbCommunityString;
            dbInsertSleepTime = props.dbInsertSleepTime;
        }

        public static void buildPropertiesFile(){
            Properties props = new Properties();
            System.Console.WriteLine("Prencha as opções abaixo. Caso não se aplique deixe em branco e aperte Enter");
            
            System.Console.WriteLine("consumerKey\n");
            string consumerKey = Console.ReadLine();
            
            System.Console.WriteLine("consumerSecret\n");
            string consumerSecret = Console.ReadLine();
            
            System.Console.WriteLine("userAccessToken\n");
            string userAccessToken = Console.ReadLine();
            
            System.Console.WriteLine("userAccessSecret\n");
            string userAccessSecret = Console.ReadLine();
            
            System.Console.WriteLine("boundingBoxBottomLeft (duas coordenadas separadas por vírgulas)\n");
            string boundingBoxBottomLeft = Console.ReadLine();
            
            System.Console.WriteLine("boundingBoxTopRight (duas coordenadas separadas por vírgulas)\n");
            string boundingBoxTopRight = Console.ReadLine();
            
            System.Console.WriteLine("filterLevel (Low, Medium ou None. Default: None)\n");
            string filterLevel = Console.ReadLine();
            
            System.Console.WriteLine("jsonCache (Default: 100) \n");
            string jsonCache = Console.ReadLine();
            
            System.Console.WriteLine("timeLimit (Default: 1 hora)\n");
            string timeLimit = Console.ReadLine();
            
            System.Console.WriteLine("tweetCountLimit (Default: 100)\n");
            string tweetCountLimit = Console.ReadLine();
            
            System.Console.WriteLine("dbCommunityString\n");
            string dbCommunityString = Console.ReadLine();

            System.Console.WriteLine("dbInsertSleepTime");
            string dbInsertSleepTime = Console.ReadLine();
        

            if(!isEmpty(consumerKey)){
                props.consumerKey = consumerKey;
            }
            if(!isEmpty(consumerSecret)){
                props.consumerSecret = consumerSecret;
            }
            if(!isEmpty(userAccessToken)){
                props.userAccessToken = userAccessToken;
            }
            if(!isEmpty(userAccessSecret)){
                props.userAcessSecret = userAccessSecret;
            }
            if(!isEmpty(boundingBoxBottomLeft)){
                string[] bb = boundingBoxBottomLeft.Split(",");
                props.boundingBoxBottomLeft = new Coordinates(Double.Parse(bb[0]), Double.Parse(bb[1]));
            }
            if(!isEmpty(boundingBoxTopRight)){
                string[] bb = boundingBoxTopRight.Split(",");
                props.boundingBoxTopRight = new Coordinates(Double.Parse(bb[0]), Double.Parse(bb[1]));
            }
            if(!isEmpty(filterLevel)){
                if(filterLevel.Equals("low",StringComparison.InvariantCultureIgnoreCase)){
                    props.filterLevel = StreamFilterLevel.Low;
                }
                else if(filterLevel.Equals("medium",StringComparison.InvariantCultureIgnoreCase)){
                    props.filterLevel = StreamFilterLevel.Medium;
                }
                else if(filterLevel.Equals("none",StringComparison.InvariantCultureIgnoreCase)){
                    props.filterLevel = StreamFilterLevel.None;
                }
                else{
                    props.filterLevel = StreamFilterLevel.None;
                }
            }
            else{
                props.filterLevel = StreamFilterLevel.None;
            }

            int number;
            if(!isEmpty(jsonCache) && Int32.TryParse(jsonCache, out number)){
                props.jsonCache = number;  
            }
            else{
                props.jsonCache = 100;
            }

            if(!isEmpty(timeLimit) && Int32.TryParse(timeLimit, out number)){
                props.timeLimit = number;  
            }
            else{
                props.timeLimit = 100;
            }

            if(!isEmpty(tweetCountLimit) && Int32.TryParse(tweetCountLimit, out number)){
                props.tweetCountLimit = number;  
            }
            else{
                props.tweetCountLimit = 100;
            }

            if(!isEmpty(dbCommunityString)){
                props.dbCommunityString = dbCommunityString;
            }

            if(!isEmpty(dbInsertSleepTime) && Int32.TryParse(dbInsertSleepTime, out number)){
                props.dbInsertSleepTime = number;
            }
        }

        private static bool isEmpty(string prop){
            if(prop == ""){
                return true;
            }
            else{
                return false;
            }
        }

        private static void SavePropertiesFile(Properties props){
            string json = JsonConvert.SerializeObject(props);
            System.IO.File.WriteAllText(@"data\props.json", json);
        }
    }
}
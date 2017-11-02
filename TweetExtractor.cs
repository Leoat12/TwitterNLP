using System;

using System.Collections.Generic;
using System.Diagnostics;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Tweetinvi.Streaming.Parameters;
using System.IO;
using System.Net.Mail;
using Newtonsoft.Json;

namespace TwitterNLP
{
    public class TweetExtractor
    {

       private readonly Properties props;

       public TweetExtractor(Properties props){
           this.props = props; 
       }

        public void SearchFeatures()
        {
            Auth.SetUserCredentials(props.consumerKey, props.consumerSecret, props.userAccessToken, props.userAcessSecret);

            string path = @"data\TweetsCanalOficial.json";

            string query = BuildSearchQuery();
            var matchingTweets = Search.SearchTweets(query);

            if(matchingTweets != null)
            {
                var list = matchingTweets.GetEnumerator();
                List<Tweet> tweets = new List<Tweet>();
                while (list.MoveNext())
                {
                    Tweet tweet = new Tweet();
                    tweet.Id = list.Current.Id;
                    tweet.CreatedById = list.Current.CreatedBy.Id;
                    tweet.CreatedAt = list.Current.CreatedAt;
                    tweet.Text = list.Current.FullText;
                    tweet.Language = list.Current.Language.ToString();
                    tweets.Add(tweet);
                }
                string newJson = "";
                if (File.Exists(path))
                {
                    string json = System.IO.File.ReadAllText(path);
                    List<Tweet> tweetsJson = JsonConvert.DeserializeObject<List<Tweet>>(json);

                    foreach (Tweet tweet in tweets)
                    {
                        if (tweetsJson.Find(i => i.Id == tweet.Id) == null)
                        {
                            tweetsJson.Add(tweet);
                        }
                    }

                    newJson = JsonConvert.SerializeObject(tweetsJson, Formatting.Indented);
                }
                else
                {
                    newJson = JsonConvert.SerializeObject(tweets, Formatting.Indented);
                }

                System.IO.File.WriteAllText(path, newJson);
            }
            else
            {
                System.Console.WriteLine("API Error or Empty Search");
            }
        }

        public string BuildSearchQuery(){
            List<string> profiles = props.profilesToSearch;
            string query = "";
            for(int i = 0; i < profiles.Count; i++){
                if(i != profiles.Count - 1){
                    query = query + "from:" + profiles[i] + " OR ";
                }
                else{
                    query = query + "from:" + profiles[i];
                }
            }

            return query;
        }

        public void FilteredStreamFeatures()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Auth.SetUserCredentials(props.consumerKey, props.consumerSecret, props.userAccessToken, props.userAcessSecret);

            List<Tweet> tweets = new List<Tweet>();

            int tweet_count = 0;
            int file_number = 0;
            var stream = Tweetinvi.Stream.CreateFilteredStream();
            stream.AddLocation(props.boundingBoxBottomLeft, props.boundingBoxTopRight);
            stream.FilterLevel = props.filterLevel;
            Stopwatch ws = new Stopwatch();
            ws.Start();

            stream.MatchingTweetReceived += (sender, args) =>
            {
                tweet_count++;
                Console.Clear();
                Console.WriteLine(tweet_count);
                Console.WriteLine(ws.Elapsed.Hours + ":" + ws.Elapsed.Minutes);

                Tweet tweet = new Tweet();
                tweet.Id = args.Tweet.Id;
                tweet.CreatedById = args.Tweet.CreatedBy.Id;
                tweet.Text = args.Tweet.FullText;
                tweet.CreatedAt = args.Tweet.CreatedAt;
                tweet.Language = args.Tweet.Language.ToString();
                tweet.Longitude = args.Tweet.Coordinates != null ? (double?)args.Tweet.Coordinates.Longitude : null;
                tweet.Latitude = args.Tweet.Coordinates != null ? (double?)args.Tweet.Coordinates.Latitude : null;
                tweets.Add(tweet);

                if (tweet_count % props.jsonCache == 0)
                {
                    string json = JsonConvert.SerializeObject(tweets, Formatting.Indented);
                    string path = @"data\tweets_" + file_number + ".json";
                    System.IO.File.AppendAllText(path, json);
                    tweets.Clear();
                    file_number++;
                }

                if(props.timeLimit > 0){
                    if (ws.Elapsed.Hours == props.timeLimit)
                    {             
                        string json = JsonConvert.SerializeObject(tweets, Formatting.Indented);
                        string path = @"data\tweets_" + file_number + ".json";
                        System.IO.File.AppendAllText(path, json);
                        tweets.Clear();
                        stream.StopStream();
                    }
                }
                if(props.tweetCountLimit > 0){
                    if (tweet_count > props.tweetCountLimit)
                    {             
                        string json = JsonConvert.SerializeObject(tweets, Formatting.Indented);
                        string path = @"data\tweets_" + file_number + ".json";
                        System.IO.File.AppendAllText(path, json);
                        tweets.Clear();
                        stream.StopStream();
                    }
                }
            };
            stream.StreamStopped += (sender, args) =>
            {
                var exceptionThatCausedTheStreamToStop = args.Exception;
                var twitterDisconnectMessage = args.DisconnectMessage;
                System.IO.File.AppendAllText(@"data\log.txt", args.Exception.Message + "   " + args.DisconnectMessage);
                Console.WriteLine(exceptionThatCausedTheStreamToStop);
                Console.WriteLine(twitterDisconnectMessage);
            };
            Console.WriteLine("Stream started.");
            stream.StartStreamMatchingAllConditions();
            
            System.IO.File.AppendAllText(@"data\log.txt", DateTime.Now.ToString() + " | " + "Tweets: " + tweet_count + " | " + ws.Elapsed.ToString());
        }  
    }
}
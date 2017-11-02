using System;
using System.Threading.Tasks;
using System.Threading;
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
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Started");

            //SearchFeatures();
            // Task tweetStream = Task.Factory.StartNew(() => FilteredStreamFeatures());
            // Task insertToDB = Task.Factory.StartNew(() => InsertToMySQLFromFile());
            // Task.WaitAll(tweetStream, insertToDB);

            Console.WriteLine("Done");
            
            return;
        }

        static void AuthUsersFeatures()
        {
            var appCredentials = new TwitterCredentials("jGiDDapHZqJNkll8fWwfHHPw6", "kMVomZ4XaRL3OeHDWxJSqyc0b0KiQY5ZnaX0UiUWhERJ3y8Szj");

            var authenticationContext = AuthFlow.InitAuthentication(appCredentials);

            Process.Start(authenticationContext.AuthorizationURL);

            System.Console.WriteLine("Please insert the PIN code received on the browser");
            var pinCode = Console.ReadLine();

            var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pinCode, authenticationContext);

            Auth.SetCredentials(userCredentials);

            var authenticatedUser = User.GetAuthenticatedUser();

            // var accountSettings = authenticatedUser.GetAccountSettings();

            string tweets = authenticatedUser.GetHomeTimeline().ToJson();

            System.IO.File.WriteAllText(@"C:\Tweetfile.txt", tweets);

            //System.Console.WriteLine(accountSettings.ScreenName);

            Console.WriteLine("Done");
            System.Console.ReadLine();

        }

        static void UsersFeatures()
        {
            var credentials = Auth.SetUserCredentials("jGiDDapHZqJNkll8fWwfHHPw6", "kMVomZ4XaRL3OeHDWxJSqyc0b0KiQY5ZnaX0UiUWhERJ3y8Szj", "285254601-jogp3oHoUUtynWyQMp1c8IYg83j4zEiPIYsDnX6B", "N6YrrOAFP1SNdxXQZdX4jufRjOgArtvhlPwVmKDuXuijv");

            var user = User.GetUserFromScreenName("Leo_at12");
            var user1Identifier = new UserIdentifier("Leo_at12");
            var user2Identifier = new UserIdentifier("wiru46");

            var relationshipDetails = Friendship.GetRelationshipDetailsBetween(user1Identifier, user2Identifier);

            System.Console.WriteLine(relationshipDetails.Following);
            System.Console.WriteLine(relationshipDetails.FollowedBy);
            System.Console.ReadLine();
        }

        static void SearchFeatures()
        {
            Auth.SetUserCredentials("jGiDDapHZqJNkll8fWwfHHPw6", "kMVomZ4XaRL3OeHDWxJSqyc0b0KiQY5ZnaX0UiUWhERJ3y8Szj",
                        "285254601-jogp3oHoUUtynWyQMp1c8IYg83j4zEiPIYsDnX6B", "N6YrrOAFP1SNdxXQZdX4jufRjOgArtvhlPwVmKDuXuijv");

            string path = @".\TweetsCanalOficial.json";

            var matchingTweets = Search.SearchTweets("from:OperacoesRio OR from:LinhaAmarelaRJ");

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

        static void StreamFeatures()
        {
            Auth.SetUserCredentials("jGiDDapHZqJNkll8fWwfHHPw6", "kMVomZ4XaRL3OeHDWxJSqyc0b0KiQY5ZnaX0UiUWhERJ3y8Szj", "285254601-jogp3oHoUUtynWyQMp1c8IYg83j4zEiPIYsDnX6B", "N6YrrOAFP1SNdxXQZdX4jufRjOgArtvhlPwVmKDuXuijv");

            var stream = Tweetinvi.Stream.CreateUserStream();
            stream.TweetCreatedByMe += (sender, args) =>
            {
                Console.WriteLine(args.Tweet.FullText);
            };
            Console.WriteLine("Stream began.");
            stream.StartStream();
        }

        static void FilteredStreamFeatures()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Auth.SetUserCredentials("jGiDDapHZqJNkll8fWwfHHPw6", "kMVomZ4XaRL3OeHDWxJSqyc0b0KiQY5ZnaX0UiUWhERJ3y8Szj", "285254601-jogp3oHoUUtynWyQMp1c8IYg83j4zEiPIYsDnX6B", "N6YrrOAFP1SNdxXQZdX4jufRjOgArtvhlPwVmKDuXuijv");

            List<Tweet> tweets = new List<Tweet>();

            int tweet_count = 0;
            int file_number = 0;
            var stream = Tweetinvi.Stream.CreateFilteredStream();
            stream.AddLocation(new Coordinates(-23.076889, -43.761292), new Coordinates(-22.742306, -43.091125));
            stream.FilterLevel = StreamFilterLevel.None;
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

                if (tweet_count % 5000 == 0)
                {
                    string json = JsonConvert.SerializeObject(tweets, Formatting.Indented);
                    string path = @".\output\tweets_" + file_number + ".json";
                    System.IO.File.AppendAllText(path, json);
                    tweets.Clear();
                    file_number++;
                }

                if (ws.Elapsed.Hours == 15 || tweet_count > 200000)
                {             
                    string json = JsonConvert.SerializeObject(tweets, Formatting.Indented);
                    string path = @".\output\tweets_" + file_number + ".json";
                    System.IO.File.AppendAllText(path, json);
                    tweets.Clear();
                    stream.StopStream();
                }
            };
            stream.StreamStopped += (sender, args) =>
            {
                var exceptionThatCausedTheStreamToStop = args.Exception;
                var twitterDisconnectMessage = args.DisconnectMessage;
                System.IO.File.AppendAllText(@".\log.txt", args.Exception.Message + "   " + args.DisconnectMessage);
                Console.WriteLine(exceptionThatCausedTheStreamToStop);
                Console.WriteLine(twitterDisconnectMessage);
            };
            Console.WriteLine("Stream started.");
            stream.StartStreamMatchingAllConditions();
            
            System.IO.File.AppendAllText(@".\log.txt", DateTime.Now.ToString() + " | " + "Tweets: " + tweet_count + " | " + ws.Elapsed.ToString());
        }

        static void InsertToMySQLFromFile()
        {
            DBConnection dbc = new DBConnection();
            int file_number = 0;
            Stopwatch ws = new Stopwatch();
            ws.Start();

            while (ws.Elapsed.Hours < 15 || Directory.GetFiles(@".\output").Length > 0)
            {
                string path = @".\output\tweets_" + file_number + ".json";
                if (System.IO.File.Exists(path))
                {

                    string json = System.IO.File.ReadAllText(path);
                    List<Tweet> tweets = JsonConvert.DeserializeObject<List<Tweet>>(json);
                    List<Tweet> cleanedtweets = new List<Tweet>();
                    foreach (Tweet tweet in tweets)
                    {
                        tweet.Text = TweetFormatter.TreatTweet(tweet.Text);
                        cleanedtweets.Add(tweet);
                    }

                    if (dbc.BulkyInsert(cleanedtweets))
                    {
                        file_number++;
                        System.IO.File.Delete(path);
                    }
                }
                else
                {
                    TimeSpan ts = new TimeSpan(0, 30, 0);
                    Thread.Sleep(ts);
                }
            }
        }
    }
}

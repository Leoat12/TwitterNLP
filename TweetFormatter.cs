using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;


namespace TwitterNLP
{
    public class TweetFormatter
    {
        public static List<string> StripURLFromText(List<string> lines)
        {
            List<string> cleanLine = new List<string>();
            foreach (string line in lines)
            {
                string cleanedLine = Regex.Replace(line, @"http[^\s]+", "");
                cleanLine.Add(cleanedLine);
            }

            return cleanLine;
        }

        public static string StripURLFromText(string line)
        {
            string cleanedLine = Regex.Replace(line, @"http[^\s]+", "");
            return cleanedLine;
        }

        public static List<string> StripEmoticons(List<string> lines)
        {
            List<string> cleanLines = new List<string>();

            foreach (string line in lines)
            {
                string cleanedLine = Regex.Replace(line, @"[^\u0000-\u007F\u00C0-\u00FF]+", "");
                cleanLines.Add(cleanedLine);
            }

            return cleanLines;
        }

        public static string StripEmoticons(string line)
        {
            string cleanedLine = Regex.Replace(line, @"[^\u0000-\u007F\u00C0-\u00FF]+", "");
            return cleanedLine;
        }

        public static string TreatTweet(string line)
        {
            line = StripURLFromText(line);
            line = StripEmoticons(line);
            return line;
        }

        public static List<string> RemoveEmptyLines(List<string> lines)
        {
            List<string> cleanLines = new List<string>();
            int i = 0;
            foreach (string line in lines)
            {
                if (!line.Equals("") && !line.Equals(".") && !line.Equals(" "))
                {
                    cleanLines.Add(line);
                }
                else
                {
                    i++;
                    Console.WriteLine(i);
                }

            }

            return cleanLines;
        }

        public static List<string> ConvertList(string[] lines)
        {
            List<string> TempLines = new List<string>();

            foreach (string line in lines)
            {
                TempLines.Add(line);
            }

            return TempLines;
        }

        public static List<string> ReadCSV(string filepath)
        {
            List<string> tweets = new List<string>();

            using (var reader = new StreamReader(filepath))
            {
                int i = 0;

                while (!reader.EndOfStream)
                {
                    if (i == 0)
                    {
                        i++;
                        continue;
                    }

                    var line = reader.ReadLine();
                    var values = line.Split('<');

                    string tweet = values[2].Replace("\"", "");
                    tweet = StripURLFromText(tweet);
                    tweet = StripEmoticons(tweet);

                    values[2] = tweet;

                    tweet = values[0] + "<>" + values[1] + "<>" + "\"" + values[2] + "\"" + "<>" + values[3] + "<>" + values[4] + "<>" + values[5];

                    if (!tweet.Equals("") && !tweet.Equals(".") && !tweet.Equals(" "))
                    {
                        tweets.Add(tweet);
                    }

                    Console.WriteLine(i);
                    i++;
                }
            }

            return tweets;
        }
    }
}

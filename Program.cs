using System;
using System.Threading.Tasks;
using System.Threading;

namespace TwitterNLP
{
    class Program
    {
        static private Properties props;
        static void Main(string[] args)
        {
            System.Console.WriteLine("Started");
            props = new Properties(@"data\props_teste.json");
            TweetExtractor te = new TweetExtractor(props);
            DBConnection dc = new DBConnection(props.dbCommunityString, props.timeLimit);

            te.SearchFeatures();
            Task tweetStream = Task.Factory.StartNew(() => te.FilteredStreamFeatures());
            Task insertToDB = Task.Factory.StartNew(() => dc.InsertToMySQLFromFile());
            Task.WaitAll(tweetStream, insertToDB);

            te.SearchFeatures();

            Console.WriteLine("Done");
            
            return;
        }
    }
}

using System;
using System.Data.SqlClient;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Newtonsoft.Json;

namespace TwitterNLP
{
    class DBConnection
    {
        private string connectionstringMySQL;

        public DBConnection()
        {
            connectionstringMySQL = "server=devtestdb.ccj3d9slsftz.us-east-2.rds.amazonaws.com;user=leoat12;database=tweetsdb;port=3306;password=2glo1gg4";
        }

        public bool AddToMySQLDB(Tweet tweet)
        {
            MySqlConnection conn = new MySqlConnection(connectionstringMySQL);

            try
            {
                if (!ExistsEntry(tweet.Id))
                {
                    conn.Open();
                    string query = "insert into tweet(tweetid, createdbyid, body, createdat, latitude, longitude)" +
                                    " values (@id, @createdbyid, @body, @createdat, @latitude, @longitude)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("id", tweet.Id);
                    cmd.Parameters.AddWithValue("createdbyid", tweet.CreatedById);
                    cmd.Parameters.AddWithValue("body", tweet.Text);
                    cmd.Parameters.AddWithValue("createdat", tweet.CreatedAt);
                    cmd.Parameters.AddWithValue("latitude", tweet.Latitude);
                    cmd.Parameters.AddWithValue("longitude", tweet.Longitude);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                System.IO.File.AppendAllText(@".\log.txt", ex.ToString());
                conn.Close();
                return false;
            }

            conn.Close();
            return true;
        }

        public bool ExistsEntry(long Id)
        {
            MySqlConnection conn = new MySqlConnection(connectionstringMySQL);

            try
            {
                conn.Open();

                string query = "select * from tweet where tweetid = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("id", Id);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    conn.Close();
                    return true;
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erro no banco de dados: " + e.Message);
                System.IO.File.AppendAllText(@".\log.txt", e.ToString());
                conn.Close();
            }

            conn.Close();

            return false;
        }

        public bool BulkyInsert(List<Tweet> tweets)
        {
            StringBuilder commandCord = new StringBuilder("insert ignore into tweet(tweetid, createdbyid, body, createdat, latitude, longitude) values ");
            StringBuilder commandNoCord = new StringBuilder("insert ignore into tweet(tweetid, createdbyid, body, createdat) values ");
            using (MySqlConnection conn = new MySqlConnection(connectionstringMySQL))
            {
                List<string> rowsCords = new List<string>();
                List<string> rowsNoCord = new List<string>();
                foreach (Tweet tweet in tweets)
                {
                    if (tweet.Latitude != null)
                    {
                        rowsCords.Add(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}')", tweet.Id, tweet.CreatedById,
                            MySqlHelper.EscapeString(tweet.Text), tweet.CreatedAt.ToString("yyyy-MM-dd H:mm:ss"), tweet.Latitude, tweet.Longitude));
                    }
                    else
                    {
                        rowsNoCord.Add(string.Format("('{0}','{1}','{2}','{3}')", tweet.Id, tweet.CreatedById,
                            MySqlHelper.EscapeString(tweet.Text), tweet.CreatedAt.ToString("yyyy-MM-dd H:mm:ss")));
                    }
                }
                commandCord.Append(string.Join(",", rowsCords));
                commandCord.Append(";");
                commandNoCord.Append(string.Join(",", rowsNoCord));
                commandNoCord.Append(";");

                StringBuilder command = new StringBuilder(commandCord.Append(commandNoCord).ToString());
                conn.Open();
                using (MySqlCommand myCmd = new MySqlCommand(command.ToString(), conn))
                {
                    try
                    {
                        myCmd.CommandType = System.Data.CommandType.Text;
                        myCmd.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
                return true;
            }
        }

        public bool ReadAllTweetsFromDB()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionstringMySQL))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connection Opened");
                    MySqlCommand timeout = new MySqlCommand("set net_write_timeout = 99999; set net_read_timeout = 99999", conn);
                    timeout.ExecuteNonQuery();
                    string query = "select * from tweet";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.CommandTimeout = 0;
                    MySqlDataReader reader = command.ExecuteReader();
                    Console.WriteLine("Query executed");
                    
                    while(reader.Read())
                    {
                        string tweet = reader["tweetid"].ToString() + "<>" + reader["createdbyid"].ToString() + "<>" +
                                reader["body"].ToString() + "<>" + reader["createdat"].ToString() + "<>" +
                                reader["latitude"].ToString() + "<>" + reader["longitude"].ToString() + "\n";
                        System.IO.File.AppendAllText(@"C:\Users\leoat\Desktop\tweets.csv", tweet);
                    }

                    return true;
                }
                catch (MySqlException er)
                {
                    Console.WriteLine("Erro no banco: " + er.Message);
                    return false;
                }
            }
        }

        public bool ReadGivenNumberofTweets(int number)
        {
            using(MySqlConnection conn = new MySqlConnection(connectionstringMySQL))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connection Opened");
                    MySqlCommand timeout = new MySqlCommand("set net_write_timeout = 99999; set net_read_timeout = 99999", conn);
                    timeout.ExecuteNonQuery();
                    string query = "select * from tweet order by id DESC limit @range";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.Parameters.AddWithValue("range", number);
                    command.CommandTimeout = 0;
                    MySqlDataReader reader = command.ExecuteReader();
                    Console.WriteLine("Query Executed");

                    while (reader.Read())
                    {
                        string tweet = reader["tweetid"].ToString() + "<>" + reader["createdbyid"].ToString() + "<>" +
                                reader["body"].ToString() + "<>" + reader["createdat"].ToString() + "<>" +
                                reader["latitude"].ToString() + "<>" + reader["longitude"].ToString() + "\n";
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        System.IO.File.AppendAllText(@".\tweets.csv", tweet);
                    }

                    return true;
                }
                catch(MySqlException ex)
                {
                    Console.WriteLine(ex.InnerException);
                    return false;
                }
            } 
        }

        public bool ReadGivenRange(int start, int end)
        {
            using(MySqlConnection conn = new MySqlConnection(connectionstringMySQL))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connection Opened");
                    MySqlCommand timeout = new MySqlCommand("set net_write_timeout = 99999; set net_read_timeout = 99999", conn);
                    timeout.ExecuteNonQuery();
                    string query = "select * from tweet where id >= @start and id <= @end";
                    MySqlCommand command = new MySqlCommand(query, conn);
                    command.Parameters.AddWithValue("start", start);
                    command.Parameters.AddWithValue("end", end);
                    command.CommandTimeout = 0;
                    MySqlDataReader reader = command.ExecuteReader();
                    Console.WriteLine("Query Executed");

                    List<Tweet> tweets = new List<Tweet>();
                    while (reader.Read())
                    {
                        Tweet tweet = new Tweet();

                        tweet.Id = long.Parse(reader["tweetid"].ToString());
                        tweet.CreatedById = long.Parse(reader["createdbyid"].ToString());
                        tweet.Text = reader["body"].ToString();
                        tweet.CreatedAt = (DateTime)reader["CreatedAt"];
                        tweet.Latitude = !DBNull.Value.Equals(reader["latitude"]) ? (double?)reader["latitude"] : null;
                        tweet.Longitude = !DBNull.Value.Equals(reader["longitude"]) ? (double?)reader["longitude"] : null;
                        
                        tweets.Add(tweet);
                    }

                    String newJson = JsonConvert.SerializeObject(tweets, Formatting.Indented);
                    System.IO.File.WriteAllText(@".\tweets_" + start + "-" + end + ".json", newJson); 
                    Console.WriteLine("Done reading...");
                    return true;
                }
                catch(MySqlException ex)
                {
                    Console.WriteLine(ex.InnerException);
                    return false;
                }
            } 
        }

        public long searchTweet(long tweetid){
            using(MySqlConnection conn = new MySqlConnection(connectionstringMySQL)){
                try{
                        conn.Open();
                        Console.WriteLine("Connection Opened");
                        MySqlCommand timeout = new MySqlCommand("set net_write_timeout = 99999; set net_read_timeout = 99999", conn);
                        timeout.ExecuteNonQuery();
                        string query = "select id from tweet where tweetid = @tweetid";
                        MySqlCommand command = new MySqlCommand(query, conn);
                        command.Parameters.AddWithValue("tweetid", tweetid);
                        command.CommandTimeout = 0;
                        MySqlDataReader reader = command.ExecuteReader();
                        Console.WriteLine("Query Executed");

                        long id = 0;
                        while (reader.Read())
                        {
                            id = (long)reader["id"];                     
                        }
                        return id;                    
                }
                catch(Exception ex){
                    Console.WriteLine(ex.StackTrace);
                    return 0;
                }
            }
        }
    }
}
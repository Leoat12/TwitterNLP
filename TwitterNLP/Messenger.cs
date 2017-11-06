using System;
using RabbitMQ.Client;
using System.Text;

namespace TwitterNLP{
    public class Messenger{
        public void SendTweet(Tweet tweet){

            string tweetJson = tweet.toJson();

            var factory = new ConnectionFactory(){ HostName = "localhost"};
            using(var connection = factory.CreateConnection()){
                using(var channel = connection.CreateModel()){
                    channel.QueueDeclare(queue: "tweet",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    string message = tweetJson;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                        routingKey: "tweet",
                                        basicProperties: null,
                                        body: body);
                }
            }
        }
    }
}
using System;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitterNLP{
    public static class Messenger{
        public static void SendTweet(Tweet tweet, string queue){

            string tweetJson = tweet.toJson();

            var factory = new ConnectionFactory(){ HostName = "localhost"};
            using(var connection = factory.CreateConnection()){
                using(var channel = connection.CreateModel()){
                    channel.QueueDeclare(queue: queue,
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

        public static void SendTweet(string json, string queue){

            var factory = new ConnectionFactory(){ HostName = "localhost"};
            using(var connection = factory.CreateConnection()){
                using(var channel = connection.CreateModel()){
                    channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    string message = json;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                        routingKey: queue,
                                        basicProperties: null,
                                        body: body);
                }
            }
        }
    }
}
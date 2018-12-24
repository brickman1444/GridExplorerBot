﻿using System;
using System.IO;
using System.Collections.Generic;

namespace GridExplorerBot
{
    static class Program
    {
        public static Stream awsLambdaHandler(Stream inputStream)
        {
            Console.WriteLine("starting via lambda");
            Main(new string[0]);
            return inputStream;
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Beginning program");

            InitializeTwitterCredentials();

            int counter = 0;

            while (true)
            {
                using (StreamWriter file = File.CreateText("output.txt") )
                {
                    file.WriteLine(Rooms.list[0].text + "\n" + counter);
                }

                counter++;

                string inputText = Console.ReadLine();
            }
        }

        static void InitializeTwitterCredentials()
        {
            string consumerKey = System.Environment.GetEnvironmentVariable ("twitterConsumerKey");
            string consumerSecret = System.Environment.GetEnvironmentVariable ("twitterConsumerSecret");
            string accessToken = System.Environment.GetEnvironmentVariable ("twitterAccessToken");
            string accessTokenSecret = System.Environment.GetEnvironmentVariable ("twitterAccessTokenSecret");

            if (consumerKey == null)
            {
                using ( StreamReader fs = File.OpenText( "localconfig/twitterKeys.txt" ) )
                {
                    consumerKey = fs.ReadLine();
                    consumerSecret = fs.ReadLine();
                    accessToken = fs.ReadLine();
                    accessTokenSecret = fs.ReadLine();
                }
            }

            Tweetinvi.Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
        }

        static void Tweet( string text )
        {
            Console.WriteLine("Publishing tweet: " + text);
            var tweet = Tweetinvi.Tweet.PublishTweet(text);
        }
    }
}

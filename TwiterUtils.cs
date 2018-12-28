using System;
using System.IO;
using Tweetinvi;

namespace GridExplorerBot
{
    static class TwitterUtils
    {
        public static void InitializeCredentials()
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

        public static void Tweet( string text )
        {
            Console.WriteLine("Publishing tweet: " + text);
            var tweet = Tweetinvi.Tweet.PublishTweet(text);
        }

        public static void RegisterWebHook()
        {
            var task = Webhooks.RegisterWebhookAsync("production",@"https://o1368ky5ac.execute-api.us-east-2.amazonaws.com/default/GridExplorerBotFunction", Auth.Credentials);
            task.Wait();
        }

        public static bool IsChallengeRequest(WebUtils.WebRequest request)
        {
            if (request.httpMethod != "GET")
            {
                return false;
            }

            return true;
        }

        public static string HandleChallengeRequest(WebUtils.WebRequest request)
        {
            return "challenge request response";
        }
    }
}
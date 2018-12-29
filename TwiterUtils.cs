using System;
using System.IO;
using Tweetinvi;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GridExplorerBot
{
    static class TwitterUtils
    {
        static string consumerSecret = "";

        public static void InitializeCredentials()
        {
            string consumerKey = System.Environment.GetEnvironmentVariable ("twitterConsumerKey");
            consumerSecret = System.Environment.GetEnvironmentVariable ("twitterConsumerSecret");
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

        class ChallengeRequestResponse
        {
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            bool isBase64Encoded = false;

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            int statusCode = 200;

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            Dictionary<string,string> headers = new Dictionary<string, string>();
           
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            Dictionary<string,string> multiValueHeaders = new Dictionary<string, string>();

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public string body = "";
        }

        class ChallengeBody
        {
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public string response_token = "";
        }

        public static string HandleChallengeRequest(WebUtils.WebRequest request)
        {
            ChallengeRequestResponse response = new ChallengeRequestResponse();

            ChallengeBody body = new ChallengeBody();

            body.response_token = "sha256=";

            byte[] key = System.Text.Encoding.Unicode.GetBytes(consumerSecret);
            byte[] crc_token = System.Text.Encoding.Unicode.GetBytes(request.queryStringParameters["crc_token"]);
            using (System.Security.Cryptography.HMACSHA256 hmac = new System.Security.Cryptography.HMACSHA256(key))
            {
                body.response_token += System.Convert.ToBase64String(hmac.ComputeHash(crc_token));
            }

            response.body = Newtonsoft.Json.JsonConvert.SerializeObject(body);

            string jsonOut = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            return jsonOut;
        }
    }
}
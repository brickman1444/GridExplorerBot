using System;
using System.IO;
using Tweetinvi;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GridExplorerBot
{
    static class TwitterUtils
    {
        const string gridExplorerBotScreenName = "gridexplorerbot";
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

        public static void SubscribeToAccountActivity()
        {
            var task = Webhooks.SubscribeToAccountActivityEventsAsync("production", Auth.Credentials);
            task.Wait();
        }

        public static bool IsChallengeRequest(WebUtils.WebRequest request)
        {
            if (!request.IsGet())
            {
                return false;
            }

            // TODO: Make this more robust?

            return true;
        }

        class WebRequestResponse
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
            Console.WriteLine("Handling Challenge Request");

            WebRequestResponse response = new WebRequestResponse();

            ChallengeBody body = new ChallengeBody();

            body.response_token = "sha256=";

            byte[] key = System.Text.Encoding.UTF8.GetBytes(consumerSecret);
            byte[] crc_token = System.Text.Encoding.UTF8.GetBytes(request.queryStringParameters["crc_token"]);
            using (System.Security.Cryptography.HMACSHA256 hmac = new System.Security.Cryptography.HMACSHA256(key))
            {
                body.response_token += System.Convert.ToBase64String(hmac.ComputeHash(crc_token));
            }

            response.body = Newtonsoft.Json.JsonConvert.SerializeObject(body);

            string jsonOut = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            return jsonOut;
        }

        public static bool IsAccountActivityRequest(WebUtils.WebRequest request)
        {
            if (!request.IsPost())
            {
                return false;
            }

            if (request.body == "")
            {
                return false;
            }

            return true;
        }

        class TweetCreateEvent
        {
            public string created_at = "";
            public long id = 0;
            public string text = "";
            public long in_reply_to_status_id = 0;
            public string in_reply_to_screen_name = "";
            public string timestamp_ms = "";
        }

        class AccountActivityBody
        {
            public string for_user_id = "";
            public TweetCreateEvent[] tweet_create_events = {};
        }

        public static string HandleAccountActivityRequest(WebUtils.WebRequest request)
        {
            Console.WriteLine("Handling Account Activity Request");

            AccountActivityBody accountActivity = JsonConvert.DeserializeObject<AccountActivityBody>(request.body);

            if (accountActivity.tweet_create_events.Length == 0)
            {
                // No tweets created. 
                Console.WriteLine("No tweets created");

                // TODO: Return valid response?
                return "";
            }

            foreach ( TweetCreateEvent tweetCreateEvent in accountActivity.tweet_create_events)
            {
                if (tweetCreateEvent.in_reply_to_status_id == 0
                || tweetCreateEvent.in_reply_to_screen_name != gridExplorerBotScreenName)
                {
                    Console.WriteLine("Not a reply to grid explorer bot. id: " + tweetCreateEvent.id);
                    continue;
                }

                // TODO: DOn't reply to self

                Tweetinvi.Models.ITweet parentTweet = Tweetinvi.Tweet.GetTweet(tweetCreateEvent.in_reply_to_status_id);

                if (parentTweet.CreatedAt < Program.oldestSupportedData)
                {
                    Console.WriteLine("Parent tweet too old. Save data may not be read in correctly. id: " + tweetCreateEvent.id);
                    continue;
                }

                Game game = new Game();
                game.GenerateFreshGame();
                game.Save();

                Tweetinvi.Models.ITweet newTweet = Tweetinvi.Tweet.PublishTweetInReplyTo(game.Render(), tweetCreateEvent.id);

                Console.WriteLine("Published new tweet. id: " + newTweet.Id);
            }

            WebRequestResponse response = new WebRequestResponse();

            return JsonConvert.SerializeObject(response);
        }
    }
}
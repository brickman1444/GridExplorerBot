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
        const string webHookEnvironmentName = "production";

        public static void InitializeCredentials()
        {
            string consumerKey = System.Environment.GetEnvironmentVariable("twitterConsumerKey");
            string consumerSecret = System.Environment.GetEnvironmentVariable("twitterConsumerSecret");
            string accessToken = System.Environment.GetEnvironmentVariable("twitterAccessToken");
            string accessTokenSecret = System.Environment.GetEnvironmentVariable("twitterAccessTokenSecret");

            if (consumerKey == null)
            {
                using (StreamReader fs = File.OpenText("localconfig/twitterKeys.txt"))
                {
                    consumerKey = fs.ReadLine();
                    consumerSecret = fs.ReadLine();
                    accessToken = fs.ReadLine();
                    accessTokenSecret = fs.ReadLine();
                }
            }

            Tweetinvi.Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
        }

        public static void Tweet(string text)
        {
            Console.WriteLine("Publishing tweet: " + text);
            var tweet = Tweetinvi.Tweet.PublishTweet(text);
        }

        public static void TweetReplyTo(string text, Tweetinvi.Models.ITweet tweet)
        {
            string screenName = tweet.CreatedBy.ScreenName;
            long parentTweetID = tweet.Id;

            Console.WriteLine("Publishing tweet in reply to : " + screenName + " " + parentTweetID);
            Console.WriteLine("Text: " + text);

            string textToPublish = string.Format("@{0} {1}", screenName, text);

            Tweetinvi.Models.ITweet newTweet = Tweetinvi.Tweet.PublishTweetInReplyTo(textToPublish, parentTweetID);

            Console.WriteLine("Published new tweet: " + newTweet.Id);
        }

        public static void RegisterWebHook()
        {
            var task = Webhooks.RegisterWebhookAsync(webHookEnvironmentName, @"https://o1368ky5ac.execute-api.us-east-2.amazonaws.com/default/GridExplorerBotFunction", Auth.Credentials);
            task.Wait();
        }

        public static void SubscribeToAccountActivity()
        {
            var task = Webhooks.SubscribeToAccountActivityEventsAsync(webHookEnvironmentName, Auth.Credentials);
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
            Dictionary<string, string> headers = new Dictionary<string, string>();

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            Dictionary<string, string> multiValueHeaders = new Dictionary<string, string>();

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

            byte[] key = System.Text.Encoding.UTF8.GetBytes(Tweetinvi.Auth.Credentials.ConsumerSecret);
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
            public string id_str = "";
        }

        class AccountActivityBody
        {
            public string for_user_id = "";
            public TweetCreateEvent[] tweet_create_events = { };
        }

        public static string HandleAccountActivityRequest(WebUtils.WebRequest request)
        {
            Console.WriteLine("Handling Account Activity Request");

            Console.WriteLine(request.body);

            AccountActivityBody accountActivity = JsonConvert.DeserializeObject<AccountActivityBody>(request.body);

            if (accountActivity.tweet_create_events.Length == 0)
            {
                // No tweets created. 
                Console.WriteLine("No tweets created");

                // TODO: Return valid response?
                return "";
            }

            foreach (TweetCreateEvent tweetCreateEvent in accountActivity.tweet_create_events)
            {
                long userReplyTweetId = long.Parse(tweetCreateEvent.id_str);
                Tweetinvi.Models.ITweet userTweet = Tweetinvi.Tweet.GetTweet(userReplyTweetId);

                if (!userTweet.InReplyToStatusId.HasValue
                || userTweet.InReplyToScreenName != gridExplorerBotScreenName)
                {
                    Console.WriteLine("Not a reply to grid explorer bot. id: " + tweetCreateEvent.id_str);
                    continue;
                }

                if (userTweet.CreatedBy == null
                || userTweet.CreatedBy.ScreenName == ""
                || userTweet.CreatedBy.ScreenName == gridExplorerBotScreenName)
                {
                    Console.WriteLine("Invalid user info or reply to self. id: " + tweetCreateEvent.id_str);
                    continue;
                }

                string userTextLower = userTweet.Text.ToLower();
                string cleanedUserText = System.Net.WebUtility.HtmlDecode(userTextLower);

                if (Game.MatchesResetCommand(cleanedUserText))
                {
                    string freshGameOutput = Program.StartFreshGame(DateTimeOffset.UtcNow);

                    Tweet(freshGameOutput);
                    continue;
                }

                if (Game.MatchesHelpCommand(cleanedUserText))
                {
                    string commandsListText = Game.GetCommandsList();

                    TweetReplyTo(commandsListText, userTweet);

                    continue;
                }

                Tweetinvi.Models.ITweet parentGridBotTweet = Tweetinvi.Tweet.GetTweet(userTweet.InReplyToStatusId.Value);

                if (parentGridBotTweet.CreatedAt < Program.oldestSupportedData)
                {
                    Console.WriteLine("Parent tweet too old. Save data may not be read in correctly. id: " + tweetCreateEvent.id_str);

                    TweetReplyTo("This tweet is too old to parse correctly.", userTweet);
                    continue;
                }

                string cleanedParentText = System.Net.WebUtility.HtmlDecode(parentGridBotTweet.Text);

                string gameOutput = Program.RunOneTick(cleanedParentText, cleanedUserText, parentGridBotTweet.CreatedAt);

                TweetReplyTo(gameOutput, userTweet);
            }

            WebRequestResponse response = new WebRequestResponse();

            return JsonConvert.SerializeObject(response);
        }

        public static void TestChallengeRequest()
        {
            var task = Webhooks.ChallengeWebhookAsync(webHookEnvironmentName, "16034296", Tweetinvi.Auth.Credentials);
            task.Wait();
        }

        public static void GetListOfSubscriptions()
        {
            Tweetinvi.Models.TwitterCredentials tempCredentials = new Tweetinvi.Models.TwitterCredentials(Tweetinvi.Auth.Credentials.ConsumerKey, Tweetinvi.Auth.Credentials.ConsumerSecret);
            Tweetinvi.Auth.InitializeApplicationOnlyCredentials(tempCredentials, true);

            Tweetinvi.Core.Public.Models.Authentication.ConsumerOnlyCredentials credentials = new Tweetinvi.Core.Public.Models.Authentication.ConsumerOnlyCredentials(tempCredentials.ConsumerKey, tempCredentials.ConsumerSecret)
            {
                ApplicationOnlyBearerToken = tempCredentials.ApplicationOnlyBearerToken
            };

            var task = Webhooks.GetListOfSubscriptionsAsync(webHookEnvironmentName, credentials);
            task.Wait();
        }
    }
}
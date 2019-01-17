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
        const string webHookEnvironmentName = "production2";
        static string webHooksEndpoint = "";

        public static void InitializeCredentials()
        {
            string consumerKey = System.Environment.GetEnvironmentVariable("twitterConsumerKey");
            string consumerSecret = System.Environment.GetEnvironmentVariable("twitterConsumerSecret");
            string accessToken = System.Environment.GetEnvironmentVariable("twitterAccessToken");
            string accessTokenSecret = System.Environment.GetEnvironmentVariable("twitterAccessTokenSecret");
            webHooksEndpoint = System.Environment.GetEnvironmentVariable("webHooksEndpoint");

            if (consumerKey == null)
            {
                using (StreamReader fs = File.OpenText("localconfig/twitterKeys.txt"))
                {
                    consumerKey = fs.ReadLine();
                    consumerSecret = fs.ReadLine();
                    accessToken = fs.ReadLine();
                    accessTokenSecret = fs.ReadLine();
                }
                using (StreamReader fs = File.OpenText("localconfig/webHooksEndpoint.txt"))
                {
                    webHooksEndpoint = fs.ReadLine();
                }
            }

            Tweetinvi.Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);

            RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackOnly;

            TweetinviEvents.QueryBeforeExecute += RateLimitCheck;
        }

        public static void RateLimitCheck(object sender, Tweetinvi.Events.QueryBeforeExecuteEventArgs args)
        {
            var queryRateLimits = RateLimit.GetQueryRateLimit(args.QueryURL);

            if (queryRateLimits == null)
            {
                // Some methods are not RateLimited. Invoking such a method will result in the queryRateLimits to be null
                return;
            }

            if (queryRateLimits.Remaining == 0)
            {
                Console.WriteLine("Insufficient rate limit for " + args.QueryURL + " cancelling query");
                args.Cancel = true;
                return;
            }

            float remainingPercent = (float)queryRateLimits.Remaining / queryRateLimits.Limit;

            if (remainingPercent < 0.5f)
            {
                Console.WriteLine("Approaching rate limit for " + args.QueryURL + " Remaining: " + queryRateLimits.Remaining);
            }
        }

        public static void Tweet(string text)
        {
            Console.WriteLine("Publishing tweet: " + text);
            Tweetinvi.Models.ITweet newTweet = Tweetinvi.Tweet.PublishTweet(text);

            if (newTweet != null)
            {
                Console.WriteLine("Published new tweet: " + newTweet.Id);
            }
            else
            {
                Console.WriteLine("Failed to publish tweet");
            }
        }

        public static void TweetReplyTo(string text, Tweetinvi.Models.ITweet tweet)
        {
            string screenName = tweet.CreatedBy.ScreenName;
            long parentTweetID = tweet.Id;

            Console.WriteLine("Publishing tweet in reply to : " + screenName + " " + parentTweetID);
            Console.WriteLine("Text: " + text);

            string textToPublish = string.Format("@{0} {1}", screenName, text);

            Tweetinvi.Models.ITweet newTweet = Tweetinvi.Tweet.PublishTweetInReplyTo(textToPublish, parentTweetID);

            if (newTweet != null)
            {
                Console.WriteLine("Published new tweet: " + newTweet.Id);
            }
            else
            {
                Console.WriteLine("Failed to publish tweet");
            }
        }

        public static void RegisterWebHook()
        {
            var task = Webhooks.RegisterWebhookAsync(webHookEnvironmentName, webHooksEndpoint, Auth.Credentials);
            task.Wait();
        }

        public static void UnregisterWebHook()
        {
            var task = Webhooks.RemoveWebhookAsync(webHookEnvironmentName, webHooksEndpoint, Auth.Credentials);
            task.Wait();
        }

        public static void SubscribeToAccountActivity()
        {
            var task = Webhooks.SubscribeToAccountActivityEventsAsync(webHookEnvironmentName, Auth.Credentials);
            task.Wait();
        }

        public static void RemoveSubscriptionToAccountActivity()
        {
            var task = Webhooks.RemoveAllAccountSubscriptionsAsync(webHookEnvironmentName, Auth.Credentials);
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

        class AccountActivityBody
        {
            public string for_user_id = "";
            public Newtonsoft.Json.Linq.JObject[] tweet_create_events = { };
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

            string tweetsCreatedJSONString = accountActivity.tweet_create_events.ToJson();
            Tweetinvi.Models.DTO.ITweetDTO[] tweetDTOs = tweetsCreatedJSONString.ConvertJsonTo<Tweetinvi.Models.DTO.ITweetDTO[]>();
            IEnumerable<Tweetinvi.Models.ITweet> userTweets = Tweetinvi.Tweet.GenerateTweetsFromDTO(tweetDTOs);

            foreach (Tweetinvi.Models.ITweet userTweet in userTweets)
            {
                if (!userTweet.InReplyToStatusId.HasValue
                || userTweet.InReplyToScreenName != gridExplorerBotScreenName)
                {
                    Console.WriteLine("Not a reply to grid explorer bot. id: " + userTweet.Id);
                    continue;
                }

                if (userTweet.CreatedBy == null
                || userTweet.CreatedBy.ScreenName == ""
                || userTweet.CreatedBy.ScreenName == gridExplorerBotScreenName)
                {
                    Console.WriteLine("Invalid user info or reply to self. id: " + userTweet.Id);
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
                    Console.WriteLine("Parent tweet too old. Save data may not be read in correctly. id: " + userTweet.Id);

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
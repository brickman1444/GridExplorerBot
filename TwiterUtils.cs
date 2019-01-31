using System;
using System.IO;
using Tweetinvi;
using Tweetinvi.Core.Extensions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GridExplorerBot
{
    static class TwitterUtils
    {
        const string gridExplorerBotScreenName = "gridexplorerbot";
        const string webHookEnvironmentName = "production2";
        const string webHookID = "1085394130304462848";
        static string webHooksEndpoint = "";
        static string consumerSecret = "";

        public static void InitializeCredentials(bool initializeTwitterLibrary = true)
        {
            Console.WriteLine("Initializing Credentials");

            string consumerKey = System.Environment.GetEnvironmentVariable("twitterConsumerKey");
            consumerSecret = System.Environment.GetEnvironmentVariable("twitterConsumerSecret");
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

            if (initializeTwitterLibrary)
            {
                Tweetinvi.Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);

                RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackOnly;

                TweetinviEvents.QueryBeforeExecute += RateLimitCheck;
            }
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

        public static Tweetinvi.Models.ITweet Tweet(string text)
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

            return newTweet;
        }

        public static Tweetinvi.Models.ITweet TweetReplyTo(string text, Tweetinvi.Models.ITweet tweet)
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

            return newTweet;
        }

        public static void TweetChain(string longText, Tweetinvi.Models.ITweet previousTweet)
        {
            Console.WriteLine("Publishing Tweet Chain");
            Console.WriteLine("Text: " + longText);

            var tweetTexts = SplitLinesIntoTweets(longText);

            foreach (string text in tweetTexts)
            {
                if (previousTweet == null)
                {
                    previousTweet = Tweet(text);
                }
                else
                {
                    previousTweet = TweetReplyTo(text, previousTweet);
                }
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

        class ChallengeBody
        {
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public string response_token = "";
        }

        class WebRequestResponse
        {
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public bool isBase64Encoded = false;

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public int statusCode = 200;

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public Dictionary<string, string> headers = new Dictionary<string, string>();

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public Dictionary<string, string> multiValueHeaders = new Dictionary<string, string>();

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public ChallengeBody body = new ChallengeBody();
        }

        public static string HandleChallengeRequest(WebUtils.WebRequest request)
        {
            Console.WriteLine("Handling Challenge Request");

            string hashString = "";

            Debug.Assert(consumerSecret.Length != 0);

            byte[] key = System.Text.Encoding.UTF8.GetBytes(consumerSecret);
            byte[] crc_token = System.Text.Encoding.UTF8.GetBytes(request.queryStringParameters["crc_token"]);
            using (System.Security.Cryptography.HMACSHA256 hmac = new System.Security.Cryptography.HMACSHA256(key))
            {
                hashString = System.Convert.ToBase64String(hmac.ComputeHash(crc_token));
            }

            return WriteChallengeRequestResponse(hashString);
        }

        private static string WriteChallengeRequestResponse(string hashString)
        {
            string response =
            "{\n" +
            "\"isBase64Encoded\": false,\n" +
            "\"statusCode\": 200,\n" +
            "\"headers\": {},\n" +
            "\"multiValueHeaders\": {},\n" +
            "\"body\": \"{\\\"response_token\\\": \\\"sha256=" + hashString + "\\\"}\"\n" +
            "}";

            return response;
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
            public Tweetinvi.Models.DTO.ITweetDTO[] tweet_create_events = null;
        }

        public static string HandleAccountActivityRequest(WebUtils.WebRequest request)
        {
            Console.WriteLine("Handling Account Activity Request");

            Console.WriteLine(request.body);

            AccountActivityBody accountActivity = request.body.ConvertJsonTo<AccountActivityBody>();

            if (accountActivity.tweet_create_events.Length == 0)
            {
                // No tweets created. 
                Console.WriteLine("No tweets created");

                // TODO: Return valid response?
                return "";
            }

            IEnumerable<Tweetinvi.Models.ITweet> userTweets = Tweetinvi.Tweet.GenerateTweetsFromDTO(accountActivity.tweet_create_events);

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

                string userTextLower = userTweet.GetSafeDisplayText().ToLower();
                string cleanedUserText = System.Net.WebUtility.HtmlDecode(userTextLower);

                Console.WriteLine("Cleaned user text: " + cleanedUserText);

                if (Game.MatchesResetCommand(cleanedUserText))
                {
                    string freshGameOutput = Program.StartFreshGame(DateTimeOffset.UtcNow);

                    Tweet(freshGameOutput);
                    continue;
                }

                if (Game.MatchesHelpCommand(cleanedUserText))
                {
                    string commandsListText = Game.GetCommandsList();

                    TweetChain(commandsListText, userTweet);
                    continue;
                }

                Console.WriteLine("Fetching parent tweet");
                Tweetinvi.Models.ITweet parentGridBotTweet = Tweetinvi.Tweet.GetTweet(userTweet.InReplyToStatusId.Value);

                if (parentGridBotTweet.CreatedAt < Program.oldestSupportedData)
                {
                    Console.WriteLine("Parent tweet too old. Save data may not be read in correctly. id: " + userTweet.Id);

                    TweetReplyTo("This tweet is too old to parse correctly.", userTweet);
                    continue;
                }

                string cleanedParentText = System.Net.WebUtility.HtmlDecode(parentGridBotTweet.Text);

                Console.WriteLine("Running one tick");
                string gameOutput = Program.RunOneTick(cleanedParentText, cleanedUserText, parentGridBotTweet.CreatedAt);

                TweetReplyTo(gameOutput, userTweet);
            }

            return WriteAccountActivityResponse();
        }

        private static string WriteAccountActivityResponse()
        {
            string response =
            "{\n" +
            "\"isBase64Encoded\": false,\n" +
            "\"statusCode\": 200,\n" +
            "\"headers\": {},\n" +
            "\"multiValueHeaders\": {},\n" +
            "\"body\": \"\"\n" +
            "}";

            return response;
        }

        private static string GetSafeDisplayText(this Tweetinvi.Models.ITweet tweet)
        {
            return tweet.Text.Substring(tweet.SafeDisplayTextRange[0]);
        }

        public static void TestChallengeRequest()
        {
            var task = Webhooks.ChallengeWebhookAsync(webHookEnvironmentName, webHookID, Tweetinvi.Auth.Credentials);
            task.Wait();
        }

        public static void GetListOfWebhooks()
        {
            Tweetinvi.Models.TwitterCredentials tempCredentials = new Tweetinvi.Models.TwitterCredentials(Tweetinvi.Auth.Credentials.ConsumerKey, Tweetinvi.Auth.Credentials.ConsumerSecret);
            Tweetinvi.Auth.InitializeApplicationOnlyCredentials(tempCredentials, true);

            Tweetinvi.Core.Public.Models.Authentication.ConsumerOnlyCredentials credentials = new Tweetinvi.Core.Public.Models.Authentication.ConsumerOnlyCredentials(tempCredentials.ConsumerKey, tempCredentials.ConsumerSecret)
            {
                ApplicationOnlyBearerToken = tempCredentials.ApplicationOnlyBearerToken
            };

            var task = Webhooks.GetAllWebhookEnvironmentsAsync(credentials);
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

        public static List<string> SplitLinesIntoTweets(string sourceText)
        {
            List<string> lines = new List<string>( sourceText.Split('\n') );

            List<string> outTweets = new List<string>();
            outTweets.Add(lines[0]);
            lines.RemoveAt(0);

            while (lines.Count != 0)
            {
                if ( StringExtension.EstimateTweetLength( outTweets[outTweets.Count - 1] + '\n' + lines[0]) <= 280 )
                {
                    outTweets[outTweets.Count - 1] += '\n' + lines[0];
                    lines.RemoveAt(0);
                }
                else
                {
                    outTweets.Add(lines[0]);
                    lines.RemoveAt(0);
                }
            }

            return outTweets;
        }
    }
}
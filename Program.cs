﻿using System;
using System.IO;
using System.Collections.Generic;

namespace GridExplorerBot
{
    static class Program
    {
        public static DateTimeOffset oldestSupportedData = DateTimeOffset.FromUnixTimeSeconds(1551579928);

        public static Stream awsLambdaHandler(Stream inputStream)
        {
            Console.WriteLine("starting via lambda");

            string input = StringUtils.GetString(inputStream);
            Console.WriteLine("Input:" + input);

            WebUtils.WebRequest request = WebUtils.GetJsonObject(input);

            string response = "default response";
            if (TwitterUtils.IsChallengeRequest(request))
            {
                TwitterUtils.InitializeCredentials(false);
                response = TwitterUtils.HandleChallengeRequest(request);
            }
            else if (TwitterUtils.IsAccountActivityRequest(request))
            {
                TwitterUtils.InitializeCredentials();
                response = TwitterUtils.HandleAccountActivityRequest(request);
            }

            return StringUtils.GetStream(response);
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Beginning program");

            using (StreamReader file = File.OpenText("input.txt"))
            {
                string input = file.ReadToEnd();
                if (input.Length > 0)
                {
                    WebUtils.WebRequest request = WebUtils.GetJsonObject(input);
                    TwitterUtils.HandleAccountActivityRequest(request);
                    return;
                }
            }

            string previousGameText;
            using (StreamReader file = File.OpenText("output.txt"))
            {
                previousGameText = file.ReadToEnd();
            }

            if (previousGameText == "")
            {
                string newGameOutput = StartFreshGame(DateTimeOffset.UtcNow);
                using (StreamWriter file = File.CreateText("output.txt"))
                {
                    file.Write(newGameOutput);
                }
            }

            while (true)
            {
                using (StreamReader file = File.OpenText("output.txt"))
                {
                    previousGameText = file.ReadToEnd();
                }

                string inputText = Console.ReadLine();

                string gameOutput = "";
                if (Game.MatchesResetCommand(inputText))
                {
                    gameOutput = StartFreshGame(DateTimeOffset.UtcNow);
                }
                else if (Game.MatchesHelpCommand(inputText))
                {
                    string commandsList = Game.GetCommandsList();
                    var tweets = TwitterUtils.SplitLinesIntoTweets(commandsList);
                    foreach (string tweet in tweets)
                    {
                        gameOutput += tweet + "\n\n";
                    }
                }
                else
                {
                    gameOutput = RunOneTick(previousGameText, inputText, DateTimeOffset.UtcNow);
                }

                using (StreamWriter file = File.CreateText("output.txt"))
                {
                    file.Write(gameOutput);
                }
            }
        }

        public static string RunOneTick(string previousGameText, string command, DateTimeOffset seedTime)
        {
            Game theGame = new Game();

            Game.InitializeRandom(seedTime);

            bool successfullyParsed = theGame.ParsePreviousText(previousGameText);

            if (!successfullyParsed)
            {
                theGame.GenerateFreshGame();
            }

            theGame.Simulate(command);

            theGame.Save();

            return theGame.Render();
        }

        public static string StartFreshGame(DateTimeOffset seedTime)
        {
            Game game = new Game();

            Game.InitializeRandom(seedTime);

            game.GenerateFreshGame();

            game.Save();

            return game.Render();
        }

        public static bool IsInLikeTemple(string gameText, DateTimeOffset seedTime)
        {
            // The favorite events don't send the full text so don't try to really parse it.

            string likeTempleWall = Emoji.GetEmoji(Objects.ID.PlaceOfWorship);
            return gameText.Contains(likeTempleWall + likeTempleWall + likeTempleWall);
        }
    }
}

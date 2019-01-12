﻿using System;
using System.IO;
using System.Collections.Generic;

namespace GridExplorerBot
{
    static class Program
    {
        public static DateTimeOffset oldestSupportedData = DateTimeOffset.FromUnixTimeSeconds(1546926620);

        public static Stream awsLambdaHandler(Stream inputStream)
        {
            Console.WriteLine("starting via lambda");

            string input = StringUtils.GetString(inputStream);
            Console.WriteLine("Input:" + input);

            WebUtils.WebRequest request = WebUtils.GetJsonObject(input);

            TwitterUtils.InitializeCredentials();

            string response = "default response";
            if (TwitterUtils.IsChallengeRequest(request))
            {
                response = TwitterUtils.HandleChallengeRequest(request);
            }
            else if (TwitterUtils.IsAccountActivityRequest(request))
            {
                response = TwitterUtils.HandleAccountActivityRequest(request);
            }

            return StringUtils.GetStream(response);
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Beginning program");

            TwitterUtils.InitializeCredentials();

            /*string fooText = File.OpenText("input.txt").ReadToEnd();
            WebUtils.WebRequest request = new WebUtils.WebRequest();
            request.body = fooText;
            request.httpMethod = "POST";
            TwitterUtils.HandleAccountActivityRequest(request);*/

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
                if (inputText == "reset")
                {
                    gameOutput = StartFreshGame(DateTimeOffset.UtcNow);
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
    }
}

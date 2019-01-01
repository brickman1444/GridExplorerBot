﻿using System;
using System.IO;
using System.Collections.Generic;

namespace GridExplorerBot
{
    static class Program
    {
        public static DateTime oldestSupportedData = new DateTime(1546383981);

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

            while (true)
            {
                string previousGameText;
                using (StreamReader file = File.OpenText("output.txt"))
                {
                    previousGameText = file.ReadToEnd();
                }

                string inputText = Console.ReadLine();

                string gameOutput = RunOneTick(previousGameText, inputText);

                using (StreamWriter file = File.CreateText("output.txt"))
                {
                    file.Write(gameOutput);
                }
            }
        }

        public static string RunOneTick(string previousGameText, string command)
        {
            Game theGame = new Game();

            bool successfullyParsed = theGame.ParsePreviousText(previousGameText);

            if (!successfullyParsed)
            {
                theGame.GenerateFreshGame();
            }

            theGame.Simulate(command);

            theGame.Save();

            return theGame.Render();
        }

        public static string StartFreshGame()
        {
            Game game = new Game();

            game.GenerateFreshGame();

            game.Save();

            return game.Render();
        }
    }
}

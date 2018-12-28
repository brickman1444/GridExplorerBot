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

            string input = StringUtils.GetString(inputStream);
            Console.WriteLine("Input:" + input);

            WebUtils.WebRequest request = WebUtils.GetJsonObject(input);

            string response = "default response";
            if ( TwitterUtils.IsChallengeRequest(request))
            {
                response = TwitterUtils.HandleChallengeRequest(request);
            }

            return StringUtils.GetStream(response);
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Beginning program");

            TwitterUtils.InitializeCredentials();
            TwitterUtils.RegisterWebHook();

            while (true)
            {
                string previousGameText;
                using (StreamReader file = File.OpenText("output.txt") )
                {
                    previousGameText = file.ReadToEnd();
                }

                Game theGame = new Game();

                bool successfullyParsed = theGame.ParsePreviousText(previousGameText);

                if (!successfullyParsed)
                {
                    theGame.GenerateFreshGame();
                }

                string inputText = Console.ReadLine();

                theGame.Simulate(inputText);

                theGame.Save();

                using (StreamWriter file = File.CreateText("output.txt") )
                {
                    string output = theGame.Render();
                    file.Write(output);
                    //TwitterUtils.Tweet(output);
                }
            }
        }
    }
}

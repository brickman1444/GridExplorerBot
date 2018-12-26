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
            Main(new string[0]);
            return inputStream;
        }

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Beginning program");

            TwitterUtils.InitializeCredentials();

            string previousGameText;

            using (StreamReader file = File.OpenText("output.txt") )
            {
                previousGameText = file.ReadToEnd();
            }

            Game theGame = new Game();

            theGame.ParsePreviousText(previousGameText);

            while (true)
            {
                using (StreamWriter file = File.CreateText("output.txt") )
                {
                    string output = theGame.Render();
                    file.Write(output);
                    //TwitterUtils.Tweet(output);
                }

                string inputText = Console.ReadLine();

                theGame.Simulate(inputText);
            }
        }
    }
}

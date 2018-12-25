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

            string commandResponse = "";

            int counter = 0;

            while (true)
            {
                using (StreamWriter file = File.CreateText("output.txt") )
                {
                    string output = commandResponse + '\n' + Rooms.TheRoom.Render() + counter;
                    file.WriteLine(output);
                    //TwitterUtils.Tweet(output);
                }

                counter++;

                string inputText = Console.ReadLine();

                commandResponse = Rooms.TheRoom.HandleCommand(inputText);
            }
        }
    }
}

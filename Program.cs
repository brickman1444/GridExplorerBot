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

            string outputText = "";

            int counter = 0;

            while (true)
            {
                using (StreamWriter file = File.CreateText("output.txt") )
                {
                    file.WriteLine(outputText + '\n' + Rooms.TheRoom.Render() + counter);
                }

                counter++;

                string inputText = Console.ReadLine();

                outputText = Rooms.TheRoom.HandleCommand(inputText);
            }
        }
    }
}

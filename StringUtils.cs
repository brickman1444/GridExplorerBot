using System.IO;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace GridExplorerBot
{
    public static class StringUtils
    {
        public static List<string> SplitEmojiString(string inputText)
        {
            List<string> outList = new List<string>();

            System.Globalization.TextElementEnumerator enumerator = System.Globalization.StringInfo.GetTextElementEnumerator(inputText);

            while (enumerator.MoveNext())
            {
                outList.Add(enumerator.GetTextElement());
            }

            return outList;
        }

        public static string GetString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static MemoryStream GetStream(string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        public static string SaveDataEncode(byte[] bytes)
        {
            return Cromulent.Encoding.Z85.ToZ85String(bytes, true);
        }

        public static byte[] SaveDataDecode(string saveData)
        {
            Debug.Assert(saveData.Length % 5 == 0, "Save data string length not a multiple of five " + saveData.Length + " [" + saveData + "]");

            return Cromulent.Encoding.Z85.FromZ85String(saveData);
        }

        public static IEnumerable<string> GetAllGenderAndSkinToneVariants(string baseEmoji)
        {
            // Append all variants of skin tone and gender characters

            string[] skinTones = {"🏻","🏼","🏽","🏾","🏿"};
            string zeroWidthJoiner = "‍";
            string[] genders = {"♀️","♂️"};

            List<string> outStrings = new List<string>();

            foreach ( string skinTone in skinTones )
            {
                foreach ( string gender in genders )
                {
                    outStrings.Add( baseEmoji + skinTone + zeroWidthJoiner + gender );
                }
            }

            return outStrings;
        }

        public static int GetNumPersonEmojiVariations()
        {
            return 10;
        }

        public static string RemoveTweetMentions(string text)
        {
            var stringMatches = System.Text.RegularExpressions.Regex.Match(text, @"^(?<prefix>(?:(?<mention>@[a-zA-Z0-9_]+)\s){0,50})?(?<content>.+)");

            var prefix = stringMatches.Groups["prefix"];
            var content = stringMatches.Groups["content"];

            string Prefix = prefix.Value;
            string Content = content.Value;

            var mentionCaptures = stringMatches.Groups["mention"].Captures;

            var mentions = new List<string>();
            foreach (var mention in mentionCaptures)
            {
                mentions.Add(mention.ToString());
            }

            var Mentions = mentions.ToArray();

            return Content;
        }
    }
}
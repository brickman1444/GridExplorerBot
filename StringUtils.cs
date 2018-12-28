using System.IO;
using System.Collections.Generic;

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
    }
}
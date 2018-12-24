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
    }
}
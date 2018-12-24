using System.Collections.Generic;

namespace GridExplorerBot
{
    public static class StringUtils
    {
        public static List<string> SplitEmojiString(string inputText)
        {
            List<string> outList = new List<string>();

            for (int i = 0; i < inputText.Length - 1; i++ )
            {
                outList.Add( new System.Globalization.StringInfo(inputText).SubstringByTextElements(i, 1));
            }

            return outList;
        }
    }
}
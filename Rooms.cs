using System.Collections.Generic;

namespace GridExplorerBot
{
    public class Room
    {
        public readonly string text;

        Objects.ID[,] grid = new Objects.ID[10,10];

        public Room( string inText )
        {
            text = inText;

            string[] lines = text.Split('\n');

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                List<string> splitLine = StringUtils.SplitEmojiString(lines[lineIndex]);

                for (int columnIndex = 0; columnIndex < splitLine.Count; columnIndex++)
                {
                    grid[lineIndex,columnIndex] = Emoji.GetID(splitLine[columnIndex]);
                }
            }
        }
    }

    static class Rooms
    {
        public static Room[] list = {
            new Room("⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜😀⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛"),
        };
    }
}
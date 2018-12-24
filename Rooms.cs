namespace GridExplorerBot
{
    public class Room
    {
        public readonly string text;

        public Room( string inText )
        {
            text = inText;
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
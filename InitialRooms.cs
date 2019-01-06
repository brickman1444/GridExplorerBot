namespace GridExplorerBot
{
    static class InitialRooms
    {
        public static bool IsValidInitialRoomIndex(int index)
        {
            return index >= 0 && index < initialRooms.Length;
        }

        public static Room[] initialRooms = {
            new Room( new string[] {
                   "⬛⬛⬛⬛⬛⬛⬛⬛⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬛⬛⬛⬜⬛⬛⬛⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬛⬛⬛⬛⬛⬛⬛⬛" },
                   new DynamicObjectSetup[] {
                       new DynamicObjectSetup(Emoji.Player.Default, new Point(5,5)),
                       new DynamicObjectSetup(Emoji.Elephant, new Point(1,1)),
                       new DynamicObjectSetup(Emoji.Pen, new Point(4,4)),
                       new DynamicObjectSetup(Emoji.Environment.LockedWithPen, new Point(3,4)),
                       } ),
            new Room( new string[] {
                    "⬛⬛⬛⬛⬛⬛⬛⬛⬛",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️⬜⬜🍯⬜⬛",
                    "⬛⬛⬛⬛⬛⬛⬛⬛⬛", },
                   new DynamicObjectSetup[] {
                       new DynamicObjectSetup(Emoji.Player.Default, new Point(5,5)),
                       } ), };
    }
}
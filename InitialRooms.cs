using System.Collections.Generic;

namespace GridExplorerBot
{
    public static class InitialRooms
    {
        public enum ID
        {
            Unknown,
            Circus,
            VampireCastleCourtyard,
        };

        public static bool IsValidInitialRoomIndex(ID id)
        {
            return initialRooms.ContainsKey(id);
        }

        public static Dictionary<InitialRooms.ID,Room> initialRooms = new Dictionary<InitialRooms.ID, Room>()
        {
            [ID.Circus] = new Room( new string[] {
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
            [ID.VampireCastleCourtyard] = new Room( new string[] {
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
                       } ),
        };
    }
}
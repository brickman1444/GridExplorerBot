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
                       new DynamicObjectSetup(Emoji.Animals.Elephant, new Point(1,1)),
                       new DynamicObjectSetup(Emoji.Pen, new Point(4,4)),
                       new LockSetup(Emoji.Environment.LockedWithPen, new Point(3,4)),
                       } ),
            [ID.VampireCastleCourtyard] = new Room( new string[] {
                    "⬛⬛⬛⬛⬛⬛⬛⬛⬛",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬜⬜🕸️🕸️⬜⬜⬜⬜⬜",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️⬜⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️⬜⬜🍯⬜⬛",
                    "⬛⬛⬛⬛⬛⬛⬛⬛⬛", },
                   new DynamicObjectSetup[] {
                       new DynamicObjectSetup(Emoji.Player.Default, new Point(4,7)),
                       new DynamicObjectSetup(Emoji.Environment.Door, new Point(4,8)),
                       new DynamicObjectSetup(Emoji.Environment.Door, new Point(4,0)),
                       new DynamicObjectSetup(Emoji.Animals.Spider, new Point(4,3)),
                       new DynamicObjectSetup(Emoji.Animals.Bee, new Point(6,6)),
                       new DynamicObjectSetup(Emoji.Plants.Rose, new Point(1,6)),
                       } ),
        };
    }
}
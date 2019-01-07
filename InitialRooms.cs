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
            Overworld,
        };

        public static bool IsValidInitialRoomIndex(ID id)
        {
            return initialRooms.ContainsKey(id);
        }

        public static Dictionary<InitialRooms.ID,Room> initialRooms = new Dictionary<InitialRooms.ID, Room>()
        {
            [ID.Circus] = new Room( "The Circus",
                    new string[] {
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
                       new DynamicObjectSetup(Emoji.InventoryItems.Pen, new Point(4,4)),
                       new LockSetup(Emoji.Environment.LockedWithPen, new Point(3,4)),
                       } ),
            [ID.VampireCastleCourtyard] = new Room( "Courtyard of Vampire Castle",
                    new string[] {
                    "⬛⬛⬛⬛⬛⬛⬛⬛⬛",
                    "⬛🥀🥀🕸️🕸️🥀⬜🥀⬛",
                    "⬛⬜⬜🕸️🕸️⬜⬜⬜⬛",
                    "⬛🏺⬜🕸️🕸️⬜⬜⬜⬛",
                    "⬜⬜⬜🕸️🕸️⬜⬜⬜⬜",
                    "⬛🏺⬜🕸️🕸️⬜⬜⬜⬛",
                    "⬛⬜⬜🕸️🕸️⬜⬜⬜⬛",
                    "⬛⬜🕸️🕸️🕸️🕸️🍯⬜⬛",
                    "⬛⬛⬛⬛⬛⬛⬛⬛⬛", },
                   new DynamicObjectSetup[] {
                       new DynamicObjectSetup(Emoji.Player.Default, new Point(4,7)),
                       new DoorSetup(Emoji.Environment.Door, new Point(4,8), InitialRooms.ID.Overworld, new Point(5,4)),
                       new DoorSetup(Emoji.Environment.Door, new Point(4,0), InitialRooms.ID.Overworld, new Point(5,4)),
                       new DynamicObjectSetup(Emoji.Animals.Spider, new Point(4,4)),
                       new DynamicObjectSetup(Emoji.Plants.Rose, new Point(1,6)),
                       new DynamicObjectSetup(Emoji.Environment.HoneyPot, new Point(7,6)),
                       new DynamicObjectSetup(Emoji.InventoryItems.Candle, new Point(3,7)),
                       new DynamicObjectSetup(Emoji.InventoryItems.Candle, new Point(5,7)),
                       } ),
            [InitialRooms.ID.Overworld] = new Room( "The World",
                    new string[] {
                    "⬛⬛⬛⬛⬛⬛⬛⬛⬛",
                    "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                    "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                    "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                    "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                    "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                    "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                    "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                    "⬛⬛⬛⬛⬛⬛⬛⬛⬛", },
                   new DynamicObjectSetup[] {
                       new DynamicObjectSetup(Emoji.Player.Default, new Point(5,4)),
                       new DoorSetup(Emoji.Environment.Door, new Point(4,4), InitialRooms.ID.VampireCastleCourtyard, new Point(4,7)),
                       } ),
        };
    }
}
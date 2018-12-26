using System.Drawing;

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
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬛⬛⬛⬛⬛⬛⬛⬛" },
                   new DynamicObjectSetup[] {
                       new DynamicObjectSetup("😀", new Point(5,5)),
                       new DynamicObjectSetup("🐘", new Point(1,1)), } ) };
    }
}
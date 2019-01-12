namespace GridExplorerBot
{
    public static class SaveUtils
    {
        public static void Write(this BitStreams.BitStream stream, InitialRooms.ID initialRoomID)
        {
            stream.WriteByte((byte)initialRoomID, 6); // 63 6
        }

        public static void Read(this BitStreams.BitStream stream, out InitialRooms.ID initialRoomID)
        {
            initialRoomID = (InitialRooms.ID)stream.ReadByte(6);
        }

        public static void Write(this BitStreams.BitStream stream, Objects.ID type)
        {
            stream.WriteByte((byte)type, 7); // 127 7 bits
        }

        public static void Read(this BitStreams.BitStream stream, out Objects.ID type)
        {
            type = (Objects.ID)stream.ReadByte(7);
        }

        public static void Write(this BitStreams.BitStream stream, Point point)
        {
            byte positionIndex = (byte)(point.mRow * Game.numRoomColumns + point.mColumn); // 81 values 7 bits

            stream.WriteByte(positionIndex, 7);
        }

        public static void Read(this BitStreams.BitStream stream, out Point point)
        {
            byte positionIndex = stream.ReadByte(7);

            point.mRow = positionIndex / Game.numRoomColumns;
            point.mColumn = positionIndex % Game.numRoomColumns;
        }
    }
}
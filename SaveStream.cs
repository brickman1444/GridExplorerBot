namespace GridExplorerBot
{
    public abstract class SaveStream
    {
        protected BitStreams.BitStream mBitStream;

        public abstract bool IsWriting();

        public abstract void Stream(ref int i, int numBits);

        public abstract void Stream(ref bool b);

        public abstract void Stream(ref InitialRooms.ID initialRoomID);

        public abstract void Stream(ref Objects.ID type);

        public abstract void Stream(ref Point point);

        public byte[] GetStreamData()
        {
            return mBitStream.GetStreamData();
        }
    }

    public class WriteStream : SaveStream
    {
        public WriteStream(int numBytes)
        {
            mBitStream = new BitStreams.BitStream(new byte[numBytes]);
        }

        public override bool IsWriting()
        {
            return true;
        }

        public override void Stream(ref int i, int numBits)
        {
            mBitStream.WriteByte((byte)i, numBits);
        }

        public override void Stream(ref bool b)
        {
            mBitStream.WriteBit((BitStreams.Bit)b);
        }

        public override void Stream(ref InitialRooms.ID initialRoomID)
        {
            mBitStream.WriteByte((byte)initialRoomID, SaveUtils.GetNumBits(initialRoomID));
        }

        public override void Stream(ref Objects.ID type)
        {
            mBitStream.WriteByte((byte)type, SaveUtils.GetNumBits(type));
        }

        public override void Stream(ref Point point)
        {
            byte positionIndex = (byte)(point.mRow * Game.numRoomColumns + point.mColumn);

            mBitStream.WriteByte(positionIndex, SaveUtils.GetNumBitsToStoreValue(Game.numRoomColumns * Game.numRoomRows - 1));
        }
    }

    public class ReadStream : SaveStream
    {
        public ReadStream(byte[] bytes)
        {
            mBitStream = new BitStreams.BitStream(bytes);
        }

        public override bool IsWriting()
        {
            return false;
        }

        public void BackUp(int numBytes)
        {
            // Because there's no interface to get the current offset of the stream,
            // we can't progress forward and then seek back to where we were.

            // HACK
            for (int returnedBit = 0; returnedBit < numBytes; returnedBit++)
            {
                mBitStream.ReturnBit();
            }
        }

        public override void Stream(ref int i, int numBits)
        {
            i = mBitStream.ReadByte(numBits);
        }

        public override void Stream(ref bool b)
        {
            b = mBitStream.ReadBit();
        }

        public override void Stream(ref InitialRooms.ID initialRoomID)
        {
            initialRoomID = (InitialRooms.ID)mBitStream.ReadByte(SaveUtils.GetNumBits(initialRoomID));
        }

        public override void Stream(ref Objects.ID type)
        {
            type = (Objects.ID)mBitStream.ReadByte(SaveUtils.GetNumBits(type));
        }

        public override void Stream(ref Point point)
        {
            byte positionIndex = mBitStream.ReadByte(SaveUtils.GetNumBitsToStoreValue(Game.numRoomColumns * Game.numRoomRows - 1));

            point.mRow = positionIndex / Game.numRoomColumns;
            point.mColumn = positionIndex % Game.numRoomColumns;
        }
    }
}
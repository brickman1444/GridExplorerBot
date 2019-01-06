using System.Diagnostics;
using System.Collections.Generic;

namespace GridExplorerBot
{
    public static class Objects
    {
        public enum ID
        {
            Unknown,
            PlayerCharacter,
            Empty,
            Wall,
            Elephant,
            Pen,
            Lock,
            SpiderWeb,
            HoneyPot,
            Door,
            Spider,
        }
    }

    public class DynamicObject
    {
        public Point mPosition = new Point();
        public Objects.ID mType = Objects.ID.Unknown;

        public DynamicObject()
        {

        }

        public virtual bool CanBePickedUp() { return false; }
        public virtual bool CanBeMovedThrough() { return false; }

        public virtual void Save(BitStreams.BitStream stream)
        {
            stream.WriteByte((byte)mType, 7);  // 127 7 bits

            mPosition.Save(stream);
        }

        public virtual void Load(BitStreams.BitStream stream)
        {
            mType = (Objects.ID)stream.ReadByte(7);

            mPosition.Load(stream);
        }

        public virtual string Simulate(string inCommand, Game game)
        {
            return "";
        }

        public virtual string Render()
        {
            return Emoji.GetEmoji(mType);
        }

        protected void MoveVerticallyToBlock(DynamicObject other, Room room)
        {
            Point prospectivePosition = mPosition;

            if (other.mPosition.mRow < mPosition.mRow)
            {
                prospectivePosition.mRow--;
            }
            else if (other.mPosition.mRow > mPosition.mRow)
            {
                prospectivePosition.mRow++;
            }

            if (room.CanSpaceBeMovedTo(prospectivePosition))
            {
                mPosition = prospectivePosition;
            }
        }
    }
}
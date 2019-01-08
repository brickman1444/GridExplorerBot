using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

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
            Bee,
            Rose,
            WiltedRose,
            Vase,
            Candle,
            Globe,
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

        public virtual bool CanBeThrownThrough()
        {
            return ObjectTraits.GetObjectTraits(mType).mCanStaticObjectBeThrownThrough;
        }

        public virtual string GetDescriptionText()
        {
            return ObjectTraits.GetObjectTraits(mType).mLookDescription;
        }

        public virtual void Save(BitStreams.BitStream stream)
        {
            stream.WriteByte((byte)mType, 7);  // 127 7 bits

            stream.Write(mPosition);
        }

        public virtual void Load(BitStreams.BitStream stream)
        {
            mType = (Objects.ID)stream.ReadByte(7);

            stream.Read(out mPosition);
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

        protected void MoveTowards(Point targetPosition, Room room)
        {
            if (MathUtils.ArePointsAdjacent(targetPosition, mPosition))
            {
                return;
            }

            var prospectivePositions = MathUtils.GetAdjacentPoints(mPosition);

            var validPositions = from point in prospectivePositions
                                 where point.IsWithinBounds() && room.CanSpaceBeMovedTo(point)
                                 orderby MathUtils.GetDistance(point, targetPosition) ascending
                                 select point;

            if (!validPositions.Any())
            {
                return;
            }

            mPosition = validPositions.First();
        }
    }
}
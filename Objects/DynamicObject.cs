using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace GridExplorerBot
{
    public class DynamicObject : GridObject
    {
        public static DynamicObject Create(Objects.ID typeID, Point startinPosition)
        {
            DynamicObject dynamicObject = Emoji.CreateObject(typeID);
            dynamicObject.mType = typeID;
            dynamicObject.mPosition = startinPosition;
            return dynamicObject;
        }

        public override void Save(BitStreams.BitStream stream)
        {
            stream.WriteByte((byte)mType, 7);  // 127 7 bits

            stream.Write(mPosition);
        }

        public override void Load(BitStreams.BitStream stream)
        {
            mType = (Objects.ID)stream.ReadByte(7);

            stream.Read(out mPosition);
        }

        public override string Simulate(string inCommand, Game game)
        {
            return "";
        }

        public override string Render()
        {
            return Emoji.GetEmoji(mType);
        }

        public void TeleportTo(Point destination)
        {
            mPosition = destination;
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
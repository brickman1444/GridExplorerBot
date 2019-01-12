using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace GridExplorerBot
{
    public class DynamicObject : GridObject
    {
        public override void Stream(SaveStream stream)
        {
            stream.Stream(ref mType);
            stream.Stream(ref mPosition);
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
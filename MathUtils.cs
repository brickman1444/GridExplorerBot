using System.Drawing;

namespace GridExplorerBot
{
    public static class MathUtils
    {
        public static Point GetAdjacentPoint(Point basePoint, Direction direction)
        {
            Point deltaVector = GetVector(direction);

            return new Point(basePoint.X + deltaVector.X, basePoint.Y + deltaVector.Y);
        }

        public static Point GetVector(Direction direction)
        {
            if (direction == Direction.North)
            {
                return new Point(-1,0);
            }
            else if (direction == Direction.South)
            {
                return new Point(1,0);
            }
            else if (direction == Direction.East)
            {
                return new Point(0, 1);
            }
            else if (direction == Direction.West)
            {
                return new Point(0, -1);
            }

            return new Point();
        }
    }
}
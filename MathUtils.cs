using System.Linq;

namespace GridExplorerBot
{
    public struct Point
    {
        public int mRow;
        public int mColumn;

        public Point(int inRow, int inColumn)
        {
            mRow = inRow;
            mColumn = inColumn;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.mRow + b.mRow, a.mColumn + b.mColumn);
        }

        public static Point operator *(Point p, int i)
        {
            return new Point(p.mRow * i, p.mColumn * i);
        }

        public static bool operator==(Point a, Point b)
        {
            return a.mRow == b.mRow && a.mColumn == b.mColumn;
        }

        public static bool operator!=(Point a, Point b)
        {
            return !(a == b);
        }

        public override bool Equals(object o)
        {
            Point? p = o as Point?;
            if (p.HasValue)
            {
                return p.Value == this;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Use the index as a hash
            return mRow * Game.numRoomColumns + mColumn;
        }

        public bool IsWithinBounds()
        {
            return mRow >= 0 && mRow < Game.numRoomRows && mColumn >= 0 && mColumn < Game.numRoomColumns;
        }
    }

    public static class MathUtils
    {
        public static Point GetAdjacentPoint(Point basePoint, Direction direction)
        {
            Point deltaVector = GetVector(direction);

            return basePoint + deltaVector;
        }

        public static Direction[] GetDirections()
        {
            return new Direction[]{Direction.North, Direction.South, Direction.East, Direction.West};
        }
        
        public static System.Collections.Generic.IEnumerable<Point> GetAdjacentPoints(Point basePoint)
        {
            return from direction in GetDirections()
                   where GetAdjacentPoint(basePoint, direction).IsWithinBounds()
                   select GetAdjacentPoint(basePoint, direction);
        }

        public static Point GetVector(Direction direction)
        {
            if (direction == Direction.North)
            {
                return new Point(-1, 0);
            }
            else if (direction == Direction.South)
            {
                return new Point(1, 0);
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

        public static bool ArePointsAdjacent(Point a, Point b)
        {
            if (a.mColumn == b.mColumn)
            {
                if (a.mRow + 1 == b.mRow)
                {
                    // a
                    // b
                    return true;
                }
                else if (a.mRow - 1 == b.mRow)
                {
                    // b
                    // a
                    return true;
                }
            }

            if (a.mRow == b.mRow)
            {
                if (a.mColumn + 1 == b.mColumn)
                {
                    // ab
                    return true;
                }
                else if (a.mColumn - 1 == b.mColumn)
                {
                    // ba
                    return true;
                }
            }

            return false;
        }

        public static float GetDistance(Point a, Point b)
        {
            return System.MathF.Sqrt(System.MathF.Pow(a.mRow - b.mRow, 2) + System.MathF.Pow(a.mColumn - b.mColumn, 2));
        }
    }
}
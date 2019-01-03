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

        public static bool operator==(Point a, Point b)
        {
            return a.mRow == b.mRow && a.mColumn == b.mColumn;
        }

        public static bool operator!=(Point a, Point b)
        {
            return !(a == b);
        }

        public void Save(BitStreams.BitStream stream)
        {
            byte positionIndex = (byte)(mRow * Game.numRoomColumns + mColumn); // 81 values 7 bits

            stream.WriteByte(positionIndex, 7);
        }

        public void Load(BitStreams.BitStream stream)
        {
            byte positionIndex = stream.ReadByte(7);

            mRow = positionIndex / Game.numRoomColumns;
            mColumn = positionIndex % Game.numRoomColumns;
        }
    }

    public static class MathUtils
    {
        public static Point GetAdjacentPoint(Point basePoint, Direction direction)
        {
            Point deltaVector = GetVector(direction);

            return basePoint + deltaVector;
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
    }
}
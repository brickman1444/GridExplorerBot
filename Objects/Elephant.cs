using System.Drawing;

namespace GridExplorerBot
{
    public class Elephant : DynamicObject
    {
        public override string Simulate(string inCommand, Room room)
        {
            Point prospectivePoint = mPosition;
            prospectivePoint.Y += 1;

            if (room.CanSpaceBeMovedTo(prospectivePoint))
            {
                mPosition = prospectivePoint;
            }

            return "";
        }
    }
}
using System.Drawing;
using System.Collections.Generic;

namespace GridExplorerBot
{
    public class Elephant : DynamicObject
    {
        bool mIsWalkingRight = true;

        public override string Simulate(string inCommand, Room room)
        {
            Point prospectivePoint = GetProspectiveMoveLocation(mIsWalkingRight);

            if (room.CanSpaceBeMovedTo(prospectivePoint))
            {
                mPosition = prospectivePoint;
            }
            else
            {
                Point alternateProspectivePoint = GetProspectiveMoveLocation(!mIsWalkingRight);

                if (room.CanSpaceBeMovedTo(alternateProspectivePoint))
                {
                    mPosition = alternateProspectivePoint;
                    mIsWalkingRight = !mIsWalkingRight;
                }
            }

            return "";
        }

        Point GetProspectiveMoveLocation(bool isWalkingRight)
        {
            Point prospectivePoint = mPosition;

            if ( isWalkingRight)
            {
                prospectivePoint.Y += 1;
            }
            else
            {
                prospectivePoint.Y -= 1;
            }

            return prospectivePoint;
        }

        protected override void Save(ref Stack<byte> bytes)
        {
            base.Save(ref bytes);

            bytes.Push( System.Convert.ToByte(mIsWalkingRight));
        }

        protected override void Load(ref Stack<byte> bytes)
        {
            base.Load(ref bytes);

            mIsWalkingRight = System.Convert.ToBoolean(bytes.Pop());
        }
    }
}
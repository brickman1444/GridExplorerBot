using System.Collections.Generic;

namespace GridExplorerBot
{
    public class Elephant : DynamicObject
    {
        bool mIsWalkingRight = true;

        public override string Simulate(string inCommand, Game game)
        {
            Point prospectivePoint = GetProspectiveMoveLocation(mIsWalkingRight);

            if (game.mRoom.CanSpaceBeMovedTo(prospectivePoint))
            {
                mPosition = prospectivePoint;
            }
            else
            {
                Point alternateProspectivePoint = GetProspectiveMoveLocation(!mIsWalkingRight);

                if (game.mRoom.CanSpaceBeMovedTo(alternateProspectivePoint))
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
                prospectivePoint.mColumn += 1;
            }
            else
            {
                prospectivePoint.mRow -= 1;
            }

            return prospectivePoint;
        }

        public override void Stream(SaveStream stream)
        {
            base.Stream(stream);

            stream.Stream(ref mIsWalkingRight);
        }
    }
}
using System.Diagnostics;

namespace GridExplorerBot
{
    class Hourglass : StaticObject
    {
        int mTimeLeft = 0;
        const int mMaxTimeLeft = 3;

        bool IsDone()
        {
            return mTimeLeft == 0;
        }

        public override string Render()
        {
            if (IsDone())
            {
                return Emoji.Environment.HourglassDone;
            }
            else
            {
                return Emoji.Environment.HourglassNotDone;
            }
        }

        public override string Simulate(string command, Game game)
        {
            if (!IsDone())
            {
                mTimeLeft--;
            }
            return "";
        }

        public override string GetDescriptionText(Game game)
        {
            if (IsDone())
            {
                return "An antique hourglass full of sand.";
            }
            else
            {
                return "An antique hourglass. Sand is steadily flowing down.";
            }
        }

        public override void Stream(SaveStream stream)
        {
            stream.Stream(ref mTimeLeft, 2);
        }

        public override ActionResult UseWithoutTarget(Game game)
        {
            mTimeLeft = mMaxTimeLeft;

            return new ActionResult()
            {
                mOutput = "You turn the hourglass over and the sand begins to slowly flow down.",
                mSuccess = true,
            };
        }
    }
}
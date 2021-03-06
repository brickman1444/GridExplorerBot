using System.Diagnostics;

namespace GridExplorerBot
{
    public class GameTime
    {
        private int mTimeInHoursAfterMidnight = 0;

        public void Increment()
        {
            mTimeInHoursAfterMidnight++;
            mTimeInHoursAfterMidnight %= 24;
        }

        public int GetHoursAfterMidnight()
        {
            return mTimeInHoursAfterMidnight;
        }

        public bool IsNight()
        {
            return mTimeInHoursAfterMidnight < 5 || mTimeInHoursAfterMidnight >= 20;
        }

        public string Render()
        {
            int twelveHourTime = GetHoursAfterMidnight() % 12;

            string outString = "";

            switch(twelveHourTime)
            {
                case 0: outString += Emoji.Time.ZeroOClock; break;
                case 1: outString += Emoji.Time.OneOClock; break;
                case 2: outString += Emoji.Time.TwoOClock; break;
                case 3: outString += Emoji.Time.ThreeOClock; break;
                case 4: outString += Emoji.Time.FourOClock; break;
                case 5: outString += Emoji.Time.FiveOClock; break;
                case 6: outString += Emoji.Time.SixOClock; break;
                case 7: outString += Emoji.Time.SevenOClock; break;
                case 8: outString += Emoji.Time.EightOClock; break;
                case 9: outString += Emoji.Time.NineOClock; break;
                case 10: outString += Emoji.Time.TenOClock; break;
                case 11: outString += Emoji.Time.ElevenOClock; break;
                default: Debug.Fail("Invalid time"); break;
            }

            if (mTimeInHoursAfterMidnight < 5)
            {
                outString += Emoji.Sky.LastQuarterMoonWithFace;
            }
            else if (mTimeInHoursAfterMidnight < 7)
            {
                outString += Emoji.CityScape.Sunrise;
            }
            else if (mTimeInHoursAfterMidnight < 17)
            {
                outString += Emoji.Sky.SunWithFace;
            }
            else if (mTimeInHoursAfterMidnight < 19)
            {
                outString += Emoji.CityScape.Sunset;
            }
            else
            {
                outString += Emoji.Sky.LastQuarterMoonWithFace;
            }

            return outString;
        }

        public void Stream(SaveStream stream)
        {
            stream.Stream(ref mTimeInHoursAfterMidnight, SaveUtils.GetNumBitsToStoreValue(23));
        }
    }
}
using System.Diagnostics;
using System.Drawing;

namespace GridExplorerBot
{
    public static class Objects
    {
        public enum ID
        {
            Unknown,
            PlayerCharacter,
            Empty,
            Wall,
            Elephant,
        }
    }

    
    public class DynamicObject
    {
        public string mDisplayText = "";
        public Point mPosition = new Point();
        public Objects.ID mType = Objects.ID.Unknown;

        public DynamicObject()
        {

        }

        public DynamicObject( DynamicObjectSetup setup )
        {
            mDisplayText = setup.mDisplayText;
            mType = Emoji.GetID(setup.mDisplayText);
            mPosition = setup.mStartingPosition;
        }

        public string Save()
        {
            string outSaveData = "";

            outSaveData += mDisplayText + ',';

            outSaveData += ((int)mType).ToString() + ',';

            outSaveData += mPosition.X.ToString() + ',';

            outSaveData += mPosition.Y.ToString();

            return outSaveData;
        }

        public void Load(string inSaveData)
        {
            string[] tokens = inSaveData.Split(',');

            Debug.Assert(tokens.Length == 4);

            mDisplayText = tokens[0];
            mType = (Objects.ID)int.Parse(tokens[1]);
            mPosition.X = int.Parse(tokens[2]);
            mPosition.Y = int.Parse(tokens[3]);
        }

        public virtual string Simulate(string inCommand, Room room)
        {
            return "";
        }
    }
}
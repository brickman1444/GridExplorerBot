using System.Collections.Generic;
using System.Drawing;


namespace GridExplorerBot
{
    public class DynamicObjectSetup
    {
        public readonly string mDisplayText;
        public readonly Point mStartingPosition;

        public DynamicObjectSetup( string inDisplayText, Point inStartingPosition )
        {
            mDisplayText = inDisplayText;
            mStartingPosition = inStartingPosition;
        }
    }

    public class DynamicObject
    {
        public string mDisplayText;
        public Point mPosition;
        public readonly Objects.ID mType;

        public DynamicObject( DynamicObjectSetup setup )
        {
            mDisplayText = setup.mDisplayText;
            mType = Emoji.GetID(setup.mDisplayText);
            mPosition = setup.mStartingPosition;
        }
    }

    public class Room
    {
        readonly string mStaticRoomText;
        Objects.ID[,] mStaticRoomGrid = new Objects.ID[10,10];
        List<DynamicObject> mDynamicObjects = new List<DynamicObject>();

        public Room( string inStaticRoomText, IEnumerable<DynamicObjectSetup> dynamicObjectSetups )
        {
            mStaticRoomText = inStaticRoomText;

            string[] lines = mStaticRoomText.Split('\n');

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                List<string> splitLine = StringUtils.SplitEmojiString(lines[lineIndex]);

                for (int columnIndex = 0; columnIndex < splitLine.Count; columnIndex++)
                {
                    mStaticRoomGrid[lineIndex,columnIndex] = Emoji.GetID(splitLine[columnIndex]);
                }
            }

            foreach ( var setup in dynamicObjectSetups )
            {
                mDynamicObjects.Add( new DynamicObject( setup ));
            }
        }

        DynamicObject GetDynamicObjectAtPosition( Point position )
        {
            foreach ( DynamicObject dynamicObject in mDynamicObjects )
            {
                if ( dynamicObject.mPosition == position)
                {
                    return dynamicObject;
                }
            }

            return null;
        }

        public string Render()
        {
            string outString = "";

            for ( int rowIndex = 0; rowIndex < mStaticRoomGrid.GetLength(0); rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < mStaticRoomGrid.GetLength(1); columnIndex++)
                {
                    DynamicObject dynamicObject = GetDynamicObjectAtPosition( new Point( rowIndex, columnIndex));

                    if ( dynamicObject == null)
                    {
                        outString += Emoji.GetEmoji(mStaticRoomGrid[rowIndex,columnIndex]);
                    }
                    else
                    {
                        outString += dynamicObject.mDisplayText;
                    }
                }

                outString += '\n';
            }

            return outString;
        }

        public string HandleCommand(string inCommand)
        {
            return "";
        }
    }

    static class Rooms
    {
        public static Room TheRoom =
            new Room("⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛\n"
                   + "⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛",
                   new DynamicObjectSetup[] { new DynamicObjectSetup("😀", new Point(5,5)) } );
    }
}
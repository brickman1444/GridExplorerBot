using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;

namespace GridExplorerBot
{
    public enum Direction
    {
        Unknown,
        North,
        West,
        East,
        South
    }

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

    public class Room
    {
        Objects.ID[,] mStaticRoomGrid = new Objects.ID[Game.numRoomRows,Game.numRoomColumns];
        List<DynamicObject> mDynamicObjects = new List<DynamicObject>();
        int mInitialRoomIndex = -1;

        public Room()
        {

        }

        public Room( ICollection<string> roomLines, IEnumerable<DynamicObjectSetup> dynamicObjectSetups )
        {
            Debug.Assert(roomLines.Count == Game.numRoomRows);

            int lineIndex = 0;
            foreach (string line in roomLines)
            {
                List<string> splitLine = StringUtils.SplitEmojiString(line);

                for (int columnIndex = 0; columnIndex < splitLine.Count; columnIndex++)
                {
                    mStaticRoomGrid[lineIndex,columnIndex] = Emoji.GetID(splitLine[columnIndex]);
                }

                lineIndex++;
            }

            foreach ( var setup in dynamicObjectSetups )
            {
                Objects.ID id = Emoji.GetID(setup.mDisplayText);
                DynamicObject dynamicObject = Emoji.CreateObject(id);
                dynamicObject.Setup(setup);

                mDynamicObjects.Add( dynamicObject);
            }
        }

        public void SetInitialRoomIndex(int roomIndex)
        {
            Debug.Assert( InitialRooms.IsValidInitialRoomIndex( roomIndex  ) );

            mInitialRoomIndex = roomIndex;
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

                // Add a new line after every line except the end
                if (rowIndex < mStaticRoomGrid.GetLength(0) - 1)
                {
                    outString += '\n';
                }
            }

            return outString;
        }

        public string Save()
        {
            string outSaveData = "";

            Debug.Assert( InitialRooms.IsValidInitialRoomIndex( mInitialRoomIndex ) );

            outSaveData += mInitialRoomIndex + " ";

            List<string> dynamicObjectTokens = new List<string>();

            foreach ( DynamicObject dynamicObject in mDynamicObjects)
            {
                dynamicObjectTokens.Add( dynamicObject.Save() );
            }

            outSaveData += string.Join(' ', dynamicObjectTokens);

            return outSaveData;
        }

        public void Load(string inSaveData)
        {
            string[] tokens = inSaveData.Split(" ");

            Debug.Assert( tokens.Length > 0 );

            SetInitialRoomIndex( int.Parse( tokens[0] ) );
            LoadStaticGridFromInitialRoom();

            mDynamicObjects.Clear();

            foreach (string dynamicObjectToken in  new System.ArraySegment<string>(tokens,1, tokens.Length - 1))
            {
                DynamicObject dynamicObject = Emoji.CreateObject(dynamicObjectToken);

                dynamicObject.Load(dynamicObjectToken);
                mDynamicObjects.Add(dynamicObject);
            }
        }

        public void LoadStaticGridFromInitialRoom()
        {
            Debug.Assert( InitialRooms.IsValidInitialRoomIndex( mInitialRoomIndex ));

            mStaticRoomGrid = InitialRooms.initialRooms[mInitialRoomIndex].mStaticRoomGrid;
        }

        public void LoadDynamicObjectsFromInitialRoom()
        {
            Debug.Assert( InitialRooms.IsValidInitialRoomIndex( mInitialRoomIndex ));

            mDynamicObjects = InitialRooms.initialRooms[mInitialRoomIndex].mDynamicObjects;
        }

        public string Simulate(string inCommand)
        {
            inCommand = inCommand.ToLower();

            string outText = "";

            foreach ( DynamicObject dynamicObject in mDynamicObjects)
            {
                string simulateResult = dynamicObject.Simulate(inCommand, this);
                if (simulateResult != "")
                {
                    outText = simulateResult;
                }
            }

            return outText;
        }

        public DynamicObject FindFirstDynamicObject(Objects.ID id)
        {
            foreach ( var dynamicObject in mDynamicObjects)
            {
                if (dynamicObject.mType == id)
                {
                    return dynamicObject;
                }
            }

            return null;
        }

        public DynamicObject FindFirstDynamicObject(Point position)
        {
            foreach ( var dynamicObject in mDynamicObjects)
            {
                if (dynamicObject.mPosition == position)
                {
                    return dynamicObject;
                }
            }

            return null;
        }

        public bool CanSpaceBeMovedTo(Point position)
        {
            DynamicObject dynamicObjectAtPosition = FindFirstDynamicObject(position);

            if (dynamicObjectAtPosition != null)
            {
                return false;
            }

            Objects.ID staticObjectAtPosition = mStaticRoomGrid[position.X,position.Y];

            if (staticObjectAtPosition != Objects.ID.Empty)
            {
                return false;
            }

            return true;
        }
    }
}
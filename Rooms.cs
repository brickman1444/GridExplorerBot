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

        public DynamicObjectSetup(string inDisplayText, Point inStartingPosition)
        {
            mDisplayText = inDisplayText;
            mStartingPosition = inStartingPosition;
        }
    }

    public class Room
    {
        Objects.ID[,] mStaticRoomGrid = new Objects.ID[Game.numRoomRows, Game.numRoomColumns];
        List<DynamicObject> mDynamicObjects = new List<DynamicObject>();
        List<DynamicObject> mDynamicObjectsToBeDeleted = new List<DynamicObject>();
        List<DynamicObject> mSpawnedDynamicObjects = new List<DynamicObject>();
        int mInitialRoomIndex = -1;

        public Room()
        {

        }

        public Room(ICollection<string> roomLines, IEnumerable<DynamicObjectSetup> dynamicObjectSetups)
        {
            Debug.Assert(roomLines.Count == Game.numRoomRows);

            int lineIndex = 0;
            foreach (string line in roomLines)
            {
                List<string> splitLine = StringUtils.SplitEmojiString(line);

                for (int columnIndex = 0; columnIndex < splitLine.Count; columnIndex++)
                {
                    mStaticRoomGrid[lineIndex, columnIndex] = Emoji.GetID(splitLine[columnIndex]);
                }

                lineIndex++;
            }

            foreach (var setup in dynamicObjectSetups)
            {
                Objects.ID id = Emoji.GetID(setup.mDisplayText);
                DynamicObject dynamicObject = Emoji.CreateObject(id);
                dynamicObject.Setup(setup);

                mDynamicObjects.Add(dynamicObject);
            }
        }

        public void SetInitialRoomIndex(int roomIndex)
        {
            Debug.Assert(InitialRooms.IsValidInitialRoomIndex(roomIndex));

            mInitialRoomIndex = roomIndex;
        }

        DynamicObject GetDynamicObjectAtPosition(Point position)
        {
            foreach (DynamicObject dynamicObject in mDynamicObjects)
            {
                if (dynamicObject.mPosition == position)
                {
                    return dynamicObject;
                }
            }

            return null;
        }

        public string Render()
        {
            string outString = "";

            for (int rowIndex = 0; rowIndex < mStaticRoomGrid.GetLength(0); rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < mStaticRoomGrid.GetLength(1); columnIndex++)
                {
                    DynamicObject dynamicObject = GetDynamicObjectAtPosition(new Point(rowIndex, columnIndex));

                    if (dynamicObject == null)
                    {
                        outString += Emoji.GetEmoji(mStaticRoomGrid[rowIndex, columnIndex], 0);
                    }
                    else
                    {
                        outString += dynamicObject.Render();
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

            Debug.Assert(InitialRooms.IsValidInitialRoomIndex(mInitialRoomIndex));

            outSaveData += mInitialRoomIndex + " ";

            List<string> dynamicObjectTokens = new List<string>();

            foreach (DynamicObject dynamicObject in mDynamicObjects)
            {
                dynamicObjectTokens.Add(dynamicObject.Save());
            }

            outSaveData += string.Join(' ', dynamicObjectTokens);

            return outSaveData;
        }

        public void Load(string inSaveData)
        {
            string[] tokens = inSaveData.Split(" ");

            Debug.Assert(tokens.Length > 0);

            SetInitialRoomIndex(int.Parse(tokens[0]));
            LoadStaticGridFromInitialRoom();

            mDynamicObjects.Clear();

            foreach (string dynamicObjectToken in new System.ArraySegment<string>(tokens, 1, tokens.Length - 1))
            {
                DynamicObject dynamicObject = Emoji.CreateObject(dynamicObjectToken);

                dynamicObject.Load(dynamicObjectToken);
                mDynamicObjects.Add(dynamicObject);
            }
        }

        public void LoadStaticGridFromInitialRoom()
        {
            Debug.Assert(InitialRooms.IsValidInitialRoomIndex(mInitialRoomIndex));

            mStaticRoomGrid = InitialRooms.initialRooms[mInitialRoomIndex].mStaticRoomGrid;
        }

        public void LoadDynamicObjectsFromInitialRoom()
        {
            Debug.Assert(InitialRooms.IsValidInitialRoomIndex(mInitialRoomIndex));

            mDynamicObjects = InitialRooms.initialRooms[mInitialRoomIndex].mDynamicObjects;
        }

        public string Simulate(string inCommand, Game game)
        {
            inCommand = inCommand.ToLower();

            Debug.Assert(mDynamicObjectsToBeDeleted.Count == 0);

            string outText = "";

            foreach (DynamicObject dynamicObject in mDynamicObjects)
            {
                string simulateResult = dynamicObject.Simulate(inCommand, game);
                if (simulateResult != "")
                {
                    outText = simulateResult;
                }
            }

            foreach (DynamicObject dynamicObject in mDynamicObjectsToBeDeleted)
            {
                mDynamicObjects.Remove(dynamicObject);
            }

            mDynamicObjects.AddRange(mSpawnedDynamicObjects);

            return outText;
        }

        public DynamicObject FindFirstDynamicObject(Objects.ID id)
        {
            foreach (var dynamicObject in mDynamicObjects)
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
            foreach (var dynamicObject in mDynamicObjects)
            {
                if (dynamicObject.mPosition == position)
                {
                    return dynamicObject;
                }
            }

            return null;
        }

        public DynamicObject FindDynamicObjectAdjacentTo(Point position, Objects.ID typeToFind)
        {
            foreach (DynamicObject dynamicObject in mDynamicObjects)
            {
                if (ArePointsAdjacent(dynamicObject.mPosition, position) && dynamicObject.mType == typeToFind)
                {
                    return dynamicObject;
                }
            }

            return null;
        }

        public void MarkObjectForDeletion(DynamicObject dynamicObject)
        {
            mDynamicObjectsToBeDeleted.Add(dynamicObject);
        }

        public void AddNewItem(DynamicObject dynamicObject)
        {
            mSpawnedDynamicObjects.Add(dynamicObject);
        }

        public bool CanSpaceBeMovedTo(Point position)
        {
            foreach (DynamicObject dynamicObject in mDynamicObjects)
            {
                if (dynamicObject.mPosition == position && !dynamicObject.CanBeMovedThrough())
                {
                    return false;
                }
            }

            Objects.ID staticObjectAtPosition = mStaticRoomGrid[position.X, position.Y];

            if (staticObjectAtPosition != Objects.ID.Empty)
            {
                return false;
            }

            return true;
        }

        public static bool ArePointsAdjacent(Point a, Point b)
        {
            if (a.Y == b.Y)
            {
                if (a.X + 1 == b.X)
                {
                    // a
                    // b
                    return true;
                }
                else if (a.X - 1 == b.X)
                {
                    // b
                    // a
                    return true;
                }
            }

            if (a.X == b.X)
            {
                if (a.Y + 1 == b.Y)
                {
                    // ab
                    return true;
                }
                else if (a.Y - 1 == b.Y)
                {
                    // ba
                    return true;
                }
            }

            return false;
        }

        public static Direction GetDirection(string directionString)
        {
            if (directionString == "north" || directionString == "up")
            {
                return Direction.North;
            }
            else if (directionString == "south" || directionString == "down")
            {
                return Direction.South;
            }
            else if (directionString == "east" || directionString == "right")
            {
                return Direction.East;
            }
            else if (directionString == "west" || directionString == "left")
            {
                return Direction.West;
            }

            return Direction.Unknown;
        }
    }
}
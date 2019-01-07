using System.Collections.Generic;
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

        public virtual DynamicObject CreateObject()
        {
            Objects.ID id = Emoji.GetID(mDisplayText);
            DynamicObject dynamicObject = Emoji.CreateObject(id);
            dynamicObject.mType = id;
            dynamicObject.mPosition = mStartingPosition;
            return dynamicObject;
        }
    }

    public class Room
    {
        Objects.ID[,] mStaticRoomGrid = new Objects.ID[Game.numRoomRows, Game.numRoomColumns];
        public List<DynamicObject> mDynamicObjects = new List<DynamicObject>();
        List<DynamicObject> mDynamicObjectsToBeDeleted = new List<DynamicObject>();
        List<DynamicObject> mSpawnedDynamicObjects = new List<DynamicObject>();
        InitialRooms.ID mInitialRoomIndex = InitialRooms.ID.Unknown;
        public string mDescription = "";

        public Room()
        {

        }

        public Room(string description, ICollection<string> roomLines, IEnumerable<DynamicObjectSetup> dynamicObjectSetups)
        {
            mDescription = description;

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

            foreach (DynamicObjectSetup setup in dynamicObjectSetups)
            {
                mDynamicObjects.Add(setup.CreateObject());
            }
        }

        public void SetInitialRoomIndex(InitialRooms.ID roomIndex)
        {
            Debug.Assert(InitialRooms.IsValidInitialRoomIndex(roomIndex));

            mInitialRoomIndex = roomIndex;

            mDescription = InitialRooms.initialRooms[roomIndex].mDescription;
        }


        public string Render()
        {
            string outString = "";

            for (int rowIndex = 0; rowIndex < mStaticRoomGrid.GetLength(0); rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < mStaticRoomGrid.GetLength(1); columnIndex++)
                {
                    DynamicObject dynamicObject = FindFirstDynamicObject(new Point(rowIndex, columnIndex));

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

        public string Save(BitStreams.BitStream stream)
        {
            Debug.Assert(InitialRooms.IsValidInitialRoomIndex(mInitialRoomIndex));

            stream.Write(mInitialRoomIndex); // 63 6

            stream.WriteByte((byte)mDynamicObjects.Count, 4); // 15 4

            foreach (DynamicObject dynamicObject in mDynamicObjects)
            {
                dynamicObject.Save(stream);
            }

            string outSaveData = StringUtils.SaveDataEncode(stream.GetStreamData());

            return outSaveData;
        }

        public void Load(BitStreams.BitStream stream)
        {
            InitialRooms.ID roomIndex;
            stream.Read(out roomIndex);
            SetInitialRoomIndex(roomIndex);
            LoadStaticGridFromInitialRoom();

            int dynamicObjectCount = stream.ReadByte(4);

            mDynamicObjects.Clear();

            for (int dynamicObjectIndex = 0; dynamicObjectIndex < dynamicObjectCount; dynamicObjectIndex++)
            {
                DynamicObject dynamicObject = Emoji.CreateObject(stream);

                dynamicObject.Load(stream);
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

        public DynamicObject SpawnObject(Objects.ID type, Point position)
        {
            DynamicObject dynamicObject = Emoji.CreateObject(type);
            dynamicObject.mPosition = position;
            dynamicObject.mType = type;
            AddNewItem(dynamicObject);
            return dynamicObject;
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
                if (MathUtils.ArePointsAdjacent(dynamicObject.mPosition, position) && dynamicObject.mType == typeToFind)
                {
                    return dynamicObject;
                }
            }

            return null;
        }

        public Point? FindStaticObjectAdjacentTo(Point position, Objects.ID typeToFind)
        {
            var adjacentPoints = MathUtils.GetAdjacentPoints(position);

            foreach (Point point in adjacentPoints)
            {
                if (GetStaticObject(point) == typeToFind)
                {
                    return point;
                }
            }

            return null;
        }

        public Objects.ID GetStaticObject(Point position)
        {
            return mStaticRoomGrid[position.mRow, position.mColumn];
        }

        public Point? GetFirstStaticObjectPosition(Objects.ID type)
        {
            for (int row = 0; row < Game.numRoomRows; row++)
            {
                for (int column = 0; column < Game.numRoomColumns; column++)
                {
                    if (mStaticRoomGrid[row, column] == type)
                    {
                        return new Point(row, column);
                    }
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

            Objects.ID staticObjectAtPosition = GetStaticObject(position);

            return ObjectTraits.GetObjectTraits(staticObjectAtPosition).mCanStaticObjectBeMovedThrough;
        }

        public bool CanSpaceBeThrownThrough(Point position)
        {
            DynamicObject dynamicObject = FindFirstDynamicObject(position);

            if (dynamicObject != null && !dynamicObject.CanBeThrownThrough())
            {
                return false;
            }

            Objects.ID staticObjectAtPosition = GetStaticObject(position);

            return ObjectTraits.GetObjectTraits(staticObjectAtPosition).mCanStaticObjectBeThrownThrough;
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

        public void TeleportPlayerTo(Point destination)
        {
            DynamicObject playerObject = FindFirstDynamicObject(Objects.ID.PlayerCharacter);

            playerObject.mPosition = destination;
        }
    }
}
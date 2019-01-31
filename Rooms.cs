using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

    public class Room
    {
        StaticObject[,] mStaticRoomGrid = new StaticObject[Game.numRoomRows, Game.numRoomColumns];
        public List<DynamicObject> mDynamicObjects = new List<DynamicObject>();
        List<DynamicObject> mDynamicObjectsToBeDeleted = new List<DynamicObject>();
        List<DynamicObject> mSpawnedDynamicObjects = new List<DynamicObject>();
        InitialRooms.ID mInitialRoomIndex = InitialRooms.ID.Unknown;
        public string mDescription = "";
        public bool mIsRenderingHallucination = false;

        const int maxNumDynamicObjects = 15;

        public Room()
        {

        }

        public Room(string description, ICollection<string> roomLines, IEnumerable<GridObjectSetup> gridObjectSetups)
        {
            mDescription = description;

            Debug.Assert(roomLines.Count == Game.numRoomRows);

            int lineIndex = 0;
            foreach (string line in roomLines)
            {
                List<string> splitLine = StringUtils.SplitEmojiString(line);

                Debug.Assert(splitLine.Count == Game.numRoomColumns);

                for (int columnIndex = 0; columnIndex < splitLine.Count; columnIndex++)
                {
                    GridObject staticObject = GridObject.Create(Emoji.GetID(splitLine[columnIndex]), new Point(lineIndex, columnIndex));
                    mStaticRoomGrid[lineIndex, columnIndex] = (StaticObject)staticObject;
                }

                lineIndex++;
            }

            foreach (GridObjectSetup setup in gridObjectSetups)
            {
                GridObject gridObject = setup.CreateObject();

                if (gridObject as DynamicObject != null)
                {
                    mDynamicObjects.Add((DynamicObject)gridObject);
                }
                else
                {
                    Point position = gridObject.GetPosition();
                    mStaticRoomGrid[position.mRow, position.mColumn] = (StaticObject)gridObject;
                }
            }
        }

        public void CreateFrom(InitialRooms.ID roomIndex, Game game)
        {
            SetInitialRoomIndex(roomIndex);
            LoadStaticGridFromInitialRoom();
            LoadDynamicObjectsFromInitialRoom();

            foreach (GridObject gridObject in mStaticRoomGrid)
            {
                gridObject.OnRoomCreated(game);
            }

            foreach (GridObject gridObject in mDynamicObjects)
            {
                gridObject.OnRoomCreated(game);
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
            List<string> lines = new List<string>();

            for (int rowIndex = 0; rowIndex < Game.numRoomRows; rowIndex++)
            {
                string line = "";
                for (int columnIndex = 0; columnIndex < Game.numRoomColumns; columnIndex++)
                {
                    GridObject gridObject = GetFirstObject(new Point(rowIndex, columnIndex));

                    line +=  RenderObject( gridObject );
                }
                lines.Add(line);
            }

            return string.Join('\n', lines);;
        }

        private string RenderObject(GridObject gridObject)
        {
            if (mIsRenderingHallucination)
            {
                if (gridObject.GetTypeID() == Objects.ID.PlayerCharacter)
                {
                    return gridObject.Render();
                }
                else
                {
                    return Emoji.GetRandomHallucinationEmoji();
                }
            }
            else
            {
                return gridObject.Render();
            }
        }

        public void Stream(SaveStream stream)
        {
            if (stream.IsWriting())
            {
                Debug.Assert(InitialRooms.IsValidInitialRoomIndex(mInitialRoomIndex));
                stream.Stream(ref mInitialRoomIndex);
            }
            else
            {
                InitialRooms.ID roomIndex = InitialRooms.ID.Unknown;
                stream.Stream(ref roomIndex);
                SetInitialRoomIndex(roomIndex);
                LoadStaticGridFromInitialRoom();
            }

            int dynamicObjectCount = mDynamicObjects.Count;
            stream.Stream(ref dynamicObjectCount, SaveUtils.GetNumBitsToStoreValue(maxNumDynamicObjects));

            if (stream.IsWriting())
            {
                foreach (DynamicObject dynamicObject in mDynamicObjects)
                {
                    dynamicObject.Stream(stream);
                }
            }
            else
            {
                mDynamicObjects.Clear();

                for (int dynamicObjectIndex = 0; dynamicObjectIndex < dynamicObjectCount; dynamicObjectIndex++)
                {
                    DynamicObject dynamicObject = (DynamicObject)Emoji.CreateObject(stream as ReadStream);

                    dynamicObject.Stream(stream);
                    mDynamicObjects.Add(dynamicObject);
                }
            }

        }

        void LoadStaticGridFromInitialRoom()
        {
            Debug.Assert(InitialRooms.IsValidInitialRoomIndex(mInitialRoomIndex));

            mStaticRoomGrid = InitialRooms.initialRooms[mInitialRoomIndex].mStaticRoomGrid;
        }

        void LoadDynamicObjectsFromInitialRoom()
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

            foreach (StaticObject staticObject in mStaticRoomGrid)
            {
                staticObject.Simulate(inCommand,game);
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
            DynamicObject dynamicObject = (DynamicObject)GridObject.Create(type, position);
            AddNewItem(dynamicObject);
            return dynamicObject;
        }

        public GridObject GetFirstObject(Point position)
        {
            foreach (DynamicObject dynamicObject in mDynamicObjects)
            {
                if (dynamicObject.GetPosition() == position)
                {
                    return dynamicObject;
                }
            }

            foreach (DynamicObject dynamicObject in mSpawnedDynamicObjects)
            {
                if (dynamicObject.GetPosition() == position)
                {
                    return dynamicObject;
                }
            }

            return mStaticRoomGrid[position.mRow,position.mColumn];
        }

        public GridObject GetFirstObject(Objects.ID typeID)
        {
            foreach (DynamicObject dynamicObject in mDynamicObjects)
            {
                if (dynamicObject.GetTypeID() == typeID)
                {
                    return dynamicObject;
                }
            }

            foreach (StaticObject staticObject in mStaticRoomGrid)
            {
                if (staticObject.GetTypeID() == typeID)
                {
                    return staticObject;
                }
            }

            return null;
        }

        public DynamicObject FindFirstDynamicObject(Objects.ID id)
        {
            foreach (var dynamicObject in mDynamicObjects)
            {
                if (dynamicObject.GetTypeID() == id)
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
                if (dynamicObject.GetPosition() == position)
                {
                    return dynamicObject;
                }
            }

            return null;
        }

        public StaticObject GetStaticObject(Point position)
        {
            return mStaticRoomGrid[position.mRow,position.mColumn];
        }

        public GridObject FindObjectAdjacentTo(Point position, Objects.ID typeToFind)
        {
            DynamicObject dynamicObject = FindDynamicObjectAdjacentTo(position, typeToFind );

            if (dynamicObject != null)
            {
                return dynamicObject;
            }

            return FindStaticObjectAdjacentTo(position, typeToFind);
        }

        public DynamicObject FindDynamicObjectAdjacentTo(Point position, Objects.ID typeToFind)
        {
            foreach (DynamicObject dynamicObject in mDynamicObjects)
            {
                if (MathUtils.ArePointsAdjacent(dynamicObject.GetPosition(), position) && dynamicObject.GetTypeID() == typeToFind)
                {
                    return dynamicObject;
                }
            }

            return null;
        }

        public StaticObject FindStaticObjectAdjacentTo(Point position, Objects.ID typeToFind)
        {
            var adjacentPoints = MathUtils.GetAdjacentPoints(position);

            foreach (Point point in adjacentPoints)
            {
                StaticObject staticObject = GetStaticObject(point);
                if (staticObject.GetTypeID() == typeToFind)
                {
                    return staticObject;
                }
            }

            return null;
        }

        public GridObject GetNearestObject(Objects.ID typeID, Point targetPosition)
        {
            List<GridObject> matchingObjects = new List<GridObject>();

            foreach (DynamicObject dynamicObject in mDynamicObjects)
            {
                if (dynamicObject.GetTypeID() == typeID)
                {
                    matchingObjects.Add(dynamicObject);
                }
            }

            foreach (StaticObject staticObject in mStaticRoomGrid)
            {
                if (staticObject.GetTypeID() == typeID)
                {
                    matchingObjects.Add(staticObject);
                }
            }

            var sortedObjects = from matchingObject in matchingObjects
                                orderby MathUtils.GetDistance(matchingObject.GetPosition(), targetPosition) ascending
                                select matchingObject;

            if (!sortedObjects.Any())
            {
                return null;
            }

            return sortedObjects.First();
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
            return GetFirstObject(position).CanBeMovedThrough();
        }

        public bool CanSpaceBeThrownThrough(Point position)
        {
            return GetFirstObject(position).CanBeThrownThrough();
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

            playerObject.TeleportTo(destination);
        }

        public Point? FindOpenSpaceAdjacentTo(Point point)
        {
            var adjacentPoints = MathUtils.GetAdjacentPoints(point);

            foreach (Point adjacentPoint in adjacentPoints)
            {
                if (CanSpaceBeMovedTo(adjacentPoint))
                {
                    return adjacentPoint;
                }
            }

            return null;
        }
    }
}
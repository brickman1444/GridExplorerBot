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
        Objects.ID[,] mStaticRoomGrid = new Objects.ID[10,10];
        List<DynamicObject> mDynamicObjects = new List<DynamicObject>();

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

                // Add a new line after every line except the end
                if (rowIndex < mStaticRoomGrid.GetLength(0) - 1)
                {
                    outString += '\n';
                }
            }

            return outString;
        }

        public string HandleCommand(string inCommand)
        {
            inCommand = inCommand.ToLower();

            string[] tokens = inCommand.Split(' ');

            string outText = "Unknown command";

            if ( tokens[0] == "go" || tokens[0] == "move" )
            {
                outText = HandleMoveCommand(tokens);
            }

            return outText;
        }

        private string HandleMoveCommand(string[] tokens)
        {
            string outText = "";

            Direction directionToMove = Direction.Unknown;
            string prospectiveMessage = "";

            if ( tokens[1] == "north")
            {
                directionToMove = Direction.North;

                prospectiveMessage = "You moved North";
            }
            else if ( tokens[1] == "south")
            {
                directionToMove = Direction.South;

                prospectiveMessage = "You moved South";
            }
            else if ( tokens[1] == "east")
            {
                directionToMove =Direction.East;

                prospectiveMessage = "You moved East";
            }
            else if ( tokens[1] == "west")
            {
                directionToMove =Direction.West;

                prospectiveMessage = "You moved West";
            }

            bool successfulMove = MovePlayerCharacter(directionToMove);

            if (successfulMove)
            {
                outText = prospectiveMessage;
            }
            else
            {
                outText = "You could not move that direction.";
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

        public bool MovePlayerCharacter(Direction direction)
        {
            DynamicObject playerCharacter = FindFirstDynamicObject(Objects.ID.PlayerCharacter);

            if (playerCharacter == null)
            {
                return false;
            }

            Point prospectivePosition = playerCharacter.mPosition;

            if (direction == Direction.North)
            {
                prospectivePosition.X -= 1;
            }
            else if (direction == Direction.South)
            {
                prospectivePosition.X += 1;
            }
            else if (direction == Direction.East)
            {
                prospectivePosition.Y += 1;
            }
            else if (direction == Direction.West)
            {
                prospectivePosition.Y -= 1;
            }
            else
            {
                return false;
            }

            if (CanSpaceBeMovedTo(prospectivePosition))
            {
                playerCharacter.mPosition = prospectivePosition;

                return true;
            }
            else
            {
                return false;
            }
        }

        bool CanSpaceBeMovedTo(Point position)
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

        public void RemoveAllDynamicObjects()
        {
            mDynamicObjects.Clear();
        }
    }

    static class Rooms
    {
        public static Room[] initialRooms = {
            new Room( new string[] {
                   "⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛" },
                   new DynamicObjectSetup[] { new DynamicObjectSetup("😀", new Point(5,5)) } ) };
    }
}
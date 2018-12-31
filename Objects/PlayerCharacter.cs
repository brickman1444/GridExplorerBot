using System.Drawing;

namespace GridExplorerBot
{
    public class PlayerCharacter : DynamicObject
    {
        public override string Simulate(string command, Room room)
        {
            if (command.Length == 0)
            {
                return "";
            }

            string[] tokens = command.Split(' ');

            string outText = "Unknown command";

            if (tokens[0] == "go" || tokens[0] == "move")
            {
                outText = HandleMoveCommand(tokens, room);
            }

            return outText;
        }

        private string HandleMoveCommand(string[] tokens, Room room)
        {
            string outText = "";

            Direction directionToMove = Direction.Unknown;
            string prospectiveMessage = "";

            if (tokens[1] == "north" || tokens[1] == "up")
            {
                directionToMove = Direction.North;

                prospectiveMessage = "You moved North";
            }
            else if (tokens[1] == "south" || tokens[1] == "down")
            {
                directionToMove = Direction.South;

                prospectiveMessage = "You moved South";
            }
            else if (tokens[1] == "east" || tokens[1] == "right")
            {
                directionToMove = Direction.East;

                prospectiveMessage = "You moved East";
            }
            else if (tokens[1] == "west" || tokens[1] == "left")
            {
                directionToMove = Direction.West;

                prospectiveMessage = "You moved West";
            }

            bool successfulMove = Move(directionToMove, room);

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

        public bool Move(Direction direction, Room room)
        {
            Point prospectivePosition = mPosition;

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

            if (room.CanSpaceBeMovedTo(prospectivePosition))
            {
                mPosition = prospectivePosition;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
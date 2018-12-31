using System.Drawing;

namespace GridExplorerBot
{
    public class PlayerCharacter : DynamicObject
    {
        public override string Simulate(string command, Game game)
        {
            if (command.Length == 0)
            {
                return "";
            }

            string[] tokens = command.Split(' ');

            string outText = "Unknown command";

            if (tokens[0] == "go" || tokens[0] == "move")
            {
                outText = HandleMoveCommand(tokens, game.mRoom);
            }
            else if (tokens[0] == "take" || tokens[0] == "grab")
            {
                outText = HandleTakeCommand(tokens, game);
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

        private string HandleTakeCommand(string[] tokens, Game game)
        {
            if (tokens.Length != 2)
            {
                return "";
            }

            Objects.ID objectTypeToPickUp = Emoji.GetID(tokens[1]);

            if (objectTypeToPickUp == Objects.ID.Unknown)
            {
                return "";
            }

            DynamicObject objectToPickUp = game.mRoom.FindDynamicObjectAdjacentTo(mPosition, objectTypeToPickUp);

            if (objectToPickUp == null)
            {
                return "There wasn't " + tokens[1] + " nearby";
            }

            if (!objectToPickUp.CanBePickedUp())
            {
                return tokens[1] + " can't be picked up";
            }

            game.mRoom.MarkObjectForDeletion(objectToPickUp);
            game.mInventory.AddItem(objectToPickUp);

            return "You picked up " + objectToPickUp.Render();
        }
    }
}
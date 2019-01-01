using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GridExplorerBot
{
    public class PlayerCharacter : DynamicObject
    {
        public enum Status
        {
            Default,
            Confused
        }

        Status mStatus = Status.Default;

        static Regex moveRegex = new Regex("^(move|go)\\s(?<direction>[a-zA-Z]+?)$");
        static Regex pickUpRegex = new Regex("^(pick up|take|grab)\\s(?<object>[a-zA-Z]+?)$");
        static Regex dropRegex = new Regex("^drop\\s(?<object>[a-zA-Z]+?)\\s(?<direction>[a-zA-Z]+?)$");

        public override string Simulate(string command, Game game)
        {
            mStatus = Status.Default;

            if (command.Length == 0)
            {
                return "";
            }

            string outText = "";

            if (moveRegex.IsMatch(command))
            {
                Match match = moveRegex.Match(command);
                outText = HandleMoveCommand(match.Groups["direction"].Value, game.mRoom);
            }
            else if (pickUpRegex.IsMatch(command))
            {
                Match match = pickUpRegex.Match(command);
                outText = HandleTakeCommand(match.Groups["object"].Value, game);
            }
            else if (dropRegex.IsMatch(command))
            {
                Match match = dropRegex.Match(command);
                outText = HandleDropCommand(match.Groups["object"].Value, match.Groups["direction"].Value, game);
            }
            else
            {
                outText = "Unknown command";
                mStatus = Status.Confused;
            }

            return outText;
        }

        public override string Render()
        {
            string emoji = Emoji.Player.Default;

            if (mStatus == Status.Confused)
            {
                emoji = Emoji.Player.Confused;
            }

            return emoji;
        }

        private string HandleMoveCommand(string directionString, Room room)
        {
            string outText = "";

            Direction directionToMove = Room.GetDirection(directionString);
            string prospectiveMessage = "You moved " + directionToMove;

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
            if (direction == Direction.Unknown)
            {
                return false;
            }

            Point prospectivePosition = MathUtils.GetAdjacentPoint(mPosition, direction);

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

        private string HandleTakeCommand(string objectString, Game game)
        {
            Objects.ID objectTypeToPickUp = Emoji.GetID(objectString);

            if (objectTypeToPickUp == Objects.ID.Unknown)
            {
                return "";
            }

            DynamicObject objectToPickUp = game.mRoom.FindDynamicObjectAdjacentTo(mPosition, objectTypeToPickUp);

            if (objectToPickUp == null)
            {
                return "There wasn't " + objectString + " nearby";
            }

            if (!objectToPickUp.CanBePickedUp())
            {
                return objectString + " can't be picked up";
            }

            game.mRoom.MarkObjectForDeletion(objectToPickUp);
            game.mInventory.AddItem(objectToPickUp);

            return "You picked up " + objectToPickUp.Render();
        }

        private string HandleDropCommand(string objectString, string directionString, Game game)
        {
            Objects.ID objectTypeToDrop = Emoji.GetID(objectString);

            if (objectTypeToDrop == Objects.ID.Unknown)
            {
                return "";
            }

            int balance = game.mInventory.GetBalance(objectTypeToDrop);

            if (balance < 1)
            {
                return "You don't have " + objectString;
            }

            Direction direction = Room.GetDirection(directionString);

            if (direction == Direction.Unknown)
            {
                return "Invalid direction";
            }

            Point prospectiveDropPosition = MathUtils.GetAdjacentPoint(mPosition, direction);

            if (!game.mRoom.CanSpaceBeMovedTo(prospectiveDropPosition))
            {
                return "Space isn't open";
            }

            game.mInventory.RemoveItem(objectTypeToDrop);
            DynamicObject dynamicObject = Emoji.CreateObject(objectTypeToDrop);
            dynamicObject.mPosition = prospectiveDropPosition;
            dynamicObject.mType = objectTypeToDrop;
            game.mRoom.AddNewItem(dynamicObject);

            return "You dropped " + objectString;
        }
    }
}
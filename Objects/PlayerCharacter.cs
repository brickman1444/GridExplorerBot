using System.Drawing;
using System.Collections.Generic;

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

        public override string Simulate(string command, Game game)
        {
            mStatus = Status.Default;

            if (command.Length == 0)
            {
                return "";
            }

            string[] tokens = command.Split(' ');

            string outText = "";

            if (tokens[0] == "go" || tokens[0] == "move")
            {
                outText = HandleMoveCommand(tokens, game.mRoom);
            }
            else if (tokens[0] == "take" || tokens[0] == "grab")
            {
                outText = HandleTakeCommand(tokens, game);
            }
            else if (tokens[0] == "drop")
            {
                outText = HandleDropCommand(tokens, game);
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

        private string HandleMoveCommand(string[] tokens, Room room)
        {
            string outText = "";

            Direction directionToMove = Room.GetDirection(tokens[1]);
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

        private string HandleDropCommand(string[] tokens, Game game)
        {
            if (tokens.Length != 3)
            {
                return "";
            }

            Objects.ID objectTypeToDrop = Emoji.GetID(tokens[1]);

            if (objectTypeToDrop == Objects.ID.Unknown)
            {
                return "";
            }

            int balance = game.mInventory.GetBalance(objectTypeToDrop);

            if (balance < 1)
            {
                return "You don't have " + tokens[1];
            }

            Direction direction = Room.GetDirection(tokens[2]);

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

            return "You dropped " + tokens[1];
        }
    }
}
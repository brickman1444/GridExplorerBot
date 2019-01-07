using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;

namespace GridExplorerBot
{
    class Command
    {
        Regex mRegex = null;
        Match mMatch = null;
        string mSimpleText = "";

        static Dictionary<string, string> regexReplacementMap = new Dictionary<string, string>()
        {
            ["<"] = "(?<",
            [">"] = ">[a-z]+?.*?)",
        };

        public Command(string simpleText)
        {
            mSimpleText = simpleText;
            string regexText = simpleText;

            foreach ( var pair in regexReplacementMap)
            {
                regexText = regexText.Replace(pair.Key, pair.Value);
            }

            regexText = "^" + regexText + "$";

            mRegex = new Regex(regexText);
        }

        public bool IsMatch(string inputText)
        {
            mMatch = mRegex.Match(inputText);
            return mMatch.Success;
        }

        public string GetParameter(string parameterName)
        {
            return mMatch.Groups[parameterName].Value;
        }

        public string GetCompactSimpleText()
        {
            return mSimpleText;
        }
    }

    public class PlayerCharacter : DynamicObject
    {
        public enum Status
        {
            Default,
            Confused,
            Sleeping,
            Thinking,
        }

        Status mStatus = Status.Default;

        static Command moveCommand = new Command("(move|go|walk) <direction>");
        static Command pickUpCommand = new Command("(pick up|take|grab) <object>");
        static Command dropCommand = new Command("(drop|put down|place) <object> <direction>");
        static Command throwCommand = new Command("(toss|throw) <object> <direction>");
        static Command useCommand = new Command("use <actor> on <target>");
        static Command inspectCommand = new Command("(inspect|look) <direction>");
        static Command waitCommand = new Command("(wait|)");

        static Command[] commands = new Command[]
        {
            moveCommand,
            pickUpCommand,
            dropCommand,
            throwCommand,
            useCommand,
            inspectCommand,
            waitCommand,
        };

        public override string Simulate(string command, Game game)
        {
            mStatus = Status.Default;

            string outText = "";

            if (waitCommand.IsMatch(command))
            {
                outText = "You wait calmly";
                mStatus = Status.Sleeping;
            }
            else if (moveCommand.IsMatch(command))
            {
                outText = HandleMoveCommand(moveCommand.GetParameter("direction"), game);
            }
            else if (pickUpCommand.IsMatch(command))
            {
                outText = HandleTakeCommand(pickUpCommand.GetParameter("object"), game);
            }
            else if (dropCommand.IsMatch(command))
            {
                outText = HandleDropCommand(dropCommand.GetParameter("object"), dropCommand.GetParameter("direction"), game);
            }
            else if (throwCommand.IsMatch(command))
            {
                outText = HandleThrowCommand(throwCommand.GetParameter("object"), throwCommand.GetParameter("direction"), game);
            }
            else if (useCommand.IsMatch(command))
            {
                outText = HandleUseCommand(useCommand.GetParameter("actor"), useCommand.GetParameter("target"), game);
            }
            else if (inspectCommand.IsMatch(command))
            {
                outText = HandleInspectCommand(inspectCommand.GetParameter("direction"), game);
            }
            
            if (outText == "")
            {
                outText = "Unknown command";
                mStatus = Status.Confused;
            }

            return outText;
        }

        public override string Render()
        {
            switch (mStatus)
            {
                case Status.Default: return Emoji.Player.Default;
                case Status.Confused: return Emoji.Player.Confused;
                case Status.Sleeping: return Emoji.Player.Sleeping;
                case Status.Thinking: return Emoji.Player.Thinking;
            }

            Debug.Fail("Unknnown status");

            return Emoji.Player.Default;
        }

        private string HandleMoveCommand(string directionString, Game game)
        {
            string outText = "";

            Direction directionToMove = Room.GetDirection(directionString);
            string prospectiveMessage = "You moved " + directionToMove;

            bool successfulMove = Move(directionToMove, game);

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

        public bool Move(Direction direction, Game game)
        {
            if (direction == Direction.Unknown)
            {
                return false;
            }

            Point prospectivePosition = MathUtils.GetAdjacentPoint(mPosition, direction);

            Door doorObject = game.mRoom.FindFirstDynamicObject(prospectivePosition) as Door;

            if (doorObject != null)
            {
                game.SetTeleport(doorObject.mDestinationRoomID, doorObject.mDestinationSpawnLocation);
                return false;
            }

            if (game.mRoom.CanSpaceBeMovedTo(prospectivePosition))
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
                return "You couldn't find that nearby";
            }

            Point? staticObjectToPickup = game.mRoom.FindStaticObjectAdjacentTo(mPosition, objectTypeToPickUp);

            if (staticObjectToPickup != null)
            {
                return objectString + " can't be picked up";
            }

            DynamicObject objectToPickUp = game.mRoom.FindDynamicObjectAdjacentTo(mPosition, objectTypeToPickUp);

            if (objectToPickUp == null)
            {
                return "There isn't " + objectString + " nearby";
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
                return "You don't have that";
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
                return "Space to the " + direction + " isn't open";
            }

            game.mInventory.RemoveItem(objectTypeToDrop);
            game.mRoom.SpawnObject(objectTypeToDrop, prospectiveDropPosition);

            return "You dropped " + objectString;
        }

        string HandleThrowCommand(string objectString, string directionString, Game game)
        {
            Objects.ID objectTypeToThrow = Emoji.GetID(objectString);

            if (objectTypeToThrow == Objects.ID.Unknown)
            {
                return "You don't have that";
            }

            int balance = game.mInventory.GetBalance(objectTypeToThrow);

            if (balance < 1)
            {
                return "You don't have " + objectString;
            }

            Direction direction = Room.GetDirection(directionString);

            if (direction == Direction.Unknown)
            {
                return "Invalid direction";
            }

            Point throwVector = MathUtils.GetVector(direction);

            Point? furthestLandingPoint = null;

            for (int distanceThrown = 1; distanceThrown <= 3; distanceThrown++)
            {
                Point testPoint = mPosition + throwVector * distanceThrown;

                if (game.mRoom.CanSpaceBeMovedTo(testPoint))
                {
                    furthestLandingPoint = testPoint;
                }

                if (!game.mRoom.CanSpaceBeThrownThrough(testPoint))
                {
                    break;
                }
            }

            if (furthestLandingPoint == null)
            {
                return "No space to throw to the " + direction;
            }

            game.mInventory.RemoveItem(objectTypeToThrow);
            game.mRoom.SpawnObject(objectTypeToThrow, furthestLandingPoint.Value);

            return "You threw " + objectString;
        }

        string HandleUseCommand(string actorString, string targetString, Game game)
        {
            Objects.ID actorType = Emoji.GetID(actorString);

            if (actorType == Objects.ID.Unknown)
            {
                return "You don't have that";
            }

            int balance = game.mInventory.GetBalance(actorType);

            if (balance < 1)
            {
                return "You don't have " + actorString;
            }

            Objects.ID targetType = Emoji.GetID(targetString);

            if (targetType == Objects.ID.Unknown)
            {
                return "You can't find that";
            }

            DynamicObject targetObject = game.mRoom.FindDynamicObjectAdjacentTo(mPosition, targetType);

            if (targetObject == null)
            {
                return "There isn't " + targetString + " nearby";
            }

            string outText = "";

            if (actorType == Objects.ID.Pen)
            {
                outText = HandlePenUse(targetObject);
            }

            if (outText == "")
            {
                outText = "You don't think you can do that.";
            }

            return outText;
        }

        string HandlePenUse(DynamicObject targetObject)
        {
            string outText = "";

            if (targetObject is Lock)
            {
                Lock targetLock = targetObject as Lock;
                if (targetLock.CanBeUnlockedWithPen())
                {
                    targetLock.Unlock();
                    outText = "You unlocked the lock";
                }
            }

            return outText;
        }

        string HandleInspectCommand(string directionString, Game game)
        {
            Direction direction = Room.GetDirection(directionString);

            if (direction == Direction.Unknown)
            {
                return "Invalid direction";
            }

            Point inspectPosition = MathUtils.GetAdjacentPoint(mPosition, direction);

            DynamicObject dynamicObject = game.mRoom.FindFirstDynamicObject(inspectPosition);

            mStatus = Status.Thinking;

            if (dynamicObject != null)
            {
                return dynamicObject.GetDescriptionText();
            }
            else
            {
                return Descriptions.GetDescription(game.mRoom.GetStaticObject(inspectPosition));
            }
        }

        public static string GetCommandsListText()
        {
            var commandTexts = from command in commands
                               select command.GetCompactSimpleText();

            return string.Join('\n', commandTexts);
        }
    }
}
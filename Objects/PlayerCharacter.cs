using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace GridExplorerBot
{
    class Command
    {
        Regex mRegex = null;
        Match mMatch = null;
        public string mSimpleText = "";

        static Dictionary<string, string> regexReplacementMap = new Dictionary<string, string>()
        {
            ["<"] = "(?<",
            [">"] = ">[a-z]+?)",
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
    }

    public class PlayerCharacter : DynamicObject
    {
        public enum Status
        {
            Default,
            Confused,
            Sleeping,
        }

        Status mStatus = Status.Default;

        static Regex moveRegex = new Regex("^(move|go|walk)\\s(?<direction>[a-z]+?)$");
        static Regex pickUpRegex = new Regex("^(pick up|take|grab)\\s(?<object>[a-z]+?)$");
        static Command dropCommand = new Command("(drop|put down|place) <object> <direction>");
        static Regex throwRegex = new Regex("^(toss|throw)\\s(?<object>[a-z]+?)\\s(?<direction>[a-z]+?)$");
        static Regex useRegex = new Regex("^use\\s(?<actor>[a-z]+?)\\son\\s(?<target>[a-z]+?)$");
        static Regex inspectRegex = new Regex("^(inspect|look)\\s(?<direction>[a-z]+?)$");
        static Regex waitRegex = new Regex("^(|wait)$");

        public override string Simulate(string command, Game game)
        {
            mStatus = Status.Default;

            string outText = "";

            if (waitRegex.IsMatch(command))
            {
                outText = "You wait calmly";
                mStatus = Status.Sleeping;
            }
            else if (moveRegex.IsMatch(command))
            {
                Match match = moveRegex.Match(command);
                outText = HandleMoveCommand(match.Groups["direction"].Value, game.mRoom);
            }
            else if (pickUpRegex.IsMatch(command))
            {
                Match match = pickUpRegex.Match(command);
                outText = HandleTakeCommand(match.Groups["object"].Value, game);
            }
            else if (dropCommand.IsMatch(command))
            {
                outText = HandleDropCommand(dropCommand.GetParameter("object"), dropCommand.GetParameter("direction"), game);
            }
            else if (throwRegex.IsMatch(command))
            {
                Match match = throwRegex.Match(command);
                outText = HandleThrowCommand(match.Groups["object"].Value, match.Groups["direction"].Value, game);
            }
            else if (useRegex.IsMatch(command))
            {
                Match match = useRegex.Match(command);
                outText = HandleUseCommand(match.Groups["actor"].Value, match.Groups["target"].Value, game);
            }
            else if (inspectRegex.IsMatch(command))
            {
                Match match = inspectRegex.Match(command);
                outText = HandleInspectCommand(match.Groups["direction"].Value, game);
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
            switch (mStatus)
            {
                case Status.Default: return Emoji.Player.Default;
                case Status.Confused: return Emoji.Player.Confused;
                case Status.Sleeping: return Emoji.Player.Sleeping;
            }

            Debug.Fail("Unknnown status");

            return Emoji.Player.Default;
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
                return "";
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
                return "";
            }

            int balance = game.mInventory.GetBalance(actorType);

            if (balance < 1)
            {
                return "You don't have " + actorString;
            }

            Objects.ID targetType = Emoji.GetID(targetString);

            if (targetType == Objects.ID.Unknown)
            {
                return "";
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

            if (dynamicObject != null)
            {
                return Descriptions.GetDescription(dynamicObject.mType);
            }
            else
            {
                return Descriptions.GetDescription(game.mRoom.GetStaticObject(inspectPosition));
            }
        }
    }
}
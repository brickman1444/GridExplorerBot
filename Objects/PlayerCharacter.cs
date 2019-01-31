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
            [">"] = ">[a-z0-9]+.*)",
        };

        public Command(string simpleText)
        {
            mSimpleText = simpleText;
            string regexText = simpleText;

            foreach (var pair in regexReplacementMap)
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
            Frustrated,
            Tripping,
            SavoringFood,
            Vomiting,
        }

        delegate string CommandHandler(Command command, Game game);

        class CommandPair
        {
            public readonly Command mCommand;
            public readonly CommandHandler mHandler;

            public CommandPair(string command, CommandHandler handler)
            {
                mCommand = new Command(command);
                mHandler = handler;
            }
        }

        static Command resetCommand = new Command("(reset|restart)");
        static Command helpCommand = new Command("(help|list commands)");

        Status mStatus = Status.Default;
        CommandPair[] mCommands = null;

        public PlayerCharacter()
        {
            mCommands = new CommandPair[]{
                new CommandPair("(move|go|walk) <direction>(| <distance>)", this.HandleMoveCommand),
                new CommandPair("(pick up|take|grab) <object>", this.HandleTakeCommand),
                new CommandPair("(drop|put down|place) <object> <direction>", this.HandleDropCommand),
                new CommandPair("(toss|throw) <object> <direction>", this.HandleThrowCommand),
                new CommandPair("use <actor> on <target>", this.HandleUseCommand),
                new CommandPair("look at <object>", this.LookAtObjectCommand),
                new CommandPair("(inspect|look) <direction>", this.HandleInspectDirectionCommand),
                new CommandPair("(wait|sleep|rest|)", this.HandleWaitCommand),
                new CommandPair("eat <object>", this.HandleEatCommand),
            };
        }

        public override string Simulate(string command, Game game)
        {
            mStatus = Status.Default;

            string outText = "";

            foreach (CommandPair pair in mCommands)
            {
                if (pair.mCommand.IsMatch(command))
                {
                    outText = pair.mHandler(pair.mCommand, game);
                    break;
                }
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
                case Status.Frustrated: return Emoji.Player.SteamOutOfNose;
                case Status.Tripping: return Emoji.Player.Zany;
                case Status.SavoringFood: return Emoji.Player.SavoringFood;
                case Status.Vomiting: return Emoji.Player.Vomiting;
            }

            Debug.Fail("Unknown status");

            return Emoji.Player.Default;
        }

        private string HandleMoveCommand(Command moveCommand, Game game)
        {
            string directionString = moveCommand.GetParameter("direction");
            string distanceString = moveCommand.GetParameter("distance");

            Direction directionToMove = Room.GetDirection(directionString);

            if (directionToMove == Direction.Unknown)
            {
                mStatus = Status.Frustrated;
                return directionString + " isn't a valid direction.";
            }

            uint requestedDistanceToMove = 0;

            uint.TryParse(distanceString, out requestedDistanceToMove);

            // User could pass in 0 so we need to catch that even if the parsing succeeded
            if (requestedDistanceToMove == 0)
            {
                requestedDistanceToMove = 1;
            }

            bool wasFirstSpaceSuccessful = Move(directionToMove, game);

            if (!wasFirstSpaceSuccessful)
            {
                mStatus = Status.Frustrated;
                return "You could not move that direction.";
            }

            uint actualDistanceMoved = 1;

            for (; actualDistanceMoved < requestedDistanceToMove; actualDistanceMoved++)
            {
                bool successfulMove = Move(directionToMove, game);

                if (!successfulMove)
                {
                    break;
                }
            }

            return "You moved " + actualDistanceMoved + " " + directionString;
        }

        public bool Move(Direction direction, Game game)
        {
            if (direction == Direction.Unknown)
            {
                return false;
            }

            Point prospectivePosition = MathUtils.GetAdjacentPoint(mPosition, direction);

            Door doorObject = game.mRoom.GetFirstObject(prospectivePosition) as Door;

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

        private string HandleTakeCommand(Command takeCommand, Game game)
        {
            string objectString = takeCommand.GetParameter("object");

            Objects.ID objectTypeToPickUp = Emoji.GetID(objectString);

            if (objectTypeToPickUp == Objects.ID.Unknown)
            {
                return "You couldn't find that nearby";
            }

            StaticObject staticObjectToPickup = game.mRoom.FindStaticObjectAdjacentTo(mPosition, objectTypeToPickUp);

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

        private string HandleDropCommand(Command dropCommand, Game game)
        {
            string objectString = dropCommand.GetParameter("object");
            string directionString = dropCommand.GetParameter("direction");

            Objects.ID objectTypeToDrop = Emoji.GetID(objectString);

            if (objectTypeToDrop == Objects.ID.Unknown)
            {
                return "You don't have that";
            }

            if (!game.mInventory.Contains(objectTypeToDrop))
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

        string HandleThrowCommand(Command throwCommand, Game game)
        {
            string objectString = throwCommand.GetParameter("object");
            string directionString = throwCommand.GetParameter("direction");

            Objects.ID objectTypeToThrow = Emoji.GetID(objectString);

            if (objectTypeToThrow == Objects.ID.Unknown)
            {
                return "You don't have that";
            }

            if (!game.mInventory.Contains(objectTypeToThrow))
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

        string HandleUseCommand(Command useCommand, Game game)
        {
            string actorString = useCommand.GetParameter("actor");
            string targetString = useCommand.GetParameter("target");

            Objects.ID actorType = Emoji.GetID(actorString);

            if (actorType == Objects.ID.Unknown)
            {
                mStatus = Status.Frustrated;
                return "You don't have that";
            }

            if (!game.mInventory.Contains(actorType))
            {
                mStatus = Status.Frustrated;
                return "You don't have " + actorString;
            }

            Objects.ID targetType = Emoji.GetID(targetString);

            if (targetType == Objects.ID.Unknown)
            {
                mStatus = Status.Frustrated;
                return "You can't find that";
            }

            GridObject targetObject = game.mRoom.FindObjectAdjacentTo(mPosition, targetType);

            if (targetObject == null)
            {
                mStatus = Status.Frustrated;
                return "There isn't " + targetString + " nearby";
            }

            string outText = "";

            if (targetType == Objects.ID.Lock)
            {
                Lock lockObject = targetObject as Lock;
                if (lockObject.CanBeUnlockedBy(actorType))
                {
                    lockObject.Unlock();
                    outText = "You unlocked the door with the " + actorString;
                }
            }

            if (outText == "")
            {
                outText = "You don't think you can do that.";
                mStatus = Status.Frustrated;
            }

            return outText;
        }

        string HandleInspectDirectionCommand(Command inspectCommand, Game game)
        {
            string directionString = inspectCommand.GetParameter("direction");

            Direction direction = Room.GetDirection(directionString);

            if (direction == Direction.Unknown)
            {
                return "Invalid direction";
            }

            Point inspectPosition = MathUtils.GetAdjacentPoint(mPosition, direction);

            GridObject inspectObject = game.mRoom.GetFirstObject(inspectPosition);

            mStatus = Status.Thinking;

            return inspectObject.GetDescriptionText();
        }

        string LookAtObjectCommand(Command lookAtCommand, Game game)
        {
            string objectString = lookAtCommand.GetParameter("object");

            Objects.ID objectType = Emoji.GetID(objectString);

            if (objectType == Objects.ID.Unknown)
            {
                mStatus = Status.Frustrated;
                return "You can't even figure out what that is.";
            }

            GridObject targetObject = game.mRoom.GetNearestObject(objectType, mPosition);

            if (targetObject == null)
            {
                mStatus = Status.Frustrated;
                return "You don't see that here.";
            }

            return targetObject.GetDescriptionText();
        }

        string HandleWaitCommand(Command waitCommand, Game game)
        {
            mStatus = Status.Sleeping;
            return "You wait calmly";
        }

        string HandleEatCommand(Command eatCommand, Game game)
        {
            string objectString = eatCommand.GetParameter("object");

            Objects.ID objectType = Emoji.GetID(objectString);

            if (objectType == Objects.ID.Unknown)
            {
                mStatus = Status.Frustrated;
                return "You don't have that";
            }

            if (!game.mInventory.Contains(objectType))
            {
                mStatus = Status.Frustrated;
                return "You don't have " + objectString + " in your inventory.";
            }

            game.mInventory.RemoveItem(objectType);

            if ( ObjectTraits.GetObjectTraits(objectType).mIsEdible )
            {
                if ( ObjectTraits.GetObjectTraits(objectType).mCausesHallucinations )
                {
                    mStatus = Status.Tripping;
                    game.mRoom.mIsRenderingHallucination = true;
                    return Emoji.Symbols.Dizzy + " Whoa " + Emoji.Symbols.Dizzy;
                }
                else if (ObjectTraits.GetObjectTraits(objectType).mIsHealthyToEat)
                {
                    mStatus = Status.SavoringFood;
                    return "That was delicious! You feel a renewed sense of determination.";
                }
                else
                {
                    Point? spaceToDropObject = game.mRoom.FindOpenSpaceAdjacentTo(mPosition);

                    if (spaceToDropObject != null)
                    {
                        game.mRoom.SpawnObject(objectType, spaceToDropObject.Value);
                    }

                    mStatus = Status.Vomiting;
                    return "Oh, that was a terrible idea.";
                }
            }
            else
            {
                mStatus = Status.Frustrated;
                return "You can't eat that!";
            }
        }

        public string GetCommandsListText()
        {
            var commandTexts = from command in mCommands
                               select command.mCommand.GetCompactSimpleText();

            return string.Join('\n', commandTexts);
        }

        public static bool MatchesResetCommand(string input)
        {
            return resetCommand.IsMatch(input);
        }

        public static bool MatchesHelpCommand(string input)
        {
            return helpCommand.IsMatch(input);
        }
    }
}
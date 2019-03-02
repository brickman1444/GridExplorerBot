using System.Diagnostics;

namespace GridExplorerBot
{
    public class GuardSetup : GridObjectSetup
    {
        public readonly Point mRelaxedPosition;

        public GuardSetup(Point startingPosition, Point relaxedPosition) : base(Emoji.People.Guard, startingPosition)
        {
            mRelaxedPosition = relaxedPosition;
        }

        public override GridObject CreateObject()
        {
            Guard guard = base.CreateObject() as Guard;

            Debug.Assert(guard != null);

            guard.Setup(this);

            return guard;
        }
    }

    public class Guard : DynamicObject
    {
        NPCIdentifier mNPCIdentity = new NPCIdentifier();
        Point mStartingPosition;
        Point mRelaxedPosition;

        public void Setup(GuardSetup setup)
        {
            mStartingPosition = setup.mStartingPosition;
            mRelaxedPosition = setup.mRelaxedPosition;
        }

        public override void OnRoomCreated(Game game)
        {
            MoveToGuardPosition(game);
        }

        public override string Simulate(string command, Game game)
        {
            MoveToGuardPosition(game);

            return "";
        }

        static bool DoesPlayerLookLikeAScientist(Game game)
        {
            return game.mInventory.Contains(Objects.ID.LabCoat);
        }

        void MoveToGuardPosition(Game game)
        {
            bool looksLikeScientist = DoesPlayerLookLikeAScientist(game);

            Point prospectivePosition = looksLikeScientist ? mRelaxedPosition : mStartingPosition;

            if (mPosition == prospectivePosition)
            {
                return;
            }

            if (!game.mRoom.CanSpaceBeMovedTo(prospectivePosition))
            {
                return;
            }

            mPosition = prospectivePosition;
        }

        public override string Render()
        {
            return mNPCIdentity.GetEmojiVariant(Emoji.GetEmoji(GetTypeID()));
        }

        public override void Stream(SaveStream stream)
        {
            base.Stream(stream);

            mNPCIdentity.Stream(stream);
            stream.Stream(ref mStartingPosition);
            stream.Stream(ref mRelaxedPosition);
        }

        public override string TalkTo(Objects.ID subject, Game game)
        {
            if (subject == Objects.ID.Unknown)
            {
                if (DoesPlayerLookLikeAScientist(game))
                {
                    return "Greetings, scientist. Welcome to the Research Facility.";
                }
                else
                {
                    return "Only scientists allowed in. You don't even look like a scientist.";
                }
            }
            else
            {
                if (subject == Objects.ID.LabCoat)
                {
                    return "Yep that's the uniform of all the scientists here.";
                }
                else
                {
                    return "I don't know anything about that.";
                }
            }
        }
    }
}
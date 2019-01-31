using System.Diagnostics;

namespace GridExplorerBot
{
    public class GuardSetup : GridObjectSetup
    {
        public readonly Point mRelaxedPosition;

        public GuardSetup(Point startingPosition, Point relaxedPosition) : base(Emoji.GetRandomEmoji(Objects.ID.Guard), startingPosition)
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
        int mDisplayEmojiIndex;
        Point mStartingPosition;
        Point mRelaxedPosition;

        public void Setup(GuardSetup setup)
        {
            mDisplayEmojiIndex = Emoji.GetEmojiIndex(mType, setup.mDisplayText);
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

        void MoveToGuardPosition(Game game)
        {
            bool looksLikeScientist = game.mInventory.Contains(Objects.ID.LabCoat);

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
            return Emoji.GetEmoji(mType, mDisplayEmojiIndex);
        }

        public override void Stream(SaveStream stream)
        {
            base.Stream(stream);

            stream.Stream(ref mDisplayEmojiIndex, SaveUtils.GetNumBitsToStoreValue(StringUtils.GetNumPersonEmojiVariations()));
            stream.Stream(ref mStartingPosition);
            stream.Stream(ref mRelaxedPosition);
        }
    }
}
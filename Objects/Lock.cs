using System.Collections.Generic;
using System.Diagnostics;

namespace GridExplorerBot
{
    public class LockSetup : GridObjectSetup
    {
        public LockSetup(string inDisplayText, Point inStartingPosition) : base(inDisplayText, inStartingPosition)
        {
        }

        public override GridObject CreateObject()
        {
            Lock lockObject = base.CreateObject() as Lock;
            
            Debug.Assert(lockObject != null);

            lockObject.Setup(this);

            return lockObject;
        }
    }

    public class Lock : StaticObject
    {
        public enum Status
        {
            Locked,
            Unlocked,
            LockedWithPen,
            LockedWithKey
        }

        Status mStatus { get => (Status)_mStatus; set => _mStatus = (int)value; }
        int _mStatus = 0;

        public override void Stream(SaveStream stream)
        {
            base.Stream(stream);

            stream.Stream(ref _mStatus, 2);
        }

        public void Setup(LockSetup setup)
        {
            if (setup.mDisplayText == Emoji.Environment.Locked)
            {
                mStatus = Status.Locked;
            }
            else if (setup.mDisplayText == Emoji.Environment.Unlocked)
            {
                mStatus = Status.Unlocked;
            }
            else if (setup.mDisplayText == Emoji.Environment.LockedWithKey)
            {
                mStatus = Status.LockedWithKey;
            }
            else if (setup.mDisplayText == Emoji.Environment.LockedWithPen)
            {
                mStatus = Status.LockedWithPen;
            }
        }

        public override string Render()
        {
            return Emoji.Environment.Door;
        }

        public override bool CanBeMovedThrough()
        {
            return mStatus == Status.Unlocked;
        }

        public override string GetDescriptionText()
        {
            string outText = "A door.";

            switch (mStatus)
            {
                case Status.Locked: outText = "A locked door."; break;
                case Status.Unlocked: outText = "An unlocked door"; break;
                case Status.LockedWithKey: outText = "A door. It's locked " + Emoji.Environment.LockedWithKey; break;
                case Status.LockedWithPen: outText = "A door. It's locked " + Emoji.Environment.LockedWithPen; break;
            }

            return outText;
        }

        public bool CanBeUnlockedBy(Objects.ID actorID)
        {
            if (mStatus == Status.LockedWithKey
            && actorID == Objects.ID.Key)
            {
                return true;
            }
            else if (mStatus == Status.LockedWithPen
            && actorID == Objects.ID.Pen)
            {
                return true;
            }

            return false;
        }

        public void Unlock()
        {
            mStatus = Status.Unlocked;
        }
    }
}
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

    public class Lock : DynamicObject
    {
        public enum Status
        {
            Locked,
            Unlocked,
            LockedWithPen,
            LockedWithKey
        }

        Status mStatus = Status.Locked;

        public override void Save(BitStreams.BitStream stream)
        {
            base.Save(stream);

            stream.WriteByte((byte)mStatus, 2);
        }

        public override void Load(BitStreams.BitStream stream)
        {
            base.Load(stream);

            mStatus = (Status)stream.ReadByte(2);
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
            string emoji = Emoji.Environment.Locked;

            switch (mStatus)
            {
                case Status.Locked: emoji = Emoji.Environment.Locked; break;
                case Status.Unlocked: emoji = Emoji.Environment.Unlocked; break;
                case Status.LockedWithKey: emoji = Emoji.Environment.LockedWithKey; break;
                case Status.LockedWithPen: emoji = Emoji.Environment.LockedWithPen; break;
            }

            return emoji;
        }

        public override bool CanBeMovedThrough()
        {
            return mStatus == Status.Unlocked;
        }

        public bool CanBeUnlockedWithPen()
        {
            return mStatus == Status.LockedWithPen;
        }

        public bool CanBeUnlockedWithKey()
        {
            return mStatus == Status.LockedWithKey;
        }

        public void Unlock()
        {
            mStatus = Status.Unlocked;
        }
    }
}
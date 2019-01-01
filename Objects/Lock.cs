using System.Collections.Generic;

namespace GridExplorerBot
{
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

        protected override void Save(ref Stack<byte> bytes)
        {
            base.Save(ref bytes);

            bytes.Push( System.Convert.ToByte(mStatus));
        }

        protected override void Load(ref Stack<byte> bytes)
        {
            base.Load(ref bytes);

            mStatus = (Status)bytes.Pop();
        }

        protected override void SetupState(DynamicObjectSetup setup)
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

            switch(mStatus)
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
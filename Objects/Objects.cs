using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;

namespace GridExplorerBot
{
    public static class Objects
    {
        public enum ID
        {
            Unknown,
            PlayerCharacter,
            Empty,
            Wall,
            Elephant,
            Pen,
            Lock,
        }
    }

    public class DynamicObject
    {
        public Point mPosition = new Point();
        public Objects.ID mType = Objects.ID.Unknown;

        public DynamicObject()
        {

        }

        public virtual bool CanBePickedUp() { return false; }

        public void Setup(DynamicObjectSetup setup)
        {
            mType = Emoji.GetID(setup.mDisplayText);
            mPosition = setup.mStartingPosition;
            SetupState(setup);
        }

        protected virtual void SetupState(DynamicObjectSetup setup)
        {
            Debug.Assert(Emoji.GetEmojiIndex(mType, setup.mDisplayText) == 0, "To use a non-default emoji you'll have to override Setup() and save some state");
        }

        public string Save()
        {
            Stack<byte> bytes = new Stack<byte>();

            Save(ref bytes);

            return System.Convert.ToBase64String(bytes.ToArray());
        }

        public void Load(string inSaveData)
        {
            Stack<byte> bytes = new Stack<byte>(System.Convert.FromBase64String(inSaveData));

            Load(ref bytes);
        }

        protected virtual void Save(ref Stack<byte> bytes)
        {
            bytes.Push((byte)mType); // 127 values 7 bits

            byte positionIndex = (byte)(mPosition.X * Game.numRoomColumns + mPosition.Y); // 81 values 7 bits

            bytes.Push(positionIndex);
        }

        protected virtual void Load(ref Stack<byte> bytes)
        {
            mType = (Objects.ID)bytes.Pop();

            byte positionIndex = bytes.Pop();

            mPosition.X = positionIndex / Game.numRoomColumns;
            mPosition.Y = positionIndex % Game.numRoomColumns;
        }

        public virtual string Simulate(string inCommand, Game room)
        {
            return "";
        }

        public virtual string Render()
        {
            return Emoji.GetEmoji(mType);
        }
    }
}
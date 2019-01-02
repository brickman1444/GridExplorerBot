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
        public virtual bool CanBeMovedThrough() { return false; }

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

        public virtual void Save(BitStreams.BitStream stream)
        {
            stream.WriteByte((byte)mType, 7);  // 127 7 bits

            byte positionIndex = (byte)(mPosition.X * Game.numRoomColumns + mPosition.Y); // 81 values 7 bits

            stream.WriteByte(positionIndex, 7);
        }

        public virtual void Load(BitStreams.BitStream stream)
        {
            mType = (Objects.ID)stream.ReadByte(7);

            byte positionIndex = stream.ReadByte(7);

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
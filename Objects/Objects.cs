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
        }
    }

    
    public class DynamicObject
    {
        private int mDisplayEmojiIndex = -1;
        public Point mPosition = new Point();
        public Objects.ID mType = Objects.ID.Unknown;

        public DynamicObject()
        {

        }

        public void Setup( DynamicObjectSetup setup )
        {
            mType = Emoji.GetID(setup.mDisplayText);
            mDisplayEmojiIndex = Emoji.GetEmojiIndex(mType, setup.mDisplayText);
            mPosition = setup.mStartingPosition;
        }

        public string Save()
        {
            List<byte> bytes = new List<byte>();

            bytes.Add( (byte)mType ); // 127 values 7 bits

            bytes.Add( (byte)mDisplayEmojiIndex ); // 63 values 6 bits

            byte positionIndex = (byte)(mPosition.X * Game.numRoomColumns + mPosition.Y); // 81 values 7 bits

            bytes.Add( positionIndex );

            return System.Convert.ToBase64String(bytes.ToArray());
        }

        public void Load(string inSaveData)
        {
            byte[] bytes = System.Convert.FromBase64String(inSaveData);

            mType = (Objects.ID)bytes[0];

            mDisplayEmojiIndex = bytes[1];

            byte positionIndex = bytes[2];

            mPosition.X = positionIndex / Game.numRoomColumns;
            mPosition.Y = positionIndex % Game.numRoomColumns;
        }

        public virtual string Simulate(string inCommand, Room room)
        {
            return "";
        }

        public string Render()
        {
            return Emoji.GetEmoji(mType, mDisplayEmojiIndex);
        }
    }
}
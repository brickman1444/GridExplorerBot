using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GridExplorerBot
{
    public static class Emoji
    {
        public static class Player
        {
            public static string Default = "😀";
            public static string Confused = "😕";
            public static string Sleeping = "😴";
            public static string Thinking = "🤔";
        }

        public static class Environment
        {
            public static string LockedWithPen = "🔏";
            public static string LockedWithKey = "🔐";
            public static string Locked = "🔒";
            public static string Unlocked = "🔓";
            public static string SpiderWeb = "🕸️";
            public static string HoneyPot = "🍯";
            public static string Wall = "⬛";
            public static string Empty = "⬜";
            public static string Door = "🚪";
            public static string Vase = "🏺";
            public static string Globe = "🌍";
        }

        public static class InventoryItems
        {
            public static string Pen = "🖋️";
            public static string Candle = "🕯️";
        }

        public static class Animals
        {
            public static string Elephant = "🐘";
            public static string Spider = "🕷️";
            public static string Bee = "🐝";
        }

        public static class Plants
        {
            public static string Rose = "🌹";
            public static string WiltedRose = "🥀";
        }

        public static Objects.ID GetID(string inputText)
        {
            foreach (KeyValuePair<Objects.ID, ObjectTraits> pair in ObjectTraits.idToTraitsMap)
            {
                if (pair.Value.mInputTokens.Contains(inputText)
                || pair.Value.mDisplayEmoji.Contains(inputText))
                {
                    return pair.Key;
                }
            }

            Debug.Fail("Couldn't find id for %s", inputText);

            return Objects.ID.Unknown;
        }

        public static string GetEmoji(Objects.ID id, int index = 0)
        {
            string[] displayChars = ObjectTraits.GetObjectTraits(id).mDisplayEmoji;

            Debug.Assert(index < displayChars.Length);

            return displayChars[index];
        }

        public static int GetEmojiIndex(Objects.ID id, string emoji)
        {
            string[] displayChars = ObjectTraits.GetObjectTraits(id).mDisplayEmoji;

            for (int index = 0; index < displayChars.Length; index++)
            {
                if (displayChars[index] == emoji)
                {
                    return index;
                }
            }

            Debug.Fail("Couldn't find emoji");

            return -1;
        }

        public static DynamicObject CreateObject(Objects.ID id)
        {
            Type dynamicObjectType = ObjectTraits.GetObjectTraits(id).mDynamicObjectType;

            Debug.Assert(dynamicObjectType == typeof(DynamicObject) || dynamicObjectType.IsSubclassOf(typeof(DynamicObject)));

            return (DynamicObject)Activator.CreateInstance(dynamicObjectType);
        }

        public static DynamicObject CreateObject(BitStreams.BitStream stream)
        {
            // Because there's no interface to get the current offset of the stream,
            // we can't progress forward and then seek back to where we were.
            Objects.ID id = (Objects.ID)stream.ReadByte(7);

            // HACK
            for (int returnedBit = 0; returnedBit < 7; returnedBit++)
            {
                stream.ReturnBit();
            }

            return CreateObject(id);
        }
    }
}
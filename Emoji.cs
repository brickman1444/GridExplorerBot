using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GridExplorerBot
{
    public static class Emoji
    {
        public static class Player
        {
            public static string Default = "😀";
            public static string Confused = "😕";
        }

        public static class Environment
        {
            public static string LockedWithPen = "🔏";
            public static string LockedWithKey = "🔐";
            public static string Locked = "🔒";
            public static string Unlocked = "🔓";
            public static string SpiderWeb = "🕸️";
            public static string HoneyPot = "🍯";
        }

        public static string Pen = "🖋️";
        public static string Elephant = "🐘";

        static Dictionary<Objects.ID, string[]> idToCharsMap = new Dictionary<Objects.ID, string[]>()
        {
            [Objects.ID.PlayerCharacter] = new string[]{Player.Default, Player.Confused},
            [Objects.ID.Wall] = new string[]{"⬛"},
            [Objects.ID.Empty] = new string[]{"⬜"},
            [Objects.ID.Elephant] = new string[]{Elephant},
            [Objects.ID.Pen] = new string[]{Pen},
            [Objects.ID.Lock] = new string[]{Environment.Locked, Environment.Unlocked, Environment.LockedWithKey, Environment.LockedWithPen},
            [Objects.ID.SpiderWeb] = new string[]{Environment.SpiderWeb},
            [Objects.ID.HoneyPot] = new string[]{Environment.HoneyPot},
        };

        static Dictionary<Objects.ID, Type> idToTypeMap = new Dictionary<Objects.ID, Type>()
        {
            [Objects.ID.PlayerCharacter] = typeof(PlayerCharacter),
            [Objects.ID.Elephant] = typeof(Elephant),
            [Objects.ID.Pen] = typeof(InventoryObject),
            [Objects.ID.Lock] = typeof(Lock),
        };

        static Dictionary<string, Objects.ID> tokenToIDMap = new Dictionary<string, Objects.ID>()
        {
            ["pen"] = Objects.ID.Pen,
            ["lock"] = Objects.ID.Lock,
        };

        public static Objects.ID GetID(string inputText)
        {
            foreach (KeyValuePair<Objects.ID, string[]> pair in idToCharsMap)
            {
                foreach (string displayString in pair.Value)
                {
                    if (displayString == inputText)
                    {
                        return pair.Key;
                    }
                }
            }

            if (tokenToIDMap.ContainsKey(inputText))
            {
                return tokenToIDMap[inputText];
            }

            Debug.Fail("Couldn't find id for %s", inputText);

            return Objects.ID.Unknown;
        }

        public static string GetEmoji(Objects.ID id, int index = 0)
        {
            Debug.Assert(idToCharsMap.ContainsKey(id));

            string[] displayChars = idToCharsMap.GetValueOrDefault(id, new string[] { "⬜" });

            Debug.Assert(index < displayChars.Length);

            return displayChars[index];
        }

        public static int GetEmojiIndex(Objects.ID id, string emoji)
        {
            string[] displayChars = idToCharsMap.GetValueOrDefault(id, new string[] { "⬜" });

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
            Type dynamicObjectType = idToTypeMap.GetValueOrDefault(id, null);

            if (dynamicObjectType == null)
            {
                dynamicObjectType = typeof(DynamicObject);
            }

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
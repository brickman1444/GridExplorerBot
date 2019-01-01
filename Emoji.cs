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
        }

        public static string Pen = "🖋️";
        public static string Elephant = "🐘";

        static Dictionary<Objects.ID, string[]> idToCharsMap = new Dictionary<Objects.ID, string[]>(
            new KeyValuePair<Objects.ID, string[]>[] {
                new KeyValuePair<Objects.ID, string[]>( Objects.ID.PlayerCharacter, new string[]{Player.Default, Player.Confused} ),
                new KeyValuePair<Objects.ID, string[]>( Objects.ID.Wall, new string[]{"⬛"} ),
                new KeyValuePair<Objects.ID, string[]>( Objects.ID.Empty, new string[]{"⬜"} ),
                new KeyValuePair<Objects.ID, string[]>( Objects.ID.Elephant, new string[]{Elephant} ),
                new KeyValuePair<Objects.ID, string[]>( Objects.ID.Pen, new string[]{Pen} ),
                new KeyValuePair<Objects.ID, string[]>( Objects.ID.Lock, new string[]{Environment.Locked, Environment.Unlocked, Environment.LockedWithKey, Environment.LockedWithPen} ), });

        static Dictionary<Objects.ID, Type> idToTypeMap = new Dictionary<Objects.ID, Type>(
            new KeyValuePair<Objects.ID, Type>[] {
                new KeyValuePair<Objects.ID, Type>( Objects.ID.PlayerCharacter, typeof(PlayerCharacter) ),
                new KeyValuePair<Objects.ID, Type>( Objects.ID.Elephant, typeof(Elephant) ),
                new KeyValuePair<Objects.ID, Type>( Objects.ID.Pen, typeof(InventoryObject) ),
                new KeyValuePair<Objects.ID, Type>( Objects.ID.Lock, typeof(Lock) ),
            }
        );

        static Dictionary<string, Objects.ID> tokenToIDMap = new Dictionary<string, Objects.ID>(
            new KeyValuePair<string, Objects.ID>[] {
                new KeyValuePair<string, Objects.ID>( "pen", Objects.ID.Pen ),
            }
        );

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

            return Objects.ID.Unknown;
        }

        public static string GetEmoji(Objects.ID id, int index = 0)
        {
            string[] displayChars = idToCharsMap.GetValueOrDefault(id, new string[] { "⬜" });

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

        public static DynamicObject CreateObject(string saveData)
        {
            // Create a fake object with the real Load code. This
            // ensures that this code and the Load code always
            // work the same.
            DynamicObject tempObject = new DynamicObject();
            tempObject.Load(saveData);

            Objects.ID id = tempObject.mType;

            return CreateObject(id);
        }
    }
}
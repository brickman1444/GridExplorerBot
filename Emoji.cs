using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GridExplorerBot
{
    public static class Emoji
    {
        static Dictionary<Objects.ID, string[]> idToCharsMap = new Dictionary<Objects.ID, string[]>( 
            new KeyValuePair<Objects.ID, string[]>[] { 
                new KeyValuePair<Objects.ID, string[]>( Objects.ID.PlayerCharacter, new string[]{"😀"} ),
                new KeyValuePair<Objects.ID, string[]>( Objects.ID.Wall, new string[]{"⬛"} ),
                new KeyValuePair<Objects.ID, string[]>( Objects.ID.Empty, new string[]{"⬜"} ), 
                new KeyValuePair<Objects.ID, string[]>( Objects.ID.Elephant, new string[]{"🐘"} ), } );

        static Dictionary<Objects.ID, Type> idToTypeMap = new Dictionary<Objects.ID, Type>(
            new KeyValuePair<Objects.ID,Type>[] {
                new KeyValuePair<Objects.ID, Type>( Objects.ID.PlayerCharacter, typeof(PlayerCharacter) ),
                new KeyValuePair<Objects.ID, Type>( Objects.ID.Elephant, typeof(Elephant) ),
            }
        );

        public static Objects.ID GetID(string inputText)
        {
            foreach ( KeyValuePair<Objects.ID, string[]> pair in idToCharsMap)
            {
                foreach ( string displayString in pair.Value )
                {
                    if ( displayString == inputText )
                    {
                        return pair.Key;
                    }
                }
            }

            return Objects.ID.Unknown;
        }

        public static string GetEmoji(Objects.ID id, int index)
        {
            string[] displayChars = idToCharsMap.GetValueOrDefault(id, new string[]{ "⬜" });

            return displayChars[index];
        }

        public static int GetEmojiIndex(Objects.ID id, string emoji)
        {
            string[] displayChars = idToCharsMap.GetValueOrDefault(id, new string[]{ "⬜" });

            for ( int index = 0; index < displayChars.Length; index++)
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

            Debug.Assert( dynamicObjectType == typeof(DynamicObject) || dynamicObjectType.IsSubclassOf(typeof(DynamicObject)));

            return (DynamicObject)Activator.CreateInstance(dynamicObjectType);
        }

        public static DynamicObject CreateObject(string saveData)
        {
            byte[] bytes = System.Convert.FromBase64String(saveData);

            Objects.ID id = (Objects.ID)bytes[0];

            return CreateObject(id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GridExplorerBot
{
    public static class Emoji
    {
        static Dictionary<string,Objects.ID> charToIDMap = new Dictionary<string, Objects.ID>( 
            new KeyValuePair<string,Objects.ID>[] { 
                new KeyValuePair<string, Objects.ID>( "😀", Objects.ID.PlayerCharacter ),
                new KeyValuePair<string, Objects.ID>( "⬛", Objects.ID.Wall ),
                new KeyValuePair<string, Objects.ID>( "⬜", Objects.ID.Empty ), 
                new KeyValuePair<string, Objects.ID>( "🐘", Objects.ID.Elephant ), } );

        static Dictionary<Objects.ID, Type> idToTypeMap = new Dictionary<Objects.ID, Type>(
            new KeyValuePair<Objects.ID,Type>[] {
                new KeyValuePair<Objects.ID, Type>( Objects.ID.PlayerCharacter, typeof(PlayerCharacter) ),
                new KeyValuePair<Objects.ID, Type>( Objects.ID.Elephant, typeof(Elephant) ),
            }
        );

        public static Objects.ID GetID(string inputText)
        {
            Objects.ID objectID = charToIDMap.GetValueOrDefault(inputText, Objects.ID.Unknown);

            return objectID;
        }

        public static string GetEmoji(Objects.ID id)
        {
            foreach ( KeyValuePair<string, Objects.ID> pair in charToIDMap)
            {
                if ( pair.Value == id)
                {
                    return pair.Key;
                }
            }

            return "⬜";
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
            string[] tokens = saveData.Split(',');

            Objects.ID id = (Objects.ID)int.Parse(tokens[0]);

            return CreateObject(id);
        }
    }
}
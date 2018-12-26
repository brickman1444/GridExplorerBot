using System.Collections.Generic;

namespace GridExplorerBot
{
    public static class Emoji
    {
        static Dictionary<string,Objects.ID> charToIDMap = new Dictionary<string, Objects.ID>( 
            new KeyValuePair<string,Objects.ID>[] { 
                new KeyValuePair<string, Objects.ID>( "😀", Objects.ID.PlayerCharacter ),
                new KeyValuePair<string, Objects.ID>( "⬛", Objects.ID.Wall ),
                new KeyValuePair<string, Objects.ID>( "⬜", Objects.ID.Empty ), 
                new KeyValuePair<string, Objects.ID>( "🐘", Objects.ID.Empty ), } );

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
    }
}
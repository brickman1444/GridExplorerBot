using System.Collections.Generic;

namespace GridExplorerBot
{
    public static class Descriptions
    {
        static Dictionary<Objects.ID, string> idToDescriptionMap = new Dictionary<Objects.ID, string>(
            new KeyValuePair<Objects.ID, string>[] {
                new KeyValuePair<Objects.ID, string>( Objects.ID.PlayerCharacter, "The loyal player character." ),
                new KeyValuePair<Objects.ID, string>( Objects.ID.Elephant, "A friendly elephant. They say hello!" ),
                new KeyValuePair<Objects.ID, string>( Objects.ID.Pen, "A simple pen." ),
                new KeyValuePair<Objects.ID, string>( Objects.ID.Lock, "A lock" ),
                new KeyValuePair<Objects.ID, string>( Objects.ID.Empty, "There's nothing there." ),
                new KeyValuePair<Objects.ID, string>( Objects.ID.Wall, "A sturdy wall." ),
            }
        );

        public static string GetDescription(Objects.ID type)
        {
            return idToDescriptionMap[type];
        }
    }
}
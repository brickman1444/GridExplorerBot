using System.Collections.Generic;

namespace GridExplorerBot
{
    public static class Descriptions
    {
        static Dictionary<Objects.ID, string> idToDescriptionMap = new Dictionary<Objects.ID, string>()
        {
            [Objects.ID.PlayerCharacter] = "The loyal player character.",
            [Objects.ID.Elephant] = "A friendly elephant. They say hello!",
            [Objects.ID.Pen] = "A simple pen.",
            [Objects.ID.Lock] = "A lock",
            [Objects.ID.Empty] = "There's nothing there.",
            [Objects.ID.Wall] = "A sturdy wall.",
        };

        public static string GetDescription(Objects.ID type)
        {
            return idToDescriptionMap[type];
        }
    }
}
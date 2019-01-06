using System.Collections.Generic;

namespace GridExplorerBot
{
    public static class Descriptions
    {
        public static string GetDescription(Objects.ID type)
        {
            return ObjectTraits.GetObjectTraits(type).mLookDescription;
        }
    }
}
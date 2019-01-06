using System.Collections.Generic;
using System.Diagnostics;

namespace GridExplorerBot
{
    public class ObjectTraits
    {
        public string[] mDisplayEmoji = null;
        public string[] mInputTokens = null;
        public System.Type mDynamicObjectType = null;
        public string mLookDescription = "";
        public bool mCanStaticObjectBeMovedThrough = false;

        public static Dictionary<Objects.ID, ObjectTraits> idToTraitsMap = new Dictionary<Objects.ID, ObjectTraits>()
        {
            [Objects.ID.PlayerCharacter] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Player.Default, Emoji.Player.Confused},
                mInputTokens = new string[]{},
                mDynamicObjectType = typeof(PlayerCharacter),
                mLookDescription = "The loyal player character.",
                mCanStaticObjectBeMovedThrough = false,
            },
            [Objects.ID.Wall] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Wall},
                mInputTokens = new string[]{},
                mDynamicObjectType = null,
                mLookDescription = "A sturdy wall.",
                mCanStaticObjectBeMovedThrough = false,
            },
            [Objects.ID.Empty] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Empty},
                mInputTokens = new string[]{},
                mDynamicObjectType = null,
                mLookDescription = "There's nothing there.",
                mCanStaticObjectBeMovedThrough = true,
            },
            [Objects.ID.Elephant] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Elephant},
                mInputTokens = new string[]{},
                mDynamicObjectType = typeof(Elephant),
                mLookDescription = "A friendly elephant. They say hello!",
                mCanStaticObjectBeMovedThrough = false,
            },
            [Objects.ID.Pen] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Pen},
                mInputTokens = new string[]{"pen"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A simple pen.",
                mCanStaticObjectBeMovedThrough = false,
            },
            [Objects.ID.Lock] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Locked, Emoji.Environment.Unlocked, Emoji.Environment.LockedWithKey, Emoji.Environment.LockedWithPen},
                mInputTokens = new string[]{"lock"},
                mDynamicObjectType = typeof(Lock),
                mLookDescription = "A lock",
                mCanStaticObjectBeMovedThrough = false,
            },
            [Objects.ID.SpiderWeb] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.SpiderWeb},
                mInputTokens = new string[]{},
                mDynamicObjectType = null,
                mLookDescription = "A web to catch bugs",
                mCanStaticObjectBeMovedThrough = true,
            },
            [Objects.ID.HoneyPot] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.HoneyPot},
                mInputTokens = new string[]{},
                mDynamicObjectType = null,
                mLookDescription = "A honeypot full of bees",
                mCanStaticObjectBeMovedThrough = false,
            },
        };

        public static ObjectTraits GetObjectTraits(Objects.ID id)
        {
            Debug.Assert(idToTraitsMap.ContainsKey(id));

            return idToTraitsMap[id];
        }
    }
}
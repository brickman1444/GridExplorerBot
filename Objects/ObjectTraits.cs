using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GridExplorerBot
{
    public class ObjectTraits
    {
        delegate string DescriptionOverrideFunction(Game game);

        public string[] mDisplayEmoji = null;
        public string[] mInputTokens = null;
        public System.Type mDynamicObjectType = null;
        string mLookDescription = "";
        DescriptionOverrideFunction mDescriptionOverrideFunction = null;
        public bool mCanStaticObjectBeMovedThrough = false;
        public bool mCanStaticObjectBeThrownThrough = true;
        public bool mIsInsect = false;
        public bool mIsEdible = false;
        public bool mIsHealthyToEat = false;
        public bool mCausesHallucinations = false;
        public Objects.ID mCrushingResultType = Objects.ID.Unknown;

        public static Dictionary<Objects.ID, ObjectTraits> idToTraitsMap = new Dictionary<Objects.ID, ObjectTraits>()
        {
            [Objects.ID.PlayerCharacter] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Player.Default, Emoji.Player.Confused},
                mInputTokens = new string[]{},
                mDynamicObjectType = typeof(PlayerCharacter),
                mLookDescription = "The loyal player character.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.Wall] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Wall},
                mInputTokens = new string[]{"wall"},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "A sturdy wall.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.Empty] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Empty},
                mInputTokens = new string[]{},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "There's nothing there.",
                mCanStaticObjectBeMovedThrough = true,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Elephant] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Animals.Elephant},
                mInputTokens = new string[]{},
                mDynamicObjectType = typeof(Elephant),
                mLookDescription = "A friendly elephant. They say hello!",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.Pen] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.InventoryItems.Pen},
                mInputTokens = new string[]{"pen"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A simple pen.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Lock] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Locked, Emoji.Environment.Unlocked, Emoji.Environment.LockedWithKey, Emoji.Environment.LockedWithPen},
                mInputTokens = new string[]{"lock", "door"},
                mDynamicObjectType = typeof(Lock),
                mLookDescription = "A lock",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.SpiderWeb] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.SpiderWeb},
                mInputTokens = new string[]{},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "A web to catch bugs",
                mCanStaticObjectBeMovedThrough = true,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.HoneyPot] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.HoneyPot},
                mInputTokens = new string[]{},
                mDynamicObjectType = typeof(HoneyPot),
                mLookDescription = "A honeypot full of bees",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Door] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Door,
                                             Emoji.Buildings.Castle, 
                                             Emoji.Buildings.NationalPark, 
                                             Emoji.Environment.SatelliteAntenna,
                                             Emoji.Environment.Hole,
                                             Emoji.Environment.Stairs,
                                             Emoji.Plants.Rosette,
                                             Emoji.Buildings.DepartmentStore},
                mInputTokens = new string[]{"door"},
                mDynamicObjectType = typeof(Door),
                mLookDescription = "A door to somewhere else",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.Spider] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Animals.Spider},
                mInputTokens = new string[]{},
                mDynamicObjectType = typeof(Spider),
                mLookDescription = "A giant hungry spider",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Bee] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Animals.Bee},
                mInputTokens = new string[]{},
                mDynamicObjectType = typeof(Bee),
                mLookDescription = "A giant bee, busy at work",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
                mIsInsect = true,
            },
            [Objects.ID.Rose] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Plants.Rose},
                mInputTokens = new string[]{"rose","flower"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A sweet smelling rose",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.WiltedRose] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Plants.WiltedRose},
                mInputTokens = new string[]{"wilted flower","wilted rose"},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "An old wilted rose",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Vase] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Vase},
                mInputTokens = new string[]{"vase","amphora"},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "An ancient stone vase",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Candle] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.InventoryItems.Candle},
                mInputTokens = new string[]{"candle"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A lit candle",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
                mIsEdible = true,
                mIsHealthyToEat = false,
            },
            [Objects.ID.Globe] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.GlobeShowingEuropeAfrica, Emoji.Environment.GlobeShowingAsiaAustralia, Emoji.Environment.GlobeShowingAmericas},
                mInputTokens = new string[]{"globe", "earth", "antique globe", "earth globe"},
                mDynamicObjectType = typeof(Globe),
                mLookDescription = "An antique globe showing all corners of the world.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.GemStone] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.InventoryItems.GemStone},
                mInputTokens = new string[]{"gem stone", "diamond"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A sparkling gem stone",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
                mIsEdible = true,
                mIsHealthyToEat = false,
            },
            [Objects.ID.Coffin] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Coffin},
                mInputTokens = new string[]{"coffin"},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "An well-crafted coffin",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Vampire] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.People.Vampire},
                mInputTokens = new string[]{"vampire"},
                mDynamicObjectType = typeof(Vampire),
                mLookDescription = "An well-dressed vampire",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Key] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.InventoryItems.Key},
                mInputTokens = new string[]{"key"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A key",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
                mIsEdible = true,
                mIsHealthyToEat = false,
            },
            [Objects.ID.FuneralUrn] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.FuneralUrn},
                mInputTokens = new string[]{"urn", "funeral urn"},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "An old funeral urn",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.DeciduousTree] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Plants.DeciduousTree},
                mInputTokens = new string[]{"tree"},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "A leafy tree that has grown larger than you'd expect.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.Mushroom] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Plants.Mushroom},
                mInputTokens = new string[]{"mushroom"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "An enticing spotted mushroom. Your mouth starts to water.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
                mIsEdible = true,
                mCausesHallucinations = true,
            },
            [Objects.ID.BloodOrange] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Food.BloodOrange},
                mInputTokens = new string[]{"orange","blood orange"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A juicy blood orange.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
                mIsEdible = true,
                mIsHealthyToEat = true,
                mCrushingResultType = Objects.ID.Blood,
            },
            [Objects.ID.LabCoat] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Clothing.LabCoat},
                mInputTokens = new string[]{"lab coat"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A new lab coat, ready for some science.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Handbag] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.InventoryItems.Handbag},
                mInputTokens = new string[]{"hand bag", "handbag"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A fashionable hand bag.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.NeckTie] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Clothing.NeckTie},
                mInputTokens = new string[]{"neck tie", "tie", "neck-tie", "necktie"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A cunning neck tie.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Scarf] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Clothing.Scarf},
                mInputTokens = new string[]{"scarf"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "A warm cozy scarf.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Guard] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.People.Guard},
                mInputTokens = new string[]{"guard", "security guard", "guardsman"},
                mDynamicObjectType = typeof(Guard),
                mLookDescription = "A stoic security guard, serious about their job.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.Alembic] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Alembic},
                mInputTokens = new string[]{"alembic"},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "A burbling alembic.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.Microscope] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Microscope},
                mInputTokens = new string[]{"microscope"},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "A powerful microscope. There's something on the slide.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.Clamp] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.Clamp},
                mInputTokens = new string[]{"clamp"},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "A clamp. Looks like it could work as a juicer.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.Blood] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.InventoryItems.Blood},
                mInputTokens = new string[]{"blood"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "Some organic blood harvested from a blood orange.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
                mIsEdible = true,
                mIsHealthyToEat = false,
            },
            [Objects.ID.Bat] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Animals.Bat},
                mInputTokens = new string[]{"bat"},
                mDynamicObjectType = typeof(Bat),
                mLookDescription = "A bat frantically flying around.",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.MantelpieceClock] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.MantelpieceClock},
                mInputTokens = new string[]{"clock", "mantelpiece clock"},
                mDynamicObjectType = typeof(StaticObject),
                mLookDescription = "",
                mDescriptionOverrideFunction = game => { return "An antique mantelpiece clock. It shows " + game.mGameTime.Render(); },
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = false,
            },
            [Objects.ID.Watch] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.InventoryItems.Watch},
                mInputTokens = new string[]{"watch", "wrist watch"},
                mDynamicObjectType = typeof(InventoryObject),
                mLookDescription = "",
                mDescriptionOverrideFunction = game => { return "An expensive looking watch. It shows " + game.mGameTime.Render(); },
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
            [Objects.ID.Hourglass] = new ObjectTraits()
            {
                mDisplayEmoji = new string[]{Emoji.Environment.HourglassDone, Emoji.Environment.HourglassNotDone},
                mInputTokens = new string[]{"hourglass"},
                mDynamicObjectType = typeof(Hourglass),
                mLookDescription = "",
                mCanStaticObjectBeMovedThrough = false,
                mCanStaticObjectBeThrownThrough = true,
            },
        };

        public static ObjectTraits GetObjectTraits(Objects.ID id)
        {
            Debug.Assert(idToTraitsMap.ContainsKey(id));

            return idToTraitsMap[id];
        }

        public string GetDescription(Game game)
        {
            if (mDescriptionOverrideFunction != null)
            {
                return mDescriptionOverrideFunction(game);
            }
            else
            {
                return mLookDescription;
            }
        }
    }
}
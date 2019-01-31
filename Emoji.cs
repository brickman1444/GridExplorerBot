using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GridExplorerBot
{
    public static class Emoji
    {
        public static class Player
        {
            public static string Default = "😀";
            public static string Confused = "😕";
            public static string Sleeping = "😴";
            public static string Thinking = "🤔";
            public static string SteamOutOfNose = "😤";
            public static string Zany = "🤪";
            public static string SavoringFood = "😋";
            public static string Vomiting = "🤮";
        }

        public static class Environment
        {
            public static string LockedWithPen = "🔏";
            public static string LockedWithKey = "🔐";
            public static string Locked = "🔒";
            public static string Unlocked = "🔓";
            public static string SpiderWeb = "🕸️";
            public static string HoneyPot = "🍯";
            public static string Wall = "⬛";
            public static string Empty = "⬜";
            public static string Door = "🚪";
            public static string Vase = "🏺";
            public static string Globe = "🌍";
            public static string SatelliteAntenna = "📡";
            public static string Hole = "🕳️";
            public static string Stairs = "📶";
            public static string Coffin = "⚰️";
            public static string FuneralUrn = "⚱️";
            public static string Alembic = "⚗️";
            public static string Microscope = "🔬";
            public static string Clamp = "🗜️";
        }

        public static class InventoryItems
        {
            public static string Pen = "🖋️";
            public static string Candle = "🕯️";
            public static string GemStone = "💎";
            public static string Key = "🔑";
            public static string Handbag = "👜";
            public static string Blood = "🅱️";
        }

        public static class Clothing
        {
            public static string LabCoat = "🥼";
            public static string NeckTie = "👔";
            public static string Scarf = "🧣";
        }

        public static class Animals
        {
            public static string Elephant = "🐘";
            public static string Spider = "🕷️";
            public static string Bee = "🐝";
        }

        public static class Plants
        {
            public static string Rose = "🌹";
            public static string WiltedRose = "🥀";
            public static string ChristmasTree = "🎄";
            public static string DeciduousTree = "🌳";
            public static string Rosette = "🏵️";
            public static string Mushroom = "🍄";
            public static string Eggplant = "🍆";
        }

        public static class People
        {
            public static string Vampire = "🧛";
            public static string HappyDemon = "😈";
            public static string Guard = "💂";
        }

        public static class Buildings
        {
            public static string Castle = "🏰";
            public static string NationalPark = "🏞️";
            public static string DepartmentStore = "🏬";
        }

        public static class Symbols
        {
            public static string Dizzy = "💫";
            public static string BlueCircle = "🔵";
            public static string RedCircle = "🔴";
            public static string PurpleHeart = "💜";
            public static string Peace = "☮";
        }

        public static class Sky
        {
            public static string StarryNight = "🌌";
        }

        public static class Food
        {
            public static string BloodOrange = "🍊";
        }

        public static Objects.ID GetID(string inputText)
        {
            foreach (KeyValuePair<Objects.ID, ObjectTraits> pair in ObjectTraits.idToTraitsMap)
            {
                if (pair.Value.mInputTokens.Contains(inputText)
                || pair.Value.mDisplayEmoji.Contains(inputText))
                {
                    return pair.Key;
                }
            }

            return Objects.ID.Unknown;
        }

        public static string GetEmoji(Objects.ID id, int index = 0)
        {
            string[] displayChars = ObjectTraits.GetObjectTraits(id).mDisplayEmoji;

            Debug.Assert(index < displayChars.Length);

            return displayChars[index];
        }

        public static string GetRandomEmoji(Objects.ID id)
        {
            string[] displayChars = ObjectTraits.GetObjectTraits(id).mDisplayEmoji;

            int displayEmojiIndex = Game.random.Next() % displayChars.Length;

            return displayChars[displayEmojiIndex];
        }

        public static int GetEmojiIndex(Objects.ID id, string emoji)
        {
            string[] displayChars = ObjectTraits.GetObjectTraits(id).mDisplayEmoji;

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

        public static GridObject CreateObject(Objects.ID id)
        {
            Type objectType = ObjectTraits.GetObjectTraits(id).mDynamicObjectType;

            Debug.Assert(objectType.IsSubclassOf(typeof(GridObject)));

            return (GridObject)Activator.CreateInstance(objectType);
        }

        public static GridObject CreateObject(ReadStream stream)
        {
            Objects.ID id = Objects.ID.Unknown;
            stream.Stream(ref id);
            stream.BackUp(SaveUtils.GetNumBits(id));

            return CreateObject(id);
        }

        public static string[] GetHallucinationEmoji()
        {
            return new string[]{
                Emoji.Plants.Eggplant,
                Emoji.People.HappyDemon,
                Emoji.Symbols.BlueCircle,
                Emoji.Symbols.RedCircle,
                Emoji.Symbols.PurpleHeart,
                Emoji.Symbols.Peace,
                Emoji.Sky.StarryNight,
            };
        }

        public static string GetRandomHallucinationEmoji()
        {
            string[] emoji = GetHallucinationEmoji();

            int emojiIndex = Game.random.Next() % emoji.Length;

            return emoji[emojiIndex];
        }
    }
}
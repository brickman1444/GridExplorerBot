using System.Collections.Generic;
using System.Diagnostics;

namespace GridExplorerBot
{
    public static class InitialRooms
    {
        public enum ID
        {
            Unknown,
            Circus,
            VampireCastleCourtyard,
            VampireCastleEntryway,
            Crypt,
            Overworld,
            MushroomGrove,
            BloodOrangeGrove,
            DepartmentStore,
            ResearchFacility,
            SpiderRoom,
            LikeTemple,
        };

        public static bool IsValidInitialRoomIndex(ID id)
        {
            return initialRooms.ContainsKey(id);
        }

        public static void Initialize()
        {
            initialRooms = _initialRooms;
        }

        public static Dictionary<int,NPCIdentityData> identityData = new Dictionary<int, NPCIdentityData>();

        public static Dictionary<InitialRooms.ID,Room> initialRooms = null;

        public static Dictionary<InitialRooms.ID,Room> _initialRooms = new Dictionary<InitialRooms.ID, Room>()
        {
            [InitialRooms.ID.Overworld] = new Room( "The World",
                    new string[] {
                    "🌍🌍🌍🌍🌍🌍🌍🌍",
                    "🌍⬜⬜⬜⬜⬜⬜🌍",
                    "🌍⬜⬜⬜⬜⬜⬜🌍",
                    "🌍⬜⬜⬜⬜⬜⬜🌍",
                    "🌍⬜⬜⬜⬜⬜⬜🌍",
                    "🌍⬜⬜⬜⬜⬜⬜🌍",
                    "🌍⬜⬜⬜⬜⬜⬜🌍",
                    "🌍🌍🌍🌍🌍🌍🌍🌍", },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(5,4)),
                       new DoorSetup(Emoji.Buildings.Castle, new Point(4,4), InitialRooms.ID.VampireCastleCourtyard, new Point(4,6), "Vampire Castle"),
                       new DoorSetup(Emoji.Buildings.NationalPark, new Point(4,2), InitialRooms.ID.MushroomGrove, new Point(6,1), "Magic Forest"),
                       new DoorSetup(Emoji.Environment.SatelliteAntenna, new Point(4,6), InitialRooms.ID.ResearchFacility, new Point(6,4), "Research Facility"),
                       new DoorSetup(Emoji.Buildings.DepartmentStore, new Point(6,4), InitialRooms.ID.DepartmentStore, new Point(6,4), "Department Store"),
                       new DoorSetup(Emoji.Buildings.ClassicalBuilding, new Point(6,2), InitialRooms.ID.LikeTemple, new Point(1,4), "Like Temple"),
                       } ),
            [ID.Circus] = new Room( "The Circus",
                    new string[] {
                   "⬛⬛⬛⬛⬛⬛⬛⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬛⬛⬛⬜⬛⬛⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬛⬛⬛⬛⬛⬛⬛" },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(5,5)),
                       new GridObjectSetup(Emoji.Animals.Elephant, new Point(1,1)),
                       new GridObjectSetup(Emoji.InventoryItems.Pen, new Point(4,4)),
                       new LockSetup(Emoji.Environment.LockedWithPen, new Point(3,4)),
                       } ),
            [ID.VampireCastleCourtyard] = new Room( "Courtyard of Vampire Castle",
                    new string[] {
                    "⬛⬛⬛⬛⬛⬛⬛⬛",
                    "⬛🥀🥀🥀🥀🥀🥀⬛",
                    "⬛⬜⬜⬜⬜⬜⬜⬛",
                    "⬛🏺⬜⬜⬜⬜⬜⬛",
                    "⬜⬜⬜⬜⬜⬜⬜⬜",
                    "⬛🏺⬜⬜⬜⬜⬜⬛",
                    "⬛🥀🥀🥀🥀🥀🥀⬛",
                    "⬛⬛⬛⬛⬛⬛⬛⬛", },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(4,6)),
                       new DoorSetup(Emoji.Environment.Door, new Point(4,7), InitialRooms.ID.Overworld, new Point(5,4)),
                       new DoorSetup(Emoji.Environment.Door, new Point(4,0), InitialRooms.ID.VampireCastleEntryway, new Point(3,6)),
                       new GridObjectSetup(Emoji.InventoryItems.Candle, new Point(3,6)),
                       new GridObjectSetup(Emoji.InventoryItems.Candle, new Point(5,6)),
                       new GridObjectSetup(Emoji.Animals.Bat, new Point(2,4)),
                       } ),
            [ID.SpiderRoom] = new Room( "Spider Room",
                    new string[] {
                    "⬛⬛⬛⬛⬛⬛⬛⬛",
                    "⬛🥀🥀🕸️🕸️🥀🥀⬛",
                    "⬛⬜⬜🕸️🕸️⬜⬜⬛",
                    "⬛🏺⬜🕸️🕸️⬜⬜⬛",
                    "⬜⬜⬜🕸️🕸️⬜⬜⬜",
                    "⬛🏺⬜🕸️🕸️⬜⬜⬛",
                    "⬛⬜🕸️🕸️🕸️🍯⬜⬛",
                    "⬛⬛⬛⬛⬛⬛⬛⬛", },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(4,6)),
                       new DoorSetup(Emoji.Environment.Door, new Point(4,7), InitialRooms.ID.Overworld, new Point(5,4)),
                       new DoorSetup(Emoji.Environment.Door, new Point(4,0), InitialRooms.ID.VampireCastleEntryway, new Point(3,6)),
                       new GridObjectSetup(Emoji.Animals.Spider, new Point(4,4)),
                       new GridObjectSetup(Emoji.Plants.Rose, new Point(1,6)),
                       new GridObjectSetup(Emoji.InventoryItems.Candle, new Point(3,6)),
                       new GridObjectSetup(Emoji.InventoryItems.Candle, new Point(5,6)),
                       } ),
            [ID.VampireCastleEntryway] = new Room( "Vampire Castle Entryway",
                    new string[] {
                    "⬛⬛⬛⬛⬜⬛⬛⬛",
                    "⬛⬜⬜⬛⬜⬜⬜⬛",
                    "⬛⬜⬜⬛⬜⬜⬜⬛",
                    "⬛⬜⬜⬜⬜⬜⬜⬜",
                    "⬛⬛⬜⬛⬛⬜⬛⬛",
                    "⬛⬜⬜⬛⬜⬜⬜⬛",
                    "⬛⬜⬜⬛⬜⬜⬜⬛",
                    "⬛⬛⬛⬛⬜⬛⬛⬛", },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(3,6)),
                       new DoorSetup(Emoji.Environment.Door, new Point(3,7), InitialRooms.ID.VampireCastleCourtyard, new Point(4,1)),
                       new DoorSetup(Emoji.Environment.Door, new Point(0,4), InitialRooms.ID.VampireCastleCourtyard, new Point(4,1)),
                       new DoorSetup(Emoji.Environment.Door, new Point(7,4), InitialRooms.ID.VampireCastleCourtyard, new Point(4,1)),
                       new DoorSetup(Emoji.Environment.Hole, new Point(1,6), InitialRooms.ID.Crypt, new Point(1,6)),
                       new DoorSetup(Emoji.Environment.Stairs, new Point(6,6), InitialRooms.ID.Crypt, new Point(6,6)),
                       new GridObjectSetup(Emoji.InventoryItems.GemStone, new Point(6,1)),
                       new LockSetup(Emoji.Environment.LockedWithKey, new Point(4,5)),
                       new LockSetup(Emoji.Environment.Unlocked, new Point(3,3)),
                       new LockSetup(Emoji.Environment.Unlocked, new Point(4,2)),
                       new GridObjectSetup(Emoji.Environment.Coffin, new Point(1,2)),
                       new GridObjectSetup(Emoji.Environment.Coffin, new Point(5,1)),
                       new GridObjectSetup(Emoji.Environment.Coffin, new Point(5,6)),
                       new VampireSetup(new Point(1,1)),
                       new GridObjectSetup(Emoji.Environment.MantelpieceClock, new Point(2,6)),
                       new GridObjectSetup(Emoji.Environment.GlobeShowingEuropeAfrica, new Point(2,4)),
                       new GridObjectSetup(Emoji.Environment.HourglassDone, new Point(2,2)),
                       } ),
            [ID.Crypt] = new Room( "The Crypt",
                    new string[] {
                    "⬛⬛⬛⬛⬛⬛⬛⬛",
                    "⬛⬜⬜⬜⬜⬜⬜⬛",
                    "⬛⬜⚱️⬛⚱️⬛⚱️⬛",
                    "⬛⬜⬛⬛⬛⬛⬛⬛",
                    "⬛⬜⬜⬛⚱️⬛⚱️⬛",
                    "⬛⬜⬜⬜⬜⬛⬛⬛",
                    "⬛⬜⬜⬜⬜⬜📶⬛",
                    "⬛⬛⬛⬛⬛⬛⬛⬛", },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(1,6)),
                       new DoorSetup(Emoji.Environment.Stairs, new Point(6,6), InitialRooms.ID.VampireCastleEntryway, new Point(6,5)),
                       new GridObjectSetup(Emoji.InventoryItems.Key, new Point(4,2)),
                       new LockSetup(Emoji.Environment.LockedWithKey, new Point(6,5)),
                       } ),
            [ID.MushroomGrove] = new Room( "Mushroom Grove",
                    new string[] {
                    "🌳🌳🌳🌳🌳🌳🌳🌳",
                    "🌳⬜⬜⬜⬜⬜⬜⬜",
                    "🌳⬜⬜⬜⬜⬜⬜🌳",
                    "🌳⬜⬜⬜⬜⬜⬜🌳",
                    "🌳⬜⬜⬜⬜⬜⬜🌳",
                    "🌳⬜⬜⬜⬜⬜⬜🌳",
                    "🌳⬜⬜⬜⬜⬜⬜🌳",
                    "🌳⬜🌳🌳🌳🌳🌳🌳", },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(6,1)),
                       new DoorSetup(Emoji.Plants.Rosette, new Point(7,1), InitialRooms.ID.Overworld, new Point(5,2)),
                       new DoorSetup(Emoji.Plants.Rosette, new Point(1,7), InitialRooms.ID.BloodOrangeGrove, new Point(1,1)),
                       new GridObjectSetup(Emoji.Plants.Mushroom, new Point(2,2)),
                       new GridObjectSetup(Emoji.Plants.Mushroom, new Point(3,2)),
                       new GridObjectSetup(Emoji.Plants.Mushroom, new Point(5,6)),
                       new GridObjectSetup(Emoji.Plants.Mushroom, new Point(2,5)),
                       new GridObjectSetup(Emoji.Plants.Mushroom, new Point(3,3)),
                       new GridObjectSetup(Emoji.Plants.Mushroom, new Point(5,3)),
                       } ),
            [ID.BloodOrangeGrove] = new Room( "Blood Orange Grove",
                    new string[] {
                    "🌳🌳🌳🌳🌳🌳🌳🌳",
                    "⬜⬜⬜⬜⬜⬜⬜🌳",
                    "🌳⬜⬜⬜⬜⬜⬜🌳",
                    "🌳⬜⬜⬜⬜⬜⬜🌳",
                    "🌳⬜⬜⬜⬜⬜⬜🌳",
                    "🌳⬜⬜⬜⬜⬜⬜🌳",
                    "🌳⬜⬜⬜⬜⬜⬜🌳",
                    "🌳🌳🌳🌳🌳🌳⬜🌳", },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(1,1)),
                       new DoorSetup(Emoji.Plants.Rosette, new Point(1,0), InitialRooms.ID.MushroomGrove, new Point(1,6)),
                       new DoorSetup(Emoji.Plants.Rosette, new Point(7,6), InitialRooms.ID.Overworld, new Point(5,2)),
                       new GridObjectSetup(Emoji.Food.BloodOrange, new Point(2,2)),
                       new GridObjectSetup(Emoji.Food.BloodOrange, new Point(3,2)),
                       new GridObjectSetup(Emoji.Food.BloodOrange, new Point(5,6)),
                       new GridObjectSetup(Emoji.Food.BloodOrange, new Point(2,5)),
                       new GridObjectSetup(Emoji.Food.BloodOrange, new Point(3,3)),
                       new GridObjectSetup(Emoji.Food.BloodOrange, new Point(5,3)),
                       } ),
            [ID.DepartmentStore] = new Room( "Department Store",
                    new string[] {
                   "⬛⬛⬛⬛⬛⬛⬛⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬛⬛⬛⬜⬛⬛⬛" },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(6,4)),
                       new DoorSetup(Emoji.Environment.Door, new Point(7,4), InitialRooms.ID.Overworld, new Point(5,4)),
                       new GridObjectSetup(Emoji.Clothing.LabCoat, new Point(6,5)),
                       new GridObjectSetup(Emoji.InventoryItems.Handbag, new Point(5,5)),
                       new GridObjectSetup(Emoji.Clothing.NeckTie, new Point(6,3)),
                       new GridObjectSetup(Emoji.Clothing.Scarf, new Point(5,3)),
                       new GridObjectSetup(Emoji.InventoryItems.Watch, new Point(4,3)),
                       } ),
            [ID.ResearchFacility] = new Room( "Research Facility. A security guard stands by.",
                    new string[] {
                   "⬛⬛⬛⬛⬛⬛⬛⬛",
                   "⬛⬜⬜⚗️⬜🗜️⬜⬛",
                   "⬛⬜⬜⬜⬜⬜⬜⬛",
                   "⬛⬜⬜🔬⬜⬜⚗️⬛",
                   "⬛⬛⬛⬛⬜⬛⬛⬛",
                   "⬛⬛⬛⬛⬜⬜⬛⬛",
                   "⬛⬛⬛⬛⬜⬛⬛⬛",
                   "⬛⬛⬛⬛⬜⬛⬛⬛" },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(6,4)),
                       new DoorSetup(Emoji.Environment.Door, new Point(7,4), InitialRooms.ID.Overworld, new Point(5,6)),
                       new LockSetup(Emoji.Environment.Unlocked, new Point(4,4)),
                       new GuardSetup(new Point(5,4), new Point(5,5)),
                       } ),
            [ID.LikeTemple] = new Room( "The Like Temple",
                    new string[] {
                   "🛐🛐🛐⬜⬜🛐🛐🛐",
                   "🛐🤳🙌⬜⬜🙏🤳🛐",
                   "🛐🤳🙌⬜⬜🙏🤳🛐",
                   "🛐🤳🙌⬜⬜🙏🤳🛐",
                   "🛐🤳🙌⬜⬜🙏🤳🛐",
                   "🛐🤳🙌⬜⬜🙏🤳🛐",
                   "🛐🔃👍🔃👍🔃👍🛐",
                   "🛐🛐🛐🛐🛐🛐🛐🛐" },
                   new GridObjectSetup[] {
                       new GridObjectSetup(Emoji.Player.Default, new Point(1,4)),
                       new DoorSetup(Emoji.Environment.Door, new Point(0,4), InitialRooms.ID.Overworld, new Point(5,2)),
                       new DoorSetup(Emoji.Environment.Door, new Point(0,3), InitialRooms.ID.Overworld, new Point(5,2)),
                       new GridObjectSetup(Emoji.People.ManInSuitLevitating, new Point(4,4)),
                       } ),
        };
    }
}
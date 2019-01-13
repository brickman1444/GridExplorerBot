using System.Collections.Generic;

namespace GridExplorerBot
{
    public class Game
    {
        public const int numRoomRows = 8;
        public const int numRoomColumns = 8;
        public const int numCommandResponseRows = 1;
        public const int numInventoryRows = 1;
        public const int numSaveDataRows = 1;
        public const int numTotalRows = numCommandResponseRows + numRoomRows + numInventoryRows + numSaveDataRows;

        public const int saveDataRowIndex = numTotalRows - 1;

        const string newGameCommand = "New Game";
        string mLastCommandResponse = newGameCommand;

        public static System.Random random = null;

        public Room mRoom = null;
        public Inventory mInventory = null;
        string mSaveDataString = "";
        InitialRooms.ID mTeleportDestinationRoomID = InitialRooms.ID.Unknown;
        Point? mTeleportDestinationSpawnLocation = null;

        // Returns true if the game was successfully parsed
        public bool ParsePreviousText(string inputText)
        {
            string[] lines = inputText.Split('\n');

            if (lines.Length != numTotalRows)
            {
                return false;
            }

            string saveDataLine = lines[saveDataRowIndex];

            mRoom = new Room();
            mInventory = new Inventory();


            Load( saveDataLine );

            return true;
        }

        public void GenerateFreshGame(InitialRooms.ID initialRoomID = InitialRooms.ID.VampireCastleEntryway)
        {
            mLastCommandResponse = newGameCommand;
            mInventory = new Inventory();
            mRoom = new Room();
            mRoom.SetInitialRoomIndex(initialRoomID);
            mRoom.LoadStaticGridFromInitialRoom();
            mRoom.LoadDynamicObjectsFromInitialRoom();
        }

        public void ChangeToRoom(InitialRooms.ID initialRoomID)
        {
            mRoom.SetInitialRoomIndex(initialRoomID);
            mRoom.LoadStaticGridFromInitialRoom();
            mRoom.LoadDynamicObjectsFromInitialRoom();
        }

        public string Render()
        {
            string commandResponse = mLastCommandResponse;
            commandResponse = commandResponse.Substring(0,1).ToUpper() + commandResponse.Substring(1);
            string roomRender = mRoom.Render();
            string inventory = mInventory.Render();
            string saveData = mSaveDataString;
            return commandResponse + '\n' + roomRender + '\n' + inventory + '\n' + saveData;
        }

        public void Simulate(string inputCommand)
        {
            mLastCommandResponse = mRoom.Simulate(inputCommand, this);

            if (mTeleportDestinationRoomID != InitialRooms.ID.Unknown
            && mTeleportDestinationSpawnLocation != null)
            {
                ChangeToRoom(mTeleportDestinationRoomID);
                mRoom.TeleportPlayerTo(mTeleportDestinationSpawnLocation.Value);
                mLastCommandResponse = mRoom.mDescription;
            }
        }

        public void Save()
        {
            // arbitrary estimated size
            WriteStream stream = new WriteStream(36);

            Stream(stream);

            mSaveDataString = StringUtils.SaveDataEncode(stream.GetStreamData());
        }

        public void Load(string saveData)
        {
            byte[] bytes = StringUtils.SaveDataDecode(saveData);
            ReadStream stream = new ReadStream(bytes);

            Stream(stream);
        }

        private void Stream(SaveStream stream)
        {
            mInventory.Stream(stream);
            mRoom.Stream(stream);
        }

        public void SetTeleport(InitialRooms.ID destinationRoomID, Point destinationSpawnLocation)
        {
            mTeleportDestinationRoomID = destinationRoomID;
            mTeleportDestinationSpawnLocation = destinationSpawnLocation;
        }

        public static void InitializeRandom(System.DateTimeOffset seedTime)
        {
            random = new System.Random((int)seedTime.ToUnixTimeSeconds());
        }

        public static bool MatchesResetCommand(string input)
        {
            return PlayerCharacter.MatchesResetCommand(input);
        }

        public static bool MatchesHelpCommand(string input)
        {
            return PlayerCharacter.MatchesHelpCommand(input);
        }

        public static string GetCommandsList()
        {
            return PlayerCharacter.GetCommandsListText();
        }
    }
}
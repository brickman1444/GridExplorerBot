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

        const InitialRooms.ID defaultInitialRoom = InitialRooms.ID.Overworld;

        const string newGameCommand = "New Game";
        string mLastCommandResponse = newGameCommand;

        public static System.Random random = null;

        public Room mRoom = null;
        public Inventory mInventory = null;
        public GameTime mGameTime = null;
        public static Dictionary<int,NPCIdentityData> mNPCIdentities = null;
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
            mGameTime = new GameTime();
            mNPCIdentities = new Dictionary<int, NPCIdentityData>();
            InitialRooms.Initialize();

            Load( saveDataLine );

            return true;
        }

        public void GenerateFreshGame(InitialRooms.ID initialRoomID = defaultInitialRoom)
        {
            mLastCommandResponse = newGameCommand;
            mInventory = new Inventory();
            mRoom = new Room();
            mGameTime = new GameTime();
            mNPCIdentities = InitialRooms.identityData;
            InitialRooms.Initialize();
            mRoom.CreateFrom(initialRoomID, this);
        }

        public void ChangeToRoom(InitialRooms.ID initialRoomID)
        {
            mRoom.CreateFrom(initialRoomID, this);
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

            mGameTime.Increment();
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
            if (!stream.IsWriting())
            {
                NPCIdentifier.currentMaxID = 0;
            }

            mInventory.Stream(stream);
            mRoom.Stream(stream);
            mGameTime.Stream(stream);

            int numNPCData = mNPCIdentities.Count;
            stream.Stream(ref numNPCData, SaveUtils.GetNumBitsToStoreValue(NPCIdentifier.maxNumNPCs));

            if (stream.IsWriting())
            {
                foreach (KeyValuePair<int,NPCIdentityData> pair in mNPCIdentities)
                {
                    int id = pair.Key;
                    stream.Stream(ref id, SaveUtils.GetNumBitsToStoreValue(NPCIdentifier.maxNumNPCs - 1));
                    pair.Value.Stream(stream);
                }
            }
            else
            {
                mNPCIdentities.Clear();
                for (int npcIdentityIndex = 0; npcIdentityIndex < numNPCData; npcIdentityIndex++)
                {
                    int id = 0;
                    NPCIdentityData data = new NPCIdentityData();
                    stream.Stream(ref id, SaveUtils.GetNumBitsToStoreValue(NPCIdentifier.maxNumNPCs - 1));
                    data.Stream(stream);
                    mNPCIdentities[id] = data;
                }
            }
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
            PlayerCharacter pc = new PlayerCharacter();
            return pc.GetCommandsListText();
        }

        public void AwardMobilePhone()
        {
            mInventory.AddItem(Objects.ID.MobilePhone);
            mLastCommandResponse = "Thank you so much for your prayers! Here's your mobile phone.";
        }
    }
}
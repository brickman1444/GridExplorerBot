namespace GridExplorerBot
{
    public class Game
    {
        public const int numRoomRows = 9;
        public const int numRoomColumns = 9;
        public const int numCommandResponseRows = 1;
        public const int numInventoryRows = 1;
        public const int numSaveDataRows = 1;
        public const int numTotalRows = numCommandResponseRows + numRoomRows + numInventoryRows + numSaveDataRows;

        public const int saveDataRowIndex = numTotalRows - 1;

        string mLastCommandResponse = "";
        Room mRoom = null;
        string mSaveDataString = "";

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

            mRoom.Load( saveDataLine );

            return true;
        }

        public void GenerateFreshGame()
        {
            mLastCommandResponse = "";
            mRoom = new Room();
            mRoom.SetInitialRoomIndex(0);
            mRoom.LoadStaticGridFromInitialRoom();
            mRoom.LoadDynamicObjectsFromInitialRoom();
        }

        public string Render()
        {
            string commandResponse = mLastCommandResponse;
            string roomRender = mRoom.Render();
            string inventory = "Inventory";
            string saveData = mSaveDataString;
            return commandResponse + '\n' + roomRender + '\n' + inventory + '\n' + saveData;
        }

        public void Simulate(string inputCommand)
        {
            mLastCommandResponse = mRoom.HandleCommand(inputCommand);
        }

        public void Save()
        {
            mSaveDataString = mRoom.Save();
        }
    }
}
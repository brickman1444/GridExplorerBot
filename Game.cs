using System.Collections.Generic;

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

        public Room mRoom = null;
        public Inventory mInventory = null;
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
            mInventory = new Inventory();

            Load( saveDataLine );

            return true;
        }

        public void GenerateFreshGame()
        {
            mLastCommandResponse = "";
            mInventory = new Inventory();
            mRoom = new Room();
            mRoom.SetInitialRoomIndex(InitialRooms.ID.Circus);
            mRoom.LoadStaticGridFromInitialRoom();
            mRoom.LoadDynamicObjectsFromInitialRoom();
        }

        public string Render()
        {
            string commandResponse = mLastCommandResponse;
            string roomRender = mRoom.Render();
            string inventory = mInventory.Render();
            string saveData = mSaveDataString;
            return commandResponse + '\n' + roomRender + '\n' + inventory + '\n' + saveData;
        }

        public void Simulate(string inputCommand)
        {
            mLastCommandResponse = mRoom.Simulate(inputCommand, this);
        }

        public void Save()
        {
            // arbitrary estimated size
            BitStreams.BitStream stream = new BitStreams.BitStream(new byte[36]);

            mInventory.Save(stream);
            mRoom.Save(stream);

            mSaveDataString = StringUtils.SaveDataEncode(stream.GetStreamData());
        }

        public void Load(string saveData)
        {
            byte[] bytes = StringUtils.SaveDataDecode(saveData);
            BitStreams.BitStream stream = new BitStreams.BitStream( bytes );

            mInventory.Load(stream);
            mRoom.Load(stream);
        }
    }
}
using System.Diagnostics;

namespace GridExplorerBot
{
    public class DoorSetup : GridObjectSetup
    {
        public InitialRooms.ID mDestinationRoomID = InitialRooms.ID.Unknown;
        public Point mDestinationSpawnLocation = new Point();
        public string mOverrideLookDescription = "";

        public DoorSetup(string inDisplayText, Point inStartingPosition, InitialRooms.ID destinationRoomID, Point destinationSpawnLocation, string overrideLookDescription = "") : base(inDisplayText, inStartingPosition)
        {
            mDestinationRoomID = destinationRoomID;
            mDestinationSpawnLocation = destinationSpawnLocation;
            mOverrideLookDescription = overrideLookDescription;
        }

        public override GridObject CreateObject()
        {
            Door doorObject = base.CreateObject() as Door;
            
            Debug.Assert(doorObject != null);

            doorObject.Setup(this);

            return doorObject;
        }
    }

    public class Door : StaticObject
    {
        public InitialRooms.ID mDestinationRoomID = InitialRooms.ID.Unknown;
        public Point mDestinationSpawnLocation = new Point();
        private string mOverrideLookDescription = "";
        private string mDisplayEmoji = "";

        public void Setup(DoorSetup setup)
        {
            mDestinationRoomID = setup.mDestinationRoomID;
            mDestinationSpawnLocation = setup.mDestinationSpawnLocation;
            mOverrideLookDescription = setup.mOverrideLookDescription;
            mDisplayEmoji = setup.mDisplayText;
        }

        public override string Render()
        {
            return mDisplayEmoji;
        }

        public override string GetDescriptionText(Game game)
        {
            if (mOverrideLookDescription.Length != 0)
            {
                return mOverrideLookDescription;
            }

            return InitialRooms.initialRooms[mDestinationRoomID].mDescription;
        }
    }
}
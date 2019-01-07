using System.Diagnostics;

namespace GridExplorerBot
{
    public class DoorSetup : DynamicObjectSetup
    {
        public InitialRooms.ID mDestinationRoomID = InitialRooms.ID.Unknown;
        public Point mDestinationSpawnLocation = new Point();

        public DoorSetup(string inDisplayText, Point inStartingPosition, InitialRooms.ID destinationRoomID, Point destinationSpawnLocation) : base(inDisplayText, inStartingPosition)
        {
            mDestinationRoomID = destinationRoomID;
            mDestinationSpawnLocation = destinationSpawnLocation;
        }

        public override DynamicObject CreateObject()
        {
            Door doorObject = base.CreateObject() as Door;
            
            Debug.Assert(doorObject != null);

            doorObject.Setup(this);

            return doorObject;
        }
    }

    public class Door : DynamicObject
    {
        public InitialRooms.ID mDestinationRoomID = InitialRooms.ID.Unknown;
        public Point mDestinationSpawnLocation = new Point();

        public void Setup(DoorSetup setup)
        {
            mDestinationRoomID = setup.mDestinationRoomID;
            mDestinationSpawnLocation = setup.mDestinationSpawnLocation;
        }

        public override string GetDescriptionText()
        {
            return InitialRooms.initialRooms[mDestinationRoomID].mDescription;
        }

        public override void Save(BitStreams.BitStream stream)
        {
            base.Save(stream);

            stream.Write(mDestinationRoomID);
            stream.Write(mDestinationSpawnLocation);
        }

        public override void Load(BitStreams.BitStream stream)
        {
            base.Load(stream);

            stream.Read(out mDestinationRoomID);
            stream.Read(out mDestinationSpawnLocation);
        }
    }
}
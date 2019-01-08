namespace GridExplorerBot
{
    public class GridObjectSetup
    {
        public readonly string mDisplayText;
        public readonly Point mStartingPosition;

        public GridObjectSetup(string inDisplayText, Point inStartingPosition)
        {
            mDisplayText = inDisplayText;
            mStartingPosition = inStartingPosition;
        }

        public virtual GridObject CreateObject()
        {
            Objects.ID id = Emoji.GetID(mDisplayText);
            return GridObject.Create(id, mStartingPosition);
        }
    }

    public abstract class GridObject
    {
        protected Point mPosition = new Point();
        protected Objects.ID mType = Objects.ID.Unknown;

        public Point GetPosition() { return mPosition; }

        public Objects.ID GetTypeID() { return mType; }

        public abstract void Save(BitStreams.BitStream stream);

        public abstract void Load(BitStreams.BitStream stream);

        public virtual string Render()
        {
            return Emoji.GetEmoji(GetTypeID());
        }

        public virtual string Simulate(string command, Game game)
        {
            return "";
        }

        public ObjectTraits GetObjectTraits()
        {
            return ObjectTraits.GetObjectTraits(GetTypeID());
        }

        public virtual bool CanBePickedUp() { return false; }

        public virtual bool CanBeMovedThrough()
        {
            return GetObjectTraits().mCanStaticObjectBeMovedThrough;
        }

        public virtual bool CanBeThrownThrough()
        {
            return GetObjectTraits().mCanStaticObjectBeThrownThrough;
        }

        public virtual string GetDescriptionText()
        {
            return GetObjectTraits().mLookDescription;
        }

        public static GridObject Create(Objects.ID typeID, Point startinPosition)
        {
            GridObject dynamicObject = Emoji.CreateObject(typeID);
            dynamicObject.mType = typeID;
            dynamicObject.mPosition = startinPosition;
            return dynamicObject;
        }
    }
}
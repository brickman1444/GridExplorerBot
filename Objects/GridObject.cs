namespace GridExplorerBot
{
    public abstract class GridObject
    {
        protected Point mPosition = new Point();
        protected Objects.ID mType = Objects.ID.Unknown;

        public Point GetPosition() { return mPosition; }

        public Objects.ID GetTypeID() { return mType; }

        public abstract string Render();

        public abstract void Save(BitStreams.BitStream stream);

        public abstract void Load(BitStreams.BitStream stream);

        public abstract string Simulate(string command, Game game);

        public ObjectTraits GetObjectTraits()
        {
            return ObjectTraits.GetObjectTraits(GetTypeID());
        }

        public virtual bool CanBePickedUp() { return false; }

        public virtual bool CanBeMovedThrough() { return false; }

        public virtual bool CanBeThrownThrough()
        {
            return GetObjectTraits().mCanStaticObjectBeThrownThrough;
        }

        public virtual string GetDescriptionText()
        {
            return GetObjectTraits().mLookDescription;
        }
    }
}
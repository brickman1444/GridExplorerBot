using System.Diagnostics;

namespace GridExplorerBot
{
    public class VampireSetup : GridObjectSetup
    {
        public VampireSetup(Point position) : base(Emoji.GetEmoji(Objects.ID.Vampire), position)
        {

        }

        public override GridObject CreateObject()
        {
            Vampire vampire = base.CreateObject() as Vampire;
            
            Debug.Assert(vampire != null);

            return vampire;
        }
    }

    public class Vampire : DynamicObject
    {
        private NPCIdentifier mIdentifier = new NPCIdentifier();

        public override string Render()
        {
            return mIdentifier.GetEmojiVariant(Emoji.GetEmoji(GetTypeID()));
        }

        public override void Stream(SaveStream stream)
        {
            base.Stream(stream);

            mIdentifier.Stream(stream);
        }
    }
}
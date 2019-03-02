using System.Diagnostics;

namespace GridExplorerBot
{
    public class VampireSetup : GridObjectSetup
    {
        public VampireSetup(Point position) : base(Emoji.People.Vampire, position)
        {

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
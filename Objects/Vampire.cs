using System.Diagnostics;

namespace GridExplorerBot
{
    public class VampireSetup : GridObjectSetup
    {
        public VampireSetup(Point position) : base(Emoji.GetRandomEmoji(Objects.ID.Vampire), position)
        {

        }

        public override GridObject CreateObject()
        {
            Vampire vampire = base.CreateObject() as Vampire;
            
            Debug.Assert(vampire != null);

            vampire.Setup(this);

            return vampire;
        }
    }

    public class Vampire : DynamicObject
    {
        int mDisplayEmojiIndex;

        public void Setup(VampireSetup setup)
        {
            mDisplayEmojiIndex = Emoji.GetEmojiIndex(mType, setup.mDisplayText);
        }

        public override string Render()
        {
            return Emoji.GetEmoji(mType, mDisplayEmojiIndex);
        }

        public override void Stream(SaveStream stream)
        {
            base.Stream(stream);

            stream.Stream(ref mDisplayEmojiIndex, SaveUtils.GetNumBitsToStoreValue(StringUtils.GetNumPersonEmojiVariations()));
        }
    }
}
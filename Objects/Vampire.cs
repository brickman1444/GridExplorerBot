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

        public override void Save(BitStreams.BitStream stream)
        {
            base.Save(stream);

            stream.WriteByte((byte)mDisplayEmojiIndex, 4); // 10 4
        }

        public override void Load(BitStreams.BitStream stream)
        {
            base.Load(stream);

            mDisplayEmojiIndex = stream.ReadByte(4);
        }
    }
}
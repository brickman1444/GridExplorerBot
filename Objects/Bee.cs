namespace GridExplorerBot
{
    public class Bee : DynamicObject
    {   
        bool mIsMovingTowardsFlower = true;

        public override string Simulate(string inCommand, Game game)
        {
            DynamicObject flower = game.mRoom.FindFirstDynamicObject(Objects.ID.Rose);

            MoveTowards(flower.mPosition, game.mRoom);

            return "";
        }

        public override void Save(BitStreams.BitStream stream)
        {
            base.Save(stream);

            stream.WriteBit((BitStreams.Bit)mIsMovingTowardsFlower);
        }

        public override void Load(BitStreams.BitStream stream)
        {
            base.Load(stream);

            mIsMovingTowardsFlower = stream.ReadBit().AsBool();
        }
    }
}
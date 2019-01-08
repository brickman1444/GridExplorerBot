using System.Diagnostics;

namespace GridExplorerBot
{
    public class Bee : DynamicObject
    {   
        bool mIsMovingTowardsFlower = true;

        public override string Simulate(string inCommand, Game game)
        {
            if (game.mRoom.GetStaticObject(mPosition).GetTypeID() == Objects.ID.SpiderWeb)
            {
                return "";
            }

            if (mIsMovingTowardsFlower)
            {
                DynamicObject flower = game.mRoom.FindFirstDynamicObject(Objects.ID.Rose);

                if (flower != null)
                {
                    if (MathUtils.ArePointsAdjacent(flower.GetPosition(), mPosition))
                    {
                        mIsMovingTowardsFlower = false;
                    }
                    else
                    {
                        MoveTowards(flower.GetPosition(), game.mRoom);
                    }
                }
            }
            else
            {
                HoneyPot honeyPot = (HoneyPot)game.mRoom.GetFirstObject(Objects.ID.HoneyPot);

                Debug.Assert(honeyPot != null);

                if (MathUtils.ArePointsAdjacent(honeyPot.GetPosition(), mPosition))
                {
                    mIsMovingTowardsFlower = true;
                }
                else
                {
                    MoveTowards(honeyPot.GetPosition(), game.mRoom);
                }
            }

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
using System.Diagnostics;

namespace GridExplorerBot
{
    public class Bee : DynamicObject
    {   
        bool mIsMovingTowardsFlower = true;

        public override string Simulate(string inCommand, Game game)
        {
            if (mIsMovingTowardsFlower)
            {
                DynamicObject flower = game.mRoom.FindFirstDynamicObject(Objects.ID.Rose);

                if (flower != null)
                {
                    if (MathUtils.ArePointsAdjacent(flower.mPosition, mPosition))
                    {
                        mIsMovingTowardsFlower = false;
                    }
                    else
                    {
                        MoveTowards(flower.mPosition, game.mRoom);
                    }
                }
            }
            else
            {
                Point? honeyPotPosition = game.mRoom.GetFirstStaticObjectPosition(Objects.ID.HoneyPot);

                Debug.Assert(honeyPotPosition != null);

                if (MathUtils.ArePointsAdjacent(honeyPotPosition.Value, mPosition))
                {
                    mIsMovingTowardsFlower = true;
                }
                else
                {
                    MoveTowards(honeyPotPosition.Value, game.mRoom);
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
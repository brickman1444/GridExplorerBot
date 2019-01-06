using System.Diagnostics;

namespace GridExplorerBot
{
    public class Spider : DynamicObject
    {
        public override string Simulate(string inCommand, Game game)
        {
            DynamicObject insectInWeb = FindInsectStuckInWeb(game.mRoom);
            
            if (insectInWeb != null)
            {
                if (MathUtils.ArePointsAdjacent(insectInWeb.mPosition, mPosition))
                {
                    game.mRoom.MarkObjectForDeletion(insectInWeb);
                }
                else
                {
                    MoveTowards(insectInWeb.mPosition, game.mRoom);
                }
            }
            else
            {
                DynamicObject playerCharacter = game.mRoom.FindFirstDynamicObject(Objects.ID.PlayerCharacter);

                Debug.Assert(playerCharacter != null);

                MoveVerticallyToBlock(playerCharacter, game.mRoom);
            }

            return "";
        }

        DynamicObject FindInsectStuckInWeb(Room room)
        {
            foreach (DynamicObject dynamicObject in room.mDynamicObjects)
            {
                if (ObjectTraits.GetObjectTraits(dynamicObject.mType).mIsInsect
                && room.GetStaticObject(dynamicObject.mPosition) == Objects.ID.SpiderWeb)
                {
                    return dynamicObject;
                }
            }

            return null;
        }
    }
}
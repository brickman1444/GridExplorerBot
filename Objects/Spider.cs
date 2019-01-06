using System.Diagnostics;

namespace GridExplorerBot
{
    public class Spider : DynamicObject
    {
        public override string Simulate(string inCommand, Game game)
        {
            DynamicObject playerCharacter = game.mRoom.FindFirstDynamicObject(Objects.ID.PlayerCharacter);

            Debug.Assert(playerCharacter != null);

            MoveVerticallyToBlock(playerCharacter, game.mRoom);

            return "";
        }
    }
}
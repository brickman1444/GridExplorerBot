
namespace GridExplorerBot
{
    public class MantelpieceClock : StaticObject
    {
        public override string GetDescriptionText(Game game)
        {
            return "An antique mantelpiece clock. It shows " + game.mGameTime.Render();
        }
    }
}
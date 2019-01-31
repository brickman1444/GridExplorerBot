
namespace GridExplorerBot
{
    public class Bat : DynamicObject
    {
        public override string Simulate(string command, Game game)
        {
            MoveRandomly(game.mRoom);
            return "";
        }

        public override string GiveObject(Objects.ID objectType, Game game)
        {
            if (objectType == Objects.ID.Blood)
            {
                game.mRoom.MarkObjectForDeletion(this);
                game.mRoom.SpawnObject(new VampireSetup(mPosition));
                return "The bat transforms into a vampire!";
            }
            else
            {
                return base.GiveObject(objectType, game);
            }
        }

        public override string TalkTo(Objects.ID subject, Game game)
        {
            if (subject == Objects.ID.Blood)
            {
                return "The bat's ears perk up when you mention \"blood\"";
            }
            else
            {
                return base.TalkTo(subject, game);
            }
        }
    }
}
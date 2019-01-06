using System.Linq;

namespace GridExplorerBot
{
    public class HoneyPot : DynamicObject
    {
        public override string Simulate(string inCommand, Game game)
        {
            DynamicObject bee = game.mRoom.FindFirstDynamicObject(Objects.ID.Bee);

            if (bee == null)
            {
                SpawnBee(game.mRoom);
            }

            return "";
        }

        void SpawnBee(Room room)
        {
            var adjacentPoints = MathUtils.GetAdjacentPoints(mPosition);

            var validPoints = from point in adjacentPoints
                              where point.IsWithinBounds() && room.CanSpaceBeMovedTo(point)
                              orderby MathUtils.GetDistance(mPosition, point) ascending
                              select point;
            
            if (!validPoints.Any())
            {
                return;
            }

            room.SpawnObject(Objects.ID.Bee, validPoints.First());
        }
    }
}
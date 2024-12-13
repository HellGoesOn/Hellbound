using Casull.Core.ECS.Components;
using Casull.Core.Overworld;

namespace Casull.Core.ECS
{
    public class TripWireSystem : IExecute
    {
        readonly Group<Entity> _group;

        public TripWireSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(HasCollidedMarker), typeof(TripWire)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                var id = entity.GetComponent<TripWire>().trigger;
                var playerId = entity.GetComponent<HasCollidedMarker>();

                var gameState = Main.instance.GetGameState();

                if (!context.entities[playerId.otherId].HasComponent<PlayerMarker>())
                    continue;

                if (gameState is World world) {
                    var trigger = World.triggers.FirstOrDefault(x => x.id == id);

                    if (trigger != null) {
                        trigger.Activate(world);
                        entity.RemoveComponent<TripWire>();
                    }
                }
            }
        }
    }
}

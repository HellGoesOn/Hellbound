using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Microsoft.Xna.Framework;

namespace Casull.Core.ECS
{
    public class TripWireSystem : IExecute
    {
        readonly Group<Entity> _group;

        public TripWireSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(HasCollidedMarker), typeof(TripWire), typeof(Transform)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                var toTrigger = entity.GetComponent<TripWire>();
                var playerId = entity.GetComponent<HasCollidedMarker>();

                var myPosition = entity.GetComponent<Transform>();

                var gameState = Main.instance.GetGameState();

                if (context.GetById(playerId.otherId) != null && !context.GetById(playerId.otherId).HasComponent<PlayerMarker>())
                    continue;

                if (gameState is World world) {
                    var trigger = World.triggers.FirstOrDefault(x => x.id == toTrigger.trigger);

                    if (trigger != null) {
                        trigger.Activate(world);
                        entity.RemoveComponent<TripWire>();
                        entity.RemoveComponent<CollisionBox>();
                    }
                }
            }
        }
    }
}

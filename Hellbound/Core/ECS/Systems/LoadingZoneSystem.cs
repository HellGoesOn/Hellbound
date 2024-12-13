using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Casull.Render;
using Microsoft.Xna.Framework;

namespace Casull.Core.ECS
{
    public class LoadingZoneSystem : IExecute
    {
        readonly Group<Entity> _group;
        public LoadingZoneSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(LoadingZone), typeof(HasCollidedMarker)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                var id = entity.GetComponent<LoadingZone>();
                var playerId = entity.GetComponent<HasCollidedMarker>();

                var gameState = Main.instance.GetGameState();

                if (!context.entities[playerId.otherId].HasComponent<PlayerMarker>())
                    continue;

                if (gameState is World world) {
                    var dir = id.direction;
                    if (dir == Vector2.Zero)
                        dir = Vector2.UnitY;

                    Main.instance.transitions.Add(new BlackFadeInFadeOut(Renderer.SaveFrame(true)));
                    Main.instance.ActiveWorld = World.LoadFromFile("\\Content\\Scenes", id.nextZone);
                    var trans = Main.instance.ActiveWorld.context.entities[0].GetComponent<Transform>();

                    if (id.newPosition != default) {
                        trans.position = id.newPosition;
                    }

                    Main.instance.ActiveWorld.GetCamera().centre = trans.position;
                    break;
                }
            }
        }
    }
}

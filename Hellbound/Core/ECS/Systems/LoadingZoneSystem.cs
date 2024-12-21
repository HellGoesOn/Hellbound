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
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(LoadingZone), typeof(CollisionBox)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                var id = entity.GetComponent<LoadingZone>();

                var gameState = Main.instance.GetGameState();

                var myBox = entity.GetComponent<CollisionBox>();

                foreach (int playerId in myBox.CollidedWith) {


                    if (context.GetById(playerId) != null && !context.GetById(playerId).HasComponent<PlayerMarker>())
                        continue;

                    if (gameState is World world) {
                        var dir = id.direction;
                        if (dir == Vector2.Zero)
                            dir = Vector2.UnitY;

                        Main.instance.transitions.Add(new BlackFadeInFadeOut(Renderer.SaveFrame(true)));
                        Main.instance.ActiveWorld = World.LoadFromFile("\\Content\\Scenes", id.nextZone);

                        var player = Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x.HasComponent<PlayerMarker>());

                        if (player != null) {
                            var trans = player.GetComponent<Transform>();
                            var tex = player.GetComponent<TextureComponent>();
                            if (id.newPosition != default) {
                                trans.position = id.newPosition;
                                tex.scale.X = Math.Sign(id.direction.X);
                            }

                            Main.instance.ActiveWorld.GetCamera().centre = trans.position;
                            Main.lastTransitionPosition = trans.position;
                        }
                        break;

                    }
                }
            }
        }
    }
}

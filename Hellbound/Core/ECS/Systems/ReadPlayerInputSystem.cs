using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Casull.Core.UI;
using Casull.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Casull.Core.ECS
{
    public class ReadPlayerInputSystem : IExecute
    {
        readonly Group<Entity> _group;

        public ReadPlayerInputSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(PlayerMarker), typeof(Velocity), typeof(TextureComponent)));
        }

        public void Execute(Context context)
        {
            bool dontListen = Main.instance.transitions.Count > 0 || UIManager.dialogueUI.dialogues.Count > 0 || World.cutscenes.Count > 0;

            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {

                var entity = entities[i];
                Velocity vel = entity.GetComponent<Velocity>();

                var marker = entity.GetComponent<PlayerMarker>();
                

                if (entity.HasComponent<NewAnimationComponent>()) {
                    var anim = entity.GetComponent<NewAnimationComponent>();

                    if (vel.value.Length() > 0)
                        anim.currentAnimation = "Run";
                    else
                        anim.currentAnimation = "Idle";
                }

                if (vel.X != 0) {
                    if (vel.X < 0)
                        entity.GetComponent<TextureComponent>().scale.X = -1;
                    else
                        entity.GetComponent<TextureComponent>().scale.X = 1;
                }
                if (!marker.preserveSpeed)
                    vel.X = vel.Y = 0;

                if (dontListen)
                    continue;
                var speed = 1.25f;
                Vector2 dir = Vector2.Zero;

                if (Input.HeldKey(Keys.LeftShift))
                    speed = 2.5f;

                if (Input.HeldKey(Keys.A)) dir.X -= speed;
                if (Input.HeldKey(Keys.W)) dir.Y -= speed;
                if (Input.HeldKey(Keys.S)) dir.Y += speed;
                if (Input.HeldKey(Keys.D)) dir.X += speed;

                vel.value = dir.SafeNormalize() * speed;
            }
        }
    }
}

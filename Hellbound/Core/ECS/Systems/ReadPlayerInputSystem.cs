using HellTrail.Core.Combat;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
using HellTrail.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
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
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                Velocity vel = entity.GetComponent<Velocity>();
                vel.X = vel.Y = 0;
                var speed = 1f;
                if (Input.HeldKey(Keys.A)) vel.X -= speed;
                if (Input.HeldKey(Keys.W)) vel.Y -= speed;
                if (Input.HeldKey(Keys.S)) vel.Y += speed;
                if (Input.HeldKey(Keys.D)) vel.X += speed;

                if (entity.HasComponent<NewAnimationComponent>())
                {
                    var anim = entity.GetComponent<NewAnimationComponent>();

                    if (vel.value.Length() > 0)
                        anim.currentAnimation = "Victory";
                    else
                        anim.currentAnimation = "Idle";
                }

                if (vel.X != 0)
                {
                    if (vel.X < 0)
                        entity.GetComponent<TextureComponent>().scale.X = -1;
                    else
                        entity.GetComponent<TextureComponent>().scale.X = 1;
                }
            }
        }
    }
}

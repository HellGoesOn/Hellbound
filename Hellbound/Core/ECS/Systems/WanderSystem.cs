using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Casull.Core.UI.Elements;
using Casull.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.ECS
{
    public class WanderSystem : IExecute
    {
        readonly Group<Entity> _group;

        public WanderSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(WanderComponent), typeof(Transform), typeof(Velocity)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                var velocity = entity.GetComponent<Velocity>();
                var pos = entity.GetComponent<Transform>().position;
                WanderComponent wander = entity.GetComponent<WanderComponent>();

                if (wander.startPosition == Vector2.Zero)
                    wander.startPosition = pos;

                if (wander.waitTime <= 0) {
                    wander.wanderTime = Main.rand.Next(wander.interestTimeMin, wander.interestTimeMax + 1);

                    var x = Main.instance.ActiveWorld.tileMap.width * DisplayTileLayer.TILE_SIZE;
                    var y = Main.instance.ActiveWorld.tileMap.height * DisplayTileLayer.TILE_SIZE;
                    if (Vector2.Distance(pos, wander.startPosition) > wander.leash)
                        wander.position = wander.startPosition;
                    else
                        wander.position = new Vector2(Main.rand.Next(0, x), Main.rand.Next(0, y));
                    wander.waitTime = Main.rand.Next(wander.waitTimeMin, wander.waitTimeMax);

                    if (entity.HasComponent<TextureComponent>()) {
                        var tex = entity.GetComponent<TextureComponent>();
                        if(Math.Abs(tex.scale.X) >= 1.0f)
                        if (wander.position.X < pos.X)
                            tex.scale.X = -1;
                        else
                            tex.scale.X = 1;
                    }
                }

                if(--wander.wanderTime > 0) {
                    velocity.value = (wander.position - pos).SafeNormalize() * wander.wanderSpeed;
                }
                else {
                    wander.waitTime--;
                    velocity.value = Vector2.Zero;
                }
            }
        }
    }
}

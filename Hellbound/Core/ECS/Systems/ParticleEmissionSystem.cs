using HellTrail.Core.ECS.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treeline.Core.Graphics;

namespace HellTrail.Core.ECS
{
    public class ParticleEmissionSystem : IExecute
    {
        readonly Group<Entity> _group;

        public ParticleEmissionSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(ParticleEmitter)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];

                Transform position = entity.GetComponent<Transform>();
                ParticleEmitter emitter = entity.GetComponent<ParticleEmitter>();

                Color clr = emitter.color[Main.rand.Next(emitter.color.Length)];

                float x = emitter.direction.X * Main.rand.Next(emitter.randMinX, emitter.randMaxX);
                float y = emitter.direction.Y * Main.rand.Next(emitter.randMinY, emitter.randMaxY);
                var velocity = new Vector2(x, y);
                Vector2 scale = emitter.scales[Main.rand.Next(emitter.scales.Length)];

                x = Main.rand.Next((int)emitter.randomOffset.X+1);
                y = Main.rand.Next((int)emitter.randomOffset.Y+1);

                Vector2 offset = emitter.offset + new Vector2(x, y);

                if (!emitter.additive)
                    for (int j = 0; j < emitter.amountPerFrame; j++)
                    {
                        Particle part = ParticleManager.NewParticle(new Vector3(position.position + offset, 0), new Vector3(velocity.X, 0, velocity.Y), emitter.lifeTime);
                        part.color = clr;
                        part.scale = scale;
                        part.diesToGravity = false;
                    } 
                else
                    for (int j = 0; j < emitter.amountPerFrame; j++)
                    {
                        Particle part = ParticleManager.NewParticleAdditive(new Vector3(position.position + offset, 0), new Vector3(velocity.X, 0, velocity.Y), emitter.lifeTime);
                        part.color = clr;
                        part.scale = scale;
                        part.diesToGravity = false;
                    }
            }
        }
    }
}

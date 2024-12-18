using Casull.Core.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Casull.Core.Graphics;

namespace Casull.Core.ECS
{
    public class ParticleEmissionSystem : IDraw
    {
        readonly Group<Entity> _group;

        public ParticleEmissionSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(ParticleEmitter)));
        }

        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];

                Transform position = entity.GetComponent<Transform>();
                ParticleEmitter emitter = entity.GetComponent<ParticleEmitter>();

                Color clr = emitter.color == null ? Color.White : emitter.color[Main.rand.Next(emitter.color.Length)];

                var getMinX = Math.Min(emitter.randMinX, emitter.randMaxX);
                var getMinY = Math.Min(emitter.randMinY, emitter.randMaxY);
                var getMaxX = Math.Max(emitter.randMaxX, emitter.randMinX);
                var getMaxY = Math.Max(emitter.randMinX, emitter.randMaxY);

                float x = emitter.direction.X * Main.rand.Next(getMinX, getMaxX);
                float y = emitter.direction.Y * Main.rand.Next(getMinY, getMaxY);
                var velocity = new Vector2(x, y);
                Vector2 scale = emitter.scales[Main.rand.Next(emitter.scales.Length)];

                x = Main.rand.Next((int)emitter.randomOffset.X + 1);
                y = Main.rand.Next((int)emitter.randomOffset.Y + 1);

                Vector2 offset = emitter.offset + new Vector2(x, y);

                if (!emitter.additive)
                    for (int j = 0; j < emitter.amountPerFrame; j++) {
                        Particle part = ParticleManager.NewParticle(new Vector3(position.position + offset, 0), new Vector3(velocity.X, velocity.Y, 0), emitter.lifeTime);
                        part.color = clr;
                        part.scale = scale;
                        part.diesToGravity = false;
                        part.castShadow = true;
                    }
                else
                    for (int j = 0; j < emitter.amountPerFrame; j++) {
                        Particle part = ParticleManager.NewParticleAdditive(new Vector3(position.position + offset, 0), new Vector3(velocity.X, velocity.Y, 0), emitter.lifeTime);
                        part.color = clr;
                        part.scale = scale;
                        part.diesToGravity = false;
                        part.castShadow = true;
                    }
            }
        }
    }
}

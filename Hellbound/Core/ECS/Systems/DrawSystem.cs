using HellTrail.Core.Combat;
using HellTrail.Core.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treeline.Core.Graphics;

namespace HellTrail.Core.ECS
{
    public class DrawSystem : IDraw
    {
        readonly Group<Entity> _group;

        public DrawSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(TextureComponent)));
        }


        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            var entities = _group.Entities;

            for(int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                TextureComponent tex = entity.GetComponent<TextureComponent>();
                Transform transform = entity.GetComponent<Transform>();

                spriteBatch.Draw(tex.texture, transform.position, null, Color.White, 0f, tex.origin, tex.scale, SpriteEffects.None, 0f);
                Color[] clrs = { Color.Red, Color.Yellow, Color.Orange, Color.Crimson };

                int xx = Main.rand.Next((32));
                int yy = Main.rand.Next((32));
                float velX = Main.rand.Next(60, 120) * 0.001f * (Main.rand.Next(2) == 0 ? -1 : 1);
                float velY = -0.2f * (Main.rand.Next(20, 60) * 0.05f);
                var particle = ParticleManager.NewParticleAdditive(new Vector3(transform.position + new Vector2(-32 * 0.5f + xx, 32 * 0.25f), 0), new Vector3(velX, 0, velY), 90);
                particle.color = clrs[Main.rand.Next(clrs.Length)];
                particle.endColor = Color.Black;
                particle.scale = Vector2.One * Main.rand.Next(1, 3);
                particle.weight = 0.01f;
            }
        }
    }
}

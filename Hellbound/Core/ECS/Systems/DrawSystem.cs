using HellTrail.Core.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class DrawSystem : IDraw
    {
        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            var entities = context.entities.Where(x => x != null && x.enabled && x.HasComponent<TextureComponent>() && x.HasComponent<Transform>()).ToArray();

            for(int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                TextureComponent tex = entity.GetComponent<TextureComponent>();
                Transform transform = entity.GetComponent<Transform>();

                spriteBatch.Draw(tex.texture, transform.position, null, Color.White, 0f, tex.origin, tex.scale, SpriteEffects.None, 0f);
            }
        }
    }
}

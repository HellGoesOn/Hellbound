using HellTrail.Core.Combat;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.Overworld;
using HellTrail.Extensions;
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
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(TextureComponent)).NoneOf(typeof(AnimationComponent)));
        }


        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            var entities = _group.Entities;

            for(int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                TextureComponent tex = entity.GetComponent<TextureComponent>();
                Transform transform = entity.GetComponent<Transform>();

                float depth = transform.position.Y / (Main.instance.ActiveWorld.tileMap.height * DisplayTileLayer.TILE_SIZE);
                spriteBatch.Draw(Assets.GetTexture(tex.textureName), transform.position, null, Color.White, 0f, tex.origin, tex.scale, SpriteEffects.None, depth);
            }
        }
    }
}

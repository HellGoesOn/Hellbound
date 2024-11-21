using HellTrail.Core.ECS.Components;
using HellTrail.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class DrawAnimationSystem : IDraw, IExecute
    {
        readonly Group<Entity> _group;

        public DrawAnimationSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(TextureComponent), typeof(AnimationComponent)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                var animation = entity.GetComponent<AnimationComponent>();

                if (animation.currentFrame >= animation.frames.Length)
                    animation.currentFrame = 0;

                if (++animation.time > animation.frameSpeed)
                {
                    if(++animation.currentFrame >= animation.frames.Length)
                    {
                        animation.currentFrame = 0;
                    }

                    animation.time = 0;
                }
            }
        }


        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            var entities = _group.Entities;
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                var texture = entity.GetComponent<TextureComponent>();
                var animation = entity.GetComponent<AnimationComponent>();
                var transform = entity.GetComponent<Transform>();
                float depth = transform.position.Y / 32.0f * 30;

                if (animation.currentFrame >= animation.frames.Length)
                    continue;

                spriteBatch.Draw(Assets.GetTexture(texture.textureName),
                    transform.position.ToInt(),
                    animation.GetRect(),
                    Color.White,
                    0f,
                    texture.origin,
                    texture.scale,
                    SpriteEffects.None,
                    depth);
            }
        }
    }
}

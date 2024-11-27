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

                if (animation.frames == null || animation.currentFrame >= animation.frames.Length)
                {
                    animation.currentFrame = 0;
                    animation.time = 0;
                    continue;
                }

                if (++animation.time > animation.frameSpeed)
                {
                    if(animation.currentFrame < animation.frames.Length-1)
                    {
                        animation.currentFrame++;
                    }
                    else
                    {
                        if(animation.looping)
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
                float depth = transform.position.Y / (Main.instance.ActiveWorld.tileMap.height * DisplayTileLayer.TILE_SIZE);

                int c = animation.currentFrame;

                if (animation.frames == null || c >= animation.frames.Length)
                    continue;
                var originalRect = animation.GetRect();
                Vector2 scale = c < animation.scales.Length ? (animation.scales[c] != Vector2.Zero ? animation.scales[c] : Vector2.One) : Vector2.One;
                Vector2 origin = c < animation.origins.Length ? (animation.origins[c] != Vector2.Zero ? animation.origins[c] : texture.origin) : texture.origin;
                spriteBatch.Draw(Assets.GetTexture(texture.textureName),
                    transform.position.ToInt(),
                    originalRect,
                    Color.White,
                    0f,
                    texture.origin,
                    texture.scale * scale,
                    SpriteEffects.None,
                    depth);
            }
        }
    }
}

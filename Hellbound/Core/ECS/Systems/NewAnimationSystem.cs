using HellTrail.Core.ECS.Components;
using HellTrail.Core.Overworld;
using HellTrail.Core.UI;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class NewAnimationSystem : IDraw
    {
        readonly Group<Entity> _group;

        public NewAnimationSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(NewAnimationComponent), typeof(Transform), typeof(TextureComponent)));
        }

        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            var entities = _group.Entities;

            int count = entities.Count;

            for (int i = 0; i < count; i++)
            {
                var entity = entities[i];

                var animations = entity.GetComponent<NewAnimationComponent>();
                var transform = entity.GetComponent<Transform>();
                var textureComponent = entity.GetComponent<TextureComponent>();

                var texture = Assets.GetTexture($"{animations.baseTextureName}_{animations.currentAnimation}");

                bool hasAnim = animations.animations.ContainsKey(animations.currentAnimation);

                var timeUntilNext = animations.frames.Length > 0 && animations.currentFrame < animations.frames.Length ? animations.frames[animations.currentFrame].timeUntilNext : -1;

                int startIndex = 0, endIndex = 0;

                var frame = new Frame(0, 0, 0, 0, 0);

                if (hasAnim)
                {
                    startIndex = animations.CurrentAnimation.start;
                    endIndex = animations.CurrentAnimation.end;
                }

                if (animations.currentFrame < startIndex || animations.currentFrame > endIndex)
                {
                    animations.elapsedTime = 0;
                    animations.currentFrame = startIndex;
                }

                if (++animations.elapsedTime > timeUntilNext)
                {
                    if (++animations.currentFrame > endIndex)
                    {
                        animations.currentFrame = startIndex;
                    }

                    animations.elapsedTime = 0;
                }

                if (timeUntilNext != -1 && animations.currentFrame < animations.frames.Length)
                    frame = animations.frames[animations.currentFrame];

                Rectangle rect = new(frame.x, frame.y, frame.width, frame.height);

                float depth = 1f + Math.Abs(transform.position.Y) + DisplayTileLayer.TILE_SIZE + 8 + transform.layer * DisplayTileLayer.TILE_SIZE + 12 * transform.layer;
                Renderer.Draw(texture, transform.position, rect, Color.White, 0f, textureComponent.origin, textureComponent.scale, SpriteEffects.None, depth);
            }
        }
    }
}

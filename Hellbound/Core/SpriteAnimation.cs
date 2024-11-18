using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core
{
    public class SpriteAnimation
    {
        public int currentFrame;
        public int frameCounter;
        public int timePerFrame;
        public bool looping;
        public bool finished;
        public float opacity;
        public float rotation;
        public float depth;
        public string texture;
        public string nextAnimation;

        public Vector2 position;
        public Vector2 scale;
        public Color color;
        public FrameData[] frameData;

        public AnimationEventHandler onAnimationPlay;
        public AnimationEventHandler onAnimationEnd;

        public SpriteAnimation(string texture, FrameData[] frameData)
        {
            color = Color.White;
            opacity = 1.0f;
            this.texture = texture;
            this.frameData = frameData;
        }

        public void Update()
        {
            if(++frameCounter > timePerFrame)
            {
                if(++currentFrame >= FrameCount)
                {
                    currentFrame = FrameCount-1;
                    if (looping)
                        currentFrame = 0;
                    else
                    {
                        if (!finished)
                            onAnimationEnd?.Invoke(this);
                        finished = true;
                    }
                }
                frameCounter = 0;
            }

            onAnimationPlay?.Invoke(this);
        }

        public void Reset()
        {
            frameCounter = 0;
            currentFrame = 0;
            finished = false;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 scale = default)
        {
            if (scale == default)
                scale = Vector2.One;

            Texture2D tex = Assets.Textures[texture];
            Vector2 origin = new Vector2(frameData[currentFrame].width, frameData[0].height) * 0.5f;

            spriteBatch.Draw(tex, position, frameData[currentFrame].AsRect, color * opacity, rotation, origin, frameData[currentFrame].scale * scale, SpriteEffects.None, depth);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 scale = default, float? depth = null)
        {
            depth ??= this.depth;

            if (scale == default)
                scale = Vector2.One;

            Texture2D tex = Assets.Textures[texture];
            Vector2 origin = new Vector2(frameData[currentFrame].width, frameData[0].height) * 0.5f;

            spriteBatch.Draw(tex, position, frameData[currentFrame].AsRect, color * opacity, rotation, origin, frameData[currentFrame].scale * scale, SpriteEffects.None, (float)depth);
        }

        public SpriteAnimation GetCopy()
        {
            SpriteAnimation anim = new(texture, frameData)
            {
                timePerFrame = this.timePerFrame
            };
            return anim;
        }

        public int FrameCount => frameData.Length;
    }

    public struct FrameData
    {
        public int x, y, width, height;

        public Vector2 scale;

        public FrameData(int x, int y, int width, int height, Vector2 scale = default)
        {
            this.scale = scale;
            if (scale == default)
                this.scale = Vector2.One;
            this.x = x; 
            this.y = y;
            this.width = width; 
            this.height = height;
        }

        public readonly Rectangle AsRect => new (x, y, width, height);
    }

    public delegate void AnimationEventHandler(SpriteAnimation sender);
}

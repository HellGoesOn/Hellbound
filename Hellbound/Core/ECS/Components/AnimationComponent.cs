using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class AnimationComponent : IComponent
    {
        internal int time;
        internal int currentFrame;
        public int width;
        public int height;
        public int frameSpeed;
        public Vector2[] frames;

        public AnimationComponent(int width, int height, int frameSpeed, params Vector2[] frames)
        {
            this.width = width;
            this.height = height;
            this.frameSpeed = frameSpeed;
            this.frames = frames;
        }

        public Rectangle GetRect()
        {
            int x = width * (int)frames[currentFrame].X;
            int y = height * (int)frames[currentFrame].Y;
            return new Rectangle(x, y, width, height);
        }
    }
}

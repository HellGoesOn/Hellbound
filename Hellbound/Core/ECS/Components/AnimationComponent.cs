//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HellTrail.Core.ECS.Components
//{
//    public class AnimationComponent : IComponent
//    {
//        internal int time;
//        internal int currentFrame;
//        public bool looping;
//        public int width;
//        public int height;
//        public int frameSpeed;
//        public Vector2[] frames;
//        public Vector2[] scales;
//        public Vector2[] origins;

//        public AnimationComponent(bool looping, int width, int height, int frameSpeed, Vector2[] frames, Vector2[] scales, Vector2[] origins)
//        {
//            this.looping = looping;
//            this.width = width;
//            this.height = height;
//            this.frameSpeed = frameSpeed;
//            this.frames = frames;
//            this.scales = scales;
//            this.origins = origins;
//        }

//        public Rectangle GetRect()
//        {
//            int x = width * (int)frames[currentFrame].X;
//            int y = height * (int)frames[currentFrame].Y;
//            return new Rectangle(x, y, width, height);
//        }
//    }
//}

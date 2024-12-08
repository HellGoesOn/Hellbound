using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class NewAnimationComponent : IComponent
    {
        internal int currentFrame;
        internal int elapsedTime;
        public string baseTextureName;
        public string currentAnimation;
        public Dictionary<string, Animation> animations;
        public Frame[] frames;
        public NewAnimationComponent(string baseTextureName, string defaultAnimation, Dictionary<string, Animation> animations, params Frame[] frames)
        {
            this.baseTextureName = baseTextureName;
            currentAnimation = defaultAnimation;
            this.animations = animations;
            this.frames = frames;
        }

        public Animation CurrentAnimation => animations[currentAnimation];
    }

    public struct Animation
    {
        public int start;
        public int end;
        public Animation(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append('(');
            sb.Append(start + " ");
            sb.Append(end);
            sb.Append(')');
            return sb.ToString();
        }

        public static bool TryParse(string s, ref Animation result)
        {
            string[] values = Regex.Replace(s, "[^0-9 ]", "").Split(" ");
            if(values.Length < 2 || !int.TryParse(values[0], out int a) || !int.TryParse(values[1], out int b))
            {
                result = default;
                return false;
            }

            result = new Animation(a, b);
            return true;
        }
    }

    public struct Frame
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public int timeUntilNext;

        public Frame(int x, int y, int width, int height, int timeUntilNext)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.timeUntilNext = timeUntilNext;
        }

        public static bool TryParse(string input, ref Frame result)
        {
            string[] values = Regex.Replace(input, "[^0-9 ]", "").Split(" ");
            if (values.Length < 5 
                || !int.TryParse(values[0], out int x) 
                || !int.TryParse(values[1], out int y)
                || !int.TryParse(values[2], out int w)
                || !int.TryParse(values[3], out int h)
                || !int.TryParse(values[4], out int tun)
                )
            {
                result = default;
                return false;
            }

            result = new Frame(x, y, w, h, tun);
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append('{');
            sb.Append(x + " ");
            sb.Append(y + " ");
            sb.Append(width + " ");
            sb.Append(height + " ");
            sb.Append(timeUntilNext);
            sb.Append('}');
            return sb.ToString();
        }
    }
}

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
        public string currentAnimation;
        public Dictionary<string, Animation> animations;
        public Frame[] frames;
        public NewAnimationComponent(string defaultAnimation, Dictionary<string, Animation> animations, params Frame[] frames)
        {
            currentAnimation = defaultAnimation;
            this.animations = animations;
            this.frames = frames;
        }
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
        public int width;
        public int height;
        public int timeUntilNext;

        public Frame(int width, int height, int timeUntilNext)
        {
            this.width = width;
            this.height = height;
            this.timeUntilNext = timeUntilNext;
        }

        public static bool TryParse(string input, ref Frame result)
        {
            string[] values = Regex.Replace(input, "[^0-9 ]", "").Split(" ");
            if (values.Length < 3 
                || !int.TryParse(values[0], out int a) 
                || !int.TryParse(values[1], out int b)
                || !int.TryParse(values[2], out int c)
                )
            {
                result = default;
                return false;
            }

            result = new Frame(a, b, c);
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append('{');
            sb.Append(width + " ");
            sb.Append(height + " ");
            sb.Append(timeUntilNext);
            sb.Append('}');
            return sb.ToString();
        }
    }
}

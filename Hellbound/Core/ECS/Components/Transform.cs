using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class Transform : IComponent
    {
        public Vector2 position;
        public float origin;
        public float layer;

        public Transform(Vector2 position, float? origin = null)
        {
            this.position = position;
            this.origin = origin ?? 32;
        }

        public Transform(float x, float y) : this(new Vector2(x, y))
        {
        }

        public Vector2 ToDraw => new(MathF.Round(position.X), MathF.Round(position.Y));
    }
}

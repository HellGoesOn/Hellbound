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

        public Transform(Vector2 position)
        {
            this.position = position;
        }

        public Transform(float x, float y) : this(new Vector2(x, y))
        {
        }

        public override string ToString()
        {
            return $"Position:[{position}]";
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class CollisionBox : IComponent
    {
        public int width;
        public int height;
        public Vector2 origin;
        public CollisionBox(int width, int height, Vector2? origin = null)
        {
            this.width = width;
            this.height = height;
            this.origin = origin ?? new Vector2(width, height) * 0.5f;
        }
    }

    public delegate void OnCollision(Entity me, Entity other);
}

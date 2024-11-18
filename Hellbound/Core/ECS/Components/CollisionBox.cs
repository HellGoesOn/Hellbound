using HellTrail.Core.ECS.Attributes;
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
        public OnCollision onCollide;
        public CollisionBox(int width, int height, Vector2? origin = null, OnCollision onCollide = null)
        {
            this.width = width;
            this.height = height;
            this.origin = origin ?? new Vector2(width, height) * 0.5f;
            this.onCollide ??= onCollide;
        }
    }

    public delegate void OnCollision(Entity me, Entity other);
}

using Microsoft.Xna.Framework;

namespace Casull.Core.ECS.Components
{
    public class CollisionBox : IComponent
    {
        public int width;
        public int height;
        public Vector2 origin;
        public float radius;
        public CollisionBox(int width, int height, Vector2? origin = null)
        {
            this.width = width;
            this.height = height;
            this.origin = origin ?? new Vector2(width, height) * 0.5f;
        }
    }

    public delegate void OnCollision(Entity me, Entity other);
}

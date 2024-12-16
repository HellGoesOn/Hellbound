using Microsoft.Xna.Framework;

namespace Casull.Core.ECS.Components
{
    public class CollisionBox : IComponent
    {
        public int width;
        public int height;
        public Vector2 origin;
        public float radius;
        List<int> collidedWith;

        internal List<int> CollidedWith {
            get {
                if (collidedWith == null) {
                    collidedWith = new List<int>();
                }
                return collidedWith;
            }
        }

        public CollisionBox(int width, int height, Vector2? origin = null)
        {
            collidedWith = new List<int>();
            this.width = width;
            this.height = height;
            this.origin = origin ?? new Vector2(width, height) * 0.5f;
        }
    }

    public delegate void OnCollision(Entity me, Entity other);
}

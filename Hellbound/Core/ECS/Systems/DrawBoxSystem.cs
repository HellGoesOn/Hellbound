using Casull.Core.ECS.Components;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.ECS
{
    public class DrawBoxSystem : IDraw
    {
        readonly Group<Entity> _group;

        public DrawBoxSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(CollisionBox)));
        }


        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            var entities = _group.Entities;
            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                var box = entity.GetComponent<CollisionBox>();
                Transform transform = entity.GetComponent<Transform>();

                Renderer.DrawRectToWorld(transform.position - box.origin, new Vector2(box.width, box.height), Color.Red * 0.15f, float.MaxValue);
                var rect = new Rectangle((int)(transform.position.X - box.origin.X), (int)(transform.position.Y - box.origin.Y), box.width, box.height);
                var point = new Point((int)Input.MousePosition.X, (int)Input.MousePosition.Y);
            }
        }
    }
}

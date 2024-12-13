using Casull.Core.ECS.Components;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.ECS
{
    public class DrawTransformBoxSystem : IDraw
    {
        readonly Group<Entity> _group;

        public DrawTransformBoxSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform)));
        }

        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            var entities = _group.Entities;
            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                Transform transform = entity.GetComponent<Transform>();

                float depth = 10000f;
                Renderer.DrawRectToWorld(transform.ToDraw - new Vector2(4), new Vector2(8), Color.Lime, depth);
            }
        }
    }
}

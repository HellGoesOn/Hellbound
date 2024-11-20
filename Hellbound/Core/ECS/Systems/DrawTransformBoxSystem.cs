using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
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
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                Transform transform = entity.GetComponent<Transform>();

                Renderer.DrawRect(spriteBatch, transform.position-new Vector2(4), new Vector2(8), 1, Color.Lime, 1f);
            }
        }
    }
}

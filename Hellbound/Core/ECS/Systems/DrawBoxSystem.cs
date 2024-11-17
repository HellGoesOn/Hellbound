using HellTrail.Core.Combat;
using HellTrail.Core.ECS.Components;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treeline.Core.Graphics;

namespace HellTrail.Core.ECS
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

            for(int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var tex = entity.GetComponent<CollisionBox>();
                Transform transform = entity.GetComponent<Transform>();

                Renderer.DrawRect(spriteBatch, transform.position - tex.origin, new Vector2(tex.width, tex.height), 1, Color.Red * 0.15f);
            }
        }
    }
}

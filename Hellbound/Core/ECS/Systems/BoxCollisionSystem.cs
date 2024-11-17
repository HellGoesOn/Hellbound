using HellTrail.Core.ECS.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    // TO-DO: proper collision checks
    public class BoxCollisionSystem : IExecute
    {
        readonly Group<Entity> _group;

        public BoxCollisionSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(CollisionBox), typeof(Transform)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var box1 = entity.GetComponent<CollisionBox>();
                var transform1 = entity.GetComponent<Transform>();
                for (int j = 0; j < entities.Count; j++)
                {
                    var entity2 = entities[j];
                    if (entity == entity2)
                        continue;

                    var box2 = entity2.GetComponent<CollisionBox>();
                    var transform2 = entity2.GetComponent<Transform>();

                    if (CheckCollision(transform1.position - box1.origin, box1, transform2.position - box1.origin, box2))
                    {
                        box1.onCollide?.Invoke(entity, entity2);
                        box2.onCollide?.Invoke(entity2, entity);
                    }
                }
            }
        }

        public bool CheckCollision(Vector2 position1, CollisionBox box1, Vector2 position2, CollisionBox box2)
        {
            Rectangle rect1 = new Rectangle((int)position1.X, (int)position1.Y, box1.width, box1.height); 
            Rectangle rect2 = new Rectangle((int)position2.X, (int)position2.Y, box2.width, box2.height); 

            return rect1.Intersects(rect2);
        }
    }
}

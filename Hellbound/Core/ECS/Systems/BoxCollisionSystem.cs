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
                var firstEntity = entities[i];
                var firstBox = firstEntity.GetComponent<CollisionBox>();
                var firstTransform = firstEntity.GetComponent<Transform>();
                for (int j = 0; j < entities.Count; j++)
                {
                    var secondEntity = entities[j];
                    if (firstEntity == secondEntity)
                        continue;

                    var secondBox = secondEntity.GetComponent<CollisionBox>();
                    var secondTransform = secondEntity.GetComponent<Transform>();

                    if (CheckCollision(firstTransform.position - firstBox.origin, firstBox, secondTransform.position - secondBox.origin, secondBox))
                    {
                        firstEntity.AddComponent(new HasCollidedMarker(secondEntity.id));
                        secondEntity.AddComponent(new HasCollidedMarker(firstEntity.id));
                    }
                }
            }
        }

        public bool CheckCollision(Vector2 position1, CollisionBox box1, Vector2 position2, CollisionBox box2)
        {
            Rectangle rect1 = new((int)position1.X, (int)position1.Y, box1.width, box1.height); 
            Rectangle rect2 = new((int)position2.X, (int)position2.Y, box2.width, box2.height); 

            return rect1.Intersects(rect2);
        }
    }
}

using Casull.Core.ECS.Components;
using Microsoft.Xna.Framework;

namespace Casull.Core.ECS
{
    // TO-DO: proper collision checks
    public class ShitCollisionSystem : IExecute
    {
        readonly Group<Entity> _group;

        public ShitCollisionSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(CollisionBox), typeof(Transform)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var firstEntity = entities[i];
                var firstBox = firstEntity.GetComponent<CollisionBox>();
                var firstTransform = firstEntity.GetComponent<Transform>();
                for (int j = 0; j < entities.Count; j++) {
                    var secondEntity = entities[j];
                    if (firstEntity == secondEntity)
                        continue;

                    var secondBox = secondEntity.GetComponent<CollisionBox>();
                    var secondTransform = secondEntity.GetComponent<Transform>();

                    if (firstBox.radius > 0) {
                        if (Vector2.Distance(firstTransform.position, secondTransform.position) <= firstBox.radius) {
                            if(!firstBox.CollidedWith.Contains(secondEntity.id))
                                firstBox.CollidedWith.Add(secondEntity.id);
                            if (!secondBox.CollidedWith.Contains(firstEntity.id))
                                secondBox.CollidedWith.Add(firstEntity.id);
                        }
                    }
                    else if (CheckCollision(firstTransform.position - firstBox.origin, firstBox, secondTransform.position - secondBox.origin, secondBox)) {

                        if (!firstBox.CollidedWith.Contains(secondEntity.id))
                            firstBox.CollidedWith.Add(secondEntity.id);
                        if (!secondBox.CollidedWith.Contains(firstEntity.id))
                            secondBox.CollidedWith.Add(firstEntity.id);
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

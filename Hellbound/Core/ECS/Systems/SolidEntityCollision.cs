using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Microsoft.Xna.Framework;

namespace Casull.Core.ECS
{
    public class SolidEntityCollision : IExecute
    {
        readonly Group<Entity> _group;

        public SolidEntityCollision(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(CollisionBox), typeof(Velocity)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;
            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];

                var transform = entity.GetComponent<Transform>();
                var box = entity.GetComponent<CollisionBox>();
                var velocity = entity.GetComponent<Velocity>();

                var hitBoxCenter = transform.position - box.origin + new Vector2(box.width, box.height) * 0.5f;
                var offsetHack = transform.position - hitBoxCenter;

                for (int k = 0; k < entities.Count; k++) {
                    if (entities[k] == entity)
                        continue;

                    var otherEntity = entities[k];

                    var otherBox = otherEntity.GetComponent<CollisionBox>();
                    var otherPos = otherEntity.GetComponent<Transform>().position;

                    float entityMinX = transform.position.X - box.origin.X;
                    float entityMaxX = transform.position.X - box.origin.X + box.width;
                    float entityMinY = transform.position.Y - box.origin.Y;
                    float entityMaxY = transform.position.Y - box.origin.Y + box.height;

                    float tileMinX = otherPos.X - otherBox.origin.X;
                    float tileMaxX = otherPos.X - otherBox.origin.X + otherBox.width;
                    float tileMinY = otherPos.Y - otherBox.origin.Y;
                    float tileMaxY = otherPos.Y - otherBox.origin.Y + otherBox.height;

                    if (otherBox.solid) {
                        bool xAxisCollision = !(entityMaxX < tileMinX || tileMaxX < entityMinX);
                        bool yAxisCollision = !(entityMaxY < tileMinY || tileMaxY < entityMinY);


                        if (xAxisCollision && yAxisCollision) {

                            // Determine which side of the entity collided
                            float overlapLeft = entityMaxX - tileMinX;
                            float overlapRight = tileMaxX - entityMinX;
                            float overlapTop = entityMaxY - tileMinY;
                            float overlapBottom = tileMaxY - entityMinY;

                            // Resolve the collision by determining which overlap is smallest
                            if (overlapLeft < overlapRight && overlapLeft < overlapTop && overlapLeft < overlapBottom) {
                                // Collision on the left side, push entity right
                                transform.position.X = tileMinX - box.width + box.origin.X;
                                velocity.X = 0;
                            }
                            else if (overlapRight < overlapLeft && overlapRight < overlapTop && overlapRight < overlapBottom) {
                                // Collision on the right side, push entity left
                                transform.position.X = tileMaxX + box.origin.X;
                                velocity.X = 0;
                            }

                            if (overlapTop <= overlapLeft && overlapTop <= overlapRight && overlapTop < overlapBottom) {
                                // Collision on the top side, push entity down
                                transform.position.Y = tileMinY + offsetHack.Y - box.height * 0.5f;
                                velocity.Y = 0;
                            }
                            else if (overlapBottom <= overlapLeft && overlapBottom <= overlapRight && overlapBottom < overlapTop) {
                                // Collision on the bottom side, push entity up
                                transform.position.Y = tileMaxY + box.height * 0.5f + offsetHack.Y;
                                velocity.Y = 0;
                            }
                        }
                    }

                    hitBoxCenter = transform.position - box.origin + new Vector2(box.width, box.height) * 0.5f;
                    offsetHack = transform.position - hitBoxCenter;

                    //Renderer.DrawRectToWorld(tilePosition * TILE_SIZE, new Vector2(32), 1, clr * 0.25f, 10000f);

                    //Renderer.DrawRectToWorld(new(tileMinX, tileMinY), new Vector2(1), 1, Color.Yellow, 10000f);
                    //Renderer.DrawRectToWorld(new Vector2(tileMaxX, tileMaxY), new Vector2(1), 1, Color.Yellow, 10000f);

                    //Renderer.DrawRectToWorld(new(entityMinX, entityMinY), new Vector2(1), 1, Color.Yellow, 10000f);
                    //Renderer.DrawRectToWorld(new Vector2(entityMaxX, entityMaxY), new Vector2(1), 1, Color.Yellow, 10000f);

                }
            }
        }
    }
}
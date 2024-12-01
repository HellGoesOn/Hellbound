using HellTrail.Core.ECS.Components;
using HellTrail.Core.Overworld;
using HellTrail.Extensions;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class TileCollisionSystem : IExecute
    {
        readonly Group<Entity> _group;

        public TileCollisionSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(CollisionBox), typeof(Velocity)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;
            for (int i = 0; i < entities.Count; i++)
            {
                const int TILE_SIZE = DisplayTileLayer.TILE_SIZE;
                var entity = entities[i];

                var transform = entity.GetComponent<Transform>();
                var box = entity.GetComponent<CollisionBox>();
                var velocity = entity.GetComponent<Velocity>();

                var hitBoxCenter = transform.position - box.origin + new Vector2(box.width, box.height) * 0.5f;
                var offsetHack = transform.position - hitBoxCenter;

                var floored = new Vector2((int)MathF.Floor(hitBoxCenter.X / TILE_SIZE), (int)(MathF.Floor(hitBoxCenter.Y) / TILE_SIZE));

                var tileMap = Main.instance.ActiveWorld.tileMap;

                Vector2[] tilesToCheck =
                {
                    new(0, -1),
                    new(1, 0),
                    new(-1, 0),
                    new(0, 1),
                    new(-1, -1),
                   new(1, -1),
                    new(1, 1),
                    new(-1, 1)
                };

                for (int k = 0; k < tilesToCheck.Length; k++)
                {
                    Vector2 tilePosition = (floored + tilesToCheck[k]);

                    if (tilePosition.X < 0 || tilePosition.X >= tileMap.width
                        || tilePosition.Y < 0 || tilePosition.Y >= tileMap.height)
                        continue;

                    bool solidTile = tileMap.GetTileElevation((int)tilePosition.X, (int)tilePosition.Y) != transform.layer;

                    Color clr =  solidTile ? Color.Red : Color.Lime;

                    float entityMinX = transform.position.X - box.origin.X;
                    float entityMaxX = transform.position.X - box.origin.X + box.width;
                    float entityMinY = transform.position.Y - box.origin.Y;
                    float entityMaxY = transform.position.Y - box.origin.Y + box.height;

                    float tileMinX = tilePosition.X * TILE_SIZE;
                    float tileMaxX = tilePosition.X * TILE_SIZE + TILE_SIZE;
                    float tileMinY = tilePosition.Y * TILE_SIZE;
                    float tileMaxY = tilePosition.Y *TILE_SIZE + TILE_SIZE;

                    if (solidTile)
                    {
                        bool xAxisCollision = !(entityMaxX < tileMinX || tileMaxX < entityMinX);
                        bool yAxisCollision = !(entityMaxY < tileMinY || tileMaxY < entityMinY);


                        if (solidTile && xAxisCollision && yAxisCollision)
                        {
                            clr = Color.Orange;

                            // Determine which side of the entity collided
                            float overlapLeft = entityMaxX - tileMinX;
                            float overlapRight = tileMaxX - entityMinX;
                            float overlapTop = entityMaxY - tileMinY;
                            float overlapBottom = tileMaxY - entityMinY;

                            // Resolve the collision by determining which overlap is smallest
                            if (overlapLeft < overlapRight && overlapLeft < overlapTop && overlapLeft < overlapBottom)
                            {
                                // Collision on the left side, push entity right
                                transform.position.X = tileMinX - box.width + box.origin.X;
                                velocity.X = 0;
                            } else if (overlapRight < overlapLeft && overlapRight < overlapTop && overlapRight < overlapBottom)
                            {
                                // Collision on the right side, push entity left
                                transform.position.X = tileMaxX + box.origin.X;
                                velocity.X = 0;
                            } 
                            
                            if (overlapTop <= overlapLeft && overlapTop <= overlapRight && overlapTop < overlapBottom)
                            {
                                // Collision on the top side, push entity down
                                transform.position.Y = tileMinY + offsetHack.Y - box.height * 0.5f;
                                velocity.Y = 0;
                            } 
                            else if (overlapBottom <= overlapLeft && overlapBottom <= overlapRight && overlapBottom < overlapTop)
                            {
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
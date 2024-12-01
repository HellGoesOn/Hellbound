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
                const int tileSize = DisplayTileLayer.TILE_SIZE;
                var entity = entities[i];

                var transform = entity.GetComponent<Transform>();
                var box = entity.GetComponent<CollisionBox>();
                var velocity = entity.GetComponent<Velocity>();
                var direction = velocity.value.SafeNormalize();

                var hitBoxCenter = transform.position - box.origin + new Vector2(box.width, box.height) * 0.5f;
                var offsetHack = transform.position - hitBoxCenter;

                var floored = new Vector2((int)MathF.Floor(hitBoxCenter.X / tileSize), (int)(MathF.Floor(hitBoxCenter.Y) / tileSize));

                var tileMap = Main.instance.ActiveWorld.tileMap;

                var xAxisTile = floored + new Vector2(Math.Clamp(MathF.Round(velocity.X, MidpointRounding.AwayFromZero), -1, 1), 0);
                var diagonalTile = floored + velocity.value;

                var xAxisCheck = xAxisTile * tileSize;

                if (xAxisTile.X >= 0 && xAxisTile.X < tileMap.width
                    && xAxisTile.Y >= 0 && xAxisTile.Y < tileMap.height)
                {
                    if (tileMap.GetTileElevation((int)xAxisTile.X, (int)xAxisTile.Y) != transform.layer)
                    {
                        if (hitBoxCenter.X + box.width * 0.5f > xAxisCheck.X && xAxisCheck.X + tileSize > hitBoxCenter.X)
                        {
                            transform.position.X = xAxisCheck.X - box.width * 0.5f + offsetHack.X;
                            velocity.value.X = 0;
                        }
                        if (hitBoxCenter.X - box.width * 0.5f < xAxisCheck.X + tileSize && xAxisCheck.X < hitBoxCenter.X)
                        {
                            transform.position.X = xAxisCheck.X + tileSize + box.width * 0.5f + offsetHack.X;
                            velocity.value.X = 0;
                        }
                    }
                }

                var yAxisTile = floored + new Vector2(0, Math.Clamp(MathF.Round(velocity.Y, MidpointRounding.AwayFromZero), -1, 1));
                var yAxisCheck = yAxisTile * tileSize;

                if (yAxisTile.Y >= 0 && yAxisTile.Y < tileMap.height
                    && yAxisTile.X >= 0 && yAxisTile.X < tileMap.width)
                {
                    if (tileMap.GetTileElevation((int)yAxisTile.X, (int)yAxisTile.Y) != transform.layer)
                    {
                        if (hitBoxCenter.Y + box.height * 0.5f > yAxisCheck.Y && yAxisCheck.Y + tileSize > hitBoxCenter.Y)
                        {
                            transform.position.Y = yAxisCheck.Y - box.height * 0.5f + offsetHack.Y;
                            velocity.value.Y = 0;
                        } 
                        if (hitBoxCenter.Y - box.height * 0.5f < yAxisCheck.Y + tileSize && yAxisCheck.Y < hitBoxCenter.Y)
                        {
                            transform.position.Y = yAxisCheck.Y + tileSize+ box.height * 0.5f + offsetHack.Y;
                            velocity.value.Y = 0;
                        }
                    }
                }

                if (diagonalTile.X >= 0 && diagonalTile.X < tileMap.width
                    && diagonalTile.Y >= 0 && diagonalTile.Y < tileMap.height)
                {
                    if (tileMap.GetTileElevation((int)diagonalTile.X, (int)diagonalTile.Y) != transform.layer)
                    {

                    }
                }

                Renderer.DrawRectToWorld(xAxisCheck, new Vector2(32), 1, Color.Red * 0.5f, 10000f);
                Renderer.DrawRectToWorld(yAxisCheck, new Vector2(32), 1, Color.Green * 0.5f, 10000f);
            }
        }
    }
}
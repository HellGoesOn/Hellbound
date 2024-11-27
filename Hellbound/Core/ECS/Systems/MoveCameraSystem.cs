using HellTrail.Core.ECS.Components;
using HellTrail.Core.Overworld;
using HellTrail.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class MoveCameraSystem : IExecute
    {
        readonly Group<Entity> _group;

        public MoveCameraSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(CameraMarker)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];

                var transform = entity.GetComponent<Transform>();
                var cameraMarker = entity.GetComponent<CameraMarker>();
                World world = Main.instance.ActiveWorld;
                Camera cam = world.GetCamera();
                cam.centre += (transform.position - cam.centre / cam.zoom) * cam.speed;
                if (world.tileMap.width * DisplayTileLayer.TILE_SIZE - cam.view.Width * 0.5f >= cam.view.Width * 0.5f &&
                    world.tileMap.height * DisplayTileLayer.TILE_SIZE - cam.view.Height * 0.5f >= cam.view.Height * 0.5f)
                {
                    var minX = Math.Clamp(cam.centre.X, cam.view.Width * 0.5f, world.tileMap.width * DisplayTileLayer.TILE_SIZE - cam.view.Width * 0.5f);
                    var minY = Math.Clamp(cam.centre.Y, cam.view.Height * 0.5f, world.tileMap.height * DisplayTileLayer.TILE_SIZE - cam.view.Height * 0.5f);

                    cam.centre = new Vector2(minX, minY);
                }
            }
        }
    }
}

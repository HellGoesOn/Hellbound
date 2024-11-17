using HellTrail.Core.ECS.Components;
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

            for(int i = 0; i <  entities.Count; i++)
            {
                var entity = entities[i];

                var transform = entity.GetComponent<Transform>();
                var cameraMarker = entity.GetComponent<CameraMarker>();
                Camera cam = cameraMarker.attachedCamera;

                cam.centre += (transform.position - cameraMarker.attachedCamera.centre) * cam.speed;
            }
        }
    }
}

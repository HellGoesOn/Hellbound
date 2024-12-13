using Casull.Core.ECS.Components;
using Microsoft.Xna.Framework;

namespace Casull.Core.ECS
{
    public class MoveCameraSystem : IExecute
    {
        readonly Group<Entity> _group;

        public MoveCameraSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(CameraMarker), typeof(Velocity)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];

                var transform = entity.GetComponent<Transform>();
                var velocity = entity.GetComponent<Velocity>();
                Camera cam = Main.instance.GetGameState().GetCamera();

                var value = (transform.position + velocity.value - cam.centre) * cam.speed;

                cam.centre += value;
                cam.Clamp(Vector2.Zero, new Vector2(30) * 32);
                //cam.centre = cam.centre.ToInt();


            }
        }
    }
}

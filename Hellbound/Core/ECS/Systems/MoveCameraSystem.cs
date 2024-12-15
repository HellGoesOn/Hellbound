using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Microsoft.Xna.Framework;

namespace Casull.Core.ECS
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

            if (World.cutscenes.Count > 0)
                return;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];

                var transform = entity.GetComponent<Transform>();
                Camera cam = Main.instance.GetGameState().GetCamera();

                var value = (transform.position - cam.centre) * cam.speed;

                cam.centre += new Vector2(value.X, value.Y);
                cam.Clamp(Vector2.Zero, new Vector2(30) * 32);
                //cam.centre = cam.centre.ToInt();


            }
        }
    }
}

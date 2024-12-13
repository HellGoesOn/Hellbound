using Casull.Core.ECS.Components;

namespace Casull.Core.ECS
{
    public class MoveSystem : IExecute
    {
        readonly Group<Entity> _group;

        public MoveSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(Velocity)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];

                var velocity = entity.GetComponent<Velocity>();

                var transform = entity.GetComponent<Transform>();
                transform.position += velocity.value;
            }
        }
    }
}

using Casull.Core.ECS.Components;

namespace Casull.Core.ECS
{
    public class FollowerSystem : IExecute
    {
        readonly Group<Entity> _group;

        public FollowerSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(FollowingOther), typeof(Transform)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];

                var fc = entity.GetComponent<FollowingOther>();

                if (fc.otherId != -1 && context.GetById(fc.otherId) != null && context.GetById(fc.otherId).enabled && context.GetById(fc.otherId).HasComponent<Transform>())
                    entity.GetComponent<Transform>().position = context.GetById(fc.otherId).GetComponent<Transform>().position;
                else
                    fc.otherId = -1;
            }
        }
    }
}

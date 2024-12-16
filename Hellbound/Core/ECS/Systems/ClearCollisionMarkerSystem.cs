using Casull.Core.ECS.Components;

namespace Casull.Core.ECS
{
    public class ClearCollisionMarkerSystem : IExecute
    {
        readonly Group<Entity> _group;

        public ClearCollisionMarkerSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(CollisionBox)));
        }

        public void Execute(Context context)
        {
            for (int i = 0; i < _group.Count; i++) {
                Entity e = _group[i];
                var box = e.GetComponent<CollisionBox>();

                box.CollidedWith.Clear();
            }
        }
    }
}

using Casull.Core.ECS.Components;
using Casull.Core.Overworld;

namespace Casull.Core.ECS
{
    public class KillBasedOnFlagSystem : IExecute
    {
        readonly Group<Entity> _group;

        public KillBasedOnFlagSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(DiesIfFlagRaised)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var getFlag = entities[i].GetComponent<DiesIfFlagRaised>();

                if (World.flags.Contains(getFlag.flag))
                    context.Destroy(entities[i]);
            }
        }
    }
}

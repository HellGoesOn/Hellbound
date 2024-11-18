using HellTrail.Core.ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class ClearCollisionMarkerSystem : IExecute
    {
        readonly Group<Entity> _group;

        public ClearCollisionMarkerSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(HasCollidedMarker)));
        }

        public void Execute(Context context)
        {
            for (int i = 0; i < _group.Count; i++)
            {
                Entity e = _group[i];

                e.RemoveComponent<HasCollidedMarker>();
            }
        }
    }
}

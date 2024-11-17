using HellTrail.Core.ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class ReadPlayerInputSystem : IExecute
    {
        readonly Group<Entity> _group;

        public ReadPlayerInputSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(PlayerMarker)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                entity.GetComponent<PlayerMarker>().onInput?.Invoke(entity, context);
            }
        }
    }
}

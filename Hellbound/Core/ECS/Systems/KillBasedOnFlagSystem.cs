using HellTrail.Core.ECS.Components;
using HellTrail.Core.Overworld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
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

            for (int i = 0; i < entities.Count; i++)
            {
                var getFlag = entities[i].GetComponent<DiesIfFlagRaised>();

                if(World.flags.Contains(getFlag.flag))
                    context.Destroy(entities[i]);
            }
        }
    }
}

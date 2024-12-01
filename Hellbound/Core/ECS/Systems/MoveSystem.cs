using HellTrail.Core.ECS.Components;
using HellTrail.Core.Overworld;
using HellTrail.Extensions;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
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

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                
                var velocity = entity.GetComponent<Velocity>();

                var transform = entity.GetComponent<Transform>();
                transform.position += velocity.value;
            }
        }
    }
}

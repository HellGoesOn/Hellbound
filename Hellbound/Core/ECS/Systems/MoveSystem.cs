using HellTrail.Core.ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class MoveSystem : IExecute

    {
        public void Execute(Context context)
        {
            var entities = context.entities.Where(x => x != null && x.enabled && x.HasComponent<Velocity>() && x.HasComponent<Transform>()).ToArray();

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                
                var velocity = entity.GetComponent<Velocity>();
                var trans = entity.GetComponent<Transform>();

                trans.position += velocity.value;
            }
        }
    }
}

using Casull.Core.ECS.Components;
using Casull.Core.UI.Elements;
using Casull.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.ECS
{
    public class SpawnerSystem : IExecute
    {
        readonly Group<Entity> _group;

        public SpawnerSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(SpawnerComponent), typeof(Transform)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                SpawnerComponent spawner = entity.GetComponent<SpawnerComponent>();

                if (spawner.currentSpawned < spawner.max) {

                    if (++spawner.timer > spawner.untilNextSpawn) {
                        spawner.currentSpawned++;
                        var loadedCopy = EntitySaver.Load("\\Content\\Prefabs\\", spawner.prefabNames[Main.rand.Next(spawner.prefabNames)]);
                        if (loadedCopy != null) {
                            var e = context.CopyFrom(loadedCopy);
                            e.AddComponent(new Transform(entity.GetComponent<Transform>().position));
                            e.AddComponent(new AppearComponent());
                            e.AddComponent(new WanderComponent() {
                                interestTimeMax = 320,
                                interestTimeMin = 30,
                                waitTimeMin = 60,
                                waitTimeMax = 180,
                                wanderSpeed = 0.01f + Main.rand.NextSingle(),
                                leash = spawner.leashRange
                            });

                            e.OnDestroy += (ent) => { spawner.currentSpawned--; };
                        }
                        spawner.timer = 0;
                    }
                }
                else {
                    spawner.timer = 0;
                }
            }
        }
    }

}

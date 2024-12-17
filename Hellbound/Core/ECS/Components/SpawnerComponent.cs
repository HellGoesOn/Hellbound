using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.ECS.Components
{
    internal class SpawnerComponent : IComponent
    {
        public string[] prefabNames;
        public int max;
        internal int currentSpawned;
        public int untilNextSpawn;
        public float leashRange;
        internal int timer;
    }
}

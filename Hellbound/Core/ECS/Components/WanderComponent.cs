using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.ECS.Components
{
    public class WanderComponent : IComponent
    {
        internal Vector2 position;
        internal Vector2 startPosition;
        public float wanderSpeed;
        public int interestTimeMin;
        public int interestTimeMax;
        public int waitTimeMin;
        public int waitTimeMax;
        public float leash;
        public int wanderTime;
        public int waitTime;
    }
}

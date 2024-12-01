using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public struct HasCollidedMarker : IComponent
    {
        public int otherId;

        public HasCollidedMarker(int otherId)
        {
            this.otherId = otherId;
        }
    }
}

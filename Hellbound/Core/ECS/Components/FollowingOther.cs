using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class FollowingOther : IComponent
    {
        public int otherId;
        public FollowingOther(int otherId) 
        {
            this.otherId = otherId;
        }
    }
}

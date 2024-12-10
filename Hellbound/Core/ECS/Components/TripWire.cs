using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class TripWire : IComponent
    {
        public string trigger;

        public TripWire(string trigger) 
        {
            this.trigger = trigger;
        }
    }
}

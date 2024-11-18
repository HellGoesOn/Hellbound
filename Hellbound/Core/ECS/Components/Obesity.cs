using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class Obesity : IComponent
    {
        public float obesityFactor;
        public Obesity(float obesityFactor) 
        {
            this.obesityFactor = obesityFactor;
        }

        public override string ToString()
        {
            return $"Obesity:[{obesityFactor}]";
        }
    }
}

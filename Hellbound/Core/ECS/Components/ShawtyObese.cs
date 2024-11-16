using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class ShawtyObese : IComponent
    {
        public float obesityFactor;
        public ShawtyObese(float obesityFactor) 
        {
            this.obesityFactor = obesityFactor;
        }
    }
}

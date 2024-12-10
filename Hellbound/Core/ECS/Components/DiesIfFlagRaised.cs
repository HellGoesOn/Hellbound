using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class DiesIfFlagRaised : IComponent
    {
        public string flag;

        public DiesIfFlagRaised(string flag)
        {
            this.flag = flag;
        }
    }
}

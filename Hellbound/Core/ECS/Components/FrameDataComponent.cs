using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class FrameDataComponent : IComponent
    {
        public FrameData[] data;

        public FrameDataComponent(params FrameData[] data)
        {
            this.data = data;
        }
    }
}

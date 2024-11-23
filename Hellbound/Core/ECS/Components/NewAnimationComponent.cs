using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class NewAnimationComponent : IComponent
    {
        public IndexTuple[] animationIndexes;
        public FrameData[] data;

        public NewAnimationComponent(IndexTuple[] animationIndexes, FrameData[] data)
        {
            this.animationIndexes = animationIndexes;
            this.data = data;
        }
    }
}

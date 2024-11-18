using HellTrail.Core.ECS.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class PlayerMarker : IComponent
    {
        public InputHandler onInput;

        public PlayerMarker(InputHandler onInput)
        {
            this.onInput = onInput;
        }
    }

    public delegate void InputHandler(Entity entity, Context context);
}

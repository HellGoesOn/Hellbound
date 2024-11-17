using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class CameraMarker : IComponent
    {
        public Camera attachedCamera;

        public CameraMarker(Camera attachedCamera)
        {
            this.attachedCamera = attachedCamera;
        }
    }
}

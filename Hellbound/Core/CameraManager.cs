using HellTrail.Render;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core
{
    public static class CameraManager
    {
        public static int currentCamera;

        public static List<Camera> cameras = new List<Camera>();

        public static void Initialize()
        {
            var momGetTheCamera = new Camera(new Viewport(0, 0, Renderer.PreferedWidth, Renderer.PreferedHeight));
            momGetTheCamera.zoom = 1f;
            momGetTheCamera.speed = 1f;
            cameras.Add(momGetTheCamera);
        }

        public static void Update()
        {
            foreach (var cam in cameras)
            {
                cam.Update();
            }
        }

        public static Camera GetCamera => cameras[currentCamera];
    }
}

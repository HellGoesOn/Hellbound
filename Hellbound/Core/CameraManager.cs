﻿using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public static Camera combatCamera;
        public static Camera neoCombatCamera;
        public static Camera overworldCamera;


        public static List<Camera> cameras = new();

        public static void Initialize()
        {
            combatCamera = new Camera(new Viewport(0, 0, Renderer.PreferedWidth, Renderer.PreferedHeight));
            combatCamera.zoom = 1f;
            combatCamera.speed = 1f;
            combatCamera.centre = new Vector2(160, 90);
            cameras.Add(combatCamera);
            
            overworldCamera = new Camera(new Viewport(0, 0, Renderer.PreferedWidth, Renderer.PreferedHeight));
            overworldCamera.zoom = 1f;
            overworldCamera.speed = 1f;
            overworldCamera.centre = new Vector2(160, 90);
            cameras.Add(overworldCamera);

            neoCombatCamera = new Camera(new Viewport(0, 0, Renderer.PreferedWidth, Renderer.PreferedHeight));
            neoCombatCamera.zoom = 1f;
            neoCombatCamera.speed = 1f;
            neoCombatCamera.centre = new Vector2(160, 90);
            cameras.Add(neoCombatCamera);
        }

        public static void Update()
        {
            foreach (var cam in cameras)
            {
                cam.Update();
            }
        }

        public static Camera GetCamera
        {
            get
            {
                if(cameras.Count > 0)
                    return cameras[currentCamera];

                return null;
            }
        }
    }
}

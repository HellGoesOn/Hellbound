using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Render
{
    public static class Renderer
    {
        public const int PreferedWidth = 320;
        public const int PreferedHeight = 180;
        public const int UIPreferedWidth = 1280;
        public const int UIPreferedHeight = 720;

        public static RenderTarget2D MainTarget { get; set; }
        public static RenderTarget2D UITarget { get; set; }

        public static void Load(GraphicsDevice device)
        {
            MainTarget = new RenderTarget2D(device, PreferedWidth, PreferedHeight);
            UITarget = new RenderTarget2D(device, UIPreferedWidth, UIPreferedHeight);
        }

        public static void Unload()
        {
            MainTarget.Dispose();
            UITarget.Dispose();
        }
    }
}

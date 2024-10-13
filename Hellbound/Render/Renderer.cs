using Microsoft.Xna.Framework;
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

        public static void DrawRect(SpriteBatch sb, Vector2 position, Vector2 size, int thickness, Color color, float depth = 0f)
        {
            var tex = AssetManager.Textures["Pixel"];
            sb.Draw(tex, position, null, color, 0f, Vector2.Zero, size, SpriteEffects.None, depth);
        }

        public static (int, int) Multiplier
        {
            get
            {
                var gdm = Main.instance.gdm;
                int multX = gdm.PreferredBackBufferWidth / PreferedWidth;
                int multY = gdm.PreferredBackBufferHeight / PreferedHeight;

                return (multX, multY);
            }
        }
    }
}

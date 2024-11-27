using HellTrail.Core;
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
        public const int UIPreferedWidth = 1280;//1920;
        public const int UIPreferedHeight = 720;//1080;
        //public const int UIPreferedHeight = 1080;
        //public const int UIPreferedWidth = 1920;

        public const float UIMultiplierX = UIPreferedWidth / (float)PreferedWidth;
        public const float UIMultiplieY = UIPreferedHeight / (float)PreferedHeight;

        public static RenderTarget2D WorldTarget { get; set; }
        public static RenderTarget2D MainTarget { get; set; }
        public static RenderTarget2D UITarget { get; set; }

        public static void Load(GraphicsDevice device)
        {
            MainTarget = new RenderTarget2D(device, UIPreferedWidth, UIPreferedHeight);
            WorldTarget = new RenderTarget2D(device, PreferedWidth, PreferedHeight);
            UITarget = new RenderTarget2D(device, UIPreferedWidth, UIPreferedHeight);
        }

        public static RenderTarget2D SaveFrame(bool withInterface = false)
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            GraphicsDeviceManager gdm = Main.instance.gdm;
            RenderTarget2D renderTarget = new(gd, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight);
            SpriteBatch spriteBatch = Main.instance.spriteBatch;
            gd.SetRenderTarget(renderTarget);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(WorldTarget, new Rectangle(0, 0, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight), Color.White);
            if(withInterface)
                spriteBatch.Draw(UITarget, new Rectangle(0, 0, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();

            return renderTarget;
        }

        public static void Unload()
        {
            MainTarget.Dispose();
            WorldTarget.Dispose();
            UITarget.Dispose();
        }

        public static void DrawRect(SpriteBatch sb, Vector2 position, Vector2 size, int thickness, Color color, float depth = 0f, float rotation = 0f, Vector2 origin = default)
        {
            var tex = Assets.GetTexture("Pixel");
            sb.Draw(tex, position, null, color, rotation, origin, size, SpriteEffects.None, depth);
        }

        public static void StartSpriteBatch(SpriteBatch spriteBatch, bool ignoreCam = false, BlendState blend = null, DepthStencilState stencil = null, SamplerState state = null, Camera overrideCamera = null, SpriteSortMode sortMode = SpriteSortMode.FrontToBack)
        {
            state ??= SamplerState.PointClamp;

            stencil ??= DepthStencilState.Default;

            blend ??= BlendState.AlphaBlend;

            Camera cam = Main.instance.GetGameState().GetCamera();

            if (cam != null && !ignoreCam)
                spriteBatch.Begin(sortMode, blend, state, stencil, RasterizerState.CullNone, null, cam.transform);
            else
                spriteBatch.Begin(sortMode, blend, state, stencil, RasterizerState.CullNone, null);
        }

        public static void DrawBorderedString(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color color, Color borderColor, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float depth)
        {
            sb.DrawString(font, text, position + Vector2.UnitX * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
            sb.DrawString(font, text, position - Vector2.UnitX * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
            sb.DrawString(font, text, position - Vector2.UnitY * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
            sb.DrawString(font, text, position + Vector2.UnitY * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
            sb.DrawString(font, text, position, color, rotation, origin, scale, spriteEffects, depth);
        }
    }
}

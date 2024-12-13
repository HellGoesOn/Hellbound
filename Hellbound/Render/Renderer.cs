using Casull.Core;
using Casull.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Render
{
    public static class Renderer
    {
        public const int PreferedWidth = 1280;
        public const int PreferedHeight = 720;
        public const int UIPreferedWidth = 1280;//1920;
        public const int UIPreferedHeight = 720;//1080;
        //public const int UIPreferedHeight = 1080;
        //public const int UIPreferedWidth = 1920;

        public static RenderTarget2D WorldTarget { get; set; }
        public static RenderTarget2D MainTarget { get; set; }
        public static RenderTarget2D UITarget { get; set; }

        public static int Layer { get; private set; }

        //private static DrawLayerEvent onDrawStrings;

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
            if (withInterface)
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


        //public static void DoDrawStrings(SpriteBatch sb)
        //{
        //    onDrawStrings?.Invoke(sb);
        //    onDrawStrings = null;
        //}

        public static void DrawRect(SpriteBatch sb, Vector2 position, Vector2 size, Color color, float depth = 0f, float rotation = 0f, Vector2 origin = default)
        {
            var tex = Assets.GetTexture("Pixel");
            sb.Draw(tex, position, null, color, rotation, origin, size, SpriteEffects.None, depth);
        }

        public static void DrawRectToWorld(Vector2 position, Vector2 size, Color color, float depth = 0f, float rotation = 0f, Vector2 origin = default)
        {
            var tex = Assets.GetTexture("Pixel");
            Draw(tex, position, null, color, rotation, origin, size, SpriteEffects.None, depth);
        }

        public static void StartSpriteBatch(SpriteBatch spriteBatch, bool ignoreCam = false, BlendState blend = null, DepthStencilState stencil = null, SamplerState state = null, SpriteSortMode sortMode = SpriteSortMode.FrontToBack)
        {
            state ??= SamplerState.PointClamp;

            stencil ??= DepthStencilState.Default;

            blend ??= BlendState.AlphaBlend;

            var gameState = Main.instance.GetGameState();

            Camera cam = gameState.GetCamera();

            if (cam != null && !ignoreCam)
                spriteBatch.Begin(sortMode, blend, state, stencil, RasterizerState.CullNone, Assets.MainEffect, cam.transform);
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

        //public static void DrawBorderedStringX(this SpriteBatch sb, SpriteFont font, string text, Vector2 position, Color color, Color borderColor, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float depth)
        //{
        //    onDrawStrings += (sb) =>
        //    {
        //        sb.DrawString(font, text, position + Vector2.UnitX * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
        //        sb.DrawString(font, text, position - Vector2.UnitX * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
        //        sb.DrawString(font, text, position - Vector2.UnitY * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
        //        sb.DrawString(font, text, position + Vector2.UnitY * 2, borderColor, rotation, origin, scale, spriteEffects, depth);
        //        sb.DrawString(font, text, position, color, rotation, origin, scale, spriteEffects, depth);
        //    };
        //}

        private static List<DrawData> _drawData = [];

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float depth, bool solid = false)
        {
            DrawData dd = new(texture, position, source, color, rotation, origin, scale, spriteEffects, depth, solid);

            _drawData.Add(dd);
        }

        public static void DoRender(SpriteBatch sb)
        {
            _drawData.Sort(SortDrawData);
            StartSpriteBatch(sb, state: SamplerState.PointWrap, sortMode: SpriteSortMode.Deferred);
            for (int i = 0; i < _drawData.Count; i++) {
                DrawData dd = _drawData[i];

                //UIManager.Debug($"{dd.texture.Name} :{dd.color.A} : {dd.color.ShaderFix(dd.solid).A}");
                sb.Draw(dd.texture, dd.position, dd.source, dd.color.ShaderFix(dd.solid), dd.rotation, dd.origin, dd.scale, dd.spriteEffects, 0);
            }
            sb.End();

            _drawData.Clear();
        }

        public static int SortDrawData(DrawData x, DrawData y)
        {
            return x.CompareTo(y);
        }

        public static Vector2 ScreenMiddle => new Vector2(UIPreferedWidth, UIPreferedHeight) * 0.5f;
    }

    public delegate void DrawLayerEvent(SpriteBatch spriteBatch);
}

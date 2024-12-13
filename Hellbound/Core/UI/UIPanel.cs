using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.UI
{
    public class UIPanel : UIElement
    {
        public Color fillColor;
        public Color outlineColor;

        public UIPanel()
        {
            fillColor = Color.DarkBlue;
            outlineColor = Color.White;
        }
        public override void OnDraw(SpriteBatch spriteBatch)
        {
            Renderer.DrawRect(spriteBatch, GetPosition() - Vector2.One * 2, size + Vector2.One * 4, outlineColor, rotation: Rotation);
            Renderer.DrawRect(spriteBatch, GetPosition(), size, fillColor, rotation: Rotation);
        }
    }
}

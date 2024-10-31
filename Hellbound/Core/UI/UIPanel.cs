using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI
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
            Renderer.DrawRect(spriteBatch, Position - Vector2.One * 2, size + Vector2.One * 4, 1, outlineColor, rotation: Rotation);
            Renderer.DrawRect(spriteBatch, Position, size, 1, fillColor, rotation: Rotation);
        }
    }
}

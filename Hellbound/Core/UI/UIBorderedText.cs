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
    public class UIBorderedText(string text) : UIElement
    {
        public string text = text;

        public Vector2 origin = Vector2.Zero;
        public Vector2 scale = Vector2.One;

        public Color color = Color.White;
        public Color borderColor = Color.Black;

        public SpriteFont font = Assets.DefaultFont;

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            Renderer.DrawBorderedString(spriteBatch, font, text, Position, color, borderColor, Rotation, origin, scale, SpriteEffects.None, 0);
        }
    }
}

using HellTrail.Extensions;
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
    public class UIBorderedText : UIElement
    {
        public UIBorderedText(string text, int lineBreak = -1)
        {
            this.lineBreak = lineBreak;
            oldText = brokenText = this.text = text.Splice(lineBreak);
            size = font.MeasureString(brokenText);
        }

        protected bool firstSplit;
        protected string oldText;
        protected string brokenText;
        public string text;

        public string LineBrokenText => brokenText;

        public Vector2 origin = Vector2.Zero;

        public Color color = Color.White;
        public Color borderColor = Color.Black;

        public int lineBreak = -1;

        public override void OnUpdate()
        {
            base.OnUpdate();

            if(text != oldText || !firstSplit)
            {
                size = font.MeasureString(brokenText);
                firstSplit = true;
                oldText = text;
                brokenText = text.Splice(lineBreak);
            }
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            if(firstSplit)
                Renderer.DrawBorderedString(spriteBatch, font, brokenText, GetPosition(), color, borderColor, Rotation, origin, scale, SpriteEffects.None, 0);
        }
    }

    public class UIText : UIBorderedText
    {
        public UIText(string text, int lineBreak = -1) : base(text, lineBreak)
        {
            this.lineBreak = lineBreak;
            oldText = brokenText = this.text = text.Splice(lineBreak);
            size = font.MeasureString(brokenText);
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            if (firstSplit)
                spriteBatch.DrawString(font, brokenText, GetPosition(), color, Rotation, origin, scale, SpriteEffects.None, 0);
        }
    }
}

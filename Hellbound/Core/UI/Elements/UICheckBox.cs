using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.Elements
{
    public class UICheckBox : UIElement
    {
        public bool isChecked;

        public Color color;

        public string hoverText;

        public UICheckBox(string text = "", Color? color = null)
        {
            size = new Vector2(16);
            capturesMouse = true;
            this.color = color ?? Color.White;
            hoverText = text;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void Click()
        {
            base.Click();

            isChecked = !isChecked;
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = Assets.Textures["UICheckBox"];
            spriteBatch.Draw(tex, this.Position, new Rectangle(0, 16 * (isChecked ? 1 : 0), 16, 16), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            if(isMouseHovering)
            {
                Renderer.DrawBorderedString(spriteBatch, Assets.DefaultFont, hoverText, Input.UIMousePosition + new Vector2(16), color, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
            }
        }
    }
}

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
        public bool drawsPanel;

        public Color color;
        public Color panelColor;
        public Color panelBorderColor;

        public string hoverText;

        public UICheckBox(string text = "", Color? color = null)
        {
            size = new Vector2(16);
            capturesMouse = true;
            this.color = color ?? Color.White;
            hoverText = text;
            this.panelColor = Color.DarkBlue;
            this.panelBorderColor = Color.White;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void Click()
        {
            isChecked = !isChecked;

            base.Click();
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = Assets.Textures["UICheckBox"];
            spriteBatch.Draw(tex, this.Position, new Rectangle(0, 16 * (isChecked ? 1 : 0), 16, 16), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            Vector2 size = Assets.DefaultFont.MeasureString(hoverText) * new Vector2(1.1f, 1.5f);
            if (isMouseHovering)
            {
                Vector2 offset = Input.UIMousePosition.Y > Renderer.UIPreferedHeight - size.Y ? new Vector2(16, -16) : new Vector2(16);
                if (drawsPanel)
                {
                    Renderer.DrawRect(spriteBatch, Input.UIMousePosition + offset - new Vector2(16, 8)-new Vector2(2), size+new Vector2(4), 1, panelBorderColor);
                    Renderer.DrawRect(spriteBatch, Input.UIMousePosition + offset - new Vector2(16, 8), size, 1, panelColor);
                }
                Renderer.DrawBorderedString(spriteBatch, Assets.DefaultFont, hoverText, Input.UIMousePosition + offset - new Vector2(8, 0), color, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
            }
        }
    }
}

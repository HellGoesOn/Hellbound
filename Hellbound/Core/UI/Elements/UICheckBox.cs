using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.UI.Elements
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
            Texture2D tex = Assets.GetTexture("UICheckBox");
            spriteBatch.Draw(tex, this.GetPosition(), new Rectangle(0, 16 * (isChecked ? 1 : 0), 16, 16), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            Vector2 size = font.MeasureString(hoverText) + new Vector2(16);
            if (isMouseHovering) {
                UIManager.tooltipText = hoverText;
            }
        }
    }
}

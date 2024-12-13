using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.UI.Elements
{
    public enum WindowButtonType
    {
        Empty,
        CheckMark,
        XMark,
        Wrench
    }

    public class UIWindowButton : UIElement
    {
        public bool drawsPanel;

        public Color color;
        public Color panelColor;
        public Color panelBorderColor;

        public string hoverText;

        public WindowButtonType buttonType;

        public UIWindowButton(WindowButtonType type, string hoverText = "", Color? color = null)
        {
            this.buttonType = type;
            size = new Vector2(16);
            capturesMouse = true;
            this.color = color ?? Color.White;
            this.hoverText = hoverText;
            this.panelColor = Color.DarkBlue;
            this.panelBorderColor = Color.White;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = Assets.GetTexture("UICheckBox");
            spriteBatch.Draw(tex, this.GetPosition(), new Rectangle(0, 16 * (int)buttonType, 16, 16), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);

            Vector2 size = font.MeasureString(hoverText) + new Vector2(16);
            if (isMouseHovering) {
                UIManager.tooltipText = hoverText;
            }
        }
    }
}

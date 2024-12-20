using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.UI.Elements
{
    public class UIProgressBar : UIElement
    {
        private float currentValue;
        public float CurrentValueVisual => currentValue;
        public float CurrentValue => currentValue;
        public float value;
        public float maxValue;
        public float bgSize = 6;
        public float moveSpeed = 0.12f;
        public Color fillColor;
        public Color bgColor;
        public UIProgressBar(Vector2 size, float maxValue)
        {
            this.maxValue = maxValue;
            this.size = size;
            fillColor = Color.White;
            bgColor = Color.Black;
        }

        public override void OnUpdate()
        {
            if (currentValue != value) {
                currentValue = MathHelper.Lerp(currentValue, value, moveSpeed);

                if (Math.Abs(value - currentValue) <= 0.1f)
                    currentValue = value;
            }
        }

        public void HardSetValue(float value)
        {
            this.value = currentValue = value;
        }


        public override void OnDraw(SpriteBatch spriteBatch)
        {
            Renderer.DrawRect(spriteBatch, GetPosition() - new Vector2(bgSize * 0.5f), (new Vector2(size.X, size.Y) + new Vector2(bgSize)) * scale, bgColor, 0, Rotation);
            Renderer.DrawRect(spriteBatch, GetPosition(), (new Vector2(CalcValue(), size.Y)) * scale, fillColor, 0, Rotation);
        }

        public float CalcValue()
        {
            return Math.Clamp((currentValue * size.X) / maxValue, 0, size.X);
        }
    }
}

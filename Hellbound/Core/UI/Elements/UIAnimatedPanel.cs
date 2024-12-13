using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.UI.Elements
{
    public class UIAnimatedPanel : UIElement
    {
        public bool isClosed;
        public bool suspended;
        public float openSpeed;
        public readonly Vector2 targetSize;
        public AnimationStyle style;

        public Color panelColor;
        public Color borderColor;

        public UIAnimatedPanel(Vector2 size, AnimationStyle style = AnimationStyle.Vertical)
        {
            openSpeed = 0.12f;
            targetSize = size;
            this.style = style;
            panelColor = Color.DarkBlue;
            borderColor = Color.White;

            switch (style) {
                case AnimationStyle.Vertical:
                    this.size.X = targetSize.X;
                    break;
                case AnimationStyle.Horizontal:
                    this.size.Y = targetSize.Y; break;
                default:
                    break;
            }
        }

        public override void OnUpdate()
        {
            if (size != targetSize && !isClosed && !suspended) {
                size = Vector2.Lerp(size, targetSize, openSpeed);
                switch (style) {
                    case AnimationStyle.Vertical:
                        if (size.Y >= targetSize.Y - 8)
                            size = targetSize;
                        break;
                    case AnimationStyle.Horizontal:
                        if (size.X >= targetSize.X - 8)
                            size = targetSize;
                        break;
                    case AnimationStyle.FourWay:
                        if (size.Y >= targetSize.Y - 8 && size.X >= targetSize.X - 8)
                            size = targetSize; ;
                        break;
                }
            }



            if (isClosed || suspended) {
                switch (style) {
                    case AnimationStyle.Vertical:
                        if (size.Y >= 0.001f)
                            size.Y = MathHelper.Lerp(size.Y, 0, openSpeed);

                        if (size.Y < targetSize.Y * openSpeed * 0.05f && isClosed) {
                            size.Y = 0;
                            this.parent.Disown(this);
                        }
                        break;
                    case AnimationStyle.Horizontal:
                        if (size.X >= 0.001f)
                            size.X = MathHelper.Lerp(size.X, 0, openSpeed);

                        if (size.X < targetSize.X * openSpeed * 0.05f && isClosed) {
                            size.X = 0;
                            this.parent.Disown(this);
                        }
                        break;
                    case AnimationStyle.FourWay:
                        if (size.Y >= 0.001f && size.X >= 0.001f)
                            size = Vector2.Lerp(size, Vector2.Zero, openSpeed);

                        if (size.X < targetSize.X * openSpeed * 0.05f && size.Y < targetSize.Y * openSpeed * 0.05f && isClosed) {

                            this.parent.Disown(this);
                        }
                        break;
                }


                return;
            }
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            base.OnDraw(spriteBatch);

            if (suspended && (size.X <= 0.5f || size.Y <= 0.5f))
                return;

            var decorativeOffset = new Vector2(-size.X * 0.5f + targetSize.X * 0.5f, -size.Y * 0.5f + targetSize.Y * 0.5f);

            Renderer.DrawRect(spriteBatch, GetPosition() - new Vector2(2) + decorativeOffset, size + new Vector2(4), borderColor);
            Renderer.DrawRect(spriteBatch, GetPosition() + decorativeOffset, size, panelColor);
        }

        public override bool PreUpdateChildren()
        {
            return this.size == targetSize;
        }

        public override bool PreDrawChildren(SpriteBatch spriteBatch)
        {
            return this.size == targetSize;
        }


        public enum AnimationStyle
        {
            Vertical,
            Horizontal,
            FourWay,
        }
    }

}

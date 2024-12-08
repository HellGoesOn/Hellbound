using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HellTrail.Core.UI.Elements
{
    public class UIScrollableMenu : UIElement
    {
        public bool focused;
        public bool drawArrows;
        public bool closed;
        public bool init;
        public int currentSelectedOption;
        public int repeatRate;

        public float sizeY;
        public float openSpeed;

        private int _optionWindowMin;
        private int _optionWindowMax;
        private int _heldTimer;

        public Color panelColor;
        public Color borderColor;

        public Color selectedColor;
        public Color notSelectedColor;

        public List<string> options;

        public readonly Vector2 targetSize;

        public UIEventHandler onSelectOption;
        public UIEventHandler onChangeOption;

        public UIScrollableMenu(int maxDisplayeedOptions, params string[] options )
        {
            openSpeed = 0.12f;
            drawArrows = true;
            repeatRate = 20;
            this.options = [];
            sizeY = font.MeasureString("Y").Y;
            float sizeX = 0;
            _optionWindowMax = maxDisplayeedOptions;

            for (int i = 0; i < options.Length; i++)
            {
                this.options.Add(options[i]);

                if(font.MeasureString(options[i]).X > sizeX)
                {
                    sizeX = font.MeasureString(options[i]).X;
                }
            }
            focused = true;
            panelColor = Color.DarkBlue;
            borderColor = Color.White;
            targetSize = new Vector2(16 + sizeX + 16, (sizeY + 4) * maxDisplayeedOptions + 32);
            size = new Vector2(targetSize.X, 0);

            selectedColor = Color.Yellow;
            notSelectedColor = Color.Gray;
        }

        public override bool PreDrawChildren(SpriteBatch spriteBatch)
        {
            return size == targetSize;
        }

        public override void OnUpdate()
        {
            if (size != targetSize && !closed)
            {
                size = Vector2.Lerp(size, targetSize, openSpeed);

                if (size.Y >= targetSize.Y - 8)
                {
                    init = true;

                    onChangeOption?.Invoke(this);

                    size = targetSize;
                }
            }

            if (closed)
            {
                size.Y = MathHelper.Lerp(size.Y, 0, openSpeed);

                if (size.Y < targetSize.Y * openSpeed * 0.5f)
                {
                    size.Y = 0;
                    this.parent.Disown(this);
                }


                return;
            }

            if (!focused || options.Count <= 0 || size != targetSize)
                return;

            if ((Input.HeldKey(Keys.S) || Input.HeldKey(Keys.W)) && _heldTimer < repeatRate)
                _heldTimer++;
            else if(!Input.HeldKey(Keys.S) && !Input.HeldKey(Keys.W))
                _heldTimer = 0;

            if(Input.PressedKey(Keys.S) || (Input.HeldKey(Keys.S) && _heldTimer >= repeatRate))
            {
                currentSelectedOption++;
                if (_heldTimer >= repeatRate)
                    _heldTimer -= (int)(repeatRate * 0.33f);

                if (currentSelectedOption >= options.Count)
                    currentSelectedOption = 0;
                onChangeOption?.Invoke(this);
            }

            if(Input.PressedKey(Keys.W) || (Input.HeldKey(Keys.W) && _heldTimer >= repeatRate))
            {
                currentSelectedOption--;
                if(_heldTimer >= repeatRate)
                    _heldTimer -= (int)(repeatRate * 0.33f);

                if (currentSelectedOption < 0)
                    currentSelectedOption = options.Count - 1;
                onChangeOption?.Invoke(this);
            }

            if(Input.PressedKey([Keys.E, Keys.Enter]))
            {
                onSelectOption?.Invoke(this);
            }

            if(currentSelectedOption >= _optionWindowMin + _optionWindowMax)
            {
                _optionWindowMin++;
            }
            else if(currentSelectedOption < _optionWindowMin)
            {
                _optionWindowMin--;
            }

            _optionWindowMin = Math.Clamp(_optionWindowMin, 0, options.Count - 1);
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            base.OnDraw(spriteBatch);

            Texture2D arrow = Assets.GetTexture("Arrow");
            Vector2 osciliation = new Vector2(0, (float)Math.Sin(Main.totalTime));

            var decorativeOffset = new Vector2(0, -size.Y * 0.5f + targetSize.Y * 0.5f);

            Renderer.DrawRect(spriteBatch, GetPosition() - new Vector2(2) + decorativeOffset, size + new Vector2(4), 1, borderColor);
            Renderer.DrawRect(spriteBatch, GetPosition() + decorativeOffset, size, 1, panelColor);

            if (options.Count > 0 && _optionWindowMin <= options.Count && size == targetSize)
            {
                int offset = 0;
                for (int i = _optionWindowMin; i < _optionWindowMin + _optionWindowMax; i++)
                {
                    if (i > _optionWindowMin + _optionWindowMax || i >= options.Count)
                        break;

                    bool selected = i == currentSelectedOption;
                    string option = options[i];
                    Color clr = selected ? selectedColor : notSelectedColor;
                    var textPosition = GetPosition() + new Vector2(16, 16 + (sizeY + 4) * offset);
                    spriteBatch.DrawBorderedString(font, option, textPosition, clr, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);

                    if (selected && focused)
                    {
                        bool evalPosition = GetPosition().X > 50;
                        Vector2 cursorScale = evalPosition ? new Vector2(3, 3) : new Vector2(-3, 3);
                        Vector2 cursorPosition = evalPosition ? textPosition + new Vector2(osciliation.Y, 0) - new Vector2(40, 0) : textPosition - new Vector2(osciliation.Y, 0) + new Vector2(size.X + 10, 0);
                        spriteBatch.Draw(Assets.GetTexture("Cursor3"), cursorPosition, null, Color.White, 0f, new Vector2(10, 0), cursorScale, SpriteEffects.None, 1f);

                    }

                    offset++;
                }
            }

            if (drawArrows)
            {
                spriteBatch.Draw(arrow, GetPosition() + new Vector2(size.X * 0.5f, -16) - osciliation +decorativeOffset, null, Color.White, 0f, new Vector2(5, 3.5f), new Vector2(3, -3), SpriteEffects.None, 0f);
                spriteBatch.Draw(arrow, GetPosition() + new Vector2(size.X * 0.5f, 16 + size.Y) + osciliation + decorativeOffset, null, Color.White, 0f, new Vector2(5, 3.5f), Vector2.One * 3, SpriteEffects.None, 0f);
            }
        }

        public string CurrentOption => options[currentSelectedOption];
    }
}

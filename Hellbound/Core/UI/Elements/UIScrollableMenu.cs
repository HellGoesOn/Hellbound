using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection.Metadata;

namespace HellTrail.Core.UI.Elements
{
    public class UIScrollableMenu : UIElement
    {
        public bool focused;
        public bool drawArrows;
        public bool closed;
        public bool suspended;
        public bool init;
        public int currentSelectedOption;
        public int repeatRate;

        public float opacity = 1f;

        private float extraSpacing;
        public float ExtraSpacing
        {
            get => extraSpacing;
            set 
            {
                extraSpacing = value;
                targetSize = baseTargetSize + new Vector2(padding, extraSpacing*OptionWindowMax);
            }
        }
        public float sizeY;
        private float padding;
        public float Padding
        { 
            get => padding; 
            set
            {
                padding = value;
                targetSize = baseTargetSize + new Vector2(padding, extraSpacing * OptionWindowMax);
            }
        }
        public float openSpeed;

        private int _optionWindowMin;
        public int OptionWindowMin => _optionWindowMin;
        private int _optionWindowMax;
        public int OptionWindowMax => _optionWindowMax;
        private int _heldTimer;

        public int HeldTimer => _heldTimer;

        public Color panelColor;
        public Color borderColor;

        public Color selectedColor;
        public Color notSelectedColor;
        public Color notAvailableColor;

        public List<string> options;
        public List<int> unavailableOptions = [];

        public Vector2 targetSize;
        private readonly Vector2 baseTargetSize;

        public UIEventHandler onSelectOption;
        public UIEventHandler onUnavailableSelectOption;
        public UIEventHandler onChangeOption;
        public UIEventHandler onScrollOption;

        public float bob = 0.0f;

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
            notAvailableColor = Color.DarkRed;
            baseTargetSize = targetSize = new Vector2(16 + padding + sizeX + 16, (sizeY + 4) * maxDisplayeedOptions + 32);
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
            if (size != targetSize && !closed && !suspended)
            {
                size = Vector2.Lerp(size, targetSize, openSpeed);

                if (size.Y >= targetSize.Y - 8)
                {
                    init = true;

                    onChangeOption?.Invoke(this);

                    size = targetSize;
                }
            }

            if (closed || suspended)
            {
                if(size.Y > 0.001f)
                    size.Y = MathHelper.Lerp(size.Y, 0, openSpeed);
                //UIManager.Debug(size.Y.ToString());

                if (size.Y < targetSize.Y * openSpeed * 0.5f && closed)
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
                onScrollOption?.Invoke(this);
                if (_heldTimer >= repeatRate)
                    _heldTimer -= (int)(repeatRate * 0.33f);

                if (currentSelectedOption >= options.Count)
                    currentSelectedOption = 0;
                onChangeOption?.Invoke(this);
            }

            if(Input.PressedKey(Keys.W) || (Input.HeldKey(Keys.W) && _heldTimer >= repeatRate))
            {
                currentSelectedOption--;
                onScrollOption?.Invoke(this);
                if(_heldTimer >= repeatRate)
                    _heldTimer -= (int)(repeatRate * 0.33f);

                if (currentSelectedOption < 0)
                    currentSelectedOption = options.Count - 1;
                onChangeOption?.Invoke(this);
            }

            if(Input.PressedKey([Keys.E, Keys.Enter]))
            {
                if(!unavailableOptions.Contains(currentSelectedOption))
                onSelectOption?.Invoke(this);
                else
                    onUnavailableSelectOption?.Invoke(this);
            }

            if(currentSelectedOption >= _optionWindowMin + _optionWindowMax)
            {
                _optionWindowMin++;
                onChangeOption?.Invoke(this);
            }
            else if(currentSelectedOption < _optionWindowMin)
            {
                _optionWindowMin--;
                onChangeOption?.Invoke(this);
            }

            _optionWindowMin = Math.Clamp(_optionWindowMin, 0, options.Count - 1);
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            base.OnDraw(spriteBatch);

            if (suspended && size.Y <= 0.5f)
                return;

            Texture2D arrow = Assets.GetTexture("Arrow");
            Vector2 osciliation = new(0, (float)Math.Sin(Main.totalTime));

            var decorativeOffset = new Vector2(0, -size.Y * 0.5f + targetSize.Y * 0.5f);

            Renderer.DrawRect(spriteBatch, GetPosition() - new Vector2(2) + decorativeOffset, size + new Vector2(4), borderColor * opacity);
            Renderer.DrawRect(spriteBatch, GetPosition() + decorativeOffset, size, panelColor * opacity);

            if (options.Count > 0 && _optionWindowMin <= options.Count && size == targetSize)
            {
                int offset = 0;
                for (int i = _optionWindowMin; i < _optionWindowMin + _optionWindowMax; i++)
                {
                    if (i > _optionWindowMin + _optionWindowMax || i >= options.Count)
                        break;

                    bool selected = i == currentSelectedOption;

                    bool selectedNFocused = selected && focused;

                    string option = options[i];

                    Color clr = (unavailableOptions.Contains(i) ? notAvailableColor : selected ? selectedColor : notSelectedColor) * opacity; ;
                    var textPosition = GetPosition() + new Vector2(16 + padding, 16 + (sizeY + 4 + extraSpacing) * offset) 
                        + (selectedNFocused ? new Vector2((float)Math.Sin(Main.totalTime) * bob, (float)Math.Cos(Main.totalTime) * bob) : Vector2.Zero);

                    spriteBatch.DrawBorderedString(font, option, textPosition, clr, Color.Black * opacity, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);

                    if (selectedNFocused)
                    {
                        bool evalPosition = GetPosition().X > 50;
                        Vector2 cursorScale = evalPosition ? new Vector2(3, 3) : new Vector2(-3, 3);
                        Vector2 cursorPosition = evalPosition ? textPosition + new Vector2(osciliation.Y, 0) - new Vector2(40, 0) : textPosition - new Vector2(osciliation.Y, 0) + new Vector2(size.X + 10, 0);
                        spriteBatch.Draw(Assets.GetTexture("Cursor3"), cursorPosition - new Vector2(padding, 0), null, Color.White * opacity, 0f, new Vector2(10, 0), cursorScale, SpriteEffects.None, 1f);

                    }

                    offset++;
                }
            }

            if (drawArrows)
            {
                spriteBatch.Draw(arrow, GetPosition() + new Vector2(size.X * 0.5f, -16) - osciliation +decorativeOffset, null, Color.White * opacity, 0f, new Vector2(5, 3.5f), new Vector2(3, -3), SpriteEffects.None, 0f);
                spriteBatch.Draw(arrow, GetPosition() + new Vector2(size.X * 0.5f, 16 + size.Y) + osciliation + decorativeOffset, null, Color.White * opacity, 0f, new Vector2(5, 3.5f), Vector2.One * 3, SpriteEffects.None, 0f);
            }
        }

        public string CurrentOption => options[currentSelectedOption];
    }
}

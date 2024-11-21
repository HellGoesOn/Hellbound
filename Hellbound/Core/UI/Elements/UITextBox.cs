using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.Elements
{
    public class UITextBox : UIElement
    {
        public string myText;
        bool isEditing;
        public bool IsEditing => isEditing;
        int cursorPosition;
        int lastCursorPosition;
        int cursorSize;
        int heldTime;
        int allowedRepeatRate;
        int sneakyDelay;
        public Color color;
        public Color boxBorderColor;
        public Color boxInnerColor;

        public UIEventHandler onTextChange;
        public UIEventHandler onTextSubmit;

        public int maxCharacters;
        public UITextBox()
        {
            myText = "Text.............";
            boxBorderColor = Color.DarkBlue;
            boxInnerColor = Color.White;

            size = font.MeasureString(myText) + new Vector2(16, 12);
            color = Color.White;
            allowedRepeatRate = 15;
            maxCharacters = 16;
            capturesMouse = true;

            onLoseParent += (sender) =>
            {
                this.onTextSubmit = null;
                this.onTextChange = null;
            };

        }

        public override void OnUpdate()
        {
            if (sneakyDelay > 0)
                sneakyDelay--;
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            Renderer.DrawRect(spriteBatch, GetPosition() - new Vector2(2), size+new Vector2(4), 1, boxInnerColor);  
            Renderer.DrawRect(spriteBatch, GetPosition(), size, 1, boxBorderColor);

            if (!string.IsNullOrEmpty(myText))
            {
                float sizeX = font.MeasureString(myText).X;

                //int maxRange = (int)(maxCharacters * 0.5f);
                string shownText = isEditing ? myText : sizeX > size.X - 16 ? "..." : myText; 

                if(isEditing && sizeX > size.X - 16)
                {
                    var newSize = font.MeasureString(myText) + new Vector2(32, 16);
                    Renderer.DrawRect(spriteBatch, GetPosition() - new Vector2(2), newSize + new Vector2(4), 1, boxInnerColor);
                    Renderer.DrawRect(spriteBatch, GetPosition(), newSize, 1, boxBorderColor);
                }

                Renderer.DrawBorderedString(spriteBatch, font, shownText, GetPosition() + new Vector2(8), color, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
                //Renderer.DrawBorderedString(spriteBatch, font, $"{heldTime}\n" + lastKeyStroked.ToString(), GetPosition() + new Vector2(8, -64), color, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);

                if(isEditing)
                {
                    int cursor = Math.Clamp(cursorPosition, 0, myText.Length);
                    float size = string.IsNullOrWhiteSpace(myText) ? 0 : font.MeasureString(myText[0..cursor]).X;
                    float opacity = (float)Math.Abs(Math.Sin(Main.totalTime * 0.25f));
                    Renderer.DrawBorderedString(spriteBatch, font, "|", GetPosition() + new Vector2(8 + size, 8), Color.Lime * opacity, Color.Black * opacity, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
                }
            }
        }

        public override void Click()
        {
            isEditing = !isEditing;
            sneakyDelay = 1;
            if (isEditing)
            {
                if (lastCursorPosition > 0 && lastCursorPosition <= myText.Length)
                    cursorPosition = lastCursorPosition;
                else
                    cursorPosition = myText.Length;
                cursorSize = 0;
                Input.OnKeyHeld += HeldKey;
                Input.OnKeyPressed += PressKey;
                Input.OnKeyReleased += ReleaseKey;
                Input.OnMousePressed += MousePress;
                Input.isTyping = true;
            }
            else
            {
                Input.OnKeyReleased -= ReleaseKey;
                Input.OnKeyHeld -= HeldKey;
                Input.OnKeyPressed -= PressKey;
                Input.OnMousePressed -= MousePress; 
                Input.isTyping = false;
            }

            base.Click();
        }

        private void MousePress(MouseButton button)
        {
            if (sneakyDelay > 0)
                return;

            onTextSubmit?.Invoke(this);
            isEditing = false;
            Input.isTyping = false;

            Input.OnKeyHeld -= HeldKey;
            Input.OnKeyPressed -= PressKey;
            Input.OnKeyReleased -= ReleaseKey;
            Input.OnMousePressed -= MousePress;

            lastCursorPosition = cursorPosition;
        }

        Keys lastKeyStroked;
        private void HeldKey(Keys key)
        {
            if (lastKeyStroked != key && key != Keys.LeftShift && key != Keys.RightShift)
                heldTime = 0;

            if (key != Keys.LeftShift && key != Keys.RightShift)
                heldTime++;

            if (heldTime > allowedRepeatRate)
            {
                PressKey(key);
            }

            if(key != Keys.LeftShift && key != Keys.RightShift)
                lastKeyStroked = key;
        }

        private void ReleaseKey(Keys key)
        {
            heldTime = 0;
        }

        private void PressKey(Keys key)
        {
            if (isEditing)
            {
                if(key == Keys.Up)
                {
                    if(parent is UIElement element)
                    {
                        var boxes = element.children.Where(x => x is UITextBox).ToList();

                        var myIndex = boxes.IndexOf(this);

                        if(myIndex-1 >= 0)
                        {
                            MousePress(MouseButton.Left);
                            boxes[myIndex - 1].Click();
                            return;
                        }
                    }
                }

                if (key == Keys.Down)
                {
                    if (parent is UIElement element)
                    {
                        var boxes = element.children.Where(x => x is UITextBox).ToList();

                        var myIndex = boxes.IndexOf(this);

                        if (myIndex + 1 < boxes.Count)
                        {
                            MousePress(MouseButton.Left);
                            boxes[myIndex + 1].Click();
                            return;
                        }
                    }
                }

                cursorPosition = Math.Clamp(cursorPosition, 0, Math.Max(0, myText.Length));
                string keyValue = "";
                if (key == Keys.Enter)
                {
                    MousePress(MouseButton.Left);
                } 
                else if(key == Keys.OemOpenBrackets)
                {
                    AddCharacter("{");
                } 
                else if (key == Keys.OemCloseBrackets)
                {
                    AddCharacter("}");
                } 
                else if(key == Keys.Delete)
                {
                    if (myText.Length > 0)
                    {
                        myText = myText.Remove(Math.Clamp(cursorPosition+1, 0, myText.Length - 1), 1);
                        onTextChange?.Invoke(this);
                    }
                }
                else if (key == Keys.Back)
                {
                    if (myText.Length > 0)
                    {
                        cursorPosition--;
                        myText = myText.Remove(Math.Clamp(cursorPosition, 0, myText.Length - 1), 1);
                        onTextChange?.Invoke(this);
                    }
                }
                else if(key== Keys.OemMinus)
                {
                    AddCharacter("-");
                }
                else if(key == Keys.OemPlus)
                {
                    AddCharacter("+");
                }
                else if (key == Keys.OemSemicolon)
                {
                    AddCharacter(";");
                } 
                else if (key == Keys.OemQuotes)
                {
                    if (Input.HeldKey(Keys.LeftShift) || Input.HeldKey(Keys.RightShift))
                        AddCharacter("\"");
                    else
                        AddCharacter("\'");
                } 
                else if ((key >= Keys.D0 && key <= Keys.D9))
                {
                    keyValue = Regex.Replace(key.ToString(), "[^0-9]", "");
                    AddCharacter(keyValue);
                } 
                else if (key == Keys.OemPeriod)
                {
                    keyValue = ".";
                    AddCharacter(keyValue);
                } 
                else if (key == Keys.OemComma)
                {
                    keyValue = ",";
                    AddCharacter(keyValue);
                } 
                else if (key == Keys.Space)
                {
                    keyValue = " ";
                    AddCharacter(keyValue);
                } 
                else if (key == Keys.Left)
                {
                    cursorPosition--;
                } 
                else if (key == Keys.Right)
                {
                    cursorPosition++;
                } 
                else if ((key >= Keys.A && key <= Keys.Z) || key == Keys.OemComma || key == Keys.OemPeriod)
                {
                    keyValue = key.ToString();
                    if (!Input.HeldKey(Keys.LeftShift) && !Input.HeldKey(Keys.RightShift))
                        keyValue = keyValue.ToLower();

                    AddCharacter(keyValue);
                }
            }
        }

        private void AddCharacter(string keyValue)
        {
            if (myText.Length >= maxCharacters)
                return;

            myText = myText.Insert(cursorPosition, keyValue);
            cursorPosition++;
            onTextChange?.Invoke(this);
        }
    }
}

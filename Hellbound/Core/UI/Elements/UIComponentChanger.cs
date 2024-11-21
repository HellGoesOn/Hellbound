using HellTrail.Core.ECS;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.Elements
{
    public class UIComponentChanger : UIElement
    {
        IComponent component;
        UITextBox[] texts;
        FieldInfo[] infos;
        public UIComponentChanger(IComponent component)
        {
            font = Assets.Arial;
            this.component = component;

            UIPanel drag = new UIDraggablePanel()
            {
                size = new Vector2(300, 30)
            };

            UIPanel panel = new UIPanel();

            infos = component.GetType().GetFields();
            texts = new UITextBox[infos.Length];

            Vector2 accumulatedSize2 = new Vector2(300, 46);

            for(int i = 0; i < infos.Length; i++)
            {
                UIBorderedText text = new UIBorderedText(infos[i].Name);
                text.SetPosition(accumulatedSize2.X + 12, accumulatedSize2.Y);
                accumulatedSize2.Y += font.MeasureString("M").Y + 16;
                panel.Append(text);
            }

            Vector2 accumulatedSize = new Vector2(300, 40);
            for (int i = 0; i < infos.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                ComponentIO.FieldToText(component, sb, infos[i]);
                texts[i] = new UITextBox()
                {
                    id = $"{i}",
                    myText = Regex.Replace(sb.ToString(), "\"", ""),
                    maxCharacters = 255
                };
                texts[i].size = new Vector2(300, 40);

                texts[i].SetPosition(0, accumulatedSize.Y);
                texts[i].onTextChange = OnTextChange;
                texts[i].onTextSubmit = OnTextSubmit;
                texts[i].onMouseEnter = (sender) =>
                {
                    (sender as UITextBox).color = Color.Yellow;
                };
                texts[i].onMouseLeave = (sender) =>
                {
                    (sender as UITextBox).color = Color.White;
                };

                accumulatedSize.Y += texts[i].size.Y;
                panel.Append(texts[i]);
            }

            UIWindowButton closeButton = new UIWindowButton(WindowButtonType.XMark, "Close", Color.Red)
            {
                scale = Vector2.One * 2,
                onClick = (_) =>
                {
                    parent.Disown(this);
                }
            };
            UIText txt = new UIText(this.component.GetType().Name);
            txt.SetPosition(36, 8);

            panel.size = accumulatedSize + new Vector2(0, 0);
            panel.SetPosition(0, 0);
            panel.Append(txt);
            drag.Append(panel);
            drag.Append(closeButton);
            this.Append(drag);
            drag.SetPosition(Renderer.UIPreferedWidth * 0.5f - 150, Renderer.UIPreferedHeight * 0.5f - panel.size.Y * 0.5f);
        }

        public void OnTextChange(UIElement sender)
        {
            int i = int.Parse(sender.id);

            var box = (sender as UITextBox);

            try
            {
                ComponentIO.TextToFieldValue(infos[i], component, box.myText);
            }
            catch
            {
            }
        }

        public void OnTextSubmit(UIElement sender)
        {
            int i = int.Parse(sender.id);

            var box = (sender as UITextBox);
            try
            {
                ComponentIO.TextToFieldValue(infos[i], component, box.myText);
            }
            catch
            {
                StringBuilder sb = new();
                ComponentIO.FieldToText(component, sb, infos[i]);
                box.myText = sb.ToString();
            }
        }


    }
}

using HellTrail.Core.ECS;
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
            this.component = component;

            UIPanel drag = new UIDraggablePanel()
            {
                size = new Vector2(300, 30)
            };

            UIPanel panel = new UIPanel();

            infos = component.GetType().GetFields();
            texts = new UITextBox[infos.Length];

            Vector2 accumulatedSize = new Vector2(300, 40);
            for (int i = 0; i < infos.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                ComponentIO.FieldToText(component, sb, infos[i]);
                UIBorderedText text = new UIBorderedText(infos[i].Name);
                text.SetPosition(accumulatedSize.X + 12, 8);
                texts[i] = new UITextBox()
                {
                    id = $"{i}",
                    myText = Regex.Replace(sb.ToString(), "\"", ""),
                    maxCharacters = 255
                };
                texts[i].size = new Vector2(300, 40);
                texts[i].Append(text);

                texts[i].SetPosition(0, accumulatedSize.Y);
                texts[i].onTextChange = OnTextChange;
                texts[i].onTextSubmit = OnTextSubmit;

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
            panel.size = accumulatedSize;
            panel.SetPosition(0, 32);
            drag.Append(panel);
            drag.Append(closeButton);
            this.Append(drag);
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

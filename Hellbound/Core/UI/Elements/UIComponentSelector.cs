using HellTrail.Core.ECS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.Elements
{
    public class UIComponentSelector : UIElement
    {
        Entity _entity;
        UITextBox searcher;

        public UIComponentSelector(Entity entity)
        {
            font = Assets.Arial;
            _entity = entity;

            this.onLoseParent = (sender) =>
            {
                _entity = null;
            };

            UIPanel panel = new();
            searcher = new UITextBox()
            {
                font = Assets.Arial,
                clearOnBeginTyping = true,
                myText = "Type to search",
                maxCharacters = 255,
                onTextSubmit = OnSubmit
            };
            searcher.size.X = 280;
            searcher.SetPosition(0, -40);

            panel.Append(searcher);

            float y = 0;
            float x = 0;
            List<UIElement> buffer = [];
            foreach (string name in Context.ComponentNameById.Values)
            {
                UIBorderedText componentName = new(name)
                {
                    font = Assets.Arial,
                    capturesMouse = true,
                    id = name,
                    size = font.MeasureString(name),
                    onMouseEnter = (sender) =>
                    {
                        (sender as UIBorderedText).color = Color.Yellow;
                    },
                    onMouseLeave = (sender) =>
                    {
                        (sender as UIBorderedText).color = Color.White;
                    }
                };
                buffer.Add(componentName);

                if(componentName.size.X + 32 > x)
                    x = componentName.size.X + 32;

                componentName.id = name;
                componentName.SetPosition(6, 6 + y);
                componentName.onClick = (sender) =>
                {
                    Type type = Context.ComponentTypeByName[sender.id];
                    AddComponent(type);
                    parent.Disown(this);
                };
                y += componentName.size.Y + 4;

            }

            foreach (UIElement element in buffer)
            {
                element.size.X = x;
                panel.Append(element);
            }

            buffer.Clear();
            buffer = null;
            panel.size.X = x + 24;
            panel.size.Y = y+4;

            Append(panel);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if(makeAttempt)
            {
                makeAttempt = false;

                var type = Context.ComponentTypeByName.FirstOrDefault(x => x.Key.ToLower().Contains(attemptedPick.ToLower())).Value;

                if (!string.IsNullOrWhiteSpace(attemptedPick) && type != null)
                {
                    AddComponent(type);
                    parent.Disown(this);
                } else
                {
                    searcher.myText = "Not Found";
                }
            }
        }

        private void AddComponent(Type type)
        {
            IComponent component = (IComponent)RuntimeHelpers.GetUninitializedObject(type);

            foreach (var field in type.GetFields())
                ComponentIO.SetDefaultField(component, field);

            _entity.AddComponent(component);
        }

        bool makeAttempt;
        string attemptedPick;
        public void OnSubmit(UIElement sender)
        {
            makeAttempt = true;
            attemptedPick = (sender as UITextBox).myText;
        }
    }
}

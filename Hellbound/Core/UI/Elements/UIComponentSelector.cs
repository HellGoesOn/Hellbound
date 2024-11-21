using HellTrail.Core.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.Elements
{
    public class UIComponentSelector : UIElement
    {
        Entity _entity;

        public UIComponentSelector(Entity entity)
        {
            font = Assets.Arial;
            _entity = entity;

            this.onLoseParent = (sender) =>
            {
                _entity = null;
            };

            UIPanel panel = new UIPanel();

            float y = 0;
            foreach (string name in Context.ComponentNameById.Values)
            {
                UIBorderedText componentName = new UIBorderedText(name)
                {
                    capturesMouse = true,
                    id = name,
                    size = font.MeasureString(name)
                };

                componentName.SetPosition(6, 6 + y);
                componentName.onClick = (sender) =>
                {
                    entity.AddComponent((IComponent)RuntimeHelpers.GetUninitializedObject(Type.GetType(sender.id)));
                    parent.Disown(this);
                };

                y += componentName.size.Y + 4;

            }
        }
    }
}

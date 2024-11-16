using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI
{
    public class UIState
    {
        public string id;

        public bool active = true;
        public bool visible = true;

        public List<UIElement> children = [];

        public UIStateEventHandler OnUpdate;

        public UIState() 
        {
            id = string.Empty;
        }

        public virtual void Update()
        {
            if (!active)
                return;

            OnUpdate?.Invoke(this);

            foreach (UIElement child in children)
            {
                child.Update();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!visible)
                return;

            foreach (UIElement child in children.Where(x=>x.Visible))
            {
                child.Draw(spriteBatch);
            }
        }

        public void Append(UIElement child)
        {
            children.Add(child);
        }

        public UIElement GetElement(Predicate<UIElement> predicate)
        {
            return children.Find(predicate);
        }

        public UIElement GetElementById(string id) => GetElement(x => x.id == id);
    }

    public delegate void UIStateEventHandler(UIState sender);
}

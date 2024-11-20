using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI
{
    public class UIState : IUIElement
    {
        public string id;

        public bool active = true;
        public bool visible = true;

        public List<UIElement> children = [];
        private List<UIElement> _childrenToRemove = [];

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

            foreach(UIElement child in _childrenToRemove)
            {
                children.Remove(child);
            }

            _childrenToRemove.Clear();
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

        public void Append(IUIElement child)
        {
            var newChild = (UIElement)child;
            newChild.parent = this;
            children.Add(newChild);
            newChild.RecalcPosition();
        }

        public void Disown(IUIElement child)
        {
            _childrenToRemove.Add((UIElement)child);
            (child as UIElement).parent = null;
        }


        public UIElement GetElement(Predicate<UIElement> predicate)
        {
            return children.Find(predicate);
        }

        public UIElement GetElementById(string id) => GetElement(x => x.id == id);
    }

    public delegate void UIStateEventHandler(UIState sender);
}

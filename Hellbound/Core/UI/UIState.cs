using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.UI
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

            int childrenCount = children.Count;

            for (int i = 0; i < childrenCount; i++) {
                children[i].Update();
            }

            foreach (UIElement child in _childrenToRemove) {
                children.Remove(child);
            }

            _childrenToRemove.Clear();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!visible)
                return;

            foreach (UIElement child in children.Where(x => x.Visible)) {
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

        public void Disown(IUIElement element, bool plannedToAdopt = false)
        {
            if (!children.Contains(element))
                return;

            var killedElement = (element as UIElement);
            killedElement.onLoseParent?.Invoke(killedElement);
            if (!plannedToAdopt) {
                killedElement.onMouseEnter = null;
                killedElement.onMouseLeave = null;
                killedElement.onClick = null;
                killedElement.onUpdate = null;
                killedElement.onLoseParent = null;
            }
            killedElement.parent = null;
            _childrenToRemove.Add(killedElement);
        }

        public void Disown(string id, bool plannedToAdopt = false)
        {
            if (!children.Any(x => x.id == id))
                return;

            var killedElement = children.First(x => x.id == id);
            killedElement.onLoseParent?.Invoke(killedElement);
            if (!plannedToAdopt) {
                killedElement.onMouseEnter = null;
                killedElement.onMouseLeave = null;
                killedElement.onClick = null;
                killedElement.onUpdate = null;
                killedElement.onLoseParent = null;
            }
            killedElement.parent = null;
            _childrenToRemove.Add(killedElement);
        }


        public UIElement GetElement(Predicate<UIElement> predicate)
        {
            return children.Find(predicate);
        }

        public UIElement GetElementById(string id) => GetElement(x => x.id == id);

        public List<UIElement> Children => children;

        public Vector2 GetPosition() => Vector2.Zero;
    }

    public delegate void UIStateEventHandler(UIState sender);
}

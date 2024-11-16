using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI
{
    public class UIElement : IUIElement
    {
        public string id;

        public UIElement()
        {
            id = string.Empty;
        }

        public bool active = true;
        private bool visible = true;
        public Vector2 size;
        public UIEventHandler onUpdate;
        public List<UIElement> children = [];
        public List<UIElement> _childrenToDisown = [];
        public IUIElement parent;
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;

                foreach (UIElement child in children)
                    child.Visible = visible;
            }
        }

        public void Update()
        {
            if (!active)
                return;

            foreach (UIElement child in children)
                child.Update();
            OnUpdate();

            onUpdate?.Invoke(this);

            foreach(UIElement child in _childrenToDisown)
                children.Remove(child);

            _childrenToDisown.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!visible)
                return;

            OnDraw(spriteBatch);

            foreach (UIElement child in children)
            {
                child.Draw(spriteBatch);
            }
        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnDraw(SpriteBatch spriteBatch)
        {

        }

        public T MakeChild<T>() where T : UIElement
        {
            T result = default;
            result.parent = this;

            return result;
        }

        public void Append(IUIElement element)
        {
            var newElement = (element as UIElement);
            newElement.parent = this;
            children.Add(newElement);
        }

        public void Disown(IUIElement element)
        {
            if (!children.Contains(element))
                return;

            var killedElement = (element as UIElement);
            killedElement.parent = null;
            _childrenToDisown.Add(killedElement); 
        }

        public void DisownById(string id) => Disown(children.Find(x=>x.id == id));


        private float rotation;
        public float Rotation
        {
            get
            {
                if (parent is UIElement element)
                {
                    return element.Rotation + rotation;
                }

                return rotation;
            }
            set => rotation = value;
        }

        public void SetPosition(Vector2 setPosition)
        {
            this.Position = setPosition;
        }

        private Vector2 position;
        public Vector2 Position
        {
            get
            { 
                if(parent is UIElement element)
                {
                    return element.Position + position;
                }

                return position;
            }
            set => position = value;
        }

        public UIElement GetElement(Predicate<UIElement> predicate)
        {
            return children.Find(predicate);
        }

        public UIElement GetElementById(string id) => GetElement(x => x.id == id);
    }

    public delegate void UIEventHandler(UIElement sender);
}

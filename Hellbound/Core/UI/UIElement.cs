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
        bool capturedMouseLastFrame;

        public string id;

        public UIElement()
        {
            id = string.Empty;
        }

        public bool active = true;
        private bool visible = true;
        public bool capturesMouse;
        public bool isMouseHovering;
        public Vector2 size;
        public Vector2 scale = Vector2.One;
        public UIEventHandler onUpdate;
        public UIEventHandler onClick;
        public UIEventHandler onLoseParent;
        public UIEventHandler onMouseEnter;
        public UIEventHandler onMouseLeave;
        public List<UIElement> children = [];
        public List<UIElement> _childrenToDisown = [];
        public IUIElement parent;
        public SpriteFont font = Assets.DefaultFont;
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

            isMouseHovering = false; 

            if (capturesMouse)
            {
                var mpos = Input.UIMousePosition;
                if (mpos.X >= GetPosition().X && mpos.X <= GetPosition().X + size.X*scale.X
                    && mpos.Y >= GetPosition().Y && mpos.Y <= GetPosition().Y + size.Y * scale.Y)
                {
                    isMouseHovering = true;

                    if (isMouseHovering && !capturedMouseLastFrame)
                    {
                        onMouseEnter?.Invoke(this);
                    }

                    UIManager.hoveredElement = this;
                }
            }

            if(capturedMouseLastFrame && !isMouseHovering)
            {
                onMouseLeave?.Invoke(this);
            }



            foreach (UIElement child in children)
                child.Update();
            OnUpdate();

            onUpdate?.Invoke(this);

            foreach(UIElement child in _childrenToDisown)
                children.Remove(child);

            _childrenToDisown.Clear();

            capturedMouseLastFrame = isMouseHovering;
        }

        public virtual void Click()
        {
            onClick?.Invoke(this);
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

            if(font != Assets.DefaultFont)
                newElement.font = this.font;
            children.Add(newElement);

            RecalcPosition();
        }

        public void Disown(IUIElement element)
        {
            if (!children.Contains(element))
                return;

            var killedElement = (element as UIElement);
            killedElement.onLoseParent?.Invoke(killedElement);
            killedElement.parent = null;
            _childrenToDisown.Add(killedElement); 
        }

        public void DisownById(string id) => Disown(children.Find(x=>x.id == id));

        public void SetFont(SpriteFont font)
        {
            this.font = font;
            foreach (UIElement child in children)
            { child.SetFont(font); }
        }


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

        public UIElement SetPosition(Vector2 setPosition)
        {
            position = setPosition;
            RecalcPosition();

            return this;
        }

        public void RecalcPosition()
        {
            if (parent is UIElement element)
                _positionAnchor = position + element._positionAnchor;
            else
                _positionAnchor = position;

            foreach (UIElement child in children)
            {
                child.RecalcPosition();
            }

        }

        public UIElement SetPosition(float x, float y)
        {
            SetPosition(new Vector2(x, y));
            return this;
        }

        public UIElement SetPosition(float value)
        {
            SetPosition(new Vector2(value));
            return this;
        }

        public Vector2 GetPosition() => _positionAnchor;

        private Vector2 position;
        private Vector2 _positionAnchor;

        public UIElement GetElement(Predicate<UIElement> predicate)
        {
            return children.Find(predicate);
        }

        public UIElement GetElementById(string id) => GetElement(x => x.id == id);
    }

    public delegate void UIEventHandler(UIElement sender);
}

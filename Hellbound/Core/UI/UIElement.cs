﻿using Microsoft.Xna.Framework;
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
        public bool capturesMouse;
        public bool isMouseHovering;
        public Vector2 size;
        public UIEventHandler onUpdate;
        public UIEventHandler onClick;
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
                if (mpos.X >= Position.X && mpos.X <= Position.X + size.X
                    && mpos.Y >= Position.Y && mpos.Y <= Position.Y + size.Y)
                {
                    isMouseHovering = true;
                    UIManager.hoveredElement = this;
                }
            }


            foreach (UIElement child in children)
                child.Update();
            OnUpdate();

            onUpdate?.Invoke(this);

            foreach(UIElement child in _childrenToDisown)
                children.Remove(child);

            _childrenToDisown.Clear();
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

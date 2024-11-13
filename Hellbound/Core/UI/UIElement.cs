using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI
{
    public class UIElement
    {
        public UIEventHandler onUpdate;
        public Vector2 size;
        public List<UIElement> children = [];
        public UIElement parent;
        private bool visible = true;
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
            foreach (UIElement child in children)
                child.Update();
            OnUpdate();

            onUpdate?.Invoke(this);
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

        public void Append(UIElement element)
        {
            element.parent = this;
            children.Add(element);
        }


        private float rotation;
        public float Rotation
        {
            get => parent == null ? rotation : parent.rotation + rotation;
            set => rotation = value;
        }

        public void SetPosition(Vector2 setPosition)
        {
            this.Position = setPosition;
        }

        private Vector2 position;
        public Vector2 Position
        {
            get => parent == null ? position : parent.Position + position;
            set => position = value;
        }
    }

    public delegate void UIEventHandler(UIElement sender);
}

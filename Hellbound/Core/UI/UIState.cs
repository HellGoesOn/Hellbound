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
        public bool active = true;
        public bool visible = true;

        public List<UIElement> children = [];

        public virtual void Update()
        {
            if (!active)
                return;

            foreach (UIElement child in children)
            {
                child.Update();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!visible)
                return;

            foreach (UIElement child in children)
            {
                child.Draw(spriteBatch);
            }
        }

        public void Append(UIElement child)
        {
            children.Add(child);
        }
    }
}

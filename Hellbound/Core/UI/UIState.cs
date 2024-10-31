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

        public void Update()
        {
            foreach (UIElement child in children)
            {
                child.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
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

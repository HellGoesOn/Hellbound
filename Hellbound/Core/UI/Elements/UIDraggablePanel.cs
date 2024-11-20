using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.Elements
{
    public class UIDraggablePanel : UIPanel
    {
        public UIDraggablePanel() 
        {
            capturesMouse = true;
        }
        Vector2 anchor;
        Vector2 oldPos;
        public override void Click()
        {
            base.Click();
            Input.OnMouseHeld += Drag;
            Input.OnMouseReleased += StopDrag;
            anchor = Input.UIMousePosition;
            oldPos = GetPosition();
        }

        private void StopDrag(MouseButton button)
        {
            Input.OnMouseHeld -= Drag;
            Input.OnMouseReleased -= StopDrag;
        }   

        private void Drag(MouseButton button)
        {
            SetPosition(oldPos - (anchor - Input.UIMousePosition));
        }
    }
}

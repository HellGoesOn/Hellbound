using HellTrail.Core.ECS;
using HellTrail.Core.UI;
using HellTrail.Core.UI.Elements;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Overworld
{
    public class OverworldUIState : UIState
    {
        public UIBorderedText debugText;
        public int debugTime;
        
        public OverworldUIState() 
        {
            debugText = new("");
            Append(debugText);
        }

        public override void Update()
        {
            base.Update();

            if (debugTime > 0)
                debugTime--;

            debugText.Visible = debugTime > 0;

            debugText.SetPosition(Input.UIMousePosition);
            debugText.origin = Vector2.Zero;
        }
    }
}

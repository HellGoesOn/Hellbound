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

            World con = Main.instance.activeWorld;
            int off = 0;
            foreach(var system in con.systems.GetAll())
            {
                UICheckBox box = new UICheckBox($"Toggle {system.GetType().Name}");
                box.Position = new Vector2(16, 64 + off);
                box.isChecked = true;
                box.color = Color.Lime;
                box.onClick = (sender) =>
                {
                    var b = (sender as UICheckBox);
                    b.color = b.isChecked ? Color.Red : Color.Lime;
                    con.systems.ToggleSystem(system.GetType());
                };
                Append(box);
                off += 18;
            }

        }

        public override void Update()
        {
            base.Update();

            if (debugTime > 0)
                debugTime--;

            debugText.Visible = debugTime > 0;

            debugText.Position = Input.UIMousePosition;
            debugText.origin = Vector2.Zero;
        }
    }
}

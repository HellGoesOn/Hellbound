using HellTrail.Core.UI;
using HellTrail.Core.UI.Elements;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Items.Consumables
{
    public class PeasItem : Item
    {
        private float opacity;

        public PeasItem() : base("Peas", "You feel disturb")
        {
            opacity = -0.25f;
            icon = "Peas";
            frames = [new FrameData(0, 0, 32, 32)];
            iconScale *= 3;
            validTargets = ValidTargets.World;
        }

        protected override void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {
            base.OnUse(caster, battle, targets);
            opacity = -0.25f;

            SoundEngine.StopMusic(35 * 60);
            SoundEngine.PlaySound("FinalFlash", 0.25f);

            UIPicture disturbingThePeace = new("14509", new FrameData(255, 81, 262, 240));
            disturbingThePeace.origin = new(131, 100);
            disturbingThePeace.scale *= 4;
            disturbingThePeace.tint = Color.White * 0.0f;
            disturbingThePeace.onUpdate = (sender) =>
            {
                if (opacity < 0.5f)
                    opacity += 0.005f;

                if(opacity >= 0)
                disturbingThePeace.tint = Color.White * opacity;
            };
            disturbingThePeace.SetPosition(Renderer.UIPreferedWidth * 0.5f, Renderer.UIPreferedHeight * 0.5f);

            UIManager.overworldUI.Append(disturbingThePeace);
        }
    }
}

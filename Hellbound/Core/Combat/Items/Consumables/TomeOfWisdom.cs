using HellTrail.Core.UI;
using HellTrail.Core.UI.Elements;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Items.Consumables
{
    public class TomeOfWisdom : Item
    {
        public TomeOfWisdom() : base("Tome of Wisdom", "Contents of this ancient manuscript evade your mind almost completely.\n\n\"C'mon you guys, you are derefencing \na NullPtr!\"\n\nGives 1001 EXP")
        {
            icon = "TomeOfWisdom";
            frames = [new FrameData(0, 0, 32, 32)];
            consumable = true;
            iconScale *= 3;
        }

        protected override void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {
            if (caster.name == "Dog")
            {
                SoundEngine.PlaySound("MeepMerp", 0.75f);

                var panel = new UIAnimatedPanel(new Vector2(600, 132));
                panel.Append(new UIBorderedText("Unfortunately, even the goodest of boys cannot read.\nNeither can you, but that's your fault. Lazybum.", 36).SetPosition(16));

                var inventoryMenu = (UIManager.overworldUI.GetElementById("inventoryMenu") as UIScrollableMenu);

                panel.openSpeed = 0.25f;

                panel.onUpdate = (sender) =>
                {
                    inventoryMenu.isActive = false;
                    if (Input.PressedKey([Keys.Escape, Keys.E, Keys.Q]))
                    {
                        panel.isClosed = true;
                    }
                };

                panel.onLoseParent = (sender) =>
                {
                    inventoryMenu.isActive = true;
                };

                panel.SetPosition(Renderer.UIPreferedWidth * 0.5f - panel.targetSize.X * 0.5f, Renderer.UIPreferedHeight * 0.5f - panel.targetSize.Y * 0.5f);

                UIManager.overworldUI.Append(panel);


                count++;
            } else
            {
                SoundEngine.PlaySound("LvlUp", 0.75f);
                caster.stats.EXP += 1001;
                caster.TryLevelUp(true);
            }
        }
    }
}

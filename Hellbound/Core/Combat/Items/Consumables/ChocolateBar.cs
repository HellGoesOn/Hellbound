using Casull.Core.Combat.Abilities;
using Casull.Core.UI;
using Casull.Core.UI.Elements;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Casull.Core.Combat.Items.Consumables
{
    public class ChocolateBar : Item
    {
        public ChocolateBar() : base("Chocolate Bar", "Tasty snack\n\n\"Chocholate?? CHOCHOLATE?! CHOCHOLATE!!!\"\n\nRestores 50 HP")
        {
            icon = "ChocolateBar";
            frames = [new FrameData(0, 0, 32, 32)];
            consumable = true;
            iconScale *= 3;
        }

        protected override void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {
            if (caster.name == "Dog") {
                SoundEngine.PlaySound("MeepMerp", 0.75f);

                var panel = new UIAnimatedPanel(new Vector2(600, 132));
                panel.Append(new UIBorderedText("Wait.. you didn't.. no way.. you. IDIOT. Absolute. Fucking. IMBECILE. Do you even know what you've done??", 36).SetPosition(16));

                var inventoryMenu = (UIManager.overworldUI.GetElementById("inventoryMenu") as UIScrollableMenu); if (Main.instance.GetGameState() is Battle) {
                    inventoryMenu = (UIManager.combatUI.GetElementById("inventoryMenu") as UIScrollableMenu);
                }

                panel.openSpeed = 0.25f;

                panel.panelColor = Color.Crimson;
                panel.onUpdate = (sender) => {
                    inventoryMenu.focused = false;
                    if (Input.PressedKey([Keys.Escape, Keys.E, Keys.Q])) {
                        panel.isClosed = true;
                    }
                };

                panel.onLoseParent = (sender) => {
                    inventoryMenu.focused = true;
                };

                panel.SetPosition(Renderer.UIPreferedWidth * 0.5f - panel.targetSize.X * 0.5f, Renderer.UIPreferedHeight * 0.5f - panel.targetSize.Y * 0.5f);

                UIManager.overworldUI.Append(panel);

                caster.Stats.HP = 0;
            }
            else {
                new Dia() { baseDamage = 50, spCost = 0 }.Use(caster, battle, targets, true);
            }
        }
    }
}

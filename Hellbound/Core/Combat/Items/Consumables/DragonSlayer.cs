using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Items.Consumables
{
    public class DragonSlayer : Item
    {
        public DragonSlayer() : base("Dragon Slayer", "A slab of metal too crude to be called a sword.")
        {
            icon = "Dragonslayer";
            frames = [new FrameData(0, 0, 88, 88)];
            iconScale *= 2;
            canTarget = ValidTargets.World;
            //iconRotation = MathHelper.PiOver4;
            iconOrigin = new Vector2(44);

            onViewed = (item) => {
                item.iconScale *= 1.05f;
            };
        }

        protected override void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {
            base.OnUse(caster, battle, targets);

            SoundEngine.PlaySound("clangberserk", 3);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Items.Consumables
{
    public class InfinitePizza : Item
    {
        public InfinitePizza() : base("Infinite Pizza", "A piece of pizza that never ends, how cool is that?!\n\n\nRestores 1 HP")
        {
            icon = "Pizza";
            frames = [new FrameData(0, 0, 32, 32)];
            iconScale *= 4;
        }

        protected override void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {
            SoundEngine.PlaySound("NomNomNom", 0.75f);
            caster.Stats.HP = Math.Clamp(caster.Stats.HP + 1, 0, caster.Stats.MaxHP);
        }
    }
}

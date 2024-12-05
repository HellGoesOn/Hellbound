using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Items.Consumables
{
    public class Tomato : Item
    {
        public Tomato() : base("Tomato", "Tasty soul replenishing tomato.\n\"I could eat those as a snack.\"\n\n\nRestores 24 SP")
        {
            icon = "Tomato";
            frames = [new FrameData(0, 0, 32, 32)];
            consumable = true;
            iconScale *= 3;
        }

        protected override void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {
            SoundEngine.PlaySound("senzu", 0.75f);
            caster.stats.SP = Math.Clamp(caster.stats.SP + 24, 0, caster.stats.MaxSP);
        }
    }
}

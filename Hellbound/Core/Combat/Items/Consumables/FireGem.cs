using HellTrail.Core.Combat.Abilities.Fire;
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
    public class FireGem : Item
    {
        public FireGem() : base("Fire Gem", "Casts Agi on a single enemy target.")
        {
            icon = "FireGem";
            frames = [new FrameData(0, 0, 32, 32)];
            consumable = true;
            iconScale *= 3;
            canUseOutOfBattle = false;
            canTarget = ValidTargets.Enemy;
        }

        protected override void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {
            new Agi()
            {
                hpCost = 0,
                spCost = 0
            }.Use(caster, battle, targets, true);
        }
    }
}

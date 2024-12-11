﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Items.Consumables
{
    public class AdrenalineShot : Item
    {
        public AdrenalineShot() : base("Adrenaline Shot", "Revives one ally with 10% HP")
        {
            icon = "AdrenalineShot";
            frames = [new FrameData(0, 0, 32, 32)];
            iconScale *= 4;
            canTarget = ValidTargets.DownedAlly;
            consumable = true;
        }

        protected override void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {
            if (!caster.Downed)
                return;

            SoundEngine.PlaySound("stimpack", 0.75f);
            caster.Stats.HP = (int)(caster.Stats.MaxHP * 0.1f);
        }
    }
}

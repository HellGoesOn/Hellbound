﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellTrail.Core.Combat.Abilities;

namespace HellTrail.Core.Combat
{
    public class BasicAI
    {
        public virtual void MakeDecision(Unit whoAmI, Battle battle)
        {
            if (whoAmI.abilities.Count > 0)
            {
                Ability abilityToUse = whoAmI.abilities[battle.rand.Next(whoAmI.abilities.Count)];

                var getTargets = battle.units.Where(GetSelector(abilityToUse, whoAmI)).ToList();

                if (!abilityToUse.aoe)
                    getTargets = [getTargets[battle.rand.Next(getTargets.Count)]]; // pick random target if used skill isn't AoE

                abilityToUse.Use(whoAmI, battle, getTargets);
            }

            battle.State = BattleState.DoAction;
        }

        public static Func<Unit, bool> GetSelector(Ability abilityToUse, Unit whoAmI)
        {
            Func<Unit, bool> selector = x => whoAmI.team != x.team && !x.Downed;

            if (abilityToUse.canTarget == ValidTargets.Ally)
                selector = x => whoAmI.team == x.team;

            if (abilityToUse.canTarget == ValidTargets.AllButSelf)
                selector = x => x != whoAmI && !x.Downed;

            if (abilityToUse.canTarget == ValidTargets.All)
                selector = x => !x.Downed;

            if (abilityToUse.canTarget == ValidTargets.Downed)
                selector = x => x.HP <= 0;

            return selector;
        }
    }
}

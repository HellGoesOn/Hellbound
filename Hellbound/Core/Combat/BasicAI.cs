using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public class BasicAI
    {
        public virtual void MakeDecision(Unit whoAmI, Battle battle)
        {
            if (whoAmI.abilities.Count > 0)
            {
                Ability abilityToUse = whoAmI.abilities[battle.rand.Next(whoAmI.abilities.Count)];

                Func<Unit, bool> selector = x => whoAmI.team != x.team && !x.Downed;

                if (abilityToUse.canTarget == ValidTargets.Ally)
                    selector = x => whoAmI.team == x.team;

                if (abilityToUse.canTarget == ValidTargets.Downed)
                    selector = x => x.HP <= 0;

                var getTargets = battle.units.Where(selector).ToList();

                if (!abilityToUse.aoe)
                    getTargets = [getTargets[battle.rand.Next(getTargets.Count)]]; // pick random target if used skill isn't AoE

                abilityToUse.Use(whoAmI, battle, getTargets);
            }

            battle.State = BattleState.BeginAction;
        }
    }
}

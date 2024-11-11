using HellTrail.Core.Combat.Status.Debuffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Abilities
{
    public class GhastlyWail : Ability
    {
        public GhastlyWail() : base("Ghastly Wail", "Kill all affected with Fear")
        {
            aoe = true;
            canTarget = ValidTargets.All;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            Sequence seq = new Sequence(battle);
            foreach (var target in targets.Where(x => x.HasStatus<Fear>())) {
                seq.Add(new DoDamageSequence(caster, target, 99999, ElementalType.Almighty));
                seq.Add(new PlaySoundSequence("Death"));
            }
            battle.sequences.Add(seq);
        }
    }
}

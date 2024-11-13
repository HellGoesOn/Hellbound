using HellTrail.Core.Combat.Sequencer;
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
            spCost = 20;
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
            seq.Add(new DelaySequence(0));
            battle.sequences.Add(seq);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellTrail.Core.Combat.Sequencer;

namespace HellTrail.Core.Combat.Abilities
{
    public class Dia : Ability
    {
        public Dia() : base("Dia", "Light heal to 1 ally")
        {
            aoe = false;
            canTarget = ValidTargets.Ally;
            spCost = 3;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            base.UseAbility(caster, battle, targets);
            Sequence sequence = battle.CreateSequence();
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(60));
            sequence.Add(new HealTargetSqequence(targets[0], 30));
            sequence.Add(new DelaySequence(60));
        }
    }
}

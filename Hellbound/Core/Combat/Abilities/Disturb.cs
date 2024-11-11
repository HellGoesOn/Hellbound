using HellTrail.Core.Combat.Status.Debuffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Abilities
{
    public class Disturb : Ability
    {
        public Disturb() : base("War Cry", "High chance of Fear to 1 foe")
        {
            aoe = false;
            canTarget = ValidTargets.Enemy;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            base.UseAbility(caster, battle, targets);

            battle.lastAction = $"{caster.name} used {Name}!";

            int markiplier = targets[0].BattleStation.X < caster.BattleStation.X ? -1 : 1;

            Sequence sequence = new Sequence(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(20));
            foreach (var target in targets)
            {
                sequence.Add(new ApplyEffectSequence(sequence, target, new Fear(), 95, true));
            }
            sequence.Add(new DelaySequence(20));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            battle.sequences.Add(sequence);
        }
    }
}

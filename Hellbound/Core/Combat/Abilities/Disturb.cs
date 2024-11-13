using HellTrail.Core.Combat.Sequencer;
using HellTrail.Core.Combat.Status.Debuffs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Abilities
{
    public class Disturb : Ability
    {
        public Disturb() : base("Disturb", "High chance of Fear to 1 foe")
        {
            spCost = 3;
            aoe = false;
            canTarget = ValidTargets.Enemy;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            base.UseAbility(caster, battle, targets);

            if (targets[0].name == "Peas")
            {
                Sequence sequence = new Sequence(battle);
                sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
                sequence.Add(new SetActorAnimation(caster, "Special"));
                sequence.Add(new DelaySequence(60));
                sequence.Add(new PlaySoundSequence("CutIn", 1));
                sequence.Add(new PlaySoundSequence("Exodia", 1));
                sequence.Add(new PlaySoundSequence("FinalFlash", 1));
                sequence.Add(new DelaySequence(90));
                foreach(Unit unit in battle.units.Where(x=>x.team == Team.Enemy))
                {
                    sequence.Delay(10);
                    sequence.DoDamage(caster, unit, 99999, ElementalType.Almighty);
                }
                battle.sequences.Add(sequence);
            } 
            else
            {
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
}

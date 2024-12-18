using Casull.Core.Combat.Sequencer;
using Casull.Core.Combat.Status.Debuffs;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Abilities
{
    public class Disturb : Ability
    {
        public Disturb() : base("Disturb", "Chance of Fear to 1 foe")
        {
            spCost = 3;
            aoe = false;
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.DoT;
            accuracy = 40;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            base.UseAbility(caster, battle, targets);

            if (targets[0].name == "Peas" && name == "Disturb") {
                Sequence sequence = CreateSequence(battle);
                sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
                sequence.Add(new SetActorAnimation(caster, "Special"));
                sequence.Add(new DelaySequence(60));
                sequence.Add(new PlaySoundSequence("CutIn", 1));
                sequence.Add(new PlaySoundSequence("Exodia", 1));
                sequence.Add(new PlaySoundSequence("FinalFlash", 1));
                sequence.Add(new DelaySequence(90));
                foreach (Unit unit in targets) {
                    sequence.Delay(10);
                    sequence.DoDamage(caster, unit, 99999, ElementalType.Almighty, 600);
                }
            }
            else {
                Sequence sequence = CreateSequence(battle);
                sequence.Add(new SetActorAnimation(caster, "Cast"));
                sequence.Add(new DelaySequence(20));
                foreach (var target in targets) {
                    sequence.Add(new ApplyEffectSequence(sequence, target, new Fear(), accuracy, true) {
                        showMiss = true
                    });
                }
                sequence.Add(new DelaySequence(20));
                sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            }
        }
    }
}

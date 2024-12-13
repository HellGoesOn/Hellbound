using Casull.Core.Combat.Sequencer;

namespace Casull.Core.Combat.Abilities
{
    public class Dia : Ability
    {
        public Dia() : base("Dia", "Light heal to 1 ally")
        {
            aoe = false;
            canTarget = ValidTargets.Ally;
            elementalType = ElementalType.Healing;
            spCost = 3;
            canUseOutOfCombat = true;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            base.UseAbility(caster, battle, targets);
            Sequence sequence = CreateSequence(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(60));
            sequence.Add(new HealTargetSqequence(targets[0], 30));
            sequence.Add(new DelaySequence(60));
        }
    }
}

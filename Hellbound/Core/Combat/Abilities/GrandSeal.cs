using Casull.Core.Combat.Sequencer;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Abilities
{
    public class GrandSeal : Ability
    {
        public GrandSeal() : base("The Grand Seal", "Causes unbelievable consequences")
        {
            canTarget = ValidTargets.Ally;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            Sequence sequence = CreateSequence(battle);
            sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
            sequence.Add(new SetActorAnimation(caster, "Special"));
            sequence.Add(new DelaySequence(60));
            sequence.Add(new PlaySoundSequence("CutIn", 1));
            sequence.Add(new PlaySoundSequence("Exodia", 1));
            sequence.Add(new PlaySoundSequence("FinalFlash", 1));
            sequence.Add(new DelaySequence(90));
            foreach (Unit target in targets) {
                sequence.Add(new DelaySequence(4));
                sequence.Add(new DoDamageSequence(caster, target, 999, ElementalType.Almighty));
                sequence.Add(new PlaySoundSequence("Death"));
            }
            sequence.Add(new DelaySequence(60));

        }
    }
}

using Casull.Core.Combat.Sequencer;
using Casull.Core.Combat.Status;
using Casull.Core.UI;

namespace Casull.Core.Combat.Abilities
{
    public class Sukunda : Ability
    {
        public Sukunda() : base("Sukunda", "Reduces accuracy of 1 Foe")
        {
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.Support;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            Sequence sequence = CreateSequence(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Delay(60);
            sequence.CustomAction(() => {
                //SoundEngine.PlaySound("SolarFlare");
                SoundEngine.PlaySound("Flash");

                if (targets[0].HasStatus<SukukajaBuff>()) {
                    targets[0].RemoveAllEffects<SukukajaBuff>();
                    sequence.Delay(60);
                }
                else {
                    sequence.AddStatusEffect(targets[0], new SukundaDebuff(), 600, canExtend: true);
                    sequence.Delay(60);
                }
            });
        }
    }
}

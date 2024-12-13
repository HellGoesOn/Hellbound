using Casull.Core.Combat.Sequencer;
using Casull.Core.Combat.Status;
using Casull.Core.UI;

namespace Casull.Core.Combat.Abilities
{
    public class Masukunda : Ability
    {
        public Masukunda() : base("Masukunda", "Reduces accuracy of all foes")
        {
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.Support;
            aoe = true;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            Sequence sequence = CreateSequence(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Delay(60);
            sequence.CustomAction(() => {
                foreach (var target in targets) {
                    if (target.HasStatus<SukukajaBuff>()) {
                        UIManager.combatUI.SetAbilityUsed("Accuracy & Evasion back to normal");
                        target.RemoveAllEffects<SukukajaBuff>();
                    }
                    else {
                        UIManager.combatUI.SetAbilityUsed("Accuracy & Evasion decreased!");
                        sequence.AddStatusEffect(target, new SukundaDebuff(), 600, canExtend: true);
                    }
                }
                sequence.Delay(60);
            });
        }
    }
}

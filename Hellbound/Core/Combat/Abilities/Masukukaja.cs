using Casull.Core.Combat.Sequencer;
using Casull.Core.Combat.Status;
using Casull.Core.UI;

namespace Casull.Core.Combat.Abilities
{
    public class Masukukaja : Ability
    {
        public Masukukaja() : base("Masukukaja", "Increases agility of all allies")
        {
            canTarget = ValidTargets.Ally;
            elementalType = ElementalType.Support;
            aoe = true;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            Sequence sequence = CreateSequence(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Delay(60);
            sequence.CustomAction(() => {
                foreach (Unit target in targets) {
                    if (target.HasStatus<SukundaDebuff>()) {
                        UIManager.combatUI.SetAbilityUsed("Accuracy & Evasion back to normal");
                        target.RemoveAllEffects<SukundaDebuff>();
                    }
                    else {
                        UIManager.combatUI.SetAbilityUsed("Accuracy & Evasion increased!");
                        sequence.AddStatusEffect(target, new SukukajaBuff(), 600, canExtend: true);
                    }
                }
                sequence.Delay(60);
            });
        }

    }
}


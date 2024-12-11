using HellTrail.Core.Combat.Sequencer;
using HellTrail.Core.Combat.Status;
using HellTrail.Core.Combat.Status.Debuffs;
using HellTrail.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Abilities
{
    public class Sukukaja : Ability
    {
        public Sukukaja() : base("Sukukaja", "Increases accuracy of 1 ally")
        {
            canTarget = ValidTargets.Ally;
            elementalType = ElementalType.Support;
            spCost = 3;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            Sequence sequence = CreateSequence(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Delay(60);
            sequence.CustomAction(() =>
            {
                if (targets[0].HasStatus<SukundaDebuff>())
                {
                    UIManager.combatUI.SetAbilityUsed("Accuracy & Evasion back to normal");
                    targets[0].RemoveAllEffects<SukundaDebuff>();
                    sequence.Delay(60);
                }
                else
                {
                    UIManager.combatUI.SetAbilityUsed("Accuracy & Evasion increased!");
                    sequence.AddStatusEffect(targets[0], new SukukajaBuff(), 600, canExtend: true);
                    sequence.Delay(60);
                }
            });
        }

    }
}

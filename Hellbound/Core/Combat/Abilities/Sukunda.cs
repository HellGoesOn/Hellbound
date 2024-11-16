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
    public class Sukunda : Ability
    {
        public Sukunda() : base("Sukunda", "Reduces accuracy of 1 Foe")
        {
            canTarget = ValidTargets.Enemy;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            Sequence sequence = battle.CreateSequence();
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Delay(60);
            sequence.CustomAction(() =>
                {
                if (targets[0].HasStatus<SukukajaBuff>())
                {
                    UIManager.combatUI.SetAbilityUsed("Accuracy & Evasion back to normal");
                    targets[0].RemoveAllEffects<SukukajaBuff>();
                        sequence.Delay(60);
                    } else
                    {
                        UIManager.combatUI.SetAbilityUsed("Accuracy & Evasion decreased!");
                        sequence.AddStatusEffect(targets[0], new SukundaDebuff(), 600, canExtend: true);
                        sequence.Delay(60);
                    }
                });
        }
    }
}

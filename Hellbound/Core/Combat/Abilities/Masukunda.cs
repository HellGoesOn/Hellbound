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
    public class Masukunda : Ability
    {
        public Masukunda() : base("Masukunda", "Reduces accuracy of all foes")
        {
            canTarget = ValidTargets.Enemy;
            aoe = true;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            Sequence sequence = CreateSequence(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Delay(60);
            sequence.CustomAction(() =>
            {
                foreach (var target in targets)
                {
                    if (target.HasStatus<SukukajaBuff>())
                    {
                        UIManager.combatUI.SetAbilityUsed("Accuracy & Evasion back to normal");
                        target.RemoveAllEffects<SukukajaBuff>();
                    } else
                    {
                        UIManager.combatUI.SetAbilityUsed("Accuracy & Evasion decreased!");
                        sequence.AddStatusEffect(target, new SukundaDebuff(), 600, canExtend: true);
                    }
                }
                sequence.Delay(60);
            });
        }
    }
}

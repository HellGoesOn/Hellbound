using Casull.Core.Combat.Status;
using Casull.Core.UI;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Sequencer
{
    public class ApplyEffectSequence(Sequence sequence, Unit target, StatusEffect effect, int chance = 100, bool requiresDamageDealt = false, bool canStack = false, bool canExtend = false) : ISequenceAction
    {
        public int chance = chance;
        public bool requiresDamageDealt = requiresDamageDealt;
        public bool canStack = canStack;
        public bool canExtendStack = canExtend;
        public Unit target = target;
        public Sequence mySequence = sequence;
        public StatusEffect effect = effect;

        public bool showMiss;

        public bool IsFinished()
        {
            return true;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            if(battle == null) return;  

            if (requiresDamageDealt) {
                int myIndex = mySequence.Actions.IndexOf(this);
                if (myIndex >= 1 && mySequence.Actions[myIndex - 1] is DoDamageSequence test && !test.dealtDamage) {
                    return;
                }
            }

            if (battle.rand.Next(101) <= chance) {
                if (effect.debuff && target.HasStatus<GuardingEffect>()) {

                    battle.damageNumbers.Add(new(DamageType.Normal, $"BLOCKED", (target.position - new Vector2(0, 12))));
                    return;
                }

                if (canExtendStack && target.HasStatus(effect.name)) {
                    UIManager.combatUI.SetAbilityUsed($"{effect.name} duration extended by {effect.turnsLeft} turns!");
                    target.ExtendEffect(effect);
                }
                else {


                    if (canStack)
                        target.AddEffect(effect);
                    else
                        target.AddReplaceEffect(effect);
                }
                DamageNumber damageNumber = new(DamageType.Normal, $"+{effect.name}", (target.position - new Vector2(0, 12)));
                battle.damageNumbers.Add(damageNumber);
            }
            else if (showMiss) {
                battle.damageNumbers.Add(new(DamageType.Normal, $"MISS", (target.position - new Vector2(0, 12))));
            }
        }
    }
}

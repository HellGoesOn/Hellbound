using HellTrail.Core.Combat.Status;
using Microsoft.Xna.Framework;

namespace HellTrail.Core.Combat.Sequencer
{
    public class ApplyEffectSequence(Sequence sequence, Unit target, StatusEffect effect, int chance = 100, bool requiresDamageDealt = false, bool canStack = false) : ISequenceAction
    {
        public int chance = chance;
        public bool requiresDamageDealt = requiresDamageDealt;
        public bool canStack = canStack;
        public Unit target = target;
        public Sequence mySequence = sequence;
        public StatusEffect effect = effect;

        public bool IsFinished()
        {
            return true;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            if (requiresDamageDealt)
            {
                int myIndex = mySequence.Actions.IndexOf(this);
                if (myIndex >= 1 && mySequence.Actions[myIndex - 1] is DoDamageSequence test && !test.dealtDamage)
                {
                    return;
                }
            }

            if (battle.rand.Next(101) <= chance)
            {
                if(canStack)
                    target.AddEffect(effect);
                else
                    target.AddReplaceEffect(effect);

                DamageNumber damageNumber = new(DamageType.Normal, $"+{effect.name}", (target.position + new Vector2(0, 12)) * 4);
                battle.damageNumbers.Add(damageNumber);
            }
        }
    }
}

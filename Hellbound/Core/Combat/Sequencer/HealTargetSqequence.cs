using Microsoft.Xna.Framework;

namespace HellTrail.Core.Combat.Sequencer
{
    public class HealTargetSqequence(Unit target, int amount) : ISequenceAction
    {
        public Unit target = target;
        public int amount = amount;
        public bool IsFinished()
        {
            return true;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            target.HP = Math.Min(target.HP + amount, target.MaxHP);
            DamageNumber damageNumber = new(DamageType.Normal, $"+{amount}", target.position * 4);
            damageNumber.color = Color.Lime;
            battle.damageNumbers.Add(damageNumber);
        }
    }
}

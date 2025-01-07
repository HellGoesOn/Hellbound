using Casull.Core.Combat.Sequencer;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Abilities
{
    public class BasicAttack : Ability
    {
        public BasicAttack() : base("Basic Attack", "Light Phys damage to 1 foe.")
        {
            baseDamage = 15;
            aoe = false;
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.Phys;
            accuracy = 100;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            int markiplier = targets[0].BattleStation.X < caster.BattleStation.X ? -1 : 1;

            Sequence sequence = CreateSequence(battle);
            sequence.Add(new MoveActorSequence(caster, targets[0].position - new Vector2(32 * markiplier, 0)));
            sequence.Add(new SetActorAnimation(caster, "BasicAttack"));
            sequence.Add(new DelaySequence(30));
            sequence.Add(new PlaySoundSequence("GunShot"));
            sequence.Add(new DoDamageSequence(caster, targets[0], baseDamage, elementalType, accuracy));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
        }
    }
}

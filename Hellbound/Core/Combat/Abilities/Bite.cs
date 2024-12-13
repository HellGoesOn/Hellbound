using Casull.Core.Combat.Sequencer;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Abilities
{
    public class Bite : Ability
    {
        public Bite() : base("Bite", "Light Phys damage to 1 foe.")
        {
            hpCost = 99;
            aoe = false;
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.Phys;
        }

        public override void AdjustCosts(Unit caster)
        {
            hpCost = (int)(caster.Stats.MaxHP * 0.08f);
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            int markiplier = targets[0].BattleStation.X < caster.BattleStation.X ? -1 : 1;

            Sequence sequence = CreateSequence(battle);
            sequence.Add(new MoveActorSequence(caster, targets[0].position - new Vector2(32 * markiplier, 0)));
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new PlaySoundSequence("GunShot"));
            sequence.Add(new DoDamageSequence(caster, targets[0], 25, elementalType, accuracy));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
        }
    }
}

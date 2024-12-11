using HellTrail.Core.Combat.Sequencer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Abilities
{
    public class BasicAttack : Ability
    {
        public int baseDamage;

        public BasicAttack() : base("Basic Attack", "Light Phys damage to 1 foe.")
        {
            baseDamage = 25;
            aoe = false;
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.Phys;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            int markiplier = targets[0].BattleStation.X < caster.BattleStation.X ? -1 : 1;

            Sequence sequence = CreateSequence(battle);
            sequence.Add(new MoveActorSequence(caster, targets[0].position - new Vector2(32 * markiplier, 0)));
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new PlaySoundSequence("GunShot"));
            sequence.Add(new DoDamageSequence(caster, targets[0], baseDamage));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
        }
    }
}

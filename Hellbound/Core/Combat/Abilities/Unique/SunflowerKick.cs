using Casull.Core.Combat.Sequencer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.Combat.Abilities.Unique
{
    public class SunflowerKick : Ability
    {
        public SunflowerKick() : base("Sunflower Kick", "Light Phys damage to 1 foe.")
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
            sequence.Add(new MoveActorSequence(caster, targets[0].position - new Vector2(54 * markiplier, 0)));
            sequence.Add(new SetActorAnimation(caster, "Sunflower_Kick"));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new PlaySoundSequence("GunShot"));
            sequence.Add(new DoDamageSequence(caster, targets[0], baseDamage, elementalType, accuracy));
            sequence.Add(new DelaySequence(60));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
        }
    }
}

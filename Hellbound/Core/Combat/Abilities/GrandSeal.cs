using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Abilities
{
    public class GrandSeal : Ability
    {
        public GrandSeal() : base("The Grand Seal", "I BRING YOU: MEGIDOLAON")
        {
            canTarget = ValidTargets.Ally;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            Sequence sequence = new Sequence(battle);
            sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(60));
            foreach (Unit target in targets)
            {
                sequence.Add(new DelaySequence(4));
                sequence.Add(new DoDamageSequence(caster, target, 200));
                sequence.Add(new PlaySoundSequence("Death"));
            }
            sequence.Add(new DelaySequence(60));
            battle.sequences.Add(sequence);

        }
    }
}

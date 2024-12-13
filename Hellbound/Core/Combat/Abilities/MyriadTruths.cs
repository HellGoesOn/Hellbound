using Casull.Core.Combat.Sequencer;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Abilities
{
    public class MyriadTruths : Ability
    {
        public MyriadTruths() : base("Myriad Truths", "3x Heavy Damage to All")
        {
            spCost = 40;
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.Almighty;
            aoe = true;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            Sequence sequence = CreateSequence(battle);
            sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(60));
            sequence.Add(new OneActionSequence(() => {
                foreach (Unit target in targets) {
                    Sequence subSeq = new(battle) {
                        active = true
                    };
                    subSeq.Add(new DoDamageSequence(caster, target, 25, ElementalType.Almighty));
                    subSeq.Add(new DelaySequence(25));
                    subSeq.Add(new DoDamageSequence(caster, target, 25, ElementalType.Almighty));
                    subSeq.Add(new DelaySequence(25));
                    subSeq.Add(new DoDamageSequence(caster, target, 25, ElementalType.Almighty));
                    battle.sequences.Add(subSeq);
                }

            }));
            sequence.Add(new DelaySequence(75));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));

        }
    }
}

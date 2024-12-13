using Casull.Core.Combat.Sequencer;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Abilities
{
    public class Megidolaon : Ability
    {
        public Megidolaon() : base("Super Bite", "I BRING YOU: MEGIDOLAON")
        {
            hpCost = 20;
            aoe = true;
            canTarget = ValidTargets.Enemy;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            Sequence sequence = CreateSequence(battle);
            sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
            sequence.Add(new PlaySoundSequence("WhatDaDogDoin"));
            sequence.Add(new DelaySequence(60));
            foreach (Unit target in targets) {
                var anim = new SpriteAnimation("TestAnimation",
                    [
                    new FrameData(0, 128, 32, 32),
                    new FrameData(0, 96, 32, 32),
                    new FrameData(0, 64, 32, 32),
                    new FrameData(0, 32, 32, 32),
                    new FrameData(0, 0, 32, 32),
                    ]);
                anim.timePerFrame = 3;
                anim.position = target.position;
                anim.color = Color.Crimson;
                sequence.Add(new AddAnimationSequence(anim));
                sequence.Add(new DelaySequence(10));
                sequence.Add(new PlaySoundSequence("GunShot"));
            }
            sequence.Add(new DelaySequence(20));
            foreach (Unit target in targets)
                sequence.Add(new DoDamageSequence(caster, target, 25));
            sequence.Add(new DelaySequence(60));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));

        }
    }
}

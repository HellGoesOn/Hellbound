using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public class Ability
    {
        public string Name;
        public string Description;
        public bool aoe;
        public ValidTargets canTarget;

        public Ability(string name, string description)
        {
            aoe = false;
            Name = name;
            Description = description;
            canTarget = ValidTargets.Active;
        }

        public virtual void Use(Unit caster, Battle battle, List<Unit> targets)
        {

        }
    }

    public class Bite : Ability
    {
        public Bite() : base("Bite", "Damage")
        {
            aoe = false;
            canTarget = ValidTargets.Enemy;
        }

        public override void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            int markiplier = targets[0].BattleStation.X < caster.BattleStation.X ? -1 : 1;

            Sequence sequence = new Sequence(battle);
            sequence.Add(new MoveActorSequence(caster, targets[0].position - new Vector2(32 * markiplier, 0)));
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new PlaySoundSequence("GunShot"));
            sequence.Add(new DoDamageSequence(targets[0], 25));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            battle.sequences.Add(sequence);
        }
    }

    public class Megidolaon : Ability
    {
        public Megidolaon() : base("Super Bite", "I BRING YOU: MEGIDOLAON")
        {
            aoe = true;
            canTarget = ValidTargets.Enemy;
        }

        public override void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            Sequence sequence = new Sequence(battle);
            sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
            sequence.Add(new PlaySoundSequence("WhatDaDogDoin"));
            sequence.Add(new DelaySequence(60));
            foreach (Unit target in targets)
            {
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
            foreach(Unit target in targets)
                sequence.Add(new DoDamageSequence(target, 25));
            sequence.Add(new DelaySequence(60));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            battle.sequences.Add(sequence);

        }
    }

    public class Singularity : Ability
    {
        public Singularity() : base("Singularity", "???")
        {
            canTarget = ValidTargets.AllButSelf;
            aoe = true;
        }

        public override void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            var anim = new SpriteAnimation("TestAnimation",
                [
                    new FrameData(0, 0, 32, 32, new Vector2(2)),
                    new FrameData(0, 32, 32, 32, new Vector2(2)),
                    new FrameData(0, 64, 32, 32, new Vector2(2)),
                    new FrameData(0, 64, 32, 32, new Vector2(2)),
                    new FrameData(0, 96, 32, 32, new Vector2(2)),
                    new FrameData(0, 128, 32, 32, new Vector2(2)),
                    ]);
            anim.timePerFrame = 90;
            anim.position = caster.position;
            anim.color = Color.White;
            anim.rotation = -0.001f;
            anim.onAnimationPlay += () =>
            {
                anim.rotation *= 1.03f;
                anim.position += (new Vector2(160, 90) - anim.position) * 0.01f;
                CameraManager.GetCamera.centre = new Vector2(160 + (float)Math.Sin(Main.totalTime * 2 * anim.rotation), 90 + (float)Math.Cos(Main.totalTime * 2 * anim.rotation));
            };
            Color oldBgColor = battle.bg.color;
            var whiteOut = new SpriteAnimation("Canvas",
                [
                    new FrameData(0, 0, GameOptions.ScreenWidth, GameOptions.ScreenHeight),
                    ]);
            whiteOut.timePerFrame = 460;
            whiteOut.color = Color.White;
            whiteOut.opacity = -1.0f;
            whiteOut.onAnimationPlay += () =>
            {
                caster.opacity *= 0.965f;
                battle.bg.color = Color.Lerp(battle.bg.color, Color.Black, 0.017f);
                whiteOut.opacity += 0.0085f;
            };
            whiteOut.onAnimationEnd += () =>
                {
                    caster.opacity = 1.0f;
                    battle.bg.color = oldBgColor;
                    CameraManager.GetCamera.centre = new Vector2(160, 90);
            };


            Sequence seq = new Sequence(battle);
            seq.Add(new SetActorAnimation(caster, "Cast"));
            seq.Add(new AddAnimationSequence(anim));
            seq.Add(new DelaySequence(180));
            seq.Add(new AddAnimationSequence(whiteOut));
            seq.Add(new OneActionSequence(() =>
            {
                foreach (var target in targets)
                {
                    Sequence subSeq = new(battle)
                    {
                        active = true
                    };
                    subSeq.Add(new MoveActorSequence(target, new Vector2(160, 90), 0.0085f));
                    subSeq.Add(new PlaySoundSequence("Death", 0.25f));
                    subSeq.Add(new MoveActorSequence(target, target.BattleStation, 1f));
                    subSeq.Add(new DoDamageSequence(target, 9999));
                    battle.sequences.Add(subSeq);
                }
            }));
            seq.Add(new DelaySequence(30));

            battle.sequences.Add(seq);
        }
    }

    public class GrandSeal : Ability
    {
        public GrandSeal() : base("The Grand Seal", "I BRING YOU: MEGIDOLAON")
        {
            canTarget = ValidTargets.Ally;
        }

        public override void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            Sequence sequence = new Sequence(battle);
            sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(60));
            foreach (Unit target in targets)
            {
                sequence.Add(new DelaySequence(4));
                sequence.Add(new DoDamageSequence(target, 200));
                sequence.Add(new PlaySoundSequence("Death"));
            }
            sequence.Add(new DelaySequence(60));
            battle.sequences.Add(sequence);

        }
    }

    public class Agi : Ability
    {
        public Agi() : base("Agi", "Light Fire damage to 1 foe")
        {
            aoe = false;
            canTarget = ValidTargets.Enemy;
        }

        public override void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            int markiplier = targets[0].BattleStation.X < caster.BattleStation.X ? -1 : 1;

            Sequence sequence = new Sequence(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new PlaySoundSequence("GunShot"));
            sequence.Add(new DoDamageSequence(targets[0], 25, ElementalType.Fire));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            battle.sequences.Add(sequence);
        }
    }

    public class MyriadTruths : Ability
    {
        public MyriadTruths() : base("Myriad Truths", "3x Heavy Damage to All")
        {
            canTarget = ValidTargets.Enemy;
            aoe = true;
        }

        public override void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            Sequence sequence = new Sequence(battle);
            sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(60));
            sequence.Add(new OneActionSequence(() =>
            {
                foreach (Unit target in targets)
                {
                    Sequence subSeq = new(battle)
                    {
                        active = true
                    };
                    subSeq.Add(new DoDamageSequence(target, 25, ElementalType.Almighty));
                    subSeq.Add(new DelaySequence(25));
                    subSeq.Add(new DoDamageSequence(target, 25, ElementalType.Almighty));
                    subSeq.Add(new DelaySequence(25));
                    subSeq.Add(new DoDamageSequence(target, 25, ElementalType.Almighty));
                    battle.sequences.Add(subSeq);
                }

            }));
            sequence.Add(new DelaySequence(75));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            battle.sequences.Add(sequence);

        }
    }
}

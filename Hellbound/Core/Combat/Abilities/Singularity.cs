using Casull.Core.Combat.Sequencer;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Abilities
{
    public class Singularity : Ability
    {
        public Singularity() : base("Singularity", "???")
        {
            spCost = 100;
            canTarget = ValidTargets.AllButSelf;
            elementalType = ElementalType.Almighty;
            aoe = true;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {name}!";

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
            anim.onAnimationPlay += (_, f) => {
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
            whiteOut.onAnimationPlay += (_, f) => {
                caster.opacity *= 0.965f;
                battle.bg.color = Color.Lerp(battle.bg.color, Color.Black, 0.017f);
                whiteOut.opacity += 0.0085f;
            };
            whiteOut.onAnimationEnd += (_, f) => {
                caster.opacity = 1.0f;
                battle.bg.color = oldBgColor;
                CameraManager.GetCamera.centre = new Vector2(160, 90);
            };
            whiteOut.depth = 1f;


            Sequence seq = new(battle);
            seq.Add(new PlaySoundSequence("Dies"));
            seq.Add(new DelaySequence(120));
            seq.Add(new SetActorAnimation(caster, "Cast"));
            seq.Add(new AddAnimationSequence(anim));
            seq.Add(new DelaySequence(180));
            seq.Add(new AddAnimationSequence(whiteOut));
            seq.Add(new OneActionSequence(() => {
                foreach (var target in targets) {
                    Sequence subSeq = new(battle) {
                        active = true
                    };
                    subSeq.Add(new MoveActorSequence(target, new Vector2(160, 90), 0.0085f));
                    subSeq.Add(new PlaySoundSequence("Death", 0.25f));
                    subSeq.Add(new MoveActorSequence(target, target.BattleStation, 1f));
                    subSeq.Add(new DoDamageSequence(caster, target, 9999));
                    battle.sequences.Add(subSeq);
                }
            }));
            seq.Add(new DelaySequence(30));

            battle.sequences.Add(seq);
        }
    }
}

using Casull.Core.Combat.Sequencer;
using Casull.Core.Combat.Status.Debuffs;
using Casull.Render;
using Microsoft.Xna.Framework;
using System.Reflection.Metadata.Ecma335;

namespace Casull.Core.Combat.Abilities
{
    public class Disturb : Ability
    {
        public Disturb() : base("Disturb", "Chance of Fear to 1 foe")
        {
            spCost = 3;
            aoe = false;
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.DoT;
            accuracy = 80;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            base.UseAbility(caster, battle, targets);

            if (targets[0].name == "Peas" && name == "Disturb") {
                Sequence sequence = CreateSequence(battle);
                SoundEngine.SetTargetMusicVolume(0);
                SoundEngine.hiddenMusicVolumeSpeed = 0.05f;
                sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
                sequence.Add(new SetActorAnimation(caster, "Special"));
                sequence.Add(new DelaySequence(60));
                sequence.Add(new PlaySoundSequence("CutIn", 1));
                sequence.Add(new PlaySoundSequence("Exodia", 1));
                sequence.Add(new PlaySoundSequence("FinalFlash", 1));
                sequence.Add(new DelaySequence(30));
                var cutInBG = new SpriteAnimation("Pixel", [new(0, 0, 320, 32), new(0, 0, 320, 32), new(0, 0, 320, 32)]); 
                cutInBG.scale = new Vector2(1.5f);
                cutInBG.depth = 1f;
                cutInBG.scale.Y = 0.0f;
                cutInBG.timePerFrame = 60;
                cutInBG.color = Color.Blue; 
                var cutInBG2 = new SpriteAnimation("Pixel", [new(0, 0, 320, 34), new(0, 0, 320, 34), new(0, 0, 320, 34)]);
                cutInBG2.scale = new Vector2(1.5f);
                cutInBG2.depth = 1f;
                cutInBG2.scale.Y = 0.0f;
                cutInBG2.timePerFrame = 60;
                cutInBG2.color = Color.White;
                var cutIn = new SpriteAnimation("MCPortrait_Combat", [new(0, 0, 32, 32), new(0, 0, 32, 32), new(0, 0, 32, 32)]);
                cutIn.scale = new Vector2(1.5f);
                cutIn.depth = 1f;
                cutIn.scale.Y = 0.0f;
                cutIn.timePerFrame = 60;
                cutIn.onAnimationPlay += (sender, unit) => {
                    if (sender.scale.Y < 1.5f && sender.currentFrame == 0) {
                        {
                            cutInBG2.scale.Y += 0.25f;
                            cutInBG.scale.Y += 0.25f;
                            sender.scale.Y += 0.25f;
                        }
                    }
                    else if (sender.scale.Y > 0.0f && sender.currentFrame == 1) {
                        sender.scale.Y -= 0.25f;
                        cutInBG.scale.Y -= 0.25f;
                        cutInBG2.scale.Y -= 0.25f;
                    }
                };
                cutInBG.position = new Vector2(0, 90);
                cutInBG2.position = new Vector2(0, 90);
                cutIn.position = new Vector2(160, 90);
                cutIn.origin = new Vector2(16);
                cutInBG.origin = new Vector2(16);
                cutInBG2.origin = new Vector2(17);
                sequence.AddAnimation(cutInBG2);
                sequence.AddAnimation(cutInBG);
                sequence.AddAnimation(cutIn);

                sequence.Add(new DelaySequence(90));
                sequence.Add(new OneActionSequence(() => {
                    SoundEngine.SetTargetMusicVolume(1);
                }));
                foreach (Unit unit in targets) {
                    sequence.Delay(10);
                    sequence.DoDamage(caster, unit, 99999, ElementalType.Almighty, 600);
                }
            }
            else {
                Sequence sequence = CreateSequence(battle);
                sequence.Add(new SetActorAnimation(caster, "Cast"));
                sequence.Add(new DelaySequence(20));
                foreach (var target in targets) {
                    sequence.Add(new ApplyEffectSequence(sequence, target, new Fear(), accuracy, true) {
                        showMiss = true
                    });
                }
                sequence.Add(new DelaySequence(20));
                sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            }
        }
    }
}

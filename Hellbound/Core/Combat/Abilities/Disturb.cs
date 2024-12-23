using Casull.Core.Combat.Sequencer;
using Casull.Core.Combat.Status.Debuffs;
using Casull.Core.Graphics;
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
        bool once;
        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            base.UseAbility(caster, battle, targets);
            once = false;
            if (targets[0].name == "Peas" && name == "Disturb") {

                var orpheus = new SpriteAnimation("Orpheus", [new(0, 0, 48, 48), new(0, 0, 48, 48), new(0, 0, 48, 48), new(0, 0, 48, 48), new(0, 0, 48, 48), new(0, 0, 48, 48)]);
                orpheus.scale.X = 0f;
                orpheus.timePerFrame = 60;
                orpheus.depth = 0.9f;
                orpheus.position = caster.position;
                orpheus.onAnimationPlay += (sender, unit) => {
                    if(!once) {
                        once = true;

                        orpheus.position = caster.position - new Vector2(16) * caster.scale.X;
                    }
                    orpheus.position.Y += (float)Math.Sin(Main.totalTime) * 0.2f;

                    if (orpheus.scale.X < 1.0f && orpheus.currentFrame < 4)
                        orpheus.scale.X += 0.1f;
                    else if(orpheus.scale.X > 0.1f && orpheus.currentFrame >= 4)
                        orpheus.scale.X -= 0.1f;



                    Color[] clrs = { Color.Blue, Color.Cyan, Color.Turquoise, Color.LightBlue };

                    if(orpheus.currentFrame < 4)
                    for (int i = 0; i < 3; i++) {
                        int xx = Main.rand.Next((int)(orpheus.frameData[0].width * 0.5f));
                        int yy = Main.rand.Next((int)(orpheus.frameData[0].height));
                        float velX = Main.rand.Next(60, 120) * 0.001f * (Main.rand.Next(2) == 0 ? -1 : 1);
                        float velY = -0.2f * (Main.rand.Next(20, 60) * 0.05f);
                        var particle = ParticleManager.NewParticleAdditive(new Vector3(orpheus.position + new Vector2(-orpheus.frameData[0].width * 0.25f + xx, orpheus.frameData[0].height * 0.5f), 0), new Vector3(velX, 0, velY), 60);

                        particle.color = clrs[Main.rand.Next(clrs.Length)];
                        particle.endColor = Color.Black;
                        particle.degradeSpeed = 0.01f;
                        particle.dissapateSpeed = 0.01f;
                        particle.scale = Vector2.One * Main.rand.Next(1, 3);
                    }

                };
                orpheus.origin = new Vector2(24);
                Sequence sequence = CreateSequence(battle);
                SoundEngine.SetTargetMusicVolume(0);
                SoundEngine.hiddenMusicVolumeSpeed = 0.05f;
                sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
                sequence.Add(new SetActorAnimation(caster, "Special"));
                sequence.Add(new DelaySequence(60));
                sequence.Add(new PlaySoundSequence("CutIn", 1));
                sequence.Add(new PlaySoundSequence("Exodia", 1));
                sequence.Add(new PlaySoundSequence("FinalFlash", 1));
                var cutInBG = new SpriteAnimation("Pixel", [new(0, 0, 320, 32), new(0, 0, 320, 32), new(0, 0, 320, 32)]); 
                cutInBG.scale = new Vector2(1.5f);
                cutInBG.depth = 0.9f;
                cutInBG.scale.Y = 0.0f;
                cutInBG.timePerFrame = 60;
                cutInBG.color = Color.Blue; 
                var cutInBG2 = new SpriteAnimation("Pixel", [new(0, 0, 320, 34), new(0, 0, 320, 34), new(0, 0, 320, 34)]);
                cutInBG2.scale = new Vector2(1.5f);
                cutInBG2.depth = 0.9f;
                cutInBG2.scale.Y = 0.0f;
                cutInBG2.timePerFrame = 60;
                cutInBG2.color = Color.White;
                var cutIn = new SpriteAnimation("MCPortrait_Combat", [new(0, 0, 32, 32), new(0, 0, 32, 32), new(0, 0, 32, 32)]);
                cutIn.scale = new Vector2(1.5f);
                cutIn.depth = 0.9f;
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

                cutInBG.position = new Vector2(0, 70);
                cutInBG2.position = new Vector2(0, 70);
                cutIn.position = new Vector2(160, 70);
                cutIn.origin = new Vector2(16);
                cutInBG.origin = new Vector2(16);
                cutInBG2.origin = new Vector2(17);
                sequence.AddAnimation(cutInBG2);
                sequence.AddAnimation(cutInBG);
                sequence.AddAnimation(cutIn);

                sequence.Add(new DelaySequence(60));
                sequence.AddAnimation(orpheus);
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

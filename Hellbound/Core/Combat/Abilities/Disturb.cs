using Casull.Core.Combat.Sequencer;
using Casull.Core.Combat.Status.Debuffs;
using Casull.Core.Graphics;
using Microsoft.Xna.Framework;

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

                var whiteBeam = new SpriteAnimation("Beam", [new(0, 0, 48, 600), new(0, 0, 48, 600), new(0, 0, 48, 600), new(0, 0, 48, 600), new(0, 0, 48, 600), new(0, 0, 48, 600)]);
                whiteBeam.scale.X = 0f;
                whiteBeam.scale.Y = 0f;
                whiteBeam.timePerFrame = 90;
                whiteBeam.depth = targets[0].depth - 0.00001f;
                whiteBeam.position = targets[0].position;
                whiteBeam.origin = new Vector2(24, 600-16);

                var orpheus = new SpriteAnimation("Orpheus", [new(0, 0, 48, 48), new(0, 0, 48, 48), new(0, 0, 48, 48), new(0, 0, 48, 48), new(0, 0, 48, 48), new(0, 0, 48, 48)]);
                orpheus.scale.X = 0f;
                orpheus.timePerFrame = 90;
                orpheus.depth = 0.9f;
                orpheus.position = caster.position;
                orpheus.onAnimationPlay += (sender, unit) => {
                    if(!once) {
                        once = true;

                        orpheus.position = caster.position - new Vector2(16) * caster.scale.X;
                    }
                    orpheus.position.Y += (float)Math.Sin(Main.totalTime) * 0.2f;

                    if (orpheus.scale.X < 1.0f && orpheus.currentFrame < 4) {
                        orpheus.scale.X += 0.1f;
                    }
                    else if (orpheus.scale.X > 0.1f && orpheus.currentFrame >= 4) {
                        orpheus.scale.X -= 0.1f;
                    }

                    targets[0].shake = 0.32f;
                    targets[0].color = Color.Lerp(targets[0].color, Color.Black, 0.072f);

                    Color[] clrs = { Color.Blue, Color.Cyan, Color.Turquoise, Color.LightBlue };

                    if (orpheus.currentFrame < 4) {
                        for (int i = 0; i < 3; i++) {
                            int xx = Main.rand.Next((int)(orpheus.frameData[0].width * 0.5f * orpheus.scale.X));
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
                    }

                    //whiteBeam.color = Color.Cyan * (0.8f + (float)Math.Abs(Math.Sin(Main.totalTime) *0.05f));
                    if (whiteBeam.currentFrame >= 3)
                        whiteBeam.scale.X = orpheus.scale.X;
                    else
                        whiteBeam.scale.X = whiteBeam.scale.Y = orpheus.scale.X;

                    if(whiteBeam.scale.X > 0.1f)
                        foreach (var target in targets) {
                            for (int i = 0; i < 10; i++) {
                                var position = new Vector3(target.position + new Vector2(0, 16), 0);
                                var xx = Main.rand.Next(-24, (25));
                                var xxx = Main.rand.Next(-10, 11) * 0.01f;
                                var zz = Main.rand.NextSingle() * -Main.rand.Next(150, 350) * 0.01f;
                                var velocity = new Vector3(0, 0, zz);
                                var part = ParticleManager.NewParticleAdditive(position + new Vector3(xx, 0, 0), velocity, 300, 0.00f, false);
                                if (part != null) {
                                    part.diesToGravity = false;
                                    part.color = Color.Lerp(Color.White, Color.Cyan, Main.rand.NextSingle());
                                    part.scale = Vector2.One * Main.rand.Next(1, 4);
                                }
                            }
                        }

                };
                orpheus.origin = new Vector2(24);
                Sequence sequence = CreateSequence(battle);
                SoundEngine.SetTargetMusicVolume(0);
                SoundEngine.hiddenMusicVolumeSpeed = 0.05f;
                sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
                sequence.Add(new SetActorAnimation(caster, "Special"));
                sequence.Add(new DelaySequence(60));
                sequence.Add(new PlaySoundSequence("CutIn", 1, 0));
                sequence.Add(new PlaySoundSequence("Exodia", 0.5f, 0));
                sequence.Add(new PlaySoundSequence("FinalFlash", 0.5f, 0));
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
                sequence.AddAnimation(whiteBeam);

                sequence.Add(new DelaySequence(60));
                sequence.AddAnimation(orpheus);
                sequence.Add(new DelaySequence(90));
                sequence.Add(new OneActionSequence(() => {
                    SoundEngine.SetTargetMusicVolume(1);
                }));
                foreach (Unit unit in targets) {
                    sequence.Delay(10);
                    sequence.DoDamage(caster, unit, 99999, ElementalType.DoT, 600);
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

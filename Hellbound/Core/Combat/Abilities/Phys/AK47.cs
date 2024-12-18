using Casull.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casull.Core.Graphics;

namespace Casull.Core.Combat.Abilities.Phys
{
    public class AK47 : Ability
    {
        public AK47() : base("AK-47", "Severe Phys damage to one enemy")
        {
            aoe = false;
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.Phys;
            baseDamage = 47;
        }

        int timer;

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            base.UseAbility(caster, battle, targets);

            var seq = CreateSequence(battle);
            var ak = new SpriteAnimation("AK47", [new(0, 0, 66, 24), new(0, 0, 66, 24), new(0, 0, 66, 24)]);
            ak.scale.X = caster.scale.X;
            ak.scale *= 0.5f;
            ak.position = caster.position + new Vector2(24 * ak.scale.X, 0);
            ak.timePerFrame = 40;
            ak.depth = 1f;

            ak.onAnimationPlay = (sender, unit) => {
                if (timer % 10 == 0) {
                    float offX = 36;
                    float offY = 2;
                    for(int i = 0; i < 10; i++) {
                        var xx = Main.rand.Next(32, 48) * 0.1f;
                        var yy = Main.rand.Next(0, 30) * 0.1f;
                        var particle = ParticleManager.NewParticle(new Vector3(ak.position + new Vector2(offX * ak.scale.X, -offY), 0), new Vector3(new Vector2(xx * ak.scale.X, yy), 0), 3, 0);
                        particle.diesToGravity = false;
                        particle.color = Color.Lerp(Color.Yellow, Color.OrangeRed, Main.rand.NextSingle());
                        particle.scale = Vector2.One * Main.rand.Next(1, 3);
                    }
                    for (int i = 0; i < 10; i++) {
                        var xx = Main.rand.Next(32, 48) * 0.1f;
                        var yy = Main.rand.Next(-30, 0) * 0.1f;
                        var particle = ParticleManager.NewParticle(new Vector3(ak.position + new Vector2(offX * ak.scale.X, -offY), 0), new Vector3(new Vector2(xx * ak.scale.X, yy), 0), 3, 0);
                        particle.diesToGravity = false;
                        particle.color = Color.Lerp(Color.Yellow, Color.OrangeRed, Main.rand.NextSingle());
                        particle.scale = Vector2.One * Main.rand.Next(1, 3);
                    }
                    for (int i = 0; i < 10; i++) {
                        var xx = Main.rand.Next(60, 80) * 0.1f;
                        var yy = Main.rand.Next(-80, 80) * 0.01f;
                        var particle = ParticleManager.NewParticle(new Vector3(ak.position + new Vector2(offX * ak.scale.X, -offY), 0), new Vector3(new Vector2(xx * ak.scale.X, yy), 0), Main.rand.Next(2, 8), 0);
                        particle.diesToGravity = false;
                        particle.color = Color.Lerp(Color.Yellow, Color.OrangeRed, Main.rand.NextSingle());
                        particle.scale = Vector2.One * Main.rand.Next(1, 3);
                    }

                    var randoffX = Main.rand.Next(-(int)targets[0].size.X * 2, (int)targets[0].size.X*2+1);
                    var randoffY = Main.rand.Next(-(int)targets[0].size.Y * 2, (int)targets[0].size.Y * 2 + 1);

                    var dist = (targets[0].position+new Vector2(randoffX, randoffY) - caster.position).Length();
                    var vel = (targets[0].position - caster.position).SafeNormalize() * dist * 0.5f;
                    var part = ParticleManager.NewParticle(new Vector3(ak.position + vel + new Vector2(offX * ak.scale.X, -offY), 0), new Vector3(0, 0, 0), 
                        1, 0);
                    part.diesToGravity = false;
                    part.color = Color.Yellow;
                    part.scale = new Vector2(dist, 1);
                    part.rotation = (targets[0].position - caster.position).ToRotation();
                    ak.rotation = 0.36f * -ak.scale.X;

                    SoundEngine.PlaySound("AKShot", 0.5f);
                }

                ak.rotation = MathHelper.Lerp(ak.rotation, 0.0f, 0.12f);

                timer++;
            };

            seq.AddAnimation(ak);
            seq.Delay(120);
            seq.DoDamage(caster, targets[0], baseDamage, elementalType, accuracy);

        }
    }
}

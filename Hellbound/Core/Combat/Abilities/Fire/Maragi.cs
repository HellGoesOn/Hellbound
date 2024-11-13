using HellTrail.Core.Combat.Sequencer;
using HellTrail.Core.Combat.Status.Debuffs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treeline.Core.Graphics;

namespace HellTrail.Core.Combat.Abilities.Fire
{
    public class Maragi : Ability
    {
        public Maragi() : base("Maragi", "Light Fire damage to all foes.\nHigh chance of Burn")
        {
            spCost = 9;
            aoe = true;
            canTarget = ValidTargets.Enemy;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            Sequence sequence = new Sequence(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(20));

            foreach (Unit target in targets)
            {
                sequence.Add(new PlaySoundSequence("GunShot"));
                sequence.Add(new DoDamageSequence(caster, target, 12, ElementalType.Fire));
                sequence.Add(new ApplyEffectSequence(sequence, target, new Burning(), 95, true));
                sequence.Add(new DelaySequence(5)); 
                sequence.Add(new OneActionSequence(() =>
                {
                    for (int i = 0; i < 250; i++)
                    {
                        var position = new Vector3(target.position, 0);
                        var xx = Main.rand.NextSingle() * (Main.rand.Next() % 2 == 0 ? -1 : 1) * Main.rand.Next(50, 150) * 0.01f;
                        var yy = Main.rand.NextSingle() * (Main.rand.Next() % 2 == 0 ? -1 : 1) * Main.rand.Next(25, 75) * 0.01f;
                        var zz = Main.rand.NextSingle() * -Main.rand.Next(255, 355) * 0.01f;
                        var velocity = new Vector3(xx, yy, zz);
                        var part = ParticleManager.NewParticle(position, velocity, 300, 0.1f, true);
                        if (part != null)
                        {
                            part.diesToGravity = true;
                            part.color = Color.Lerp(Color.Yellow, Color.OrangeRed, Main.rand.NextSingle());
                            part.scale = Vector2.One * Main.rand.Next(1, 5);
                        }
                    }
                }));
            }
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            battle.sequences.Add(sequence);
        }
    }
}

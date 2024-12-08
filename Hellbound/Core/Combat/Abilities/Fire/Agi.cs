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
    public class Agi : Ability
    {
        public Agi() : base("Agi", "Light Fire damage to 1 foe.\nHigh chance of Burn")
        {
            spCost = 3;
            aoe = false;
            canTarget = ValidTargets.Enemy;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            int markiplier = targets[0].BattleStation.X < caster.BattleStation.X ? -1 : 1;

            Sequence sequence = new(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new PlaySoundSequence("GunShot"));
            sequence.Add(new OneActionSequence(() =>
            {
                for (int i = 0; i < 250; i++)
                {
                    var position = new Vector3(targets[0].position, 0);
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
            sequence.Add(new DoDamageSequence(caster, targets[0], 12, ElementalType.Fire));
            sequence.Add(new ApplyEffectSequence(sequence, targets[0], new Burning(), 100, true));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            battle.sequences.Add(sequence); 
        }
    }
}

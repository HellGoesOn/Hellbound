using Casull.Core.Combat.Sequencer;
using Casull.Core.Combat.Status.Debuffs;
using Microsoft.Xna.Framework;
using Treeline.Core.Graphics;

namespace Casull.Core.Combat.Abilities.Wind
{
    public class Garu : Ability
    {
        public Garu() : base("Garu", "Light Wind damage to 1 foe.\nHigh chance of Burn")
        {
            spCost = 3;
            aoe = false;
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.Wind;
            baseDamage = 12;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            int markiplier = targets[0].BattleStation.X < caster.BattleStation.X ? -1 : 1;

            Sequence sequence = CreateSequence(battle);
            sequence.Add(new SetActorAnimation(caster, "Cast"));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new PlaySoundSequence("GunShot"));
            sequence.DoFor(() => {
                for (int i = 0; i < 10; i++) {
                    var position = new Vector3(targets[0].position + new Vector2(0, targets[0].size.Y * 0.25f), 0);
                    var xx = Main.rand.Next(-(int)(targets[0].size.X * 0.35f), (int)(targets[0].size.X * 0.35f)+1);
                    var xxx = Main.rand.Next(-10, 11) * 0.01f;
                    var zz = Main.rand.NextSingle() * -Main.rand.Next(150, 250) * 0.01f;
                    var velocity = new Vector3(xxx, 0, zz);
                    var part = ParticleManager.NewParticleAdditive(position + new Vector3(xx, 0, 0), velocity, 300, 0.05f, true);
                    if (part != null) {
                        part.diesToGravity = false;
                        part.color = Color.Lerp(Color.LimeGreen, Color.DarkGreen, Main.rand.NextSingle());
                        part.scale = Vector2.One * Main.rand.Next(1, 5);
                    }
                }
            }, 30);
            sequence.Add(new DoDamageSequence(caster, targets[0], baseDamage, elementalType, accuracy));
            sequence.Add(new DelaySequence(40));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
        }
    }
}

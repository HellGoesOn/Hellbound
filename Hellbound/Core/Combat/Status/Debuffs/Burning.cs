using HellTrail.Core.Combat.Sequencer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treeline.Core.Graphics;

namespace HellTrail.Core.Combat.Status.Debuffs
{
    public class Burning : StatusEffect
    {
        public Burning()
        {
            name = "Burn";
            description = "Takes fire damage at the beginning of the turn";
            turnsLeft = 3;
        }

        public override void OnTurnBegin(Unit unit, Battle battle)
        {
            base.OnTurnBegin(unit, battle);
            Sequence seq = new Sequence(battle);
            seq.Add(new DoDamageSequence(unit, unit, (int)(unit.stats.MaxHP * 0.08f), ElementalType.DoT));
            seq.Add(new DelaySequence(30));
            battle.sequences.Add(seq);
            turnsLeft--;
        }

        public override void OnTurnEnd(Unit unit, Battle battle)
        {
            base.OnTurnEnd(unit, battle);
        }

        public override void UpdateVisuals(SpriteBatch spriteBatch, Unit unit, Battle battle)
        {   
            Color[] clrs = { Color.Red, Color.Yellow, Color.Orange, Color.Crimson};

            int xx = Main.rand.Next((int)(unit.size.X));
            int yy = Main.rand.Next((int)(unit.size.Y));
            float velX = Main.rand.Next(60, 120) * 0.001f * (Main.rand.Next(2) == 0 ? -1 : 1);
            float velY = -0.2f * (Main.rand.Next(20, 60) * 0.05f);
            var particle = ParticleManager.NewParticleAdditive(new Vector3(unit.position + new Vector2(-unit.size.X * 0.5f + xx, unit.size.Y * 0.25f), 0), new Vector3(velX, 0, velY), 90);
            particle.color = clrs[Main.rand.Next(clrs.Length)];
            particle.endColor = Color.Black;
            particle.scale = Vector2.One * Main.rand.Next(1,3);
            particle.weight = 0.01f;
        }
    }
}

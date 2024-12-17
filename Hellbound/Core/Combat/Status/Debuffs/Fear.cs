using Casull.Core.Combat.Sequencer;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.Combat.Status.Debuffs
{
    public class Fear : StatusEffect
    {
        public Fear()
        {
            turnsLeft = 4;
            name = "Fear";
            description = "High chance to miss a turn";
            debuff = true;
        }

        public override void OnTurnBegin(Unit unit, Battle battle)
        {
            turnsLeft--;
            if (battle.rand.Next(101) <= 50) {
                Sequence seq = new(battle);
                seq.Add(new DelaySequence(60));
                battle.sequences.Add(seq);

                DamageNumber damageNumber = new(DamageType.Normal, "SCARED", (unit.position));
                battle.damageNumbers.Add(damageNumber);
                unit.shake = 0.32f;
                battle.State = BattleState.VictoryCheck;
                return;
            }
        }

        public override void OnApply(Unit unit)
        {
            unit.damageReceivedMultiplier += 0.2f;
        }

        public override void OnRemove(Unit unit)
        {
            unit.damageReceivedMultiplier -= 0.2f;
        }

        public override void UpdateVisuals(SpriteBatch spriteBatch, Unit unit, Battle battle)
        {
            Renderer.Draw(Assets.GetTexture("Tear"), unit.position - new Vector2(12, 16), null, Color.White, (float)Math.Cos(Main.totalTime) * 0.25f, new Vector2(8, 8), Vector2.One * 0.5f, SpriteEffects.None, 1f);
            Renderer.Draw(Assets.GetTexture("Tear"), unit.position - new Vector2(10, 20), null, Color.White, (float)Math.Sin(Main.totalTime) * 0.25f, new Vector2(8, 8), Vector2.One * 0.5f, SpriteEffects.None, 1f);
        }
    }
}

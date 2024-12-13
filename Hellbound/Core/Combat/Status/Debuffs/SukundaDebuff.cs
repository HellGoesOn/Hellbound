using Casull.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.Combat.Status
{
    public class SukundaDebuff : StatusEffect
    {
        public SukundaDebuff()
        {
            name = "Sukunda";
            description = "Lowered accuracy";
            turnsLeft = 3;
        }

        public override void OnApply(Unit unit)
        {
            unit.Stats.accuracy -= 0.5f;
            unit.Stats.evasion -= 0.5f;
        }

        public override void UpdateVisuals(SpriteBatch spriteBatch, Unit unit, Battle battle)
        {
            spriteBatch.Draw(Assets.GetTexture("Arrow"), unit.position - new Vector2(10, 16), null, Color.Crimson, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            spriteBatch.Draw(Assets.GetTexture("Arrow"), unit.position - new Vector2(10, 20), null, Color.Crimson, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
        }

        public override void OnTurnEnd(Unit unit, Battle battle)
        {
            turnsLeft--;
        }

        public override void OnRemove(Unit unit)
        {
            unit.Stats.accuracy += 0.5f;
            unit.Stats.evasion += 0.5f;
            UIManager.combatUI.SetAbilityUsed("Agility decrease reverted!");
        }
    }
}

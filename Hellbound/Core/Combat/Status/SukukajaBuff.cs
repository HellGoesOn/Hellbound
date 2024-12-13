using Casull.Core.UI;
using Casull.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.Combat.Status
{
    public class SukukajaBuff : StatusEffect
    {
        public SukukajaBuff()
        {
            name = "Sukukaja";
            description = "Increased accuracy";
            turnsLeft = 3;
        }

        public override void OnApply(Unit unit)
        {
            unit.Stats.accuracy += 0.5f;
            unit.Stats.evasion += 0.5f;
        }

        public override void OnTurnBegin(Unit unit, Battle battle)
        {
        }

        public override void UpdateVisuals(SpriteBatch spriteBatch, Unit unit, Battle battle)
        {
            var offset = new Vector2((float)Math.Sin(Main.totalTime), (float)Math.Cos(Main.totalTime));
            if (unit.animations.TryGetValue(unit.currentAnimation, out var anim)) {
                anim.Draw(spriteBatch, unit.GetPosition() + offset * 2, Color.Blue * 0.15f, unit.rotation, unit.scale, unit.depth - 0.01f);
                anim.Draw(spriteBatch, unit.GetPosition() - offset * 2, Color.Blue * 0.15f, unit.rotation, unit.scale, unit.depth - 0.01f);

            }
            else {
                spriteBatch.DrawFixed(Assets.GetTexture(unit.sprite), new Vector2((int)(unit.GetPosition().X), (int)(unit.GetPosition().Y)) + offset * 2, null, Color.Blue * 0.25f, 0f, new Vector2(16), unit.scale, SpriteEffects.None, unit.depth - 0.01f);
                spriteBatch.DrawFixed(Assets.GetTexture(unit.sprite), new Vector2((int)(unit.GetPosition().X), (int)(unit.GetPosition().Y)) - offset * 2, null, Color.Blue * 0.25f, 0f, new Vector2(16), unit.scale, SpriteEffects.None, unit.depth - 0.01f);
            }
        }

        public override void OnTurnEnd(Unit unit, Battle battle)
        {
            turnsLeft--;
        }

        public override void OnRemove(Unit unit)
        {
            unit.Stats.accuracy -= 0.5f;
            unit.Stats.evasion -= 0.5f;

            UIManager.combatUI.SetAbilityUsed("Agility increase reverted!");
        }
    }
}

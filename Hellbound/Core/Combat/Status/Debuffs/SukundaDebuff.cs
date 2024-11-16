using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Status
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
            unit.stats.accuracy -= 0.5f;
            unit.stats.evasion -= 0.5f;
        }

        public override void OnTurnBegin(Unit unit, Battle battle)
        {
            turnsLeft--;
        }

        public override void UpdateVisuals(SpriteBatch spriteBatch, Unit unit, Battle battle)
        {
            spriteBatch.Draw(Assets.Textures["Arrow"], unit.position - new Vector2(10, 16), null, Color.Crimson, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            spriteBatch.Draw(Assets.Textures["Arrow"], unit.position - new Vector2(10, 20), null, Color.Crimson, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
        }

        public override void OnRemove(Unit unit)
        {
            unit.stats.accuracy += 0.5f;
            unit.stats.evasion += 0.5f;
        }
    }
}

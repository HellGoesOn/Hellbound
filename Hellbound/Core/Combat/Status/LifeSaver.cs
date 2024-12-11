using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Status
{
    public class LifeSaver : StatusEffect
    {
        public LifeSaver()
        {
            turnsLeft = 1;
        }

        public override void OnTurnBegin(Unit unit, Battle battle)
        {
            base.OnTurnBegin(unit, battle);
            turnsLeft--;
        }

        public override void UpdateVisuals(SpriteBatch spriteBatch, Unit unit, Battle battle)
        {
            unit.Stats.HP = Math.Max(8, unit.Stats.HP);
        }

        public override void OnTurnEnd(Unit unit, Battle battle)
        {
            base.OnTurnEnd(unit, battle);
            turnsLeft--;
        }
    }
}

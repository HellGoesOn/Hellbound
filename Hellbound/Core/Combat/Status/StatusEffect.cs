using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Status
{
    public class StatusEffect : IStatus
    {
        public int turnsLeft;

        public string name = "???";
        public string description = "???";

        public virtual void OnApply(Unit unit)
        {
        }

        public virtual void OnRemove(Unit unit)
        {
        }

        public virtual void OnTurnBegin(Unit unit, Battle battle)
        {
        }

        public virtual void OnTurnEnd(Unit unit, Battle battle)
        {
        }

        public virtual void UpdateVisuals(SpriteBatch spriteBatch, Unit unit, Battle battle)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Status
{
    public class StatusEffect
    {
        public int turnsLeft;

        public virtual void Update(Unit unit)
        {

        }

        public virtual void OnApply(Unit unit)
        {
        }

        public virtual void OnRemove(Unit unit)
        {
        }

        public virtual void OnTurnBegin(Unit unit)
        {
        }

        public virtual void OnTurnEnd(Unit unit)
        {
        }
    }

    public delegate void EffectHandler(Unit unite);
}

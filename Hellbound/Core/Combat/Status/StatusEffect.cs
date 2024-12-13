using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.Combat.Status
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

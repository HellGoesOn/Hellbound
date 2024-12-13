namespace Casull.Core.Combat.Status
{
    public class GuardingEffect : StatusEffect
    {
        public GuardingEffect()
        {
            name = "Guarding";
            description = "Reduces damage taken and prevents weakness from being exploited";
            turnsLeft = 1;
        }

        public override void OnTurnBegin(Unit unit, Battle battle)
        {
            base.OnTurnBegin(unit, battle);
            turnsLeft--;
        }

        public override void OnTurnEnd(Unit unit, Battle battle)
        {
            base.OnTurnEnd(unit, battle);
            turnsLeft--;
        }
    }
}

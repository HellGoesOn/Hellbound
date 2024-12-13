namespace Casull.Core.Combat.Scripting
{
    public class Script
    {
        public bool activated;

        public Func<Battle, bool> condition;

        public Action<Battle> action;

        public void TryRunScript(Battle battle)
        {
            if (condition.Invoke(battle)) {
                activated = true;
                action?.Invoke(battle);
            }
        }
    }
}

namespace HellTrail.Core.Overworld
{
    public class Trigger
    {
        public bool activated;

        public Func<World, bool> condition;

        public Action<World> action;

        public void TryRunScript(World world)
        {
            if (condition.Invoke(world))
            {
                activated = true;
                action?.Invoke(world);
            }
        }
    }

    public delegate void Action(World world);
}

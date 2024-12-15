namespace Casull.Core.Overworld
{
    public class Trigger(string id)
    {
        public bool activated;

        public bool repeatadble;

        public string id = id;

        public Func<World, bool> condition;

        public Action<World> action;

        public bool TryRunScript(World world)
        {
            if (condition == null)
                return false;

            if (condition?.Invoke(world) == true) {
                activated = true;
                action?.Invoke(world);

                return true;
            }
            return false;
        }

        public void Activate(World world)
        {
            activated = true;
            action?.Invoke(world);
        }
    }

    public delegate void WorldAction(World world);
}

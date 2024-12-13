namespace Casull.Core.ECS.Components
{
    public class TripWire : IComponent
    {
        public string trigger;

        public TripWire(string trigger)
        {
            this.trigger = trigger;
        }
    }
}

namespace Casull.Core.ECS.Components
{
    public struct HasCollidedMarker : IComponent
    {
        public int otherId;

        public HasCollidedMarker(int otherId)
        {
            this.otherId = otherId;
        }
    }
}

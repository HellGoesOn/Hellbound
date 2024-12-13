namespace Casull.Core.ECS.Components
{
    public class FollowingOther : IComponent
    {
        public int otherId;
        public FollowingOther(int otherId)
        {
            this.otherId = otherId;
        }
    }
}

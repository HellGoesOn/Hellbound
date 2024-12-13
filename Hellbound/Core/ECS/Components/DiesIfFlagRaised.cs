namespace Casull.Core.ECS.Components
{
    public class DiesIfFlagRaised : IComponent
    {
        public string flag;

        public DiesIfFlagRaised(string flag)
        {
            this.flag = flag;
        }
    }
}

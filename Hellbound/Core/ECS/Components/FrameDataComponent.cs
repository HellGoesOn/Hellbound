namespace Casull.Core.ECS.Components
{
    public class FrameDataComponent : IComponent
    {
        public FrameData[] data;

        public FrameDataComponent(params FrameData[] data)
        {
            this.data = data;
        }
    }
}

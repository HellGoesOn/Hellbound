namespace Casull.Core.ECS.Components
{
    public class PlayerMarker : IComponent
    {
        //public InputHandler onInput;

        public PlayerMarker()
        {
            //this.onInput = onInput;
        }
    }

    public delegate void InputHandler(Entity entity, Context context);
}

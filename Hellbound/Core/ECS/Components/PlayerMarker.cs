namespace Casull.Core.ECS.Components
{
    public class PlayerMarker : IComponent
    {
        internal bool preserveSpeed;

        public PlayerMarker()
        {
            //this.onInput = onInput;
        }
    }

    public delegate void InputHandler(Entity entity, Context context);
}

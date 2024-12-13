using Microsoft.Xna.Framework;

namespace Casull.Core.ECS.Components
{
    public class LoadingZone : IComponent
    {
        public string nextZone;
        public Vector2 newPosition;
        public Vector2 direction;
        public LoadingZone(string nextZone, Vector2 direction = default, Vector2 newPosition = default)
        {
            this.direction = direction;
            if (direction == default) {
                this.direction = -Vector2.UnitY;
            }
            this.newPosition = newPosition;
            this.nextZone = nextZone;
        }
    }
}

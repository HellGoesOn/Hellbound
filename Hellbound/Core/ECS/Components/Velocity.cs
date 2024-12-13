using Microsoft.Xna.Framework;

namespace Casull.Core.ECS.Components
{
    public class Velocity(Vector2 value) : IComponent
    {
        public Vector2 value = value;

        public float X {
            get => value.X;
            set => this.value.X = value;
        }

        public float Y {
            get => value.Y;
            set => this.value.Y = value;
        }

        public Velocity(float x, float y) : this(new Vector2(x, y)) { }

        public override string ToString()
        {
            return $"Velocity:[{value}]";
        }
    }
}

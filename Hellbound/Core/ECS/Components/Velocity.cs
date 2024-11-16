using Microsoft.Xna.Framework;

namespace HellTrail.Core.ECS.Components
{
    public class Velocity(Vector2 value) : IComponent
    {
        public Vector2 value = value;

        public Velocity(float x, float y) : this (new Vector2(x, y)) { }

        public override string ToString()
        {
            return $"[{value}]";
        }
    }
}

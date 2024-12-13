using Microsoft.Xna.Framework;

namespace Casull.Core.ECS.Components
{
    public class ParticleEmitter : IComponent
    {
        public int amountPerFrame;
        public int lifeTime;
        public int randMinX;
        public int randMaxX;
        public int randMinY;
        public int randMaxY;
        public Vector2 direction;
        public Vector2 offset;
        public Vector2 randomOffset;
        public Vector2[] scales;
        public Color[] color;
        public bool additive;

        public ParticleEmitter(int amountPerFrame, int lifeTime, int randMinX, int randMaxX, int randMinY, int randMaxY, Vector2 dir, Vector2[] scales, Color[] color, Vector2? offset = null, Vector2? randomOffset = null, bool additive = false)
        {
            this.lifeTime = lifeTime;
            this.randMinX = randMinX;
            this.randMaxX = randMaxX;
            this.randMinY = randMinY;
            this.randMaxY = randMaxY;
            this.amountPerFrame = amountPerFrame;
            this.color = color;
            this.scales = scales;
            this.direction = dir;
            this.randomOffset = randomOffset ?? Vector2.Zero;
            this.offset = offset ?? Vector2.Zero;
            this.additive = additive;
        }
    }
}

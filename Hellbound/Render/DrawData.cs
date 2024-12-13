using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Render
{
    public readonly struct DrawData : IComparable
    {
        public readonly Texture2D texture;
        public readonly Vector2 position;
        public readonly Rectangle? source;
        public readonly Vector2 scale;
        public readonly Vector2 origin;
        public readonly Color color;
        public readonly float rotation;
        public readonly SpriteEffects spriteEffects;
        public readonly float depth;
        public readonly bool solid;

        public DrawData(Texture2D texture, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float depth, bool solid = false)
        {
            this.source = source;
            this.texture = texture;
            this.position = position;
            this.scale = scale;
            this.color = color;
            this.rotation = rotation;
            this.spriteEffects = spriteEffects;
            this.depth = depth;
            this.origin = origin;
            this.solid = solid;
        }

        public int CompareTo(object obj)
        {
            return CompareTo((DrawData)obj);
        }

        public int CompareTo(DrawData other)
        {
            if (this.depth < other.depth) return -1;
            if (this.depth > other.depth) return 1;

            return 0;
        }
    }
}

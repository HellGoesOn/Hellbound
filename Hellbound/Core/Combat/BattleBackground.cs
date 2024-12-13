using Microsoft.Xna.Framework;

namespace Casull.Core.Combat
{
    public class BattleBackground
    {
        public string texture;
        public float opacity;
        public Color color;

        public BattleBackground(string texture)
        {
            opacity = 1f;
            this.texture = texture;
            color = Color.White;
        }
    }
}

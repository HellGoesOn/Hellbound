using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.ECS
{
    public interface IDraw : ISystem
    {
        void Draw(Context context, SpriteBatch spriteBatch);
    }
}

using Casull.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Casull
{
    public interface IGameState
    {
        Camera GetCamera();
        void Update();
        void Draw(SpriteBatch spriteBatch);
    }
}

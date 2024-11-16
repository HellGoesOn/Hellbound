using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public interface IDraw : ISystem
    {
        void Draw(Context context, SpriteBatch spriteBatch);
    }
}

using HellTrail.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail
{
    public interface IGameState
    {
        Camera GetCamera();
        void Update();
        void Draw(SpriteBatch spriteBatch);
    }
}

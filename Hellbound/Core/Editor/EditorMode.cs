using HellTrail.Core.Overworld;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Editor
{
    public class EditorMode : IGameState
    {
        public World world;

        public EditorMode()
        {
            world = new World();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            world.Draw(spriteBatch);
        }

        public Camera GetCamera()
        {
            return world.GetCamera();
        }

        public void Update()
        {
            world.Update();
        }
    }
}

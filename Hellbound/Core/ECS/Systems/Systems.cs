using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class Systems : IExecute, IDraw
    {
        private readonly List<IExecute> _executeSystems = [];
        private readonly List<IDraw> _drawSystems = [];

        public void AddSystem(ISystem system)
        {
            if(system is IExecute executeSystem) _executeSystems.Add(executeSystem);
            if(system is IDraw drawSystem) _drawSystems.Add(drawSystem);
        }

        public void Execute(Context context)
        {
            for(int i = 0; i < _executeSystems.Count; i++)
                _executeSystems[i].Execute(context);
        }

        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            for(int i = 0; i < _drawSystems.Count;i++)
                _drawSystems[i].Draw(context, spriteBatch);
        }
    }
}

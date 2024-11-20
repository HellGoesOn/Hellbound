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
        private readonly List<ISystem> _allSystems = [];

        private readonly Dictionary<Type, bool> _enabledSystems = [];

        public void AddSystem(ISystem system)
        {
            if(system is IExecute executeSystem) _executeSystems.Add(executeSystem);
            if(system is IDraw drawSystem) _drawSystems.Add(drawSystem);

            _allSystems.Add(system);
            _enabledSystems.Add(system.GetType(), true);
        }

        public void Execute(Context context)
        {
            for (int i = 0; i < _executeSystems.Count; i++)
            {
                if (_enabledSystems[_executeSystems[i].GetType()])
                    _executeSystems[i].Execute(context);
            }
        }

        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _drawSystems.Count; i++)
            {
                if (_enabledSystems[_drawSystems[i].GetType()])
                    _drawSystems[i].Draw(context, spriteBatch);
            }
        }

        public void ToggleSystem(Type type, bool? forceTo = null)
        {
            forceTo ??= !_enabledSystems[type];
            _enabledSystems[type] = (bool)forceTo;
        }

        public List<ISystem> GetAll() => _allSystems;

        public bool IsEnabled(Type type) => _enabledSystems.Count > 0 && _enabledSystems[type];
    }
}

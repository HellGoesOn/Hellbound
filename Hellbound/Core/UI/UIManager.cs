using HellTrail.Core.UI.CombatUI;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI
{
    public static class UIManager
    {
        public static readonly List<UIState> UIStates = [];

        public static CombatUIState combatUI;

        public static void Init()
        {
            combatUI = new CombatUIState();
            //panel.Rotation = -MathHelper.PiOver2;
            var state = CreateState();
            //state.Append(panel);

            UIStates.Add(combatUI);
        }

        public static void Update()
        {
            foreach (UIState state in UIStates)
            {
                state.Update();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (UIState state in UIStates)
            {
                state.Draw(spriteBatch);
            }
        }

        public static UIState CreateState()
        {
            var uiState = new UIState();
            UIStates.Add(uiState);
            return uiState;
        }
    }
}

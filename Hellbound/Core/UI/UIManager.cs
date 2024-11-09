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

            var panel = new UIPanel
            {
                size = new Vector2(280, 120),
                Position = new Vector2(16, Renderer.UIPreferedHeight - 140),
            };
            string text = "[W][A][S][D] Navigate\n[E] Confirm\n[Q] Cancel";
            var tutorialText = new UIBorderedText(text);
            tutorialText.Position = new Vector2(16);
            panel.Append(tutorialText);
            //panel.Rotation = -MathHelper.PiOver2;
            var state = CreateState();
            state.Append(panel);

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

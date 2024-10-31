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
        public static List<UIState> UIStates = [];

        public static void Init()
        {
            var panel = new UIPanel
            {
                size = new Vector2(1000, 60),
                Position = new Vector2(16, Renderer.UIPreferedHeight - 78),
            };
            string text = "[W][A][S][D] -> Navigate menu. [E] or [LMB] -> Confirm. [Q] or [RMB] -> Cancel";
            var tutorialText = new UIBorderedText(text);
            tutorialText.Position = new Vector2(16);
            panel.Append(tutorialText);
            //panel.Rotation = -MathHelper.PiOver2;
            var basePos = new Vector2(16, Renderer.UIPreferedHeight - 78);
            var uiState = new UIState();
            CreateState().Append(panel);
            
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

using HellTrail.Core.DialogueSystem;
using HellTrail.Core.Overworld;
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
        public static DialogueUIState dialogueUI;
        public static OverworldUIState overworldUI;

        public static UIElement hoveredElement;

        public static void Init()
        {
            combatUI = new CombatUIState();
            dialogueUI = new DialogueUIState();
            overworldUI = new OverworldUIState();
            //panel.Rotation = -MathHelper.PiOver2;
            var state = CreateState();
            //state.Append(panel);

            UIStates.Add(combatUI);
            UIStates.Add(dialogueUI);

            UIState debugState = new()
            {
                id = "debugState"
            };
            debugState.Append(new UIBorderedText("")
            {
                id = "debugText",
                Position = new Vector2(16),
            });
            UIStates.Add(debugState);
            UIStates.Add(overworldUI);
        }

        public static void Update()
        {
            hoveredElement = null;
            foreach (UIState state in UIStates)
            {
                state.Update();
            }

            if(Input.LMBClicked && hoveredElement != null)
            {
                hoveredElement.Click();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (UIState state in UIStates)
            {
                state.Draw(spriteBatch);
            }
        }

        public static UIState CreateState(string id = "")
        {
            var uiState = new UIState()
            {
                id = id
            };
            UIStates.Add(uiState);
            return uiState;
        }

        public static UIState GetState(Predicate<UIState> predicate) => UIStates.Find(predicate);

        public static UIState GetStateByName(string name) => GetState(x=>x.id == name);
    }
}

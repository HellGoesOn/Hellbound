using HellTrail.Core.DialogueSystem;
using HellTrail.Core.Editor;
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
using static System.Net.Mime.MediaTypeNames;

namespace HellTrail.Core.UI
{
    public static class UIManager
    {
        public static readonly List<UIState> UIStates = [];

        public static CombatUIState combatUI;
        public static DialogueUIState dialogueUI;
        public static OverworldUIState overworldUI;
        public static EditorUI editorUI;

        public static UIElement hoveredElement;

        public static string tooltipText;

        public static void Init()
        {
            combatUI = new CombatUIState();
            dialogueUI = new DialogueUIState();
            overworldUI = new OverworldUIState();
            editorUI = new EditorUI();
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
            }.SetPosition(16));
            
            UIStates.Add(debugState);
            UIStates.Add(overworldUI);
            UIStates.Add(editorUI);
        }

        public static void Update()
        {
            hoveredElement = null;
            tooltipText = "";
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

            if (!string.IsNullOrWhiteSpace(tooltipText))
            {
                Vector2 size = Assets.Arial.MeasureString(tooltipText) + new Vector2(16);
                Vector2 offset = Input.UIMousePosition.Y > Renderer.UIPreferedHeight - size.Y ? new Vector2(16, -16) : new Vector2(16);
                Renderer.DrawRect(spriteBatch, Input.UIMousePosition + offset - new Vector2(16, 8) - new Vector2(2), size + new Vector2(4), 1, Color.White);
                Renderer.DrawRect(spriteBatch, Input.UIMousePosition + offset - new Vector2(16, 8), size, 1, Color.DarkBlue);

                Renderer.DrawBorderedString(spriteBatch, Assets.Arial, tooltipText, Input.UIMousePosition + offset - new Vector2(8, 0), Color.White, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
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

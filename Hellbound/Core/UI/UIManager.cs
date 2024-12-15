using Casull.Core.DialogueSystem;
using Casull.Core.Editor;
using Casull.Core.Overworld;
using Casull.Core.UI.CombatUI;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.UI
{
    public static class UIManager
    {
        public static readonly List<UIState> UIStates = [];

        public static CombatUIState combatUI;
        public static DialogueUIState dialogueUI;
        public static OverworldUIState overworldUI;
        public static EditorUI editorUI;

        public static UIElement hoveredElement;

        public static UIBorderedText debugText;

        public static string tooltipText;

        private static List<string> showInDebug = [];

        public static void Init()
        {
            combatUI = new CombatUIState();
            dialogueUI = new DialogueUIState();
            overworldUI = new OverworldUIState();
            //panel.Rotation = -MathHelper.PiOver2;
            var state = CreateState("testState");
            debugText = new UIBorderedText("");
            debugText.SetPosition(16);
            state.Append(debugText);

            UIStates.Add(combatUI);
            UIStates.Add(overworldUI);
            UIStates.Add(dialogueUI);
            //RelaunchEditor();
        }

        public static void RelaunchEditor()
        {
            if (editorUI != null) {
                editorUI.active = false;
                editorUI.CheckSubscriptions();
            }
            UIStates.Remove(editorUI);
            editorUI = new EditorUI();
            UIStates.Add(editorUI);
        }

        public static void Update()
        {
            hoveredElement = null;
            tooltipText = "";

            for (int i = 0; i < UIStates.Count; i++) {
                var state = UIStates[i];
                state.Update();
            }

            if (Input.LMBClicked && hoveredElement != null) {
                hoveredElement.Click();
            }
        }

        public static void Debug(string text) => showInDebug.Add(text);
        public static void Debug(object someObject) => showInDebug.Add(someObject.ToString());

        public static void Draw(SpriteBatch spriteBatch)
        {
            debugText.text = "";
            foreach (string str in showInDebug) {
                debugText.text += str + "\n";
            }

            foreach (UIState state in UIStates) {
                state.Draw(spriteBatch);
            }

            showInDebug.Clear();

            if (!string.IsNullOrWhiteSpace(tooltipText)) {
                Vector2 size = Assets.Arial.MeasureString(tooltipText) + new Vector2(16);
                Vector2 offset = Input.UIMousePosition.Y > Renderer.UIPreferedHeight - size.Y ? new Vector2(16, -16) : new Vector2(16);
                Renderer.DrawRect(spriteBatch, Input.UIMousePosition + offset - new Vector2(16, 8) - new Vector2(2), size + new Vector2(4), Color.White);
                Renderer.DrawRect(spriteBatch, Input.UIMousePosition + offset - new Vector2(16, 8), size, Color.DarkBlue);

                Renderer.DrawBorderedString(spriteBatch, Assets.Arial, tooltipText, Input.UIMousePosition + offset - new Vector2(8, 0), Color.White, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
            }
        }

        public static UIState CreateState(string id = "")
        {
            var uiState = new UIState() {
                id = id
            };
            UIStates.Add(uiState);
            return uiState;
        }

        public static UIState GetState(Predicate<UIState> predicate) => UIStates.Find(predicate);

        public static UIState GetStateByName(string name) => GetState(x => x.id == name);
    }
}

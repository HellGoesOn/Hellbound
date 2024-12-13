using Casull.Core.UI;
using Casull.Render;
using Microsoft.Xna.Framework;

namespace Casull.Core.DialogueSystem
{
    public class DialogueUIState : UIState
    {
        public List<Dialogue> dialogues = [];

        public UIPanel darkeningPanel;
        public UIPanel dialoguePanel;
        public UIBorderedText dialogueText;
        public UIPanel speakerPanel;
        public UIBorderedText speakerText;
        public UIBorderedText actionsText;
        public UIPortrait portrait;

        public DialogueUIState()
        {
            id = "dialogueUIState";
            portrait = new UIPortrait(null);
            portrait.SetPosition(new Vector2(Renderer.UIPreferedWidth * 0.5f, Renderer.UIPreferedHeight - 240));

            darkeningPanel = new UIPanel() {
                size = new Vector2(Renderer.UIPreferedWidth, Renderer.UIPreferedHeight),
                fillColor = Color.Black * 0.25f,
                outlineColor = Color.Black * 0.25f
            };
            Append(darkeningPanel);
            Append(portrait);
            dialoguePanel = new UIPanel() {
                size = new Vector2(Renderer.UIPreferedWidth - 64, 180),
            };
            dialoguePanel.SetPosition(new Vector2(32, Renderer.UIPreferedHeight - 180 - 16));
            dialogueText = new UIBorderedText("") {
                lineBreak = 80,
            };
            Append(dialoguePanel);
            dialoguePanel.SetPosition(16);

            speakerPanel = new UIPanel() {
                size = new Vector2(180, 40),
            };
            dialoguePanel.Append(speakerPanel);
            speakerPanel.SetPosition(new Vector2(16, -48));

            speakerText = new UIBorderedText("") {
            };
            dialoguePanel.Append(dialogueText);
            speakerPanel.SetPosition(new Vector2(16, 8));
            actionsText = new("[E] Next") {
            };
            dialoguePanel.Append(actionsText);
            actionsText.SetPosition(new Vector2(16, 140));
            speakerPanel.Append(speakerText);
            dialogueText.SetPosition(16);
            speakerPanel.SetPosition(16, -48);
            speakerText.SetPosition(16, 8);

            dialoguePanel.SetPosition(16, Renderer.UIPreferedHeight - 16 - dialoguePanel.size.Y);
        }

        public override void Update()
        {
            base.Update();

            if (dialogues.Count > 0) {
                visible = true;

                DialoguePage page = dialogues[0].CurrentPage;
                speakerText.text = page.title;
                speakerText.color = page.speakerColor;
                dialogueText.text = page.VisibleText;
                dialogueText.color = page.textColor;
                dialoguePanel.fillColor = page.fillColor;
                dialoguePanel.outlineColor = page.borderColor;

                speakerPanel.Visible = !string.IsNullOrWhiteSpace(speakerText.text);

                portrait.portrait = page.portraits;

                dialogues[0].Update();

                if (dialogues[0].hasEnded) {
                    dialogues.RemoveAt(0);
                }
            }
            else {
                visible = false;
            }
        }
    }
}

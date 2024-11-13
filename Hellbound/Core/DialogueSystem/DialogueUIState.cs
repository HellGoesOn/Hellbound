using Cyotek.Drawing.BitmapFont;
using HellTrail.Core.UI;
using HellTrail.Extensions;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.DialogueSystem
{
    public class DialogueUIState : UIState
    {
        public List<Dialogue> dialogues = [];

        public UIPanel darkeningPanel;
        public UIPanel dialoguePanel;
        public UIBorderedText dialogueText;
        public UIPanel speakerPanel;
        public UIBorderedText speakerText;

        public DialogueUIState()
        {
            darkeningPanel = new UIPanel()
            {
                size = new Vector2(Renderer.UIPreferedWidth, Renderer.UIPreferedHeight),
                fillColor = Color.Black * 0.25f,
                outlineColor = Color.Black * 0.25f
            };
            Append(darkeningPanel);
            dialoguePanel = new UIPanel()
            {
                size = new Vector2(Renderer.UIPreferedWidth - 64, 180),
                Position = new Vector2(32, Renderer.UIPreferedHeight - 180 - 16)
            };
            dialogueText = new UIBorderedText("")
            {
                Position = new Vector2(16)
            };
            dialoguePanel.Append(dialogueText);
            Append(dialoguePanel);

            speakerPanel = new UIPanel()
            {
                size = new Vector2(180, 40),
                Position = new Vector2(16, -48)
            };
            speakerText = new UIBorderedText("")
            {
                Position = new Vector2(16, 8)
            };
            speakerPanel.Append(speakerText);
            dialoguePanel.Append(speakerPanel);
        }

        public override void Update()
        {
            base.Update();

            if (dialogues.Count > 0)
            {
                visible = true;

                DialoguePage page = dialogues[0].CurrentPage;
                speakerText.text = page.title;
                dialogueText.text = page.VisibleText.Splice(80);

                speakerPanel.Visible = !string.IsNullOrWhiteSpace(speakerText.text);

                dialogues[0].Update();

                if(dialogues[0].hasEnded)
                {
                    dialogues.RemoveAt(0);
                }
            }
            else
            {
                visible = false;
            }
        }
    }
}

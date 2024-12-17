using Casull.Core.UI;
using Microsoft.Xna.Framework.Input;

namespace Casull.Core.DialogueSystem
{
    public class Dialogue
    {
        public int currentPage;

        public bool hasEnded;

        public static bool auto;

        public List<DialoguePage> pages = [];

        public void Update()
        {
            if (pages.Count <= 0)
                return;

            if (Input.PressedKey(Keys.Q))
                auto = !auto;
            pages[currentPage].Update(this);
        }

        public DialoguePage CurrentPage {
            get => pages[currentPage];
        }

        public static Dialogue Create()
        {
            Dialogue dialogue = new();
            UIManager.dialogueUI.dialogues.Add(dialogue);

            return dialogue;
        }
    }
}

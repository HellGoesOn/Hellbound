namespace HellTrail.Core.DialogueSystem
{
    public class Dialogue
    {
        public int currentPage;

        public bool hasEnded;

        public List<DialoguePage> pages = [];

        public void Update()
        {
            if (pages.Count <= 0)
                return;

            pages[currentPage].Update(this);
        }

        public DialoguePage CurrentPage
        { 
            get => pages[currentPage]; 
        }
    }
}

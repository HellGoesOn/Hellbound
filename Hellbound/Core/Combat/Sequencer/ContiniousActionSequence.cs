namespace Casull.Core.Combat.Sequencer
{
    public class ContiniousActionSequence : ISequenceAction
    {
        public int timeLeft;

        public Action action;

        public ContiniousActionSequence(Action action, int timeLeft)
        {
            this.timeLeft = timeLeft;
            this.action = action;
        }

        public bool IsFinished()
        {
            return timeLeft <= 0;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            action?.Invoke();
            timeLeft--;
        }
    }
}

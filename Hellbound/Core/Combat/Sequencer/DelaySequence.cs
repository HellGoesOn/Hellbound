namespace HellTrail.Core.Combat.Sequencer
{
    public class DelaySequence : ISequenceAction
    {
        public int timeLeft;

        public DelaySequence(int timeLeft)
        {
            this.timeLeft = timeLeft;
        }

        public bool IsFinished()
        {
            return timeLeft <= 0;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            timeLeft--;
        }
    }
}

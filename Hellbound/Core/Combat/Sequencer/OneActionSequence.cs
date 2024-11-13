namespace HellTrail.Core.Combat.Sequencer
{
    public class OneActionSequence : ISequenceAction
    {
        public Action action;

        public OneActionSequence(Action action)
        {

            this.action = action;
        }

        public bool IsFinished()
        {
            action?.Invoke();
            return true;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
        }
    }
}

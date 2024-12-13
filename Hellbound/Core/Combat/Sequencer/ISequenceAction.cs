namespace Casull.Core.Combat.Sequencer
{
    public interface ISequenceAction
    {
        void Update(List<Unit> actors, Battle battle);

        bool IsFinished();
    }
}

namespace Casull.Core.Combat.Sequencer
{
    public class SetActorAnimation(Unit actor, string animationName) : ISequenceAction
    {
        public string animationName = animationName;
        public Unit actor = actor;

        public bool IsFinished()
        {
            return true;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            actor.animations.TryGetValue(actor.CurrentAnimation, out var anim);
            anim?.Reset();
            actor.CurrentAnimation = animationName;
        }
    }
}

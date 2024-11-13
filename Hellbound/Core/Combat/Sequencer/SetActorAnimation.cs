namespace HellTrail.Core.Combat.Sequencer
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
            actor.animations.TryGetValue(actor.currentAnimation, out var anim);
            anim?.Reset();
            actor.currentAnimation = animationName;
        }
    }
}

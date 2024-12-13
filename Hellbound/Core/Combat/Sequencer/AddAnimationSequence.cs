namespace Casull.Core.Combat.Sequencer
{
    public class AddAnimationSequence : ISequenceAction
    {
        public bool oneUpdate;
        public SpriteAnimation anim;
        public AddAnimationSequence(SpriteAnimation animation)
        {
            anim = animation;
        }

        public bool IsFinished()
        {
            return oneUpdate;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            oneUpdate = true;
            battle.fieldAnimations.Add(anim);
        }
    }
}

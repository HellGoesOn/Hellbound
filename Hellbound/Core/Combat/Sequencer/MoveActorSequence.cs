using Microsoft.Xna.Framework;

namespace HellTrail.Core.Combat.Sequencer
{
    public class MoveActorSequence : ISequenceAction
    {
        public Unit actor;

        public Vector2 targetPosition;

        public float speed;

        public MoveActorSequence(Unit whoToMove, Vector2 toWhere, float speed = 0.12f)
        {
            actor = whoToMove;
            targetPosition = toWhere;
            this.speed = speed;
        }

        public bool IsFinished()
        {
            return Vector2.Distance(actor.position, targetPosition) <= 2;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            actor.position += (targetPosition - actor.position) * speed;

            if (IsFinished())
                actor.position = targetPosition;
        }
    }
}

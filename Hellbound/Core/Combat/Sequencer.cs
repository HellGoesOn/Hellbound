using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public class Sequence
    {
        public int currentAction;

        public bool isFinished;

        public bool active;

        public Battle battle;

        public List<Unit> Actors = [];

        public List<ISequenceAction> Actions { get; } = [];

        public Sequence(Battle battle) 
        {
            active = false;
            this.battle = battle;
        }

        public void Update()
        {
            if (isFinished)
                return;

            Actions[currentAction].Update(Actors, battle);

            if (Actions[currentAction].IsFinished())
            {
                if(++currentAction >= Actions.Count)
                {
                    isFinished = true;
                    currentAction = 0;
                }
            }
        }

        public void Add(ISequenceAction action)
        {
            Actions.Add(action); 
        }
    }

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

    public class DoDamageSequence : ISequenceAction
    {
        public Unit target;

        public int damage;

        private bool dealtDamage;

        public DoDamageSequence(Unit target, int damage)
        {
            this.target = target;
            this.damage = damage;
        }

        public bool IsFinished()
        {
            return dealtDamage;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            target.HP = Math.Max(0, target.HP - damage);
            dealtDamage = true;
        }
    }

    public class PlaySoundSequence : ISequenceAction
    {
        public string sound;
        public float volume;

        public PlaySoundSequence(string sound, float volume = -1)
        {
            this.sound = sound;
            this.volume = volume;
        }

        public void Update(List<Unit> actors, Battle battle)
        {

        }

        public bool IsFinished()
        {
            SoundEngine.PlaySound(sound, volume);
            return true;
        }
    }

    public interface ISequenceAction
    {
        void Update(List<Unit> actors, Battle battle);

        bool IsFinished();
    }
}

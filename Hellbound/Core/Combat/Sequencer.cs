using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
        public int damage;

        private bool dealtDamage;

        public ElementalType type;

        public Unit target;

        public DoDamageSequence(Unit target, int damage, ElementalType type = ElementalType.Phys)
        {
            this.type = type;
            this.target = target;
            this.damage = damage;
        }

        public bool IsFinished()
        {
            return dealtDamage;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            int damageTaken = (int)(damage * (1 - target.resistances[type]));
            target.HP = Math.Max(0, target.HP - damageTaken);

            var xx = battle.rand.Next((int)(target.size.X * 0.5f));
            var yy = battle.rand.Next((int)(target.size.Y * 0.5f));
            var offset = -target.size * 0.25f + new Vector2(xx, yy);

            float shakeAmount = 0.16f;
            DamageNumber damageNumber = new(DamageType.Normal, damageTaken, (target.position+offset) * 4);

            if (damageTaken == 0)
            {
                shakeAmount = 0;
                damageNumber.DamageType = DamageType.Blocked; 
                damageNumber.position = (target.position) * 4;
            }
            else if(damageTaken > damage)
            {
                if (!battle.unitsHitLastRound.Contains(target))
                {
                    battle.weaknessHitLastRound = true;
                    battle.unitsHitLastRound.Add(target);
                }
                shakeAmount = 0.32f;

                damageNumber.DamageType = DamageType.Weak;
                damageNumber.position = (target.position + offset) * 4;
            }
            else if(damageTaken < 0)
            {
                damageNumber.position = (target.position) * 4;
                damageNumber.DamageType = DamageType.Repelled;
            }
            else if(damageTaken < damage)
            {
                shakeAmount = 0.08f;
                damageNumber.DamageType = DamageType.Resisted;
                damageNumber.position = (target.position + offset) * 4;
            }

            // TO-DO: add elem types, reaction to repel/block/resist/weak;
            battle.damageNumbers.Add(damageNumber);

            target.shake += shakeAmount;

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

    public interface ISequenceAction
    {
        void Update(List<Unit> actors, Battle battle);

        bool IsFinished();
    }
}

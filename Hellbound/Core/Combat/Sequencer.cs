using HellTrail.Core.Combat.Status;
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

        public int accuracy;

        public bool dealtDamage;

        public ElementalType type;

        public Unit target;
        public Unit caster;

        public DoDamageSequence(Unit caster, Unit target, int damage, ElementalType type = ElementalType.Phys, int accuracy = 100)
        {
            this.accuracy = accuracy;
            this.type = type;
            this.target = target;
            this.caster = caster;
            this.damage = damage;
            dealtDamage = false;
        }

        public bool IsFinished()
        {
            return true;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            float guardFactor = target.HasStatus<GuardingEffect>() ? 0.75f : 1.0f;
            int damageTaken = (int)(damage * (1 - target.resistances[type]) * guardFactor);
            DamageNumber damageNumber;

            if (battle.rand.Next(101) > accuracy)
            {
                dealtDamage = false;
                damageNumber = new(DamageType.Normal, "MISS", (target.position) * 4);
                battle.damageNumbers.Add(damageNumber);
                return;
            }
            target.HP = Math.Max(0, target.HP - damageTaken);

            var xx = Main.rand.Next((int)(target.size.X * 0.5f));
            var yy = Main.rand.Next((int)(target.size.Y * 0.5f));
            var offset = -target.size * 0.25f + new Vector2(xx, yy);

            float shakeAmount = 0.16f;
            damageNumber = new(DamageType.Normal, damageTaken.ToString(), (target.position+offset) * 4);

            dealtDamage = true;
            if (damageTaken == 0) // nulled
            {
                shakeAmount = 0;
                damageNumber.DamageType = DamageType.Blocked; 
                damageNumber.position = (target.position) * 4;

                dealtDamage = false;
            }
            else if(damageTaken > damage) // weakness hit
            {
                if (!battle.unitsHitLastRound.Contains(target) && !target.HasStatus<GuardingEffect>())
                {
                    battle.weaknessHitLastRound = true;
                    battle.unitsHitLastRound.Add(target);
                }
                shakeAmount = 0.32f;

                damageNumber.DamageType = DamageType.Weak;
                damageNumber.position = (target.position + offset) * 4;
            }
            else if(damageTaken < 0)// repelled
            {
                damageNumber.position = (target.position) * 4;
                damageNumber.DamageType = DamageType.Repelled;
                dealtDamage = false;

                shakeAmount = 0;

                damageTaken = (int)(damage * (1 - caster.resistances[type]));
                caster.HP = Math.Max(0, caster.HP - damageTaken);
                float casterResist = caster.resistances[type];
                DamageType repelledType = DamageType.Normal;
                if (casterResist > 0 && casterResist < 1)
                {
                    repelledType = DamageType.Resisted;
                } 
                else if (casterResist < 0)
                {
                    repelledType = DamageType.Weak;
                } 
                else if (casterResist >= 1)
                {
                    repelledType = DamageType.Blocked;
                }

                DamageNumber damageNumber2 = new(repelledType, damageTaken.ToString(), (caster.position + offset) * 4);
                battle.damageNumbers.Add(damageNumber2);
            }
            else if(damageTaken < damage) // resisted
            {
                shakeAmount = 0.08f;
                damageNumber.DamageType = DamageType.Resisted;
                damageNumber.position = (target.position + offset) * 4;
            }

            if (dealtDamage && target.HasStatus<GuardingEffect>())
                target.RemoveAllEffects<GuardingEffect>();

            // TO-DO: add elem types, reaction to repel/block/resist/weak;
            battle.damageNumbers.Add(damageNumber);

            target.shake += shakeAmount;
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

    public class ApplyEffectSequence(Sequence sequence, Unit target, StatusEffect effect, int chance = 100, bool requiresDamageDealt = false) : ISequenceAction
    {
        public int chance = chance;
        public bool requiresDamageDealt = requiresDamageDealt;
        public Unit target = target;
        public Sequence mySequence = sequence;
        public StatusEffect effect = effect;

        public bool IsFinished()
        {
            return true;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            if (requiresDamageDealt)
            {
                int myIndex = mySequence.Actions.IndexOf(this);
                if (myIndex >= 1 && ((mySequence.Actions[myIndex-1] is DoDamageSequence test) && !test.dealtDamage))
                {
                    return;
                }
            }

            if (battle.rand.Next(101) <= chance)
            {
                target.AddEffect(effect);
                DamageNumber damageNumber = new(DamageType.Normal, $"+{effect.name}", (target.position + new Vector2(0, 12)) * 4);
                battle.damageNumbers.Add(damageNumber);
            }
        }
    }

    public interface ISequenceAction
    {
        void Update(List<Unit> actors, Battle battle);

        bool IsFinished();
    }
}

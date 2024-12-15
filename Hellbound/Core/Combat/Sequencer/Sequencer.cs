using Casull.Core.Combat.Status;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Sequencer
{
    public class Sequence
    {
        public int currentAction;

        public int updateCounter;

        public bool isFinished;

        public bool active;

        public string source = "";

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

            updateCounter++;

            //UIManager.Debug(updateCounter.ToString() + $" from {source}");

            Actions[currentAction].Update(Actors, battle);

            if (Actions[currentAction].IsFinished()) {
                if (++currentAction >= Actions.Count) {
                    isFinished = true;
                    currentAction = 0;
                }
            }
        }

        public void Add(ISequenceAction action)
        {
            Actions.Add(action);
        }

        public void DoFor(Action action, int ticks)
        {
            Actions.Add(new ContiniousActionSequence(action, ticks));
        }

        public void MoveActor(Unit whoToMove, Vector2 toWhere, float speed = 0.12f)
        {
            Actions.Add(new MoveActorSequence(whoToMove, toWhere, speed));
        }

        public void DoDamage(Unit caster, Unit target, int damage, ElementalType type = ElementalType.Phys, int accuracy = 100)
        {
            Actions.Add(new DoDamageSequence(caster, target, damage, type, accuracy));
        }

        public void Delay(int time)
        {
            Actions.Add(new DelaySequence(time));
        }

        public void HealTarget(Unit target, int amount)
        {
            Actions.Add(new HealTargetSqequence(target, amount));
        }

        public void SetAnimation(Unit actor, string animationNam)
        {
            Actions.Add(new SetActorAnimation(actor, animationNam));
        }

        public void AddStatusEffect(Unit target, StatusEffect effect, int chance = 100, bool requiresDamageDealt = false, bool canStack = false, bool canExtend = false)
        {
            Actions.Add(new ApplyEffectSequence(this, target, effect, chance, requiresDamageDealt, canStack, canExtend));
        }

        public void CustomAction(Action action)
        {
            Actions.Add(new OneActionSequence(action));
        }

        public void PlaySound(string name, float volume = -1)
        {
            Actions.Add(new PlaySoundSequence(name, volume));
        }

        public void AddAnimation(SpriteAnimation animation)
        {
            Actions.Add(new AddAnimationSequence(animation));
        }
    }
}

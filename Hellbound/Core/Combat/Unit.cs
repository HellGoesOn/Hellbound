using HellTrail.Core.Combat.Abilities;
using HellTrail.Core.Combat.Status;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public class Unit
    {
        public float depth;
        public float shake;
        public string name;
        public string sprite;
        public string currentAnimation = "";
        public string defaultAnimation = "";
        public Team team;
        private Vector2 battleStation;
        public Vector2 position;
        public Vector2 size = new (32);
        public BasicAI ai = null;
        public CombatStats stats;
        public CombatStats statsGrowth;
        public ElementalResistances resistances;
        public List<Ability> abilities = [];
        public List<StatusEffect> statusEffects = [];
        public Dictionary<string, SpriteAnimation> animations = new Dictionary<string, SpriteAnimation>();

        public float opacity;

        public Unit()
        {
            stats = new CombatStats();
            statsGrowth = new CombatStats(1, 1, 15, 5, 0.5f);
            currentAnimation = defaultAnimation = "Idle";
            resistances = new ElementalResistances();
            depth = 0f;
            sprite = "Slime3";
            opacity = 1.25f;
            name = "???";
        }

        public void TryLevelUp()
        {
            if (stats.EXP < stats.toNextLevel)
                return;
            stats.level++;
            stats += statsGrowth;
            stats.HP = stats.MaxHP;
            stats.SP = stats.MaxSP;
            stats.EXP -= stats.toNextLevel;
            stats.toNextLevel = (int)(stats.toNextLevel * 1.05f);
            TryLevelUp();
        }

        // unit should not be updating its own logic outside of combat system
        // therefore this should only be used for visuals;
        public void UpdateVisuals()
        {
            if (Downed && opacity > 0)
            {
                opacity -= 0.02f;
            }

            if (shake > 0)
            {
                shake -= 0.01f;

                if (shake <= 0)
                    shake = 0;
            }

            if(animations.TryGetValue(currentAnimation, out var anim))
            {
                anim.position = position;
                anim.depth = depth;
                anim.color = Downed ? Color.Crimson : Color.White;
                anim.opacity = opacity;
                anim.Update();

                if (anim.finished)
                {
                    anim.Reset();
                    currentAnimation = string.IsNullOrWhiteSpace(anim.nextAnimation) ? currentAnimation : anim.nextAnimation;
                }
            }

            foreach (Ability ability in abilities)
            {
                ability.AdjustCosts(this);
            }
        }

        public bool Downed => stats.HP <= 0;

        public Vector2 BattleStation
        {
            get => battleStation;
            set
            {
                position = battleStation = value;
            }
        }

        public bool ContainsMouse(Vector2 offset)
        {
            var rect = new Rectangle((int)position.X + (int)offset.X, (int)position.Y + (int)offset.Y, (int)size.X, (int)size.Y);
            var mousePoint = new Point((int)Input.MousePosition.X, (int)Input.MousePosition.Y);
            return rect.Contains(mousePoint);
        }


        public bool ContainsMouse()
        {
            var rect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            var mousePoint = new Point((int)Input.MousePosition.X, (int)Input.MousePosition.Y);
            return rect.Contains(mousePoint);
        }

        public void AddEffect(StatusEffect effect)
        {
            statusEffects.Add(effect);
            effect.OnApply(this);
        }

        public void AddReplaceEffect<T>(T effect) where T : StatusEffect
        {
            if (statusEffects.Any(x => x is T))
            {
                RemoveAllEffects<T>();
            }

            AddEffect(effect);
        }

        public void RemoveEffect(StatusEffect effect)
        {
            statusEffects.Remove(effect);
            effect.OnRemove(this);
        }

        public void RemoveAllEffects<T>() where T : StatusEffect
        {
            statusEffects.RemoveAll(x => x is T);
        }

        public bool HasStatus<T>() where T : StatusEffect
        {
            return statusEffects.Any(x => x is T);
        }
    }
}

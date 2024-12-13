using Casull.Core.Combat.Abilities;
using Casull.Core.Combat.Items;
using Casull.Core.Combat.Status;
using Casull.Core.UI;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace Casull.Core.Combat
{
    public class Unit
    {
        public int lastItemIndex, lastAbilityIndex;
        public float depth;
        public float shake;
        public float rotation;
        public string name;
        public string sprite;
        public string currentAnimation = "";
        public string defaultAnimation = "";
        public string portrait = "";
        public string portraitCombat = "";
        public Team team;
        private Vector2 battleStation;
        public Vector2 position;
        public Vector2 size = new(32);
        public Vector2 scale = new(1);
        public BasicAI ai = null;
        private CombatStats _stats;
        public CombatStats Stats { get => _stats; }
        public CombatStats baseStats;
        public CombatStats statsGrowth;
        public ElementalResistances resistances;
        public List<Ability> abilities;
        public List<ItemDrop> loot;
        public List<LearnableAbility> learnableAbilities;
        public List<StatusEffect> statusEffects;
        public Dictionary<string, SpriteAnimation> animations = new();

        public float opacity;

        public Unit()
        {
            loot = [];
            abilities = [];
            learnableAbilities = [];
            statusEffects = [];
            baseStats = new CombatStats();
            _stats = new CombatStats();
            statsGrowth = new CombatStats(1, 1, 15, 5, 0.5f);
            currentAnimation = defaultAnimation = "Idle";
            resistances = new ElementalResistances();
            depth = 0.01f;
            sprite = "Slime3";
            opacity = 1.25f;
            name = "???";
        }

        public void SetBaseStats(CombatStats newStats)
        {
            baseStats = newStats.GetCopy();
            _stats = newStats;
        }

        public void IncreaseStats(CombatStats newStats)
        {
            _stats += newStats;
        }

        public bool TryLevelUp(bool silent = false)
        {
            if (_stats.EXP < Stats.toNextLevel)
                return false;

            var oldStats = Stats.GetCopy();

            float hpPercentage = _stats.HP / (float)_stats.MaxHP;
            float spPercentage = _stats.SP / (float)_stats.MaxSP;

            _stats.level++;
            _stats += statsGrowth;
            _stats.HP = (int)(Stats.MaxHP * hpPercentage);
            _stats.SP = (int)(Stats.MaxSP * spPercentage);
            _stats.EXP -= Stats.toNextLevel;
            _stats.toNextLevel = (int)(_stats.toNextLevel * 1.25f);
            if (!silent)
                UIManager.combatUI.CreateLevelUp(name, oldStats, _stats);
            TryLevelUp(silent);

            TryLearnLevelUpAbilites();

            return true;
        }

        public void TryLearnLevelUpAbilites()
        {
            foreach (LearnableAbility ability in learnableAbilities) {

                if(Stats.level >= ability.requiredLevel) {
                    abilities.Add(ability.abilityToLearn);
                }
            }

            learnableAbilities.RemoveAll(x => x.requiredLevel <= Stats.level);
        }

        // unit should not be updating its own logic outside of combat system
        // therefore this should only be used for visuals;
        public void UpdateVisuals()
        {
            if (Downed && opacity > 0) {
                opacity -= 0.02f;

                if (opacity < 0)
                    opacity = 0;
            }
            else if (opacity < 1.2f && !Downed) {
                opacity += 0.02f;
            }

            if (shake > 0) {
                shake -= 0.01f;

                if (shake <= 0)
                    shake = 0;
            }

            if (animations.TryGetValue(currentAnimation, out var anim)) {
                anim.position = position;
                anim.depth = depth;
                anim.color = Downed ? Color.Crimson : Color.White;
                anim.opacity = opacity;
                anim.rotation = rotation;
                anim.Update(this);

                if (anim.finished) {
                    anim.Reset();
                    currentAnimation = string.IsNullOrWhiteSpace(anim.nextAnimation) ? currentAnimation : anim.nextAnimation;
                }
            }

            foreach (Ability ability in abilities) {
                ability.AdjustCosts(this);
            }
        }

        public bool Downed => Stats.HP <= 0;

        public Vector2 BattleStation {
            get => battleStation;
            set {
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

        public void AddEffect<T>(T effect) where T : StatusEffect
        {
            this.statusEffects.Add(effect);
            effect.OnApply(this);
        }

        public void AddReplaceEffect<T>(T effect) where T : StatusEffect
        {
            if (this.HasStatus(effect.name)) {
                this.RemoveAllEffects(effect.name);
            }

            this.AddEffect(effect);
        }

        public void RemoveEffect(StatusEffect effect)
        {
            statusEffects.Remove(effect);
            effect.OnRemove(this);
        }

        public void ClearEffects()
        {
            foreach (var effect in statusEffects) {
                effect.OnRemove(this);
            }

            statusEffects.Clear();
        }

        public void RemoveAllEffects<T>() where T : StatusEffect
        {
            for (int i = statusEffects.Count - 1; i >= 0; i--) {
                if (statusEffects[i] is T) {
                    statusEffects[i].OnRemove(this);
                    statusEffects.RemoveAt(i);
                }
            }
        }

        public void RemoveAllEffects(string name)
        {
            for (int i = statusEffects.Count - 1; i >= 0; i--) {
                if (statusEffects[i].name == name) {
                    statusEffects[i].OnRemove(this);
                    statusEffects.RemoveAt(i);
                }
            }
        }

        public bool HasStatus<T>() where T : StatusEffect
        {
            return statusEffects.Any(x => x is T);
        }

        public bool HasStatus(Type type)
        {
            return statusEffects.Any(x => x.GetType() == type);
        }

        public bool HasStatus(string name)
        {
            return statusEffects.Any(x => x.name == name);
        }

        public void ExtendEffect<T>(T effect) where T : StatusEffect
        {
            statusEffects.First(x => x.name == effect.name).turnsLeft += effect.turnsLeft;
        }

        public Unit GetCopy()
        {
            Unit copy = new();
            copy.name = name;
            copy.sprite = sprite;
            copy.portrait = portrait;
            copy.portraitCombat = portraitCombat;
            copy.ai = ai;
            copy.animations = [];
            copy._stats = _stats.GetCopy();
            copy.statsGrowth = statsGrowth.GetCopy();
            copy.resistances = resistances.GetCopy();
            copy.currentAnimation = currentAnimation;
            copy.defaultAnimation = defaultAnimation;

            foreach (var ab in learnableAbilities) {
                FieldInfo[] fields = ab.abilityToLearn.GetType().GetFields();
                Ability finalAbility = (Ability)Activator.CreateInstance(ab.abilityToLearn.GetType());
                foreach (var field in fields) {
                    field.SetValue(finalAbility, field.GetValue(finalAbility));
                }
                copy.learnableAbilities.Add(new(ab.requiredLevel, finalAbility));


            }

            foreach (var ab in loot) {
                FieldInfo[] fields = ab.item.GetType().GetFields();
                Item finalItem = (Item)Activator.CreateInstance(ab.item.GetType());
                foreach (var field in fields) {
                    field.SetValue(finalItem, field.GetValue(ab.item));
                }
                copy.loot.Add(new (ab.chance, finalItem, ab.min, ab.max));
            }

            copy.TryLearnLevelUpAbilites();

            foreach (var anim in animations) {
                copy.animations.Add(anim.Key, anim.Value.GetCopy());
            }

            foreach (var ab in abilities) {
                FieldInfo[] fields = ab.GetType().GetFields();
                Ability finalAbility = (Ability)Activator.CreateInstance(ab.GetType());
                foreach (var field in fields) {
                    field.SetValue(finalAbility, field.GetValue(ab));
                }
                copy.abilities.Add(finalAbility);


            }

            return copy;
        }

        public void Learns(int atLevel, Ability abilityToLearn)
        {
            learnableAbilities.Add(new(atLevel, abilityToLearn));

            learnableAbilities.Sort(Compare);
        }

        public void Drops(int chance, Item item, int min = 1, int max = 1)
        {
            loot.Add(new(chance, item, min, max));
        }

        public int Compare(LearnableAbility a, LearnableAbility b) 
        {
            if(a.requiredLevel > b.requiredLevel)
                return 1;
            if(a.requiredLevel < b.requiredLevel) 
                return -1;
            return 0;
        }

    }
}

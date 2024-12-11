using HellTrail.Core.Combat.Sequencer;
using HellTrail.Core.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Abilities
{
    public class Ability : ICanTarget
    {
        public string Name;
        public string Description;
        public bool aoe;
        public ValidTargets canTarget;
        public int hpCost;
        public int spCost;

        public Ability(string name, string description)
        {
            aoe = false;
            Name = name;
            Description = description;
            canTarget = ValidTargets.Active;
        }

        public void Use(Unit caster, Battle battle, List<Unit> targets, bool useBaseStats = false)
        {
            AdjustCosts(caster);
            if (!CanCast(caster))
                return;

            UIManager.combatUI.showUsedAbilityTime = 90;
            UIManager.combatUI.usedAbilityPanel.Visible = true;
            UIManager.combatUI.usedAbilityText.text = Name;
            if (!useBaseStats)
                UseAbility(caster, battle, targets);
            else
            {
                var copy = caster.GetCopy();
                copy.SetBaseStats(caster.baseStats);
                UseAbility(copy, battle, targets);
            }
            battle.lastUsedAbility = this;

            caster.Stats.HP -= hpCost;
            caster.Stats.SP -= spCost;
        }

        public bool CanCast(Unit caster)
        {
            if ((hpCost > 0 && caster.Stats.HP <= hpCost) || (spCost > 0 && caster.Stats.SP < spCost))
                return false;

            return true;
        }

        public virtual void AdjustCosts(Unit caster)
        {

        }

        protected virtual void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {

        }

        public Sequence CreateSequence(Battle battle)
        {
            var seq = battle.CreateSequence(true);
            seq.source = GetType().ToString();
            return seq;
        }

        public ValidTargets CanTarget() => canTarget;

        public bool AoE() => aoe;
    }
}

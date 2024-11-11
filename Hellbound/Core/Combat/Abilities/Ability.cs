using HellTrail.Core.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Abilities
{
    public class Ability
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

        public void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            AdjustCosts(caster);
            if (!CanCast(caster))
                return;

            UIManager.combatUI.showUsedAbilityTime = 90;
            UIManager.combatUI.usedAbilityPanel.Visible = true;
            UIManager.combatUI.usedAbilityText.text = Name;
            UseAbility(caster, battle, targets);

            caster.HP -= hpCost;
            caster.SP -= spCost;
        }

        public bool CanCast(Unit caster)
        {
            if ((hpCost > 0 && caster.HP <= hpCost) || (spCost > 0 && caster.SP < spCost))
                return false;

            return true;
        }

        public virtual void AdjustCosts(Unit caster)
        {

        }

        protected virtual void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {

        }
    }
}

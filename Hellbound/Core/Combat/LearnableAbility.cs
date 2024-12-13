using Casull.Core.Combat.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.Combat
{
    public class LearnableAbility
    {
        public int requiredLevel;

        public Ability abilityToLearn;

        public LearnableAbility(int requiredLevel, Ability abilityToLearn)
        {
            this.requiredLevel = requiredLevel;
            this.abilityToLearn = abilityToLearn;
        }
    }
}

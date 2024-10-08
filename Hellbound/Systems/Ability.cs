using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Systems
{
//{
//    public class Ability
//    {
//        public string Name;
//        public string Description;
//        public ValidTargets targetRange;
//        public int Accuracy;

//        public Ability(string name, string description, ValidTargets targetRange = ValidTargets.Any)
//        {
//            Accuracy = 100;
//            Name = name;
//            Description = description;
//            this.targetRange = targetRange;
//        }

//        public void AddStrategy(IAbilityStrategy strategy)
//        {
//            Strategies.Add(strategy);
//        }

//        public List<IAbilityStrategy> Strategies { get; } = new List<IAbilityStrategy>();

//        public void UseAbility(Unit caster, List<Unit> targets)
//        {
//            List<Unit> finalTargets = new List<Unit>();

//            foreach (Unit target in targets)
//            {
//                if(Accuracy > Main.rand.Next(100))
//                {
//                    finalTargets.Add(target);
//                }
//            }

//            foreach (IAbilityStrategy strategy in Strategies)
//            {
//                strategy.Apply(caster, finalTargets);
//            }
//        }
//    }
}

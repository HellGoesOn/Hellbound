using HellTrail.Core.Combat.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public static class EnemyDefinitions
    {
        private static Dictionary<string, Unit> _definitions = [];

        public static void DefineEnemies()
        {
            Unit slime = new()
            {
                name = "Slime",
                ai = new BasicAI()
            };
            slime.resistances[ElementalType.Phys] = 0.20f;
            slime.resistances[ElementalType.Fire] = -0.5f;
            var ooze = new BasicAttack()
            {
                Name = "Ooze",
                baseDamage = 10
            };
            slime.abilities.Add(ooze);

            DefineEnemy(slime);
        }

        private static void DefineEnemy(Unit unit) => _definitions.Add(unit.name, unit);

        public static Unit GetDefinedEnemy(string name) => _definitions[name].GetCopy();

    }
}

using HellTrail.Core.Combat.Abilities;
using HellTrail.Core.Combat.Abilities.Fire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public static class UnitDefinitions
    {
        private static Dictionary<string, Unit> _definitions = [];
        public static void DefineUnits()
        {
            Unit protag = DefineUnit("Doorkun");
            protag.name = "Doorkun";
            protag.sprite = "Dumbass";

            protag.abilities.Add(new BasicAttack());
            protag.abilities.Add(new Agi());
            protag.abilities.Add(new Maragi());
            protag.abilities.Add(new Dia());
            protag.abilities.Add(new Sukukaja());
            protag.abilities.Add(new Sukunda());
            protag.statsGrowth = new CombatStats(0.5f, 1.5f, 10, 7, 0.15f);
            protag.stats.magic = 100;
            protag.stats.speed = 7;
            GlobalPlayer.ProtagAnimations(protag);

            Unit slime = DefineUnit("Slime");
            slime.name = "Slime";
            slime.ai = new BasicAI();
            slime.resistances[ElementalType.Phys] = 0.20f;
            slime.resistances[ElementalType.Fire] = -0.5f;
            var ooze = new BasicAttack()
            {
                Name = "Ooze",
                baseDamage = 10
            };
            slime.abilities.Add(ooze);
        }

        private static Unit DefineUnit(string name)
        {
            Unit unit = new();
            _definitions.Add(name, unit);
            return _definitions[name];
        }

        public static Unit Get(string name) => _definitions[name].GetCopy();

    }
}

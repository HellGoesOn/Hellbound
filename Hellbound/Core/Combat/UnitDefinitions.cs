using HellTrail.Core.Combat.Abilities;
using HellTrail.Core.Combat.Abilities.Fire;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public static class UnitDefinitions
    {
        private readonly static Dictionary<string, Unit> _definitions = [];
        public static void DefineUnits()
        {
            Unit protag = DefineUnit("Doorkun");
            protag.name = "Doorkun";
            protag.sprite = "Dumbass";
            protag.portrait = "MCPortrait";
            protag.portraitCombat = "MCPortrait_Combat";
            protag.resistances[ElementalType.Elec] = 0.5f; 
            protag.resistances[ElementalType.Wind] = -0.5f; 

            protag.abilities.Add(new BasicAttack());
            protag.abilities.Add(new Agi());
            protag.abilities.Add(new Dia());
            protag.statsGrowth = new CombatStats(0.5f, 1.5f, 10, 7, 0.15f);
            protag.Stats.speed = 7;
            GlobalPlayer.ProtagAnimations(protag);

            Unit peas = DefineUnit("Peas");
            peas.sprite = "Peas";
            peas.name = "Peas";
            peas.ai = new BasicAI();

            peas.resistances = new ElementalResistances(1f, 1f, 1f, 1f, 1f, 0f);
            peas.BattleStation = new Vector2(90, 90);

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
            Unit dog = DefineUnit("Dog");
            dog.resistances[ElementalType.Almighty] = -1.0f;
            dog.name = "Dog";
            dog.sprite = "WhatDaDogDoin2";
            dog.portrait = "DogPortrait2";
            dog.portraitCombat = "DogPortrait_Combat";
            dog.ai = new BasicAI();
            dog.abilities.Add(new BasicAttack()
            {
                Name = "Bite",
                baseDamage = 10
            });
            dog.abilities.Add(new Agi());
            dog.abilities.Add(new Maragi());
            dog.abilities.Add(new Dia());
            dog.animations.Add("Idle", new SpriteAnimation("WhatDaDogDoin2", [new FrameData(0, 0, 32, 32)])
            {
            });
            dog.animations.Add("Victory", new SpriteAnimation("WhatDaDogDoin2", [new FrameData(0, 0, 32, 32)])
            {
                onAnimationPlay = (_, f)
                =>
                {
                    f.position = f.BattleStation + new Vector2(0, (float)Math.Cos(Main.totalTime * 1.5f));
                    _.scale = new Vector2((float)Math.Sin(Main.totalTime * 2), 1);
                }
            });
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

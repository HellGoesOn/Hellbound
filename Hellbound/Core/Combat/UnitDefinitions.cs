using Casull.Core.Combat.Abilities;
using Casull.Core.Combat.Abilities.Fire;
using Casull.Core.Combat.Items.Consumables;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat
{
    public static partial class UnitDefinitions
    {
        private readonly static Dictionary<string, Unit> _definitions = [];
        public static void DefineUnits()
        {
            Unit placeHolder = DefineUnit("Dud");
            placeHolder.Stats.HP = 0;
            placeHolder.Stats.value = 0;

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
            protag.Stats.speed = 6.5f;

            protag.Learns(2, new Sukukaja());

            GlobalPlayer.ProtagAnimations(protag);

            Unit peas = DefineUnit("Peas");
            peas.sprite = "Peas";
            peas.name = "Peas";
            peas.ai = new BasicAI();

            peas.resistances = new ElementalResistances(1f, 1f, 1f, 1f, 1f, 0f);
            peas.BattleStation = new Vector2(90, 90);

            Unit slime = DefineUnit("Slime");
            slime.Stats.MaxHP = 40;
            slime.name = "Slime";
            slime.ai = new BasicAI();
            slime.resistances[ElementalType.Phys] = 0.20f;
            slime.resistances[ElementalType.Fire] = -0.5f;
            var ooze = new BasicAttack() {
                Name = "Ooze",
                baseDamage = 10
            };
            slime.abilities.Add(ooze);

            slime.Drops(100, new Tomato(), 1, 3);
            slime.Drops(50, new DragonSlayer());

            Unit EXPSlime = DefineUnit("EXPSlime");
            EXPSlime.name = "Weird Slime";
            EXPSlime.ai = new BasicAI();
            EXPSlime.resistances[ElementalType.Phys] = 0.20f;
            EXPSlime.resistances[ElementalType.Fire] = -50f;
            EXPSlime.abilities.Add(ooze);
            EXPSlime.Stats.value = 10000;


            Unit dog = DefineUnit("Dog");
            dog.Learns(3, new Sukunda());
            dog.resistances[ElementalType.Almighty] = -1.0f;
            dog.name = "Dog";
            dog.sprite = "WhatDaDogDoin2";
            dog.portrait = "DogPortrait2";
            dog.portraitCombat = "DogPortrait_Combat";
            dog.ai = new BasicAI();
            dog.abilities.Add(new BasicAttack() {
                Name = "Bite",
                baseDamage = 10
            });
            dog.abilities.Add(new Agi());
            dog.abilities.Add(new Dia());
            dog.animations.Add("Idle", new SpriteAnimation("WhatDaDogDoin2", [new FrameData(0, 0, 32, 32)]) {
            });
            dog.animations.Add("Victory", new SpriteAnimation("WhatDaDogDoin2", [new FrameData(0, 0, 32, 32)]) {
                onAnimationPlay = (_, f)
                => {
                    f.position = f.BattleStation + new Vector2(0, (float)Math.Cos(Main.totalTime * 1.5f));
                    _.scale = new Vector2((float)Math.Sin(Main.totalTime * 2), 1);
                }
            });


            dog.SetLevel(3);
            DefineSunFlowerBoss();
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

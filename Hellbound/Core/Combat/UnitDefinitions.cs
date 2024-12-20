using Casull.Core.Combat.Abilities;
using Casull.Core.Combat.Abilities.Fire;
using Casull.Core.Combat.Abilities.Phys;
using Casull.Core.Combat.Abilities.Wind;
using Casull.Core.Combat.AI;
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

            Unit bird = DefineUnit("Bird");
            bird.sprite = "Chimken";
            bird.name = "Eagle";
            bird.Stats.MaxHP = 32;
            bird.Stats.speed = 7.5f;
            bird.ai = new SequentialAI();
            bird.abilities.AddRange([new AK47(), new Garu() {
                spCost = 0,
                baseDamage = 8
            }, new BasicAttack()
            {
                name = "Perch",
                baseDamage = 8
            }]);
            bird.resistances[ElementalType.Fire] = -0.5f;

            var birdAnim = new SpriteAnimation("Chimken", [new(0, 0, 32, 32)]);
            birdAnim.onAnimationPlay = (sender, unit) => {
                unit.position.Y += (float)Math.Cos(Main.totalTime*3);
            }; 
            
            var birdBasicAttack = new SpriteAnimation("Chimken", [new(0, 0, 32, 32), new(0, 0, 32, 32), new(0, 0, 32, 32), new(0, 0, 32, 32), new(0, 0, 32, 32)]);
            birdBasicAttack.timePerFrame = 20;
            birdBasicAttack.looping = false;
            birdBasicAttack.nextAnimation = "Idle";
            birdBasicAttack.onAnimationPlay = (sender, unit) => {
                if (sender.currentFrame == 0) {
                    unit.position.Y -= 2f;
                    unit.position.X -= Math.Sign(unit.scale.X) * 2f;
                    return;
                }
                if (sender.currentFrame != sender.FrameCount) {
                    unit.position.Y += 2f;
                    unit.position.X += Math.Sign(unit.scale.X) * 4f;
                }
            };

            bird.defaultAnimation = "Idle";
            bird.animations.Add("Idle", birdAnim);
            bird.animations.Add("BasicAttack", birdBasicAttack);

            bird.Drops(50, new ChocolateBar());

            Unit protag = DefineUnit("Doorkun");
            protag.name = "Doorkun";
            protag.sprite = "Dumbass";
            protag.portrait = "MCPortrait";
            protag.portraitCombat = "MCPortrait_Combat";
            protag.resistances[ElementalType.Elec] = 0.5f;
            protag.resistances[ElementalType.Wind] = -0.5f;

            protag.abilities.Add(new BasicAttack());
            protag.statsGrowth = new CombatStats(0.5f, 1.5f, 10, 7, 0.15f);
            protag.Stats.speed = 6.5f;

            protag.Learns(2, new Disturb() {
                name = "War Cry",
            });
            protag.Learns(4, new Agi());
            protag.Learns(6, new Maragi());
            protag.Learns(20, new Singularity());

            GlobalPlayer.ProtagAnimations(protag);

            Unit peas = DefineUnit("Peas");
            peas.sprite = "Peas";
            peas.name = "Peas";
            peas.ai = new BasicAI();

            peas.resistances = new ElementalResistances(1f, 1f, 1f, 1f, 1f, 0f);
            peas.BattleStation = new Vector2(90, 90);

            Unit slime = DefineUnit("Slime");
            slime.Stats.MaxHP = 36;
            slime.name = "Slime";
            slime.ai = new BasicAI();
            slime.resistances[ElementalType.Phys] = 0.20f;
            slime.resistances[ElementalType.Fire] = -0.5f;
            var ooze = new BasicAttack() {
                name = "Ooze",
                baseDamage = 6
            };
            slime.abilities.Add(ooze);

            slime.Drops(100, new Tomato(), 1, 1);

            Unit EXPSlime = DefineUnit("EXPSlime");
            EXPSlime.name = "Weird Slime";
            EXPSlime.ai = new BasicAI();
            EXPSlime.resistances[ElementalType.Phys] = 0.20f;
            EXPSlime.resistances[ElementalType.Fire] = -50f;
            EXPSlime.abilities.Add(ooze);
            EXPSlime.Stats.value = 10000;


            Unit dog = DefineUnit("Dog");

            dog.Learns(4, new Sukunda());

            dog.resistances[ElementalType.Almighty] = -1.0f;
            dog.name = "Dog";
            dog.sprite = "WhatDaDogDoin2";
            dog.portrait = "DogPortrait2";
            dog.portraitCombat = "DogPortrait_Combat";
            dog.ai = new BasicAI();
            dog.abilities.Add(new BasicAttack() {
                name = "Bite",
                baseDamage = 10
            });
            dog.abilities.Add(new Garu());
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

        private static Unit DefineUnit(string name, string internalName = null)
        {
            Unit unit = new();
            internalName ??= name;
            unit.internalName = internalName;
            _definitions.Add(name, unit);
            return _definitions[name];
        }

        public static Unit Get(string name) => _definitions[name].GetCopy();

    }
}

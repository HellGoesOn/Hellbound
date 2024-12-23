using Casull.Core.Combat.Abilities;
using Casull.Core.Combat.Abilities.Fire;
using Casull.Core.Combat.Abilities.Phys;
using Casull.Core.Combat.Abilities.Wind;
using Casull.Core.Combat.Items;
using Casull.Core.Combat.Items.Consumables;
using Casull.Core.Overworld;
using Casull.Core.UI;
using Microsoft.Xna.Framework;
using System.Text;
using System.Text.RegularExpressions;
using Casull.Extensions;
using Casull.Core.Graphics;
using System.Globalization;
using System.Reflection;

namespace Casull.Core.Combat
{
    public static class GlobalPlayer
    {
        private static List<Unit> party = [];
        private readonly static List<Item> _items = [];

        public static List<Unit> ActiveParty { get => party; set => party = value; }

        public readonly static List<CombatStats> preBattleStats = [];

        public static List<Item> Inventory => _items;

        public static Unit Copy(string name) => UnitDefinitions.Get(name);

        public static void Init()
        {
            ActiveParty.Clear();
            Inventory.Clear();
            preBattleStats.Clear();

            Unit protag = Copy("Doorkun");
            protag.name = Main.newSlotName;

            //ProtagAnimations(protag);

            protag.resistances[ElementalType.DoT] = -0.5f;


            AddPartyMember(protag);
            //ActiveParty.Add(sidekick);

            AddItem(new ChocolateBar() { count = 3});
            AddItem(new AdrenalineShot() { count = 3 });
        }

        public static void AddPartyMember(Unit newUnit)
        {
            ActiveParty.Add(newUnit);
            newUnit.ai = null;
            DefaultBattleStations(ActiveParty);
        }

        public static void DefaultBattleStations(List<Unit> units)
        {
            for (int i = units.Count - 1; i >= 0; i--) {
                Unit unit = units[units.Count - 1 - i];
                unit.BattleStation = new Vector2(70 + 4 * i + 32 * (i % 2), 80 + 16 * i - 24 * (i % 2));
            }
        }

        public static void AddItem(Item newItem)
        {
            if (_items.Any(x => x.name == newItem.name && x.count + newItem.count <= x.maxCount)) {
                var item = _items.Find(x => x.name == newItem.name && x.count + newItem.count <= x.maxCount);
                item.count += newItem.count;
            }
            else {
                _items.Add(newItem);
            }

            newItem.OnObtain();

            UIManager.overworldUI.Notify($"Found <FFFF00/{newItem.name}> x {newItem.count}", Color.White, 120);
        }

        public static void ResetToPrebattle()
        {
            for(int i = 0; i < ActiveParty.Count; i++) {
                var unit = ActiveParty[i];
                unit.Stats.MaxHP = preBattleStats[i].MaxHP;
                unit.Stats.HP = preBattleStats[i].HP;
                unit.Stats.MaxSP = preBattleStats[i].MaxSP;
                unit.Stats.SP = preBattleStats[i].SP;
                unit.Stats.EXP = preBattleStats[i].EXP;
                unit.Stats.toNextLevel = preBattleStats[i].toNextLevel;
                unit.Stats.level = preBattleStats[i].level;
                unit.Stats.accuracy = preBattleStats[i].accuracy;
            }

            
        }

        public static void Update()
        {
            _items.RemoveAll(x => x.count <= 0);

            foreach(var unit in ActiveParty) {
                if (unit.Stats.HP > 0 && unit.CurrentAnimation == "Dead")
                    unit.CurrentAnimation = "Idle";
            }
        }

        // to do: create json file, pull from there instead
        public static void ProtagAnimations(Unit f)
        {
            SpriteAnimation idle = new("MC_CombatIdle",
                [
                new FrameData(0, 0, 32, 32),
                new FrameData(0, 32, 32, 32),
                ]
                );
            idle.looping = true;
            idle.timePerFrame = 20;

            SpriteAnimation dead = new("MC_Death",
                [
                new FrameData(0, 0, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 32, 32, 32),
                ]
                );
            dead.looping = false;
            dead.timePerFrame = 20;
            dead.nextAnimation = "Dead";

            SpriteAnimation victoryPose = new("VictoryPose", [
                new FrameData(0, 0, 32, 32),
                new FrameData(0, 32, 32, 32),
                ]);

            SpriteAnimation physAttack = new("MC_BasicAttack",
                [new(0, 0, 32, 32),
                new(0, 0, 32, 32),
                new(0, 0, 32, 32),
                new(0, 32, 32, 32),
            new(0, 64, 32, 32),
            new(0, 64, 32, 32),
            new(0, 64, 32, 32),
            new(0, 64, 32, 32),
            new(0, 64, 32, 32),
            new(0, 96, 32, 32),
            new(0, 128, 32, 32),
            new(0, 160, 32, 32),
            new(0, 160, 32, 32),
            new(0, 160, 32, 32),
            new(0, 160, 32, 32),
            new(0, 160, 32, 32),
            ]) {
                timePerFrame = 3,
                nextAnimation = "Idle"
            };

            SpriteAnimation recoil = new("MC_Recoil",
                [
                new(0,0,32,32),
                new(0,32,32,32),
                new(0,32,32,32),
                new(0,64,32,32),
                new(0,64,32,32),
                new(0,64,32,32),
                new(0,64,32,32),
                new(0,64,32,32),
                ]
                ) {
                timePerFrame = 4,
                nextAnimation = "Idle"
            };

            SpriteAnimation combatVictory = new("MC_CombatVictory",
                [new(0, 0, 32, 32),
                new(0, 0, 32, 32),
                new(0, 0, 32, 32),
                new(0, 0, 32, 32),
                new(0, 32, 32, 32),
            new(0, 64, 32, 32),
            new(0, 64, 32, 32),
            new(0, 64, 32, 32),
            new(0, 96, 32, 32),
            new(0, 96, 32, 32),
            new(0, 128, 32, 32),
            new(0, 160, 32, 32),
            new(0, 128, 32, 32),
            new(0, 160, 32, 32),
            new(0, 128, 32, 32),
            new(0, 160, 32, 32),
            new(0, 128, 32, 32),
            new(0, 160, 32, 32),
            new(0, 128, 32, 32),
            new(0, 160, 32, 32),
            new(0, 128, 32, 32),
            new(0, 160, 32, 32),
            new(0, 128, 32, 32),
            new(0, 160, 32, 32),
            new(0, 128, 32, 32),
            new(0, 160, 32, 32),
            new(0, 128, 32, 32),
            new(0, 160, 32, 32),
            ]) {
                timePerFrame = 3,
                nextAnimation = "Victory",
                looping = true,
                onAnimationPlay = (sender, unit) => {
                    if (sender.currentFrame >= 26) {
                        sender.currentFrame = 6;
                        }
                }
            };
            //victoryPose.onAnimationPlay += (_) =>
            //{
            //    _.scale = new Vector2((float)Math.Sin(Main.totalTime * 2), 1f);
            //};
            victoryPose.looping = true;
            victoryPose.timePerFrame = 10;
            SpriteAnimation flipOff = new("FlipOff",
                [
                new FrameData(0, 0, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 64, 32, 32),
                new FrameData(0, 64, 32, 32),
                new FrameData(0, 64, 32, 32),
                new FrameData(0, 64, 32, 32),
                new FrameData(0, 96, 32, 32),
                new FrameData(0, 96, 32, 32),
                new FrameData(0, 96, 32, 32),
                new FrameData(0, 96, 32, 32),
                new FrameData(0, 96, 32, 32),
                new FrameData(0, 96, 32, 32),
                new FrameData(0, 64, 32, 32),
                ]) {
                timePerFrame = 5,
                nextAnimation = "Idle"
            };
            SpriteAnimation special = new("EndLife",
                [
                new(0, 0, 32, 32),
                new(0, 32, 32, 32),
                new(0, 32, 32, 32),
                new(0, 32, 32, 32),
                new(0, 32, 32, 32),
                new(0, 32, 32, 32),
                new(0, 32, 32, 32),
                new(0, 64, 32, 32),
                new(0, 96, 32, 32),
                new(0, 96, 32, 32),
                new(0, 96, 32, 32),
                new(0, 96, 32, 32),
                new(0, 128, 32, 32),
                new(0, 160, 32, 32),
                new(0, 160, 32, 32),
                new(0, 128, 32, 32),
                new(0, 128, 32, 32),
                new(0, 128, 32, 32),
                new(0, 128, 32, 32),
                new(0, 64, 32, 32),
                new(0, 0, 32, 32),
                ]) {
                timePerFrame = 6,
                nextAnimation = "Idle",
                onAnimationPlay = (_, f) => {
                    Color[] clrs = { Color.Blue, Color.Cyan, Color.Turquoise, Color.LightBlue };

                    for (int i = 0; i < 3; i++) {
                        int xx = Main.rand.Next((int)(f.size.X * 0.5f));
                        int yy = Main.rand.Next((int)(f.size.Y));
                        float velX = Main.rand.Next(60, 120) * 0.001f * (Main.rand.Next(2) == 0 ? -1 : 1);
                        float velY = -0.2f * (Main.rand.Next(20, 60) * 0.05f);
                        var particle = ParticleManager.NewParticleAdditive(new Vector3(f.position + new Vector2(-f.size.X * 0.25f + xx, f.size.Y * 0.5f), 0), new Vector3(velX, 0, velY), 60);

                        particle.color = clrs[Main.rand.Next(clrs.Length)];
                        particle.endColor = Color.Black;
                        particle.degradeSpeed = 0.01f;
                        particle.dissapateSpeed = 0.01f;
                        particle.scale = Vector2.One * Main.rand.Next(1, 3);
                    }

                    if (_.currentFrame == 13) {
                        _.currentFrame = 14;
                        for (int i = 0; i < 45; i++) {
                            int xx = Main.rand.Next((int)(f.size.X * 0.5f));
                            int yy = Main.rand.Next((int)(f.size.Y));
                            float velX = Main.rand.Next(55, 135) * 0.01f;
                            float velY = -0.2f * (Main.rand.Next(0, 24) * 0.05f) * (Main.rand.Next(2) == 0 ? -1 : 1);
                            var particle = ParticleManager.NewParticleAdditive(new Vector3(f.position + new Vector2(-f.size.X * 0.15f, -f.size.Y * 0.35f), 0), new Vector3(velX, velY, 0), 60);
                            particle.color = clrs[Main.rand.Next(clrs.Length)];
                            particle.endColor = Color.Black;
                            particle.degradeSpeed = 0.01f;
                            particle.dissapateSpeed = 0.01f;
                            particle.weight = 0.06f;
                            particle.scale = Vector2.One * Main.rand.Next(1, 3);
                        }
                    }
                }
            };
            f.animations.Add("Idle", idle);
            f.animations.Add("Cast", flipOff);
            f.animations.Add("BasicAttack", physAttack);
            f.animations.Add("Victory", combatVictory);
            f.animations.Add("Special", special);
            f.animations.Add("Recoil", recoil);
            f.animations.Add("Dead", dead);
        }

        public static void SaveProgress()
        {
            var oldCult = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"World: {{[{UIManager.combatUI.tutorialProgress}][{Main.currentZone}][{Main.lastTransitionPosition}]}}");

            sb.AppendLine("Flags: {");

            foreach(string flag in World.flags) {
                sb.AppendLine($"{flag}");
            }
            sb.AppendLine("}");

            sb.AppendLine("Party: {");
            foreach(Unit unit in ActiveParty) {
                sb.AppendLine($"[{unit.internalName}][{unit.name}][{unit.Stats.totalEXP}][{unit.Stats.HP}][{unit.Stats.SP}]");
            }
            sb.AppendLine("}");

            sb.AppendLine("Inventory: {");
            foreach(Item item in Inventory) {
                sb.AppendLine($"[{item.GetType().Name}][{item.count}]");
            }
            sb.AppendLine("}");

            if (!Directory.Exists(Environment.CurrentDirectory + "\\SaveData")) {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\SaveData");
            }

            File.WriteAllText(Environment.CurrentDirectory + $"\\SaveData\\Save{Main.saveSlot}.sdt", sb.ToString());

            Thread.CurrentThread.CurrentCulture = oldCult;
        }

        public static void LoadProgress(int slot)
        {
            var oldCult = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            if (!File.Exists(Environment.CurrentDirectory + $"\\SaveData\\Save{slot}.sdt")) {
                return;
            }

            string text = File.ReadAllText(Environment.CurrentDirectory + $"\\SaveData\\Save{slot}.sdt");

            var entries = text.Split("}" + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (entries.Length < 4)
                return;

            string worldValues = entries[0];
            var worldMatches = Regex.Matches(worldValues, @"\[(.+?)\]");

            UIManager.combatUI.tutorialProgress = int.Parse(worldMatches[0].Groups[1].Value);
            Main.lastTransitionPosition = new Vector2().FromString(worldMatches[2].Groups[1].Value);
            Main.currentZone = worldMatches[1].Groups[1].Value;

            World.flags.Clear();

            List<string> flags = [.. entries[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
            flags.RemoveAt(0);

            foreach (var flag in flags) {
                World.RaiseFlag(flag);
            }

            ActiveParty.Clear();

            string[] characters = entries[2].Substring(8+Environment.NewLine.Length).Split(Environment.NewLine);

            foreach(string character in characters) {
                var matches = Regex.Matches(character, @"\[(.+?)\]");
                Unit unit = UnitDefinitions.Get(matches[0].Groups[1].Value);
                unit.name = matches[1].Groups[1].Value.Replace("\"", "");

                unit.SetExp(uint.Parse(matches[2].Groups[1].Value));
                unit.Stats.HP = int.Parse(matches[3].Groups[1].Value);
                unit.Stats.SP = int.Parse(matches[4].Groups[1].Value);
                unit.ai = null;
                ActiveParty.Add(unit);
            }

            Inventory.Clear();
            string[] items = entries[3].Substring(12 + Environment.NewLine.Length).Split(Environment.NewLine);

            foreach (var item in items) {
                var matches = Regex.Matches(item, @"\[(.+?)\]");
                var itemTypeName = matches[0].Groups[1].Value;
                var itemType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name.Contains(itemTypeName));
                var madeItem = Activator.CreateInstance(itemType);
                madeItem.GetType().GetField("count").SetValue(madeItem, int.Parse(matches[1].Groups[1].Value));

                Inventory.Add((Item)madeItem);
            }


            DefaultBattleStations(ActiveParty);
            Thread.CurrentThread.CurrentCulture = oldCult;
        }
    }
}

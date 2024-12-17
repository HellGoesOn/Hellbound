using Casull.Core.Combat.Abilities;
using Casull.Core.Combat.Abilities.Fire;
using Casull.Core.Combat.Abilities.Phys;
using Casull.Core.Combat.Abilities.Wind;
using Casull.Core.Combat.Items;
using Casull.Core.Combat.Items.Consumables;
using Casull.Core.UI;
using Microsoft.Xna.Framework;
using Treeline.Core.Graphics;

namespace Casull.Core.Combat
{
    public static class GlobalPlayer
    {
        private static List<Unit> party = [];
        private readonly static List<Item> _items = [];

        public static List<Unit> ActiveParty { get => party; set => party = value; }

        public static List<CombatStats> preBattleStats = [];

        public static List<Item> Inventory => _items;

        public static Unit Copy(string name) => UnitDefinitions.Get(name);

        public static void Init()
        {
            Unit protag = Copy("Doorkun");

            protag.animations.Clear();

            ProtagAnimations(protag);

            protag.resistances[ElementalType.DoT] = -0.5f;


            AddPartyMember(protag);
            //ActiveParty.Add(sidekick);

            AddItem(new ChocolateBar() { count = 3});
            AddItem(new AdrenalineShot() { count = 3});
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
                unit.Stats.HP = preBattleStats[i].HP;
                unit.Stats.MaxHP = preBattleStats[i].MaxHP;
                unit.Stats.SP = preBattleStats[i].SP;
                unit.Stats.MaxSP = preBattleStats[i].MaxSP;
                unit.Stats.EXP = preBattleStats[i].EXP;
                unit.Stats.toNextLevel = preBattleStats[i].toNextLevel;
                unit.Stats.level = preBattleStats[i].level;
                unit.Stats.accuracy = preBattleStats[i].accuracy;
            }

            
        }

        public static void Update()
        {
            _items.RemoveAll(x => x.count <= 0);
        }

        // to do: create json file, pull from there instead
        public static void ProtagAnimations(Unit mc)
        {
            SpriteAnimation idle = new("Dumbass_Idle",
                [
                new FrameData(0, 0, 32, 32),
                new FrameData(0, 32, 32, 32),
                new FrameData(0, 64, 32, 32)
                ]
                );
            idle.looping = true;
            idle.timePerFrame = 20;
            SpriteAnimation victoryPose = new("VictoryPose", [
                new FrameData(0, 0, 32, 32),
                new FrameData(0, 32, 32, 32),
                ]);
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
                        int xx = Main.rand.Next((int)(mc.size.X * 0.5f));
                        int yy = Main.rand.Next((int)(mc.size.Y));
                        float velX = Main.rand.Next(60, 120) * 0.001f * (Main.rand.Next(2) == 0 ? -1 : 1);
                        float velY = -0.2f * (Main.rand.Next(20, 60) * 0.05f);
                        var particle = ParticleManager.NewParticleAdditive(new Vector3(mc.position + new Vector2(-mc.size.X * 0.25f + xx, mc.size.Y * 0.5f), 0), new Vector3(velX, 0, velY), 60);

                        particle.color = clrs[Main.rand.Next(clrs.Length)];
                        particle.endColor = Color.Black;
                        particle.degradeSpeed = 0.01f;
                        particle.dissapateSpeed = 0.01f;
                        particle.scale = Vector2.One * Main.rand.Next(1, 3);
                    }

                    if (_.currentFrame == 13) {
                        _.currentFrame = 14;
                        for (int i = 0; i < 45; i++) {
                            int xx = Main.rand.Next((int)(mc.size.X * 0.5f));
                            int yy = Main.rand.Next((int)(mc.size.Y));
                            float velX = Main.rand.Next(55, 135) * 0.01f;
                            float velY = -0.2f * (Main.rand.Next(0, 24) * 0.05f) * (Main.rand.Next(2) == 0 ? -1 : 1);
                            var particle = ParticleManager.NewParticleAdditive(new Vector3(mc.position + new Vector2(-mc.size.X * 0.15f, -mc.size.Y * 0.35f), 0), new Vector3(velX, velY, 0), 60);
                            particle.color = clrs[Main.rand.Next(clrs.Length)];
                            particle.endColor = Color.Black;
                            particle.degradeSpeed = 0.01f;
                            particle.dissapateSpeed = 0.01f;
                            particle.weight = 0.06f;
                            particle.scale = Vector2.One * Main.rand.Next(1, 3);
                        }
                    }
                    GameOptions.MusicVolume *= 0.95f;
                },
                onAnimationEnd = (_, _) => {
                    GameOptions.MusicVolume = GameOptions.OldMusicVolume;
                }
            };
            mc.animations.Add("Idle", idle);
            mc.animations.Add("Cast", flipOff);
            mc.animations.Add("BasicAttack", flipOff);
            mc.animations.Add("Victory", victoryPose);
            mc.animations.Add("Special", special);
        }
    }
}

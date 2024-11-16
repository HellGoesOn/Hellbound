using HellTrail.Core.Combat.Abilities;
using HellTrail.Core.Combat.Abilities.Fire;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treeline.Core.Graphics;

namespace HellTrail.Core.Combat
{
    public static class GlobalPlayer
    {
        private static List<Unit> party = [];

        public static List<Unit> ActiveParty { get => party; set => party = value; }

        public static void Init()
        {
            Unit protag = new()
            {
                name = "Doorkun",
                sprite = "Dumbass",
            };
            protag.abilities.Add(new BasicAttack());
            protag.abilities.Add(new Agi());
            protag.abilities.Add(new Maragi());
            protag.abilities.Add(new Dia());
            protag.abilities.Add(new Sukukaja());
            protag.abilities.Add(new Sukunda());
            protag.statsGrowth = new CombatStats(0.5f, 1.5f, 10, 7, 0.15f);
            ProtagAnimations(protag);

            Unit sidekick = new()
            {
                name = "Dog",
                sprite = "WhatDaDogDoin2",
                ai = new BasicAI()
            };
            sidekick.abilities.Add(new BasicAttack()
            {
                Name = "Bite",
                baseDamage = 10
            });
            sidekick.abilities.Add(new Agi());
            sidekick.abilities.Add(new Maragi());
            sidekick.abilities.Add(new Dia());

            ActiveParty.Add(protag);
            //ActiveParty.Add(sidekick);

            int i = 0;
            foreach (Unit unit in ActiveParty)
            {
                unit.team = Team.Player;
                unit.stats.speed = 7;
                unit.BattleStation = new Vector2(60 + 4 * i, 80 + 32 * i);
                i++;
            }
        }

        // to do: create json file, pull from there instead
        public static void ProtagAnimations(Unit mc)
        {
            SpriteAnimation idle = new("DumbassIdle", 
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
                ])
            {
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
                ])
            {
                timePerFrame = 6,
                nextAnimation = "Idle",
                onAnimationPlay = (_) =>
                {
                    Color[] clrs = { Color.Blue, Color.Cyan, Color.Turquoise, Color.LightBlue };

                    for (int i = 0; i < 3; i++)
                    {
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

                    if(_.currentFrame == 13)
                    {
                        _.currentFrame = 14;
                        for (int i = 0; i < 45; i++)
                        {
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
                onAnimationEnd = (_) =>
                {
                    GameOptions.MusicVolume = GameOptions.OldMusicVolume;
                }
            };
            mc.animations.Add("Idle", idle);
            mc.animations.Add("Cast", flipOff);
            mc.animations.Add("Victory", victoryPose);
            mc.animations.Add("Special", special);
        }
    }
}

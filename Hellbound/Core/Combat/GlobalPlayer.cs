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
    public static class GlobalPlayer
    {
        private static List<Unit> party = [];

        public static List<Unit> ActiveParty { get => party; set => party = value; }

        public static void Init()
        {
            Unit protag = new()
            {
                name = "Doorkun",
                sprite = "Dumbass"
            };
            protag.resistances[ElementalType.Phys] = 0.2f;
            protag.resistances[ElementalType.Fire] = 0.8f;
            protag.resistances[ElementalType.DoT] = 1f;
            protag.abilities.Add(new MyriadTruths());
            protag.abilities.Add(new Agi());
            protag.abilities.Add(new Maragi());
            protag.abilities.Add(new Singularity());
            protag.abilities.Add(new Disturb());
            protag.abilities.Add(new Bite());
            protag.abilities.Add(new GhastlyWail());

            ProtagAnimations(protag);

            Unit sidekick = new()
            {
                name = "Dog",
                sprite = "WhatDaDogDoin2",
                ai = new BasicAI()
            };
            sidekick.abilities.Add(new Agi());
            sidekick.abilities.Add(new Megidolaon());

            ActiveParty.Add(protag);
            ActiveParty.Add(sidekick);

            int i = 0;
            foreach (Unit unit in ActiveParty)
            {
                unit.team = Team.Player;
                unit.Speed = 7;
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
            mc.animations.Add("Idle", idle);
            mc.animations.Add("Cast", flipOff);
            mc.animations.Add("Victory", victoryPose);
        }
    }
}

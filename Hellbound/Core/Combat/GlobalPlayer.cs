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

        public static List<Unit> Party { get => party; set => party = value; }

        public static void Init()
        {
            Unit protag = new()
            {
                name = "Doorkun",
                sprite = "Dumbass"
            };
            protag.abilities.Add(new GrandSeal());
            protag.abilities.Add(new Agi());
            protag.abilities.Add(new Singularity());

            ProtagAnimations(protag);

            Unit sidekick = new()
            {
                name = "Dog",
                sprite = "WhatDaDogDoin2",
                ai = new BasicAI()
            };
            sidekick.abilities.Add(new Megidolaon());

            Party.Add(protag);
            Party.Add(sidekick);

            int i = 0;
            foreach (Unit unit in Party)
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
            SpriteAnimation idle = new("Dumbass", [new FrameData(0, 0, 32, 32)]);
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
                ]);

            flipOff.timePerFrame = 5;

            mc.animations.Add("Idle", idle);
            mc.animations.Add("Cast", flipOff);
        }
    }
}

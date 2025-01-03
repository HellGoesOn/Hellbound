using Casull.Core.Combat.Abilities;
using Casull.Core.Combat.Abilities.Fire;
using Casull.Core.Combat.Abilities.Unique;
using Casull.Core.Combat.Abilities.Wind;
using Casull.Core.Combat.AI;
using Casull.Core.Combat.Items;
using Casull.Core.Combat.Items.Consumables;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.Combat
{
    public static partial class UnitDefinitions
    {
        public static void DefineSunFlowerBoss()
        {
            Unit sunFlower = DefineUnit("Sunflower");
            sunFlower.sprite = "Pixel";
            sunFlower.name = "Sun Gigas";

            const int frame = 48;

            var death = new SpriteAnimation("Sunflower_Dead",
                [
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, 0, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame * 2, frame, frame),
                ]
                ) {
                scale = new Vector2(-1, 1),
                timePerFrame = 4,
                looping = false
            };

            var kick = new SpriteAnimation("Sunflower_Kick",
                [
                new(0, 0, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, 0, frame, frame),
                ]
                ) {
                timePerFrame = 6,
                nextAnimation = "Idle"
            };

            kick.onAnimationPlay = (sender, unit) => {
                if (sender.currentFrame >= 3 && sender.currentFrame < sender.FrameCount-1)
                    unit.position.X += Math.Sign(unit.scale.X) * 4f;
            };

            var transformation = new SpriteAnimation("Sunflower_TrueIdle",
                [
                new(0, 0, frame, frame),
                new(0, frame, frame, frame),
                ]) {
                timePerFrame = 10,
                nextAnimation = "Idle",
                looping = true,
            };

            var cast = new SpriteAnimation("Sunflower_Cast",
                [
                new(0, 0, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 3, frame, frame),
                ]) {
                timePerFrame = 2,
                nextAnimation = "Idle",
            };

            cast.onAnimationPlay = (sender, _) => {
                if (sender.currentFrame == 3) {
                    SoundEngine.PlaySound("Blender");
                    sender.currentFrame++;
                }
            };

            var solarFlare = new SpriteAnimation("Sunflower_SolarFlare",
                [
                new(0, 0, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 3, frame, frame),
                ]) {
                timePerFrame = 6,
                nextAnimation = "Idle",
            };

            solarFlare.onAnimationPlay = (sender, unit) => {
                if (sender.currentFrame == 4) {
                    SoundEngine.PlaySound("SolarFlare");
                    sender.currentFrame++;
                }
            };

            sunFlower.animations.Add("Idle", transformation);
            sunFlower.animations.Add("Cast", cast);
            sunFlower.animations.Add("SolarFlare", solarFlare);
            sunFlower.animations.Add("Sunflower_Kick", kick);
            sunFlower.animations.Add("Dead", death);
            sunFlower.defaultAnimation = "Idle";
            sunFlower.size = new Vector2(48, 48);

            sunFlower.Stats.MaxHP = 2000;
            sunFlower.Stats.MaxSP = 18;
            sunFlower.Stats.value = 500;
            sunFlower.Stats.speed = 15;
            sunFlower.origin.Y = -12;

            sunFlower.abilities.Add(new SolarFlare() {
                spCost = 3,
                accuracy = 600,
                name = "Solar Flare"
            });

            sunFlower.abilities.Add(new Sukukaja() {
                spCost = 3,
                name = "Bob & Weave"
            });

            sunFlower.abilities.Add(new Garu() {
                name = "Wind Gust",
                baseDamage = 16,
            });

            sunFlower.abilities.Add(new SunflowerKick() {
                name = "Sunflower Kick",
                baseDamage = 24,
            });

            sunFlower.abilities.Add(new SunflowerKick() {
                name = "Sunflower Punch",
                baseDamage = 24,
            });

            sunFlower.abilities.Add(new Garu() {
                name = "Wind Gust",
                baseDamage = 16,
            });

            sunFlower.Drops(100, new SunflowerHead(), 1, 1);
            sunFlower.Drops(100, new Betsy(), 1, 1);

            sunFlower.ai = new SequentialAI();
        }
    }
}

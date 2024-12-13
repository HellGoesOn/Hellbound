using Casull.Core.Combat.Abilities;
using Casull.Core.Combat.AI;
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

            var transformation = new SpriteAnimation("Sunflower_TrueIdle",
                [
                new(0, 0, frame, frame),
                new(0, frame, frame, frame),
                ]) {
                timePerFrame = 10,
                nextAnimation = "Idle",
                looping = true,
            };

            var cast = new SpriteAnimation("Sunflower_SolarFlare",
                [
                new(0, 0, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 3, frame, frame),
                new(0, frame * 3, frame, frame),
                ]) {
                timePerFrame = 6,
                nextAnimation = "Idle",
            };

            var solarFlare = new SpriteAnimation("Sunflower_SolarFlare",
                [
                new(0, 0, frame, frame),
                new(0, frame, frame, frame),
                new(0, frame * 2, frame, frame),
                new(0, frame * 2, frame, frame),
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
                if (sender.currentFrame == 3) {
                    SoundEngine.PlaySound("SolarFlare");
                    sender.currentFrame++;
                }
            };

            sunFlower.animations.Add("Idle", transformation);
            sunFlower.animations.Add("Cast", cast);
            sunFlower.animations.Add("SolarFlare", solarFlare);
            sunFlower.defaultAnimation = "Idle";
            sunFlower.size = new Vector2(48, 48);

            sunFlower.Stats.MaxHP = 500;
            sunFlower.Stats.MaxSP = 150;
            sunFlower.Stats.value = 500;
            sunFlower.Stats.speed = 15;
            sunFlower.origin.Y = -12;

            sunFlower.abilities.Add(new SolarFlare() {
                spCost = 0,
                accuracy = 100,
                Name = "Solar Flare"
            });

            sunFlower.abilities.Add(new Sukukaja() {
                spCost = 0,
                Name = "Bob & Weave"
            });

            sunFlower.abilities.Add(new BasicAttack() {
                Name = "Wind Gust",
                elementalType = ElementalType.Wind,
                baseDamage = 10,
                accuracy = 75
            });

            sunFlower.abilities.Add(new BasicAttack() {
                Name = "Sunflower Kick",
                baseDamage = 12,
                accuracy = 50
            });

            sunFlower.abilities.Add(new BasicAttack() {
                Name = "Sunflower Kick",
                baseDamage = 12,
                accuracy = 50
            });

            sunFlower.Drops(100, new Tomato(), 10, 10);

            sunFlower.ai = new SequentialAI();
        }
    }
}

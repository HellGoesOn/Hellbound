using Casull.Core.Combat.Sequencer;
using Casull.Core.Combat.Status;
using Casull.Core.UI;
using Casull.Render;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Abilities
{
    public class SolarFlare : Ability
    {
        public SolarFlare() : base("Solar Flare", "Reduces accuracy of all Foes")
        {
            aoe = true;
            canTarget = ValidTargets.Enemy;
            elementalType = ElementalType.Support;
        }

        protected override void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {
            Sequence sequence = CreateSequence(battle);
            sequence.Add(new SetActorAnimation(caster, "SolarFlare"));
            sequence.Delay(60);

            SpriteAnimation flashBang = new SpriteAnimation("Pixel", [new FrameData(0, 0, Renderer.PreferedWidth, Renderer.PreferedHeight)]);
            flashBang.looping = true;
            flashBang.opacity = 4f;
            flashBang.color = Color.White;
            flashBang.onAnimationPlay = (sender, unit) => {

                sender.opacity *= 0.95f;

                if (sender.opacity <= 0.1f)
                    flashBang.finished = true;
            };

            flashBang.depth = 100;

            sequence.CustomAction(() => {
                //SoundEngine.PlaySound("SolarFlare");
                SoundEngine.PlaySound("Flash");
                battle.fieldAnimations.Add(flashBang);
                foreach (var target in targets) {
                    if (target.HasStatus<SukukajaBuff>()) {
                        target.RemoveAllEffects<SukukajaBuff>();
                    }
                    else {
                        sequence.AddStatusEffect(target, new SukundaDebuff(), 600, canExtend: true);
                    }
                }
                sequence.Delay(60);
                sequence.CustomAction(() => {
                    if (battle.units.Any(x => x.team != caster.team && x.HasStatus<SukundaDebuff>()))
                        battle.weaknessHitLastRound = true;
                });
            });

        }
    }
}

﻿using HellTrail.Core.Combat.Status;
using Microsoft.Xna.Framework;

namespace HellTrail.Core.Combat.Sequencer
{
    public class DoDamageSequence : ISequenceAction
    {
        public int damage;

        public int accuracy;

        public bool dealtDamage;

        public ElementalType type;

        public Unit target;
        public Unit caster;

        public DoDamageSequence(Unit caster, Unit target, int damage, ElementalType type = ElementalType.Phys, int accuracy = 100)
        {
            this.accuracy = accuracy;
            this.type = type;
            this.target = target;
            this.caster = caster;
            this.damage = damage;
            dealtDamage = false;
        }

        public bool IsFinished()
        {
            return true;
        }

        public void Update(List<Unit> actors, Battle battle)
        {
            float guardFactor = target.HasStatus<GuardingEffect>() ? 0.75f : 1.0f;
            int damageTaken = (int)(damage * (1 - target.resistances[type]) * guardFactor);
            DamageNumber damageNumber;

            if (battle.rand.Next(101) > accuracy)
            {
                dealtDamage = false;
                damageNumber = new(DamageType.Normal, "MISS", target.position * 4);
                battle.damageNumbers.Add(damageNumber);
                return;
            }
            target.HP = Math.Max(0, target.HP - damageTaken);

            var xx = Main.rand.Next((int)(target.size.X * 0.5f));
            var yy = Main.rand.Next((int)(target.size.Y * 0.5f));
            var offset = -target.size * 0.25f + new Vector2(xx, yy);

            float shakeAmount = 0.16f;
            damageNumber = new(DamageType.Normal, damageTaken.ToString(), (target.position + offset) * 4);

            dealtDamage = true;
            if (damageTaken == 0) // nulled
            {
                shakeAmount = 0;
                damageNumber.DamageType = DamageType.Blocked;
                damageNumber.position = target.position * 4;

                dealtDamage = false;
            }
            else if (damageTaken > damage) // weakness hit
            {
                if (!battle.unitsHitLastRound.Contains(target) && !target.HasStatus<GuardingEffect>())
                {
                    battle.weaknessHitLastRound = true;
                }
                shakeAmount = 0.32f;

                damageNumber.DamageType = DamageType.Weak;
                damageNumber.position = (target.position + offset) * 4;
            }
            else if (damageTaken < 0)// repelled
            {
                damageNumber.position = target.position * 4;
                damageNumber.DamageType = DamageType.Repelled;
                dealtDamage = false;

                shakeAmount = 0;

                damageTaken = (int)(damage * (1 - caster.resistances[type]));
                caster.HP = Math.Max(0, caster.HP - damageTaken);
                float casterResist = caster.resistances[type];
                DamageType repelledType = DamageType.Normal;
                if (casterResist > 0 && casterResist < 1)
                {
                    repelledType = DamageType.Resisted;
                }
                else if (casterResist < 0)
                {
                    repelledType = DamageType.Weak;
                }
                else if (casterResist >= 1)
                {
                    repelledType = DamageType.Blocked;
                }

                DamageNumber damageNumber2 = new(repelledType, damageTaken.ToString(), (caster.position + offset) * 4);
                battle.damageNumbers.Add(damageNumber2);
            }
            else if (damageTaken < damage) // resisted
            {
                shakeAmount = 0.08f;
                damageNumber.DamageType = DamageType.Resisted;
                damageNumber.position = (target.position + offset) * 4;
            }

            if (dealtDamage)
            {
                if (target.HasStatus<GuardingEffect>())
                    target.RemoveAllEffects<GuardingEffect>();
            }
            // TO-DO: add elem types, reaction to repel/block/resist/weak;
            battle.unitsHitLastRound.Add(target);
            battle.damageNumbers.Add(damageNumber);

            target.shake += shakeAmount;
        }
    }
}

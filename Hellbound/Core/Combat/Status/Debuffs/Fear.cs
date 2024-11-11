using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Status.Debuffs
{
    public class Fear : StatusEffect
    {
        public Fear()
        {
            turnsLeft = 4;
            name = "Fear";
            description = "High chance to miss a turn";
        }

        public override void OnTurnBegin(Unit unit, Battle battle)
        {
            turnsLeft--;
            if(battle.rand.Next(101) <= 50)
            {
                Sequence seq = new(battle);
                seq.Add(new DelaySequence(60));
                battle.sequences.Add(seq);

                DamageNumber damageNumber = new(DamageType.Normal, "SCARED", (unit.position) * 4);
                battle.damageNumbers.Add(damageNumber);
                unit.shake = 0.32f;
                battle.State = BattleState.VictoryCheck;
                return;
            }
        }

        public override void UpdateVisuals(SpriteBatch spriteBatch, Unit unit, Battle battle)
        {
        }
    }
}

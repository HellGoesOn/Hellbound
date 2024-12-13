using Casull.Core.Combat.Abilities;

namespace Casull.Core.Combat.AI
{
    public class SequentialAI : BasicAI
    {
        private int currentAbility;
        public override void MakeDecision(Unit whoAmI, Battle battle)
        {
            if (whoAmI.abilities.Count > 0) {
                Ability abilityToUse = whoAmI.abilities[currentAbility];

                var getTargets = battle.units.Where(GetSelector(abilityToUse, whoAmI)).ToList();

                if (!abilityToUse.aoe)
                    getTargets = [getTargets[battle.rand.Next(getTargets.Count)]]; // pick random target if used skill isn't AoE

                abilityToUse.Use(whoAmI, battle, getTargets);

                if (++currentAbility >= whoAmI.abilities.Count) {
                    currentAbility = 0;
                }

            }

            battle.State = BattleState.DoAction;
        }
    }
}

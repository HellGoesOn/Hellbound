using Casull.Core.Combat.Abilities;

namespace Casull.Core.Combat
{
    public class BasicAI
    {
        public virtual void MakeDecision(Unit whoAmI, Battle battle)
        {
            if (whoAmI.abilities.Count > 0) {
                var updatedList = whoAmI.abilities.Where(x => x.CanCast(whoAmI)).ToList();
                if (updatedList.Count > 0) {
                    Ability abilityToUse = updatedList[battle.rand.Next(updatedList.Count)];

                    var getTargets = battle.units.Where(GetSelector(abilityToUse, whoAmI)).ToList();

                    if (!abilityToUse.aoe)
                        getTargets = [getTargets[battle.rand.Next(getTargets.Count)]]; // pick random target if used skill isn't AoE

                    abilityToUse.Use(whoAmI, battle, getTargets);
                }
            }

            battle.State = BattleState.DoAction;
        }

        public static Func<Unit, bool> GetSelector(Ability abilityToUse, Unit whoAmI)
        {
            Func<Unit, bool> selector = x => whoAmI.team != x.team && !x.Downed;

            if (abilityToUse.canTarget == ValidTargets.Ally)
                selector = x => whoAmI.team == x.team && !x.Downed;

            if (abilityToUse.canTarget == ValidTargets.AllButSelf)
                selector = x => x != whoAmI && !x.Downed;

            if (abilityToUse.canTarget == ValidTargets.All)
                selector = x => !x.Downed;

            if (abilityToUse.canTarget == ValidTargets.DownedAlly)
                selector = x => x.Stats.HP <= 0 && x.team == whoAmI.team;

            if (abilityToUse.canTarget == ValidTargets.DownedEnemy)
                selector = x => x.Stats.HP <= 0 && x.team != whoAmI.team;

            return selector;
        }
    }
}

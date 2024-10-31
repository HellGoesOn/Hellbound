using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public enum Elem
    {
        None,
        Phys,
        Fire,
        Ice,
        Elec,
        Wind
    }

    public enum CostType
    {
        None,
        SP,
        HP,
        Item,
        SPHP
    }

    public enum ValidTargets
    {
        Enemy,
        Ally,
        All,
        AllButSelf,
        Active,
        Downed
    }

    public enum Team
    {
        Player,
        Enemy
    }

    public enum BattleState
    {
        BeginTurn,
        CheckInput,
        BeginAction,
        DoAction,
        VictoryCheck,
        TurnProgression,
        Victory,
        Loss
    }

    public enum ElementalType
    {
        Phys,
        Fire,
        Ice,
        Elec,
        Wind,
        Almighty
    }
}

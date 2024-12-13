namespace Casull.Core.Combat
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
        DownedAlly,
        DownedEnemy,
        World
    }

    public enum Team
    {
        Player,
        Enemy
    }

    public enum BattleState
    {
        BeginTurn,
        BeginAction,
        CheckInput,
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
        Almighty,
        DoT,
        Support,
        Healing,
        None
    }
}

namespace Casull.Core.Combat
{
    /*
    public class CombatSystem
    {
        public int currentActor = 0;
        public BattleState state;
        public List<Actor> actors = new List<Actor>();

        public void Init()
        {
            state = BattleState.TurnBegin;
            actors = [.. actors.OrderBy(x => x.Speed)];
        }

        public void Update()
        {
            if (PlayerCharacters <= 0)
            {
                state = BattleState.Lost;
                return;
            }

            if (EnemyCharacters <= 0)
            {
                state = BattleState.Won;
                return;
            }

            if (CurrentActor.state == ActorState.Idle)
            {
                if (CurrentActor.isAiControlled)
                {
                    // make AI controlled decisions. There's no reason to differentiate between teams here
                    // enemies will always be AI controlled

                } else if (CurrentActor.team == Team.Player)
                {
                    // wait for player input
                }
            }

            if (CurrentActor.state == ActorState.Acting)
            {
            }
        }

        public void SelectNextActor()
        {
            if(CurrentActor.state == ActorState.Defeated)
            {
                currentActor++;
            }
        }

        public int AliveActors => actors.Count(x => x.state != ActorState.Defeated);
        public int PlayerCharacters => actors.Count(x => x.team == Team.Player);
        public int EnemyCharacters => actors.Count(x => x.team == Team.Enemy);
        public Actor CurrentActor => actors[currentActor];
    }

    public class Actor
    {
        public int MaxHP;
        public int HP;
        public int MaxMP;
        public int MP;
        public int Speed;
        public bool isAiControlled;

        public ActorState state;
        public Team team;
        public Actor() 
        {
            team = Team.Player;
            state = ActorState.Busy;
            MaxHP = HP = 100;
            MaxMP = MP = 20;
            Speed = 6;
        }
    }




    public enum Team
    {
        Player,
        Enemy
    }

    public enum ActorState
    {
        Idle,
        Acting,
        Busy,
        Defeated
    }

    public enum BattleState
    {
        Init,
        TurnBegin,
        ActionTaken,
        TurnEnd,
        Won,
        Lost
    }
    */
}

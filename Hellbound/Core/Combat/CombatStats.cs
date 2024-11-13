namespace HellTrail.Core.Combat
{
    public class CombatStats
    {
        public int HP;
        public int MaxHP;
        public int SP;
        public int MaxSP;
        public int strength;
        public int magic;
        public int EXP;
        public int toNextLevel;
        public int value;
        public int level;
        public float speed;

        public CombatStats()
        {
            value = 20;
            EXP = 0;
            toNextLevel = 100;
            level = 1;
            HP = MaxHP = 100;
            SP = MaxSP = 60;
            strength = 3;
            magic = 3;
            speed = 6.0f;
        }

        public CombatStats(int attack, int magic, int maxHP, int maxSP, float speed)
        {
            EXP = 0;
            toNextLevel = 100;
            level = 1;
            this.strength = attack;
            this.magic = magic;
            this.speed = speed;
            this.MaxHP = maxHP;
            this.MaxSP = maxSP;
        }

        public static CombatStats operator +(CombatStats a, CombatStats b)
        {
            a.strength += b.strength;
            a.magic += b.magic;
            a.speed += b.speed;
            a.MaxHP += b.MaxHP;
            a.MaxSP += b.MaxSP;

            return a;
        }

        public override string ToString()
        {
            return $"STR{strength} MA{magic} SPD{speed}";
        }
    }
}

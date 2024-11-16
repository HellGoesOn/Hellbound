using System.Text;

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

        public CombatStats GetCopy()
        {
            return new CombatStats()
            {
                HP = this.HP,
                SP = this.SP,
                MaxHP = this.MaxHP,
                MaxSP = this.MaxSP,
                EXP = this.EXP,
                strength = this.strength,
                magic = this.magic,
                speed = this.speed,
                toNextLevel = this.toNextLevel,
                value = this.value,
                level = this.level,
            };
        }

        public string ListStats()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"LVL: {level}");
            sb.AppendLine($"EXP: {EXP} / {toNextLevel}");
            sb.AppendLine($"HP: {HP} / {MaxHP}");
            sb.AppendLine($"SP: {SP} / {MaxSP}");
            sb.AppendLine($"STR: {strength}");
            sb.AppendLine($"MA: {magic}");
            sb.AppendLine($"SPD: {speed}");
            return sb.ToString();
        }
    }
}

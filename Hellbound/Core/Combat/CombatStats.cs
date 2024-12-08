using System.Text;

namespace HellTrail.Core.Combat
{
    public class CombatStats
    {
        public int HP;
        public float accuracy;
        public float evasion;
        public int MaxHP;
        public int SP;
        public int MaxSP;
        public float strength;
        public float magic;
        public int EXP;
        public int toNextLevel;
        public int value;
        public int level;
        public float speed;

        public CombatStats()
        {
            accuracy = 1f;
            evasion = 0f;
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

        public CombatStats(float attack, float magic, int maxHP, int maxSP, float speed)
        {
            evasion = 1f;
            EXP = 0;
            value = 20;
            toNextLevel = 100;
            level = 1;
            this.strength = attack;
            this.magic = magic;
            this.speed = speed;
            this.MaxHP = maxHP;
            this.MaxSP = maxSP;
            accuracy = 1f;
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

        public string ListStats(bool full = true)
        {
            StringBuilder sb = new();
            sb.AppendLine($"LVL: {level}");
            sb.AppendLine($"EXP: {EXP} / {toNextLevel}");
            sb.AppendLine($"HP: {HP} / {MaxHP}");
            sb.AppendLine($"SP: {SP} / {MaxSP}");
            if (full)
            {
                sb.AppendLine($"STR: {(int)strength}");
                sb.AppendLine($"MA: {(int)magic}");
                sb.AppendLine($"SPD: {Math.Round(speed, 1)}");
            }
            return sb.ToString();
        }
    }
}

using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HellTrail.Core.Combat
{
    public class DamageNumber
    {
        public int damage;
        public int timeLeft;
        private int baseTimeLeft;
        private float y;
        public float jump;
        public float maxHeight;
        public Vector2 position;
        public Vector2 velocity;
        public string text;
        public Color color;
        private DamageType damageType;

        public DamageType DamageType 
        { 
            get => damageType;
            set
            {
                damageType = value;
                switch (damageType)
                {
                    case DamageType.Normal:
                        text = damage.ToString();
                        break;
                    case DamageType.Resisted:
                        color = Color.DarkGray;
                        text = $"{damage}";
                        break;
                    case DamageType.Weak:
                        color = Color.Red;
                        text = $"{damage}";
                        break;
                    case DamageType.Blocked:
                        color = Color.DarkGray;
                        text = "BLOCK";
                        break;
                    case DamageType.Repelled:
                        color = Color.DarkGray;
                        text = "REPEL";
                        break;
                    default:
                        text = "";
                        break;
                }
            }
        }

        public DamageNumber(DamageType type, int damage, Vector2 position)
        {
            text = "";
            this.damage = damage;
            velocity = new Vector2(0.01f * Main.rand.Next(2) == 0 ? -1 : 1, -2);
            this.position = position;
            baseTimeLeft = timeLeft = 60;
            color = Color.White;
            DamageType = type;
        }

        public void Update()
        {
            /*if (position.Y > y)
                velocity.Y *= -1;

            position += velocity;

            velocity.X *= 0.995f;
            velocity.Y += 0.18f;*/

            var airTime = Math.Min(15, baseTimeLeft - timeLeft);

            y = (float)Math.Sin(airTime) * 3;

            if (timeLeft > 0)
                timeLeft--;
        }

        public void Draw(SpriteBatch sb)
        {
            var orig = AssetManager.CombatMenuFont.MeasureString(text) * 0.5f;
            sb.DrawBorderedString(AssetManager.CombatMenuFont, text, position + new Vector2(0, y), color, Color.Black, 0f, orig, Vector2.One, SpriteEffects.None, 0f);
        }
    }

    public enum DamageType
    {
        Normal,
        Weak,
        Resisted,
        Blocked,
        Repelled,
        
    }
}

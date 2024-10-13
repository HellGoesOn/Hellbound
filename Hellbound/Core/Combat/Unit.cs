using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public class Unit
    {
        public int HP;
        public int MaxHP;
        public int SP;
        public int MaxSP;
        public float Speed;
        public string name;
        public string sprite;
        public Team team;
        private Vector2 battleStation;
        public Vector2 position;
        public Vector2 size = new Vector2(32);
        public BasicAI? ai = null;
        public List<Ability> abilities = [];

        public float opacity;

        public Unit()
        {
            sprite = "Slime3";
            opacity = 1.25f;
            name = "???";
            HP = MaxHP = 100;
            SP = MaxSP = 27;
            Speed = 6;
        }

        // unit should not be updating its own logic outside of combat system
        // therefore this should only be used for visuals;
        public void UpdateVisuals()
        {
            if (Downed && opacity > 0)
            {
                opacity -= 0.02f;
            }
        }

        public bool Downed => HP <= 0;

        public Vector2 BattleStation
        {
            get => battleStation;
            set
            {
                position = battleStation = value;
            }
        }

        public bool ContainsMouse(Vector2 offset)
        {
            var rect = new Rectangle((int)position.X + (int)offset.X, (int)position.Y + (int)offset.Y, (int)size.X, (int)size.Y);
            var mousePoint = new Point((int)Input.MousePosition.X, (int)Input.MousePosition.Y);
            return rect.Contains(mousePoint);
        }


        public bool ContainsMouse()
        {
            var rect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            var mousePoint = new Point((int)Input.MousePosition.X, (int)Input.MousePosition.Y);
            return rect.Contains(mousePoint);
        }
    }
}

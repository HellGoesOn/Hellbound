using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public class BattleBackground
    {
        public string texture;
        public float opacity;
        public Color color;

        public BattleBackground(string texture)
        {
            opacity = 1f;
            this.texture = texture;
            color = Color.White;
        }
    }
}

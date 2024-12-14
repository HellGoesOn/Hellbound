using Casull.Core.Overworld;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.Combat.Items
{
    public class SunflowerHead : Item
    {
        public SunflowerHead() : base("Sunflower Head", "Not a single sunflower will now dare to\nchallenge you.")
        {
            icon = "SunflowerHead";
            frames = [new(0, 0, 32, 32)];
            iconOrigin = new Vector2(16);
            iconScale = new Vector2(6);
            canUseOutOfBattle = false;
        }

        public override void OnObtain()
        {
            World.RaiseFlag("SunflowerIsDead");
        }
    }
}

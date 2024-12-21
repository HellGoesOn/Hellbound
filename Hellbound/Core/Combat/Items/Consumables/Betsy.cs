using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.Combat.Items.Consumables
{
    public class Betsy : Item
    {
        public Betsy() : base("'Betsy'", "An antique tool, but in hands of a \nprofessional even this could do the trick")
        {
            icon = "Betsy";
            frames = [new(0, 0, 32, 32)];
            iconScale = new Vector2(6);
            iconOrigin = new Vector2(16);
            canUseOutOfBattle = false;
        }

        public override void OnObtain()
        {
        }
    }
}

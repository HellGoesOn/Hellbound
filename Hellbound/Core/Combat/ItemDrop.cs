using Casull.Core.Combat.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.Combat
{
    public class ItemDrop
    {
        public int chance;
        public int min;
        public int max;
        public Item item;

        public ItemDrop(int chance, Item item, int min = 1, int max = 1)
        {
            this.chance = chance;
            this.item = item;
            this.min = min;
            this.max = max;
        }
    }
}

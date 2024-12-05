using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Items
{
    public abstract class Item
    {
        public string name;
        public string description;
        public string icon = "";
        public int count;
        public int maxCount = 99;
        public float iconRotation;
        public bool consumable;
        public bool canUseOutOfBattle;

        public EquipSlot equipSlot;

        public FrameData[] frames;
        public Vector2 iconScale;
        public Vector2 iconOrigin;

        public ItemEventHandler onViewed;

        public ValidTargets validTargets = ValidTargets.Ally;

        public Item(string name, string description)
        {
            canUseOutOfBattle = true;
            equipSlot = EquipSlot.NotEquippable;
            iconScale = Vector2.One;
            iconOrigin = new Vector2(16);
            count = 1;
            frames = [];
            this.name = name;
            this.description = description;
        }

        protected virtual void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {

        }

        public void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            if(count > 0)
            {
                OnUse(caster, battle, targets);

                if (consumable)
                    count--;
            }
        }

        public enum EquipSlot
        {
            NotEquippable,
            Armor,
            Weapon,
            Accessory,
        }
    }

    public delegate void ItemEventHandler(Item item);
}

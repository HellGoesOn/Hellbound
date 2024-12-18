using Casull.Core.Combat.Abilities;
using Microsoft.Xna.Framework;

namespace Casull.Core.Combat.Items
{
    public abstract class Item : ICanTarget
    {
        public string name;
        private string description;
        public string icon = "";
        public int count;
        public int maxCount = 99;
        public int damage;
        public int spDamage;
        public float iconRotation;
        public bool consumable;
        public bool aoe;
        public bool canUseOutOfBattle;

        public EquipSlot equipSlot;

        public FrameData[] frames;
        public Vector2 iconScale;
        public Vector2 iconOrigin;

        public ItemEventHandler onViewed;

        public ValidTargets canTarget = ValidTargets.Ally;

        public string Description {
            get {
                return Ability.MarkdownElements(description);
            }
            set => description = value;
        }

        public Item(string name, string description)
        {
            canUseOutOfBattle = true;
            equipSlot = EquipSlot.NotEquippable;
            iconScale = Vector2.One;
            iconOrigin = new Vector2(16);
            count = 1;
            frames = [];
            this.name = name;
            this.Description = description;
        }

        protected virtual void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {

        }

        public void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            if (count > 0) {
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

        public virtual void OnObtain()
        {

        }


        public ValidTargets CanTarget() => canTarget;

        public bool AoE() => aoe;
    }

    public delegate void ItemEventHandler(Item item);
}

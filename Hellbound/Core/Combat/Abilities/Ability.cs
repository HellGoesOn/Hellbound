using Casull.Core.Combat.Sequencer;
using Casull.Core.UI;

namespace Casull.Core.Combat.Abilities
{
    public class Ability : ICanTarget
    {
        public string name;
        private string description;
        public bool aoe;
        public bool canUseOutOfCombat;
        public ValidTargets canTarget;
        public int hpCost;
        public int spCost;
        public int accuracy = 95;
        public int baseDamage;

        public ElementalType elementalType = ElementalType.None;

        public Ability(string name, string description)
        {
            aoe = false;
            this.name = name;
            Description = description;
            canTarget = ValidTargets.Active;
        }

        public void Use(Unit caster, Battle battle, List<Unit> targets, bool useBaseStats = false)
        {
            AdjustCosts(caster);
            if (!CanCast(caster))
                return;

            if (!useBaseStats)
                UseAbility(caster, battle, targets);
            else {
                var copy = caster.GetCopy();
                copy.SetBaseStats(caster.baseStats);
                UseAbility(copy, battle, targets);
            }
            if (battle != null) {
                UIManager.combatUI.showUsedAbilityTime = 90;
                UIManager.combatUI.usedAbilityPanel.Visible = true;
                UIManager.combatUI.usedAbilityText.text = name;
                battle.lastUsedAbility = this;
            }
            caster.Stats.HP -= hpCost;
            caster.Stats.SP -= spCost;
        }

        public bool CanCast(Unit caster)
        {
            if ((hpCost > 0 && caster.Stats.HP <= hpCost) || (spCost > 0 && caster.Stats.SP < spCost))
                return false;

            return true;
        }

        public virtual void AdjustCosts(Unit caster)
        {

        }

        protected virtual void UseAbility(Unit caster, Battle battle, List<Unit> targets)
        {

        }

        public Sequence CreateSequence(Battle battle)
        {
            if (battle == null) {
                var sq = new Sequence(null);
                Main.instance.outOfBoundsSequences.Add(sq);
                return sq;
            };
            var seq = battle.CreateSequence(true);
            seq.source = GetType().ToString();
            return seq;
        }

        public ValidTargets CanTarget() => canTarget;

        public string GetHpCostString => $"<20b2aa/{hpCost} HP>";
        public string GetSpCostString => $"<ff69b4/{spCost} SP>";

        public string Description 
            { 
            get {

                return MarkdownElements(description);
            }
            set => description = value; }

        public bool AoE() => aoe;

        public static string MarkdownElements(string input)
        {
            string[] hexValues = {
                    "9c9792",
                    "e3841e",
                    "5cfffa",
                    "f0da35",
                    "32CD32"
                };

            var liarLiarPantsOnFire = input;
            for (int i = 0; i < hexValues.Length; i++) {
                var name = Enum.GetName(typeof(ElementalType), i);
                liarLiarPantsOnFire = liarLiarPantsOnFire.Replace(name, $"<{hexValues[i]}/{name}>", StringComparison.OrdinalIgnoreCase);
            }

            return liarLiarPantsOnFire;
        }
    }
}

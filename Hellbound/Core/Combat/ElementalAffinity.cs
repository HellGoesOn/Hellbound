using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public class ElementalResistances
    {
        /// <summary>
        /// Positive value -> resist
        /// Negative value -> weak
        /// </summary>
        private readonly float[] values = [0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f];

        public float this[ElementalType type]
        {
            get => values[(int)type];
            set {  values[(int)type] = value; }
        }
        public ElementalResistances()
        { 
        }

        public ElementalResistances(float phys, float fire, float ice, float elec, float wind, float almighty)
        {
            this.values[0] = phys;
            this.values[1] = fire;
            this.values[2] = ice;
            this.values[3] = elec;
            this.values[4] = wind;
            this.values[5] = almighty;
        }

        public ElementalResistances GetCopy()
        {
            return new ElementalResistances()
            {
                [ElementalType.Phys] = this[ElementalType.Phys],
                [ElementalType.Fire] = this[ElementalType.Fire],
                [ElementalType.Ice] = this[ElementalType.Ice],
                [ElementalType.Elec] = this[ElementalType.Elec],
                [ElementalType.Wind] = this[ElementalType.Wind],
                [ElementalType.Almighty] = this[ElementalType.Almighty],
                [ElementalType.DoT] = this[ElementalType.DoT]

            };
        }
    }
}

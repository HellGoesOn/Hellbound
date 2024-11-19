using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class TestComponent : IComponent
    {
        public int testField;
        public int[] testField2;
        public Vector2[] testField4;
        public int testField3;

        public TestComponent(int testField, int[] testField2, Vector2[] testField4, int testField3 )
        {
            this.testField = testField;
            this.testField2 = testField2;
            this.testField4 = testField4;
            this.testField3 = testField3;
        }

        public override string ToString()
        {
            var field2 = testField2 != null ? string.Join(", ", testField2.Select(x => x.ToString())) : "";
            var field4 = testField4 != null ? string.Join(", ", testField4.Select(x => x.ToString())) : "";
            return $"testField1={testField} \ntestField2={field2} \ntestField3={testField3} \ntestField4={field4}";
        }
    }
}

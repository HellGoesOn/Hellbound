using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Attributes
{
    public class SaveCustomAttribute : Attribute
    {
        public SaveCustomData onSave;
        public SaveCustomAttribute(Type delegateType, string delegateName)
        {
            onSave = (SaveCustomData)Delegate.CreateDelegate(typeof(SaveCustomData), delegateType, delegateName);
        }
    }

    public delegate string SaveCustomData(IComponent component);
}

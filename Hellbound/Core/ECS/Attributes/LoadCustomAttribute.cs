using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Attributes
{
    public class LoadCustomAttribute : Attribute
    {
        public LoadCustomData onLoad;
        public LoadCustomAttribute(Type delegateType, string delegateName)
        {
            onLoad = (LoadCustomData)Delegate.CreateDelegate(typeof(LoadCustomData), delegateType, delegateName);
        }
    }

    public delegate void LoadCustomData(IComponent component, string data);
}

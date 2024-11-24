using System.Reflection;

namespace HellTrail.Core.ECS
{
    public static partial class ComponentIO
    {
        public class TryParserContainer
        {
            public readonly MethodInfo methodInfo;

            public bool isNativeImplementation;

            public TryParserContainer(MethodInfo info, bool isNative)
            {
                this.methodInfo = info;
                this.isNativeImplementation = isNative;
            }
        }
    }
}
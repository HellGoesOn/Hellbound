using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI
{
    public interface IUIElement
    {
        void Append(IUIElement element);
        void Disown(IUIElement element);
    }
}

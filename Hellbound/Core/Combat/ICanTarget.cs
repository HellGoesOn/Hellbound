using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public interface ICanTarget
    {
        ValidTargets CanTarget();
        bool AoE();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public interface IGroup<T> where T : Entity
    {
        static List<T> Entities { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public interface IExecute : ISystem
    {
        void Execute(Context context);
    }
}

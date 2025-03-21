﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public interface IMatcher<T> where T : Entity
    {
        bool Matches(T entity);
    }
}

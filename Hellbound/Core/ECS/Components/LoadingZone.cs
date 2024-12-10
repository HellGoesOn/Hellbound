﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class LoadingZone : IComponent
    {
        public string nextZone;
        public Vector2 newPosition;
        public Vector2 direction;
        public LoadingZone(string nextZone, Vector2 direction = default, Vector2 newPosition = default)
        {
            this.direction = direction;
            if (direction == default)
            {
                this.direction = -Vector2.UnitY;
            }
            this.newPosition = newPosition;
            this.nextZone = nextZone;
        }
    }
}

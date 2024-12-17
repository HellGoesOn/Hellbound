using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.Overworld
{
    public class OverworldNotification(string message, Color color, int timeLeft)
    {
        public bool started;
        public string message = message;
        public Color color = color;
        public int timeLeft = timeLeft;
        internal int index;
    }
}

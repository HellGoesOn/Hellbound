using HellTrail.Core.Combat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.CombatUI
{
    public class LevelUpElement : UIElement
    {
        public LevelUpElement(string name, CombatStats oldStats, CombatStats newStats)
        {
            id = "LevelUp";

            UIPanel panel = new()
            {
                size = new Vector2(540, 260),
                Position = new Vector2(32, 128)
            };

            UIBorderedText luckyGuy = new(name)
            {
                Position = new Vector2(panel.size.X * 0.5f, 16),
                origin = Assets.DefaultFont.MeasureString(name) * 0.5f,
                color = Color.LightBlue
            };

            UIBorderedText oldStatsText = new(oldStats.ListStats())
            {
                Position = new Vector2(16, 46)
            };
            string str = "";
            for (int i = 0; i < oldStats.ListStats().Split("\n").Length-1; i++)
            {
                str += "->\n";
            }
            UIBorderedText funnyArrowsText = new(str)
            {
                Position = new Vector2(240, 46)
            };
            UIBorderedText newStatsText = new(newStats.ListStats())
            {
                Position = new Vector2(300, 46),
                color = Color.Yellow
            };

            panel.Append(oldStatsText);
            panel.Append(funnyArrowsText);
            panel.Append(newStatsText);
            panel.Append(luckyGuy);

            Append(panel);
        }
    }
}

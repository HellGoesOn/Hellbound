using Casull.Core.Combat;
using Casull.Core.UI.Elements;
using Casull.Render;
using Microsoft.Xna.Framework;

namespace Casull.Core.UI.CombatUI
{
    public class LevelUpElement : UIElement
    {
        public LevelUpElement(string name, CombatStats oldStats, CombatStats newStats)
        {
            id = "LevelUp";

            UIAnimatedPanel panel = new(new Vector2(540, 260), UIAnimatedPanel.AnimationStyle.FourWay);
            panel.openSpeed = 0.25f;
            panel.SetPosition(new Vector2(Renderer.UIPreferedWidth * 0.5f - 270, Renderer.UIPreferedHeight * 0.5f - 130));

            UIBorderedText luckyGuy = new(name) {
                origin = Assets.DefaultFont.MeasureString(name) * 0.5f,
                color = Color.LightBlue
            };
            luckyGuy.SetPosition(new Vector2(panel.targetSize.X * 0.5f, 16));
            UIBorderedText oldStatsText = new(oldStats.ListStats()) {
            };
            oldStatsText.SetPosition(16, 46);
            string str = "";
            for (int i = 0; i < oldStats.ListStats().Split("\n").Length - 1; i++) {
                str += "->\n";
            }
            UIBorderedText funnyArrowsText = new(str) {
            };
            funnyArrowsText.SetPosition(240, 46);
            UIBorderedText newStatsText = new(newStats.ListStats()) {
                color = Color.Yellow
            };
            newStatsText.SetPosition(300, 46);

            panel.Append(oldStatsText);
            panel.Append(funnyArrowsText);
            panel.Append(newStatsText);
            panel.Append(luckyGuy);

            Append(panel);
        }
    }
}

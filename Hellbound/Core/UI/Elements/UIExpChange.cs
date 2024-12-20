using Casull.Core.Combat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.UI.Elements
{
    public class UIExpChange : UIElement
    {
        readonly Unit unit;

        int delayFill;
        uint oldExp;
        uint expValue;

        UIBorderedText levelText;
        UIBorderedText toNextLevel;
        UIProgressBar expBar;
        UIProgressBar expBarCover;
        UIAnimatedPanel panel;

        public UIExpChange(Unit whomst, uint oldExp, uint expValue)
        {
            id = "expchange";
            delayFill = 120;
            unit = whomst;
            panel = new UIAnimatedPanel(new Vector2(600, 128), UIAnimatedPanel.AnimationStyle.FourWay);
            panel.borderColor = Color.Transparent;
            panel.panelColor = Color.Black * 0.35f;
            Append(panel);

            onLoseParent = (sender) => {
                if (unit != null) {
                    while (unit.TryLevelUp()) 
                    {
                        // ? there gotta be a better way to do this
                    }
                }
            };

            UIPicture pic = new UIPicture(unit.portrait, [new(0, 0, 32, 32)]);
            pic.scale = new Vector2(3);
            pic.SetPosition(16);
            panel.Append(pic);

            levelText = new UIBorderedText("Level");
            levelText.SetPosition(128, 16);
            panel.Append(levelText);

            toNextLevel = new UIBorderedText("toNext");
            toNextLevel.SetPosition(panel.targetSize.X - toNextLevel.size.X, 16);
            panel.Append(toNextLevel);

            this.oldExp = oldExp;
            this.expValue = expValue;

            expBar = new UIProgressBar(new Vector2(460, 8), unit.Stats.toNextLevel);
            expBar.SetPosition(128, 96);
            expBar.fillColor = Color.Yellow;
            expBar.bgColor = Color.DarkGoldenrod;
            panel.Append(expBar);

            expBarCover = new UIProgressBar(new Vector2(460, 8), unit.Stats.toNextLevel);
            expBarCover.SetPosition(128, 96);
            expBarCover.fillColor = Color.Yellow;
            expBarCover.bgColor = Color.Transparent;
            panel.Append(expBarCover);

            expBar.value = oldExp;
            expBarCover.value = oldExp;

            this.size = panel.targetSize;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            levelText.text = $"{unit.name}\nLevel {unit.Stats.level}\nEXP {oldExp}.."; 
            toNextLevel.text = $"\n\n..{unit.Stats.toNextLevel}"; //lazy alignment fix;
            toNextLevel.SetPosition(panel.targetSize.X - toNextLevel.size.X - 16, 16);

            if (delayFill > 0)
                delayFill--;
            else {
                expBar.HardSetValue(0);
                expBarCover.fillColor = Color.Lerp(Color.Transparent, Color.Yellow, (float)Math.Abs(Math.Sin(Main.totalTime)));

                expBarCover.value = unit.Stats.EXP;
                if (expBarCover.CurrentValueVisual == expBarCover.value) {
                    if(unit.TryLevelUp()) {
                        expBarCover.HardSetValue(0);
                    }
                }

                levelText.text = $"{unit.name}\nLevel {unit.Stats.level}\nEXP {unit.Stats.EXP}..<00FF00/+{expValue}>";
            }
                
            if (Input.PressedKey([Keys.E, Keys.Enter]))
                parent.Disown(this);
        }
    }
}

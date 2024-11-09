using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.CombatUI
{
    public class CombatUIState : UIState
    {
        private float acceleration;

        public bool isRunning;

        public UIPanel oneMorePanel;

        public Vector2 basePosition;

        public CombatUIState()
        {
            acceleration = 0.12f;
            const string text = "1 MORE";
            oneMorePanel = new()
            {
                fillColor = Color.Black,
                outlineColor = Color.White,
                size = new Vector2(AssetManager.CombatMenuFont.MeasureString(text).X * 1.25f, AssetManager.CombatMenuFont.MeasureString(text).Y*2) 
            };
            var textElement = new UIBorderedText(text)
            {
                font = AssetManager.CombatMenuFont,
                Position = oneMorePanel.size * 0.5f,
                origin = AssetManager.CombatMenuFont.MeasureString(text) * 0.5f
            };
            basePosition = new Vector2(-oneMorePanel.size.X * 2, Renderer.UIPreferedHeight * 0.5f - oneMorePanel.size.Y * 0.5f);
            oneMorePanel.Position = basePosition;
            oneMorePanel.Append(textElement);

            children.Add(oneMorePanel);
        }

        public override void Update()
        {
            base.Update();

            if(isRunning)
            {
                //y = x * x + 0.12f;
                // x = Renderer.UIPreferedWidth - 
                // y = ax*x+bx+c
                // y = 

                // rescaled = -1 + 2 * (value - xMin) / (xMax - xMin)

                float xMin = 0;
                float xMax = Renderer.UIPreferedWidth;
                float rescaled = -1 + 2 * (oneMorePanel.Position.X - xMin) / (xMax - xMin);

                float speed = (rescaled*rescaled)*30+acceleration;

                acceleration *= 1.09f;
                oneMorePanel.Position += new Vector2(speed, 0);

                if (oneMorePanel.Position.X >= Renderer.UIPreferedWidth)
                {
                    oneMorePanel.Position = basePosition;
                    acceleration = 0.12f;
                    isRunning = false;
                }
            }
        }
    }
}

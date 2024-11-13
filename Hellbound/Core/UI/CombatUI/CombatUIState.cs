using HellTrail.Core.Combat;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treeline.Core.Graphics;

namespace HellTrail.Core.UI.CombatUI
{
    public class CombatUIState : UIState
    {
        private float acceleration;

        public bool isRunning;

        public UIPanel oneMorePanel;

        public Vector2 basePosition;

        public UIBorderedText skillDescription;
        public UIBorderedText skillCost;
        public UIPanel skillPanel;
        //public UIBorderedText debug;

        public UIPanel teamStatus;
        public UIBorderedText teamNamesText;
        public UIBorderedText teamHPText;
        public UIBorderedText teamSPText;
        public UIPanel usedAbilityPanel;
        public UIBorderedText usedAbilityText;
        public int showUsedAbilityTime = 0;

        public CombatUIState()
        {
            teamStatus = new()
            {
                //fillColor = Color.White * 0f,
                //outlineColor = Color.White * 0f,
                size = new Vector2(500, 140),
                Position = new Vector2(Renderer.UIPreferedWidth * 0.5f - 200, Renderer.UIPreferedHeight - 150)
            };
            teamNamesText = new("");
            teamHPText = new("");
            teamSPText = new("");

            teamNamesText.Position = new Vector2(16);
            teamHPText.Position = new Vector2(160, 16);
            teamSPText.Position = new Vector2(340, 16);

            teamStatus.Append(teamNamesText);
            teamStatus.Append(teamHPText);
            teamStatus.Append(teamSPText);

            usedAbilityPanel = new();
            usedAbilityText = new("");
            usedAbilityPanel.Visible = false;
            usedAbilityPanel.Append(usedAbilityText);

            //debug = new("")
            //{
            //    Position = new Vector2(16)
            //};

            skillPanel = new UIPanel()
            {
                size = new Vector2(400, 140),
                Position = new Vector2(16, Renderer.UIPreferedHeight - 150)
            };

            skillDescription = new UIBorderedText("");
            skillCost = new UIBorderedText("");
            skillCost.Position = new Vector2(12, 100);
            skillDescription.Position = new Vector2(12);
            skillPanel.Append(skillDescription);
            skillPanel.Append(skillCost);

            var panel = new UIPanel()
            {
                size = new Vector2(280, 120),
                Position = new Vector2(Renderer.UIPreferedWidth-284, Renderer.UIPreferedHeight - 140),
            };
            const string tutorialTex = "[W][A][S][D] Navigate\n[E] Confirm\n[Q] Cancel";
            var tutorialText = new UIBorderedText(tutorialTex);
            tutorialText.Position = new Vector2(16);
            panel.Append(tutorialText);

            this.Append(panel);
            this.Append(skillPanel);

            acceleration = 1;
            const string text = "1 MORE";

            oneMorePanel = new()
            {
                fillColor = Color.Black,
                outlineColor = Color.White,
                size = new Vector2(Assets.CombatMenuFont.MeasureString(text).X * 1.25f, Assets.CombatMenuFont.MeasureString(text).Y*2) 
            };

            var textElement = new UIBorderedText(text)
            {
                font = Assets.CombatMenuFont,
                Position = oneMorePanel.size * 0.5f,
                origin = Assets.CombatMenuFont.MeasureString(text) * 0.5f
            };

            basePosition = new Vector2(-oneMorePanel.size.X * 2, Renderer.UIPreferedHeight * 0.5f - oneMorePanel.size.Y * 0.5f);
            oneMorePanel.Position = basePosition;
            oneMorePanel.Append(textElement);

            Append(oneMorePanel);
            //Append(debug);
            Append(usedAbilityPanel);
            Append(teamStatus);
        }

        public override void Update()
        {
            base.Update();


            //debug.text = $"Particles: {ParticleManager.count}\nParticles Additive{ParticleManager.countAdditive}";
            string partyStatus = "";
            string partyHP = "";
            string partySP = "";
            foreach(Unit unit in GlobalPlayer.ActiveParty)
            {
                partyStatus += $"{(unit.name)}\n";
                partyHP += $"HP: {unit.HP}/{unit.MaxHP}\n";
                partySP += $"SP: {unit.SP}/{unit.MaxSP}\n";
            }

            teamNamesText.text = partyStatus;
            teamHPText.text = partyHP;
            teamSPText.text = partySP;

            if(showUsedAbilityTime > 0)
            {
                showUsedAbilityTime--;
                var textSize = Assets.DefaultFont.MeasureString(usedAbilityText.text);
                usedAbilityPanel.size = new Vector2(4 + textSize.X * 1.5f, textSize.Y * 2);
                usedAbilityText.origin = Assets.DefaultFont.MeasureString(usedAbilityText.text) * 0.5f;
                usedAbilityText.Position = usedAbilityPanel.size * 0.5f;
                usedAbilityPanel.Position = new Vector2(Renderer.UIPreferedWidth * 0.5f - usedAbilityPanel.size.X * 0.5f, 80);

                if (showUsedAbilityTime <= 0)
                    usedAbilityPanel.Visible = false;
            }

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

                if(oneMorePanel.Position.X > xMax * 0.5f - 120)
                    acceleration *= 1.2f;
                oneMorePanel.Position += new Vector2(speed, 0);

                if (oneMorePanel.Position.X >= Renderer.UIPreferedWidth)
                {
                    oneMorePanel.Position = basePosition;
                    acceleration = 1f;
                    isRunning = false;
                }
            }
        }
    }
}

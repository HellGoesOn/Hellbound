using Casull.Core.Combat;
using Casull.Core.Combat.Abilities;
using Casull.Core.Combat.Items;
using Casull.Core.UI.Elements;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Casull.Core.UI.CombatUI
{
    public class CombatUIState : UIState
    {
        public int tutorialProgress;
 
        private float acceleration;

        public bool isRunning;
        public bool showLevelUps;
        public bool isTargetingWithItem;

        public bool beganActing;

        public UIPanel oneMorePanel;

        public Vector2 basePosition;

        public UIPanel teamStatus;
        public UIBorderedText teamNamesText;
        public UIBorderedText teamHPText;
        public UIBorderedText teamSPText;
        public UIPanel usedAbilityPanel;
        public UIBorderedText usedAbilityText;
        public int showUsedAbilityTime = 0;

        public List<LevelUpElement> levelUpElements;
        public List<UICombatPortrait> portraits = [];

        public List<UIProgressBar> enemyHPBars = [];
        public List<UIBorderedText> enemyHPTexts = [];

        public UIBorderedText victoryText;

        public readonly List<string[]> tutorialCombatOptions = new(){
            { ["Attack"] },
            { ["Item"] },
            { ["Attack", "Item"] },
            { ["Guard"] },
            { ["Item"] }
        };

        public CombatUIState()
        {

            for (int i = 0; i < 5; i++) {
                var newBar = new UIProgressBar(new Vector2(100, 8), 0);
                newBar.Visible = false;
                newBar.fillColor = Color.LightSeaGreen;
                newBar.Rotation = -0.2f;

                var hpText = new UIBorderedText("HP");
                hpText.color = Color.White;
                hpText.SetPosition(12, 12);
                newBar.Append(hpText);
                enemyHPBars.Add(newBar);
                enemyHPTexts.Add(hpText);
                Append(enemyHPBars[i]);
            }

            levelUpElements = [];
            id = "combatUIState";
            teamStatus = new() {
                //fillColor = Color.White * 0f,
                //outlineColor = Color.White * 0f,
                size = new Vector2(500, 140),
            };
            teamStatus.SetPosition(Renderer.UIPreferedWidth * 0.5f - 200, Renderer.UIPreferedHeight - 150);
            teamNamesText = new("");
            teamHPText = new("");
            teamSPText = new("");

            teamNamesText.SetPosition(new Vector2(16));
            teamHPText.SetPosition(new Vector2(200, 16));
            teamSPText.SetPosition(new Vector2(360, 16));

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

            var panel = new UIPanel() {
                size = new Vector2(Renderer.UIPreferedWidth - 8, 40),
            };
            panel.SetPosition(4, 4);
            const string tutorialTex = "[W][A][S][D] Navigate [E] Confirm [Q] Cancel";
            var tutorialText = new UIBorderedText(tutorialTex);
            tutorialText.SetPosition(new Vector2(16, 8));
            panel.Append(tutorialText);

            //this.Append(panel);

            acceleration = 1;
            const string text = "1 MORE";

            oneMorePanel = new() {
                fillColor = Color.Black,
                outlineColor = Color.White,
                size = new Vector2(Assets.CombatMenuFont.MeasureString(text).X * 1.25f, Assets.CombatMenuFont.MeasureString(text).Y * 2)
            };

            var textElement = new UIBorderedText(text) {
                font = Assets.CombatMenuFont,
                origin = Assets.CombatMenuFont.MeasureString(text) * 0.5f
            };
            textElement.SetPosition(oneMorePanel.size * 0.5f);

            basePosition = new Vector2(-oneMorePanel.size.X * 2, Renderer.UIPreferedHeight * 0.5f - oneMorePanel.size.Y * 0.5f);
            oneMorePanel.SetPosition(basePosition);
            oneMorePanel.Append(textElement);

            Append(oneMorePanel);
            //Append(debug);
            Append(usedAbilityPanel);
            //Append(teamStatus);

            victoryText = new UIBorderedText("Press E to continue");
            victoryText.opacity = 0.0f;
            victoryText.SetPosition(Renderer.ScreenMiddle.X - victoryText.size.X * 0.5f, Renderer.ScreenMiddle.Y + 250);

            Append(victoryText);
        }

        public void CreatePortraits(Battle battle)
        {
            foreach (UICombatPortrait p in portraits) {
                Disown(p);
            }
            portraits.Clear();

            float off = 0;
            foreach (var unit in battle.playerParty) {
                UICombatPortrait newPortrait = new UICombatPortrait(unit.portraitCombat, unit.Stats.MaxHP, unit.Stats.MaxSP);
                newPortrait.SetPosition(Renderer.UIPreferedWidth - 128 * battle.playerParty.Count + off, Renderer.UIPreferedHeight - 160);
                off += 120;
                newPortrait.hpBar.Rotation = -0.25f;
                newPortrait.spBar.Rotation = -0.25f;
                newPortrait.hpText.Rotation = -0.25f;
                newPortrait.spText.Rotation = -0.25f;
                newPortrait.predictedHpBar.Rotation = -0.25f;
                newPortrait.predictedSpBar.Rotation = -0.25f;
                newPortrait.hpValueText.SetPosition(80, 50);
                newPortrait.spValueText.SetPosition(80, 100);


                Append(newPortrait);
                portraits.Add(newPortrait);
            }

            int index = 0;
            foreach (var unit in battle.units.Where(x => x.team == Team.Enemy)) {
                enemyHPBars[index].SetPosition(unit.BattleStation * 4 + new Vector2(-16, 40));
                enemyHPBars[index].value = unit.Stats.HP;
                enemyHPBars[index].maxValue = unit.Stats.MaxHP;
                index++;
            }
        }

        public void CreateLevelUp(string forWho, CombatStats oldStats, CombatStats newStats)
        {
            levelUpElements.Add(new LevelUpElement(forWho, oldStats, newStats));
        }

        public override void Update()
        {
            var state = Main.instance.GetGameState();
            visible = active = state is Battle;

            if (!active) {
                return;
            }

            var battle = (state as Battle);

            if (battle.State == BattleState.Victory)
            {
                victoryText.opacity = 1.0f;
            }
            else {
                victoryText.opacity = 0.0f;
            }

            if (levelUpElements.Count > 0 && showLevelUps) {
                if (GetElementById("LevelUp") == null) {
                    Append(levelUpElements[0]);
                }

                if (Input.PressedKey([Keys.E, Keys.Enter])) {
                    Disown(levelUpElements[0]);
                    levelUpElements.RemoveAt(0);
                }

                if (levelUpElements.Count <= 0)
                    showLevelUps = false;
            }

            base.Update();
            //debug.text = $"Particles: {ParticleManager.count}\nParticles Additive{ParticleManager.countAdditive}";
            string partyStatus = "";
            string partyHP = "";
            string partySP = "";
            foreach (Unit unit in battle.playerParty) {
                partyStatus += $"[ {unit.Stats.level} ]{(unit.name)}\n";
                partyHP += $"HP: {unit.Stats.HP}/{unit.Stats.MaxHP}\n";
                partySP += $"SP: {unit.Stats.SP}/{unit.Stats.MaxSP}\n";
            }

            teamNamesText.text = partyStatus;
            teamHPText.text = partyHP;
            teamSPText.text = partySP;

            if (showUsedAbilityTime > 0) {
                showUsedAbilityTime--;
                var textSize = Assets.DefaultFont.MeasureString(usedAbilityText.text);
                usedAbilityPanel.size = new Vector2(4 + textSize.X * 1.5f, textSize.Y * 2);
                usedAbilityText.origin = Assets.DefaultFont.MeasureString(usedAbilityText.text) * 0.5f;
                usedAbilityText.SetPosition(usedAbilityPanel.size * 0.5f);
                usedAbilityPanel.SetPosition(new Vector2(Renderer.UIPreferedWidth * 0.5f - usedAbilityPanel.size.X * 0.5f, 80));

                if (showUsedAbilityTime <= 0)
                    usedAbilityPanel.Visible = false;
            }

            if (isRunning) {
                float xMin = 0;
                float xMax = Renderer.UIPreferedWidth;
                float rescaled = -1 + 2 * (oneMorePanel.GetPosition().X - xMin) / (xMax - xMin);

                float speed = (rescaled * rescaled) * 45 + acceleration;

                if (oneMorePanel.GetPosition().X > xMax * 0.5f - 120)
                    acceleration *= 1.2f;
                oneMorePanel.SetPosition(oneMorePanel.GetPosition() + new Vector2(speed, 0));

                if (oneMorePanel.GetPosition().X >= Renderer.UIPreferedWidth) {
                    oneMorePanel.SetPosition(basePosition);
                    acceleration = 1f;
                    isRunning = false;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            IGameState state = Main.instance.GetGameState();

            if (state is Battle battle) {
                DrawUI(spriteBatch, battle);
            }

            base.Draw(spriteBatch);

        }

        public void DrawUI(SpriteBatch spriteBatch, Battle activeBattle)
        {
            Vector2 sway = new((float)Math.Sin(Main.totalTime), (float)Math.Cos(Main.totalTime));

            foreach (Unit unit in activeBattle.units) {
                //    string buffs = "";

                //    foreach (StatusEffect fx in unit.statusEffects)
                //        buffs += $"[{fx.name}]={fx.turnsLeft}\n";

                //    Color clr = unit.Downed ? Color.Crimson : Color.White;
                //    Vector2 adjPos = unit.position * 4;
                //    Vector2 orig = Assets.SmallFont.MeasureString(buffs) * 0.5f;
                //    Vector2 finalPos = new Vector2((int)adjPos.X, (int)(adjPos.Y + 64));
                //    spriteBatch.DrawBorderedString(Assets.SmallFont, buffs, finalPos, Color.White, Color.Black, 0f, orig, Vector2.One, SpriteEffects.None, 0);

                if (activeBattle.isPickingTarget && activeBattle.TargetingWith != null) {
                    var targets = activeBattle.TryGetTargets(activeBattle.TargetingWith);

                    if (targets.Contains(unit)) {
                        var position = unit.position * 4;
                        spriteBatch.Draw(Assets.GetTexture("Cursor3"), new Vector2(position.X - 40, position.Y) + sway, null, Color.White, 0f, new Vector2(10, 0), 3f, SpriteEffects.None, 0f);
                    }
                }
            }

            foreach (var p in portraits) {
                p.penisEnlargmentPills = activeBattle.ActingUnit != null && activeBattle.ActingUnit.portraitCombat == p.portrait;
            }

            for (int i = 0; i < activeBattle.playerParty.Count; i++) {
                var unit = activeBattle.playerParty[i];

                portraits[i].penisEnlargmentPills = activeBattle.ActingUnit == unit && unit.portraitCombat == portraits[i].portrait;
                portraits[i].SetValues(unit.Stats.HP, unit.Stats.SP, unit.Stats.MaxHP, unit.Stats.MaxSP);

                portraits[i].SetPredictedValues(unit.Stats.HP, unit.Stats.SP, unit.Stats.MaxHP, unit.Stats.MaxSP);
                if (activeBattle.TargetingWith is Item item && item.canTarget == ValidTargets.Ally && activeBattle.TryGetTargets(item).Contains(unit)) {

                    portraits[i].SetPredictedValues(Math.Clamp( unit.Stats.HP + item.damage, 0, unit.Stats.MaxHP), Math.Clamp(unit.Stats.SP + item.spDamage, 0, unit.Stats.MaxSP), unit.Stats.MaxHP, unit.Stats.MaxSP);
                }
                else if(activeBattle.isPickingTarget && activeBattle.TargetingWith is Ability ability && ability.elementalType == ElementalType.Healing && activeBattle.TryGetTargets(ability).Contains(unit)) {
                    portraits[i].SetPredictedValues(Math.Clamp(unit.Stats.HP + ability.baseDamage, 0, unit.Stats.MaxHP), unit.Stats.SP, unit.Stats.MaxHP, unit.Stats.MaxSP);
                }
            }

            //if (activeBattle.ActingUnit != null)
            //{

            //    //    string text2 = $"Delay: {nextStateDelay} State: {state}";
            //    //    Vector2 orig2 = Assets.DefaultFont.MeasureString(text2) * 0.5f;
            //    //    spriteBatch.DrawBorderedString(Assets.DefaultFont, text2, new Vector2(640, 80), Color.White, Color.Black, 0f, orig2, Vector2.One, SpriteEffects.None, 0f);

            //    if (activeBattle.menus.Count > 0)
            //    {
            //        foreach (var menu in activeBattle.menus)
            //        {
            //            if (!menu.visible)
            //                continue;

            //            var boxPos = menu.position;
            //            Vector2 boxSize = menu.GetSize;
            //            spriteBatch.Draw(Assets.GetTexture("Pixel"), boxPos - new Vector2(2), new Rectangle(0, 0, (int)boxSize.X + 16, (int)boxSize.Y * menu.Count + 16), Color.White);
            //            spriteBatch.Draw(Assets.GetTexture("Pixel"), boxPos, new Rectangle(0, 0, (int)boxSize.X + 12, (int)boxSize.Y * menu.Count + 12), Color.DarkBlue);
            //            for (int i = 0; i < menu.items.Count; i++)
            //            {
            //                var option = menu[i];
            //                Vector2 size = Assets.DefaultFont.MeasureString(option.name);
            //                Vector2 orig3 = size * 0.5f;
            //                var position = new Vector2(boxPos.X + 4, boxPos.Y + 4 + i * size.Y);
            //                Color clr = option.color != Color.White ? option.color : menu.selectedOption == i ? Color.White : Color.Gray;
            //                if (menu.selectedOption == i && menu.active)
            //                {
            //                    spriteBatch.Draw(Assets.GetTexture("Cursor3"), new Vector2(position.X - 40, 6 + menu.position.Y + menu.GetSize.Y * option.index) + sway, null, Color.White, 0f, new Vector2(10, 0), 3f, SpriteEffects.None, 0f);
            //                }

            //                spriteBatch.DrawBorderedString(Assets.CombatMenuFont, option.name, menu.position + new Vector2(8, 4 + menu.GetSize.Y * option.index), clr, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            //                //Renderer.DrawRect(spriteBatch, menu.position + new Vector2(4, 4+menu.GetSize.Y * option.index), menu.GetSize, 1, Color.Orange * 0.75f);
            //            }
            //        }
            //    }
            //}

            foreach (DamageNumber number in activeBattle.damageNumbers) {
                number.Draw(spriteBatch);
            }

            //if (activeBattle.battleEnded)
            //{
            //    string text = $"Battle Result: {activeBattle.State} !";
            //    Vector2 orig = Assets.DefaultFont.MeasureString(text) * 0.5f;
            //    Color color = activeBattle.State == BattleState.Victory ? Color.Gold : activeBattle.State == BattleState.Loss ? Color.Red : Color.Crimson;
            //    spriteBatch.DrawBorderedString(Assets.DefaultFont, text, new Vector2(640, 160), color, Color.Black, 0f, new Vector2((int)orig.X, (int)orig.Y), Vector2.One, SpriteEffects.None, 0f);
            //    //spriteBatch.DrawString(AssetManager.DefaultFont, text, new Vector2(GameOptions.ScreenWidth * 0.5f, GameOptions.ScreenHeight * 0.25f), color, 0f, new Vector2((int)orig.X, (int)orig.Y), Vector2.One, SpriteEffects.None, 0f);
            //}

            int enemyHpBarIndex = 0;
            foreach (var unit in activeBattle.units.Where(x => x.team == Team.Enemy)) {
                var targetWith = activeBattle.TargetingWith;
                enemyHPBars[enemyHpBarIndex].value = unit.Stats.HP;
                enemyHPBars[enemyHpBarIndex].maxValue = unit.Stats.MaxHP;
                enemyHPBars[enemyHpBarIndex].Visible = (activeBattle.unitsHitLastRound.Contains(unit)
                    || (activeBattle.isPickingTarget && targetWith != null &&
                    (targetWith.CanTarget() == ValidTargets.Enemy
                    || targetWith.CanTarget() == ValidTargets.All
                    || targetWith.CanTarget() == ValidTargets.AllButSelf)));
                if (unit.Downed)
                    enemyHPBars[enemyHpBarIndex].Visible = enemyHPBars[enemyHpBarIndex].CurrentValue > 0;
                enemyHPBars[enemyHpBarIndex].SetPosition(unit.position * 4 + new Vector2(-16, 64));
                enemyHPTexts[enemyHpBarIndex].text = unit.name;
                enemyHPTexts[enemyHpBarIndex].SetPosition(-16, -24);
                enemyHpBarIndex++;
            }
        }

        public void SetAbilityUsed(string text, int duration = 90)
        {
            showUsedAbilityTime = duration;
            usedAbilityText.text = text;
            usedAbilityPanel.Visible = true;
        }
    }

}

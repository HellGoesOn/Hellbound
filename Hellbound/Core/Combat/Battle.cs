using HellTrail.Core.Combat.Abilities;
using HellTrail.Core.Combat.Status;
using HellTrail.Core.UI;
using HellTrail.Render;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HellTrail.Core.Combat
{
    public class Battle
    {
        public int turnCount;

        public int actingUnit;

        public int nextStateDelay;

        public int selectedTarget;

        public bool weaknessHitLastRound;

        public bool battleEnded;

        public bool isPickingTarget;

        public string lastAction = "-";

        public BattleBackground bg;

        private BattleState state;

        public Random rand;

        public Ability selectedAbility;

        public List<Unit> units;

        public List<Unit> unitsNoSpeedSort;

        public List<Func<Battle, bool>> winConditions = [];

        public List<Menu> menus = [];

        public List<Menu> menusToRemove = [];

        public List<Menu> menusToAdd = [];

        public List<Sequence> sequences = [];

        public List<SpriteAnimation> fieldAnimations = [];

        public List<DamageNumber> damageNumbers = [];

        public List<Unit> unitsHitLastRound = [];

        public Battle() 
        {
            rand = new Random();
            units = [];
            unitsNoSpeedSort = [];
            State = BattleState.BeginTurn;
            bg = new BattleBackground("TestBG");
        }

        public static Battle Create(List<Unit> enemies)
        {
            Battle battle = new()
            {
                actingUnit = 0
            };
            battle.units.AddRange(GlobalPlayer.ActiveParty);

            foreach (Unit unit in enemies)
            {
                unit.team = Team.Enemy;
                unit.Speed += battle.rand.Next(1, 10) * 0.01f;
            }
            battle.units.AddRange(enemies);
            battle.unitsNoSpeedSort.AddRange(battle.units);
            battle.units = battle.units.OrderByDescending(x => x.Speed).ToList();

            battle.winConditions.Add(x => !x.units.Any(x => !x.Downed && x.team == Team.Enemy));

            SoundEngine.StartMusic("SMT", true);

            int troll = 0;
            foreach (Unit unit in battle.units)
            {
                unit.position = unit.BattleStation - new Vector2(200 * (unit.BattleStation.X > 160 ? -1 : 1), 0);
                Sequence sequence = new(battle)
                {
                    active = true
                };
                int baseTime = unit.team == Team.Enemy ? 90 : 60;
                sequence.Add(new DelaySequence(baseTime + troll));
                sequence.Add(new MoveActorSequence(unit, unit.BattleStation, 0.22f));

                troll += 6;
                battle.sequences.Add(sequence);
            }

            return battle;
        }

        public void Update()
        {
            fieldAnimations.RemoveAll(x => x.finished);

            damageNumbers.RemoveAll(x => x.timeLeft <= 0);

            foreach (DamageNumber number in damageNumbers)
            {
                number.Update();
            }

            foreach(SpriteAnimation animation in fieldAnimations)
            {
                animation.Update();
            }

            foreach(Unit unit in units)
            {
                unit.UpdateVisuals();

                if (unit.Downed)
                    unit.statusEffects.Clear();
            }

            if (nextStateDelay > 0)
            {
                nextStateDelay--;
                return;
            }

            if (sequences.Count > 0 && state != BattleState.CheckInput)
            {
                if (!sequences.Any(x => x.active))
                    sequences[0].active = true;

                for (int i = sequences.Count-1; i >= 0; i--)
                {
                    Sequence seq = sequences[i];
                    if (!seq.active)
                        continue;

                    seq.Update();

                    if(seq.isFinished)
                        sequences.RemoveAt(i);
                }

                return;
            }

            switch (State)
            {
                case BattleState.BeginTurn:
                    //do begin turn;
                    BeginTurn();
                    break;
                case BattleState.CheckInput:
                    // check for input
                    CheckInput();
                    break;
                case BattleState.BeginAction:
                    // setup for upcoming action
                    BeginAction();
                    break;
                case BattleState.DoAction:
                    // play the action out
                    DoAction();
                    break;
                case BattleState.VictoryCheck:
                    // check victory conditions
                    VictoryCheck();
                    break;
                case BattleState.TurnProgression:
                    // progress turns;
                    TurnProgression();
                    break;
            }
        }

        public void DrawField(SpriteBatch spriteBatch)
        {
            if(bg != null)
            {
                spriteBatch.Draw(Assets.Textures[bg.texture], Vector2.Zero, bg.color);
            }

            foreach (Unit unit in units)
            {
                Color clr = unit.Downed ? Color.Crimson : Color.White;
                Vector2 position = unit.position + new Vector2(unit.shake * Main.rand.Next(2) % 2 == 0 ? 1 : -1, 0f);
                if (unit.animations.TryGetValue(unit.currentAnimation, out var anim))
                {
                    anim.position = position;
                    anim.Draw(spriteBatch);
                } 
                else
                {
                    spriteBatch.Draw(Assets.Textures[unit.sprite], new Vector2((int)(position.X), (int)(position.Y)), null, clr * unit.opacity, 0f, new Vector2(16), Vector2.One, SpriteEffects.None, unit.depth);
                    //Renderer.DrawRect(spriteBatch, unit.position-unit.size*0.5f, unit.size, 1, Color.Orange * 0.25f);
                }

                foreach (StatusEffect effect in unit.statusEffects)
                {
                    effect.UpdateVisuals(spriteBatch, unit, this);
                }
            }

            foreach (SpriteAnimation animation in fieldAnimations)
            {
                animation.Draw(spriteBatch);
            }
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            Vector2 sway = new Vector2((float)Math.Sin(Main.totalTime), (float)Math.Cos(Main.totalTime));

            UIManager.combatUI.skillPanel.Visible = menus.Count > 0 && !isPickingTarget;

            foreach (Unit unit in units)
            {
                //Color clr = unit.Downed ? Color.Crimson : Color.White;
                //Vector2 adjPos = unit.position * 4;
                //Vector2 orig = Assets.DefaultFont.MeasureString($"[HP:{unit.HP}]") * 0.5f;
                //Vector2 finalPos = new Vector2((int)adjPos.X, (int)(adjPos.Y + 64));
                //spriteBatch.DrawBorderedString(AssetManager.DefaultFont, $"[HP:{unit.HP}]", finalPos, Color.White, Color.Black, 0f, orig, Vector2.One, SpriteEffects.None, 0);

                if (isPickingTarget)
                {
                    var targets = TryGetTargets(selectedAbility);
                    if (targets.Contains(unit))
                    {
                        var position = unit.position * 4;
                        spriteBatch.Draw(Assets.Textures["Cursor3"], new Vector2(position.X - 40, position.Y) + sway, null, Color.White, 0f, new Vector2(10, 0), 3f, SpriteEffects.None, 0f);
                    }
                }
            }

            if (ActingUnit != null)
            {
            //    string text2 = $"Delay: {nextStateDelay} State: {state}";
            //    Vector2 orig2 = Assets.DefaultFont.MeasureString(text2) * 0.5f;
            //    spriteBatch.DrawBorderedString(Assets.DefaultFont, text2, new Vector2(640, 80), Color.White, Color.Black, 0f, orig2, Vector2.One, SpriteEffects.None, 0f);

                if (menus.Count > 0)
                {
                    foreach (var menu in menus)
                    {
                        if (!menu.visible)
                            continue;

                        var boxPos = menu.position;
                        Vector2 boxSize = menu.GetSize;
                        spriteBatch.Draw(Assets.Textures["Pixel"], boxPos - new Vector2(2), new Rectangle(0, 0, (int)boxSize.X + 16, (int)boxSize.Y * menu.Count + 16), Color.White);
                        spriteBatch.Draw(Assets.Textures["Pixel"], boxPos, new Rectangle(0, 0, (int)boxSize.X + 12, (int)boxSize.Y * menu.Count + 12), Color.DarkBlue);
                        for (int i = 0; i < menu.items.Count; i++)
                        {
                            var option = menu[i];
                            Vector2 size = Assets.DefaultFont.MeasureString(option.name);
                            Vector2 orig3 = size * 0.5f;
                            var position = new Vector2(boxPos.X + 4, boxPos.Y + 4 + i * size.Y);
                            Color clr = Color.Gray;
                            if (menu.selectedOption == i && menu.active)
                            {
                                clr = Color.White;
                                spriteBatch.Draw(Assets.Textures["Cursor3"], new Vector2(position.X - 40, 6+menu.position.Y + menu.GetSize.Y * option.index) + sway, null, Color.White, 0f, new Vector2(10, 0), 3f, SpriteEffects.None, 0f);
                            }
                            spriteBatch.DrawBorderedString(Assets.CombatMenuFont, option.name, menu.position + new Vector2(8, 4+menu.GetSize.Y * option.index), clr, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
                            //Renderer.DrawRect(spriteBatch, menu.position + new Vector2(4, 4+menu.GetSize.Y * option.index), menu.GetSize, 1, Color.Orange * 0.75f);
                        }
                    }
                }
            }

            foreach (DamageNumber number in damageNumbers)
            {
                number.Draw(spriteBatch);
            }

            if (battleEnded)
            {
                string text = $"Battle Result: {state} !";
                Vector2 orig = Assets.DefaultFont.MeasureString(text) * 0.5f;
                Color color = State == BattleState.Victory ? Color.Gold : State == BattleState.Loss ? Color.Red : Color.Crimson;
                spriteBatch.DrawBorderedString(Assets.DefaultFont, text, new Vector2(640, 160), color, Color.Black, 0f, new Vector2((int)orig.X, (int)orig.Y), Vector2.One, SpriteEffects.None, 0f);
                //spriteBatch.DrawString(AssetManager.DefaultFont, text, new Vector2(GameOptions.ScreenWidth * 0.5f, GameOptions.ScreenHeight * 0.25f), color, 0f, new Vector2((int)orig.X, (int)orig.Y), Vector2.One, SpriteEffects.None, 0f);
            }
        }

        public void BeginTurn()
        {
            // calc buffs/debuffs/etc

            lastAction = "-";

            State = BattleState.BeginAction;

            for (int i = ActingUnit.statusEffects.Count - 1; i >= 0; i--)
            {
                var eff = ActingUnit.statusEffects[i];
                if (eff.turnsLeft <= 0 || ActingUnit.Downed)
                    ActingUnit.RemoveEffect(eff);
            }

            foreach (StatusEffect effect in ActingUnit.statusEffects)
            {
                effect.OnTurnBegin(ActingUnit, this);
            }
        }

        private void HandleMenus()
        {
            var playerMenu = new Menu()
            {
                active = true,
                position = new Vector2(80, Renderer.UIPreferedHeight / 2 - 80)
            };

            playerMenu.AddOption("Attack", ()
                =>
            {
                var attacks = new Menu()
                {
                    position = playerMenu.position + new Vector2(playerMenu.GetSize.X + 8, 0),
                    active = true,
                    parentMenu = playerMenu
                }; 
                int option = attacks.selectedOption > ActingUnit.abilities.Count ? 0 : attacks.selectedOption < 0 ? ActingUnit.abilities.Count - 1 : attacks.selectedOption;

                UIManager.combatUI.skillDescription.text = ActingUnit.abilities[option].Description;

                AddMenu(attacks);

                foreach (var ability in ActingUnit.abilities)
                {
                    attacks.AddOption(ability.Name,
                        () =>
                        {
                            isPickingTarget = true;
                            selectedAbility = ability;
                            //ability.Use(ActingUnit, this, [ActingUnit]);
                            //ClearMenus();
                            //state = BattleState.BeginAction;

                            Menu fakeMenu = new()
                            {
                                position = attacks.position + new Vector2(attacks.GetSize.X + 8, 0),
                                parentMenu = attacks,
                                active = true,
                                visible = false,
                                sideStep = 3
                            };

                            fakeMenu.onSelectOption = () =>
                            {
                                ability.Use(ActingUnit, this, TryGetTargets(ability));
                                ClearMenus();
                                state = BattleState.DoAction;
                                isPickingTarget = false;
                            };

                            fakeMenu.onCancel = () =>
                            {
                                foreach (var menu in menus)
                                {
                                    menu.visible = true;
                                }
                                isPickingTarget = false;
                                int option = attacks.selectedOption > ActingUnit.abilities.Count ? 0 : attacks.selectedOption < 0 ? ActingUnit.abilities.Count - 1 : attacks.selectedOption;

                                UIManager.combatUI.skillDescription.text = ActingUnit.abilities[option].Description;
                                RemoveMenu(fakeMenu);
                            };

                            fakeMenu.onChangeOption = () =>
                            {
                                selectedTarget += fakeMenu.selectedOption;
                            };

                            foreach (var menu in menus)
                            {
                                menu.visible = false;
                            }

                            AddMenu(fakeMenu);
                            attacks.active = false;
                            UIManager.combatUI.skillDescription.text = "";
                        });
                }

                attacks.onChangeOption = () =>
                {
                    int option = attacks.selectedOption >= ActingUnit.abilities.Count ? 0 : attacks.selectedOption < 0 ? ActingUnit.abilities.Count - 1 : attacks.selectedOption;
                    
                    UIManager.combatUI.skillDescription.text = ActingUnit.abilities[option].Description;
                };

                attacks.onCancel = () =>
                {
                    UIManager.combatUI.skillDescription.text = "";
                    RemoveMenu(attacks);
                };

                playerMenu.active = false;
                //subMenus
            });
            playerMenu.AddOption("Item", () => { });
            playerMenu.AddOption("Guard", () =>
            {
                RemoveMenu(playerMenu);
                playerMenu = null;
                ActingUnit.AddEffect(new GuardingEffect());
                state = BattleState.DoAction;
            }
            );

            playerMenu.onChangeOption = () =>
            {
                int option = playerMenu.selectedOption >= 3 ? 0 : playerMenu.selectedOption < 0 ? 2 : playerMenu.selectedOption;
                var text = "";
                switch (option)
                {
                    default:
                    case 0:
                        text = "Use an Ability";
                        break;
                    case 1:
                        text = "Use an Item";
                        break;
                    case 2:
                        text = "Reduces damage by 25%.\nProtects from Weaknesses";
                        break;
                }

                UIManager.combatUI.skillDescription.text = text;
            };

            playerMenu.onCancel = () =>
            {
                playerMenu.selectedOption = playerMenu.Count - 1;
                var text = "Reduces damage by 25%.\nProtects from Weaknesses";
                UIManager.combatUI.skillDescription.text = text;
            };

            menus.Add(playerMenu);
        }

        public void BeginAction()
        {
            if (ActingUnit.Downed)
            {
                State = BattleState.VictoryCheck;
                return;
            }

            if (ActingUnit.team == Team.Player && ActingUnit.ai == null)
            {
                HandleMenus();
            }

            State = BattleState.CheckInput;
        }

        public void DoAction()
        {
            State = BattleState.VictoryCheck;
        }

        public void TurnProgression()
        {
            for (int i = ActingUnit.statusEffects.Count - 1; i >= 0; i--)
            {
                var eff = ActingUnit.statusEffects[i];
                if (eff.turnsLeft <= 0 || ActingUnit.Downed)
                    ActingUnit.RemoveEffect(eff);
            }

            foreach (StatusEffect effect in ActingUnit.statusEffects)
            {
                effect.OnTurnEnd(ActingUnit, this);
            }

            turnCount++;
            State = BattleState.BeginTurn;
            ActingUnit.depth = 0f;

            foreach(var unit in units)
            {
                Sequence seq = new(this)
                {
                    active = true
                };

                seq.Add(new MoveActorSequence(unit, unit.BattleStation));

                this.sequences.Add(seq);
            }

            if (weaknessHitLastRound)
            {
                Sequence seq = new(this);
                seq.Add(new DelaySequence(45));
                sequences.Add(seq);
                UIManager.combatUI.isRunning = true;
                weaknessHitLastRound = false;
                return;
            }

            unitsHitLastRound.Clear();
            if (++actingUnit >= units.Count)
                actingUnit = 0;

        }

        public void CheckInput()
        {
            ActingUnit.depth = 1;
            if (ActingUnit.ai != null)
            {
                ActingUnit.ai.MakeDecision(ActingUnit, this);
                return;
            }

            //if(Input.PressedKey(Keys.Space))
            //{
            //    this.State = BattleState.BeginAction;
            //}

            for(int i = 0; i < menus.Count; i++)
            {
                menus[i].Update();
            }

            foreach(var menu in menusToRemove)
            {
                menus.Remove(menu);
            }

            foreach (var menu in menusToAdd)
            {
                menus.Add(menu);
            }

            menusToAdd.Clear();
            menusToRemove.Clear();
        }

        public void VictoryCheck()
        {
            if(winConditions.All(x=> x?.Invoke(this) == true))
            {
                SoundEngine.StartMusic("Victory", false);
                // end Battle;
                battleEnded = true;
                state = BattleState.Victory; 
                foreach (var unit in units)
                {
                    Sequence seq = new Sequence(this)
                    {
                        active = true
                    };

                    seq.Add(new SetActorAnimation(unit, "Victory"));

                    this.sequences.Add(seq);
                }
                return;
            }

            if (!units.Any(x => !x.Downed && x.team == Team.Player))
            {
                battleEnded = true;
                state = BattleState.Loss;
                return;
            }

            State = BattleState.TurnProgression;
        }

        public void RemoveMenu(Menu menu)
        {
            menusToRemove.Add(menu);
        }

        public void ClearMenus()
        {
            menusToRemove.AddRange(menus);
        }

        public void AddMenu(Menu menu)
        {
            menusToAdd.Add(menu);
        }

        public List<Unit> TryGetTargets(Ability ability)
        {
            Func<Unit, bool> selector = x => ActingUnit.team != x.team && !x.Downed;

            switch (ability.canTarget) {
                case ValidTargets.Ally:
                selector = x => ActingUnit.team == x.team && !x.Downed;
                    break;
                case ValidTargets.All:
                    selector = x => !x.Downed;
                    break;
                case ValidTargets.AllButSelf:
                    selector = x => x != ActingUnit && !x.Downed;
                    break;
            };

            List<Unit> targets = unitsNoSpeedSort.Where(selector).ToList();

            if (targets.Count == 0)
                return [];

            if (ability.aoe)
            {
                return targets;
            }

            var tryMouse = targets.FirstOrDefault(x => x.ContainsMouse(-x.size * 0.5f));
            if (tryMouse != null && Menu.mouseEnabled)
                selectedTarget = targets.IndexOf(tryMouse);

            if (selectedTarget < 0)
                selectedTarget = targets.Count-1;

            if(selectedTarget > targets.Count-1)
                selectedTarget = 0;

            return [targets[selectedTarget]];
        }

        public Unit ActingUnit => units[actingUnit];

        public BattleState State 
        { 
            get => state;
            set {
                nextStateDelay = 0;
                state = value; 
            } 
        }
    }
}

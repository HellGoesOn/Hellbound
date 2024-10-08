using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool battleEnded;

        public bool isPickingTarget;

        public string lastAction = "-";

        private BattleState state;

        public Random rand;

        public Ability selectedAbility;

        public List<Unit> units;

        public List<Unit> unitsNoSpeedSort;


        public List<Func<Battle, bool>> winConditions = [];

        public List<Sequence> sequences = [];

        public List<Menu> menus = [];

        public List<Menu> menusToRemove = [];

        public List<Menu> menusToAdd = [];

        public Battle() 
        {
            rand = new Random();
            units = [];
            unitsNoSpeedSort = [];
            State = BattleState.BeginTurn;
        }

        public static Battle Create(List<Unit> enemies)
        {
            Battle battle = new()
            {
                actingUnit = 0
            };
            battle.units.AddRange(GlobalPlayer.Party);

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
                unit.position = unit.BattleStation - new Vector2(0, 200);
                Sequence sequence = new(battle)
                {
                    active = true
                };
                int baseTime = unit.team == Team.Enemy ? 180 : 60;
                sequence.Add(new DelaySequence(baseTime + troll));
                sequence.Add(new PlaySoundSequence("Summon", 0.05f));
                sequence.Add(new MoveActorSequence(unit, unit.BattleStation, 0.22f));

                troll += 6;
                battle.sequences.Add(sequence);
            }

            return battle;
        }

        public void Update()
        {
            foreach(Unit unit in units)
            {
                unit.UpdateVisuals();
            }

            if (nextStateDelay > 0)
            {
                nextStateDelay--;
                return;
            }

            if (sequences.Count > 0)
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
            foreach(Unit unit in units)
            {
                Color clr = unit.Downed ? Color.Crimson : Color.White;
                spriteBatch.Draw(AssetManager.Textures[unit.sprite], new Vector2((int)(unit.position.X), (int)(unit.position.Y)), null, clr * unit.opacity, 0f, new Vector2(16), Vector2.One, SpriteEffects.None, 0f);
            }
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            Vector2 sway = new Vector2((float)Math.Sin(Main.totalTime), (float)Math.Cos(Main.totalTime));

            foreach (Unit unit in units)
            {
                Color clr = unit.Downed ? Color.Crimson : Color.White;
                Vector2 adjPos = unit.position * 4;
                Vector2 orig = AssetManager.DefaultFont.MeasureString($"[HP:{unit.HP}]") * 0.5f;
                Vector2 finalPos = new Vector2((int)adjPos.X, (int)(adjPos.Y + 60));
                spriteBatch.DrawString(AssetManager.DefaultFont, $"[HP:{unit.HP}]", finalPos, Color.White, 0f, orig, 1f, SpriteEffects.None, 0);

                if (isPickingTarget)
                {
                    var targets = TryGetTargets(selectedAbility);
                    if (targets.Contains(unit))
                    {
                        var position = unit.position * 4;
                        spriteBatch.Draw(AssetManager.Textures["Cursor3"], new Vector2(position.X - 40, position.Y) + sway, null, Color.White, 0f, new Vector2(10, 0), 3f, SpriteEffects.None, 0f);
                    }
                }
            }

            if (ActingUnit != null)
            {
                string text = $"{ActingUnit.team} {ActingUnit.name} turn [HP:{ActingUnit.HP} of {ActingUnit.MaxHP}]" +
                    $"\n{lastAction}";
                Vector2 orig = AssetManager.DefaultFont.MeasureString(text) * 0.5f;
                spriteBatch.DrawString(AssetManager.DefaultFont, text, new Vector2(GameOptions.ScreenWidth * 0.5f, GameOptions.ScreenHeight * 0.2f), Color.White, 0f, new Vector2((int)orig.X, (int)orig.Y), Vector2.One, SpriteEffects.None, 0f);

                string text2 = $"Delay: {nextStateDelay} State: {state}";
                Vector2 orig2 = AssetManager.DefaultFont.MeasureString(text2) * 0.5f;
                spriteBatch.DrawString(AssetManager.DefaultFont, text2, new Vector2(GameOptions.ScreenWidth * 0.5f, GameOptions.ScreenHeight * 0.1f), Color.White, 0f, new Vector2((int)orig2.X, (int)orig2.Y), Vector2.One, SpriteEffects.None, 0f);

                if (menus.Count > 0)
                {
                    foreach (var menu in menus)
                    {
                        if (!menu.visible)
                            continue;

                        var boxPos = menu.position;
                        Vector2 boxSize = menu.GetSize;
                        spriteBatch.Draw(AssetManager.Textures["Pixel"], boxPos - new Vector2(2), new Rectangle(0, 0, (int)boxSize.X + 16, (int)boxSize.Y * menu.Count + 16), Color.DarkGray);
                        spriteBatch.Draw(AssetManager.Textures["Pixel"], boxPos, new Rectangle(0, 0, (int)boxSize.X + 12, (int)boxSize.Y * menu.Count + 12), Color.DarkBlue);
                        for (int i = 0; i < menu.items.Count; i++)
                        {
                            var option = menu[i];
                            Vector2 size = AssetManager.DefaultFont.MeasureString(option.name);
                            Vector2 orig3 = size * 0.5f;
                            var position = new Vector2(boxPos.X + 4, boxPos.Y + 4 + i * size.Y);
                            Color clr = Color.Gray;
                            if (menu.selectedOption == i && menu.active)
                            {
                                clr = Color.White;
                                spriteBatch.Draw(AssetManager.Textures["Cursor3"], new Vector2(position.X - 40, position.Y) + sway, null, Color.White, 0f, new Vector2(10, 0), 3f, SpriteEffects.None, 0f);
                            }
                            spriteBatch.DrawString(AssetManager.DefaultFont, option.name, position, clr, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
                        }
                    }
                }
            }

            if (battleEnded)
            {
                string text = $"Battle Result: {state} !";
                Vector2 orig = AssetManager.DefaultFont.MeasureString(text) * 0.5f;
                Color color = State == BattleState.Victory ? Color.Gold : State == BattleState.Loss ? Color.Red : Color.Crimson;
                spriteBatch.DrawString(AssetManager.DefaultFont, text, new Vector2(GameOptions.ScreenWidth * 0.5f + 2, GameOptions.ScreenHeight * 0.25f + 2), Color.Black, 0f, new Vector2((int)orig.X, (int)orig.Y), Vector2.One, SpriteEffects.None, 0f);
                spriteBatch.DrawString(AssetManager.DefaultFont, text, new Vector2(GameOptions.ScreenWidth * 0.5f, GameOptions.ScreenHeight * 0.25f), color, 0f, new Vector2((int)orig.X, (int)orig.Y), Vector2.One, SpriteEffects.None, 0f);
            }
        }

        public void BeginTurn()
        {
            // calc buffs/debuffs/etc
            lastAction = "-";

            if (ActingUnit.Downed)
            {
                State = BattleState.VictoryCheck;
                return;
            }

            if(ActingUnit.team == Team.Player && ActingUnit.ai == null)
            {
                var playerMenu = new Menu()
                {
                    active = true,
                    position = new Vector2((int)ActingUnit.position.X * 4 - 160 - 6, (int)ActingUnit.position.Y * 4 - 38)
                };

                playerMenu.AddOption("Attack", () 
                    => 
                {
                    var submenuTest = new Menu()
                    {
                        active = true,
                        position = playerMenu.position + new Vector2(playerMenu.GetSize.X + 8, 0),
                        parentMenu = playerMenu
                    };

                    submenuTest.AddOption("Attack", () => { });
                    submenuTest.AddOption("Spells",
                        () =>
                        {
                            var attacks = new Menu()
                            {
                                position = submenuTest.position + new Vector2(submenuTest.GetSize.X + 8, 0),
                                active = true,
                                parentMenu = submenuTest
                            };

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
                                            sideStep = 4
                                        };

                                        fakeMenu.onSelectOption = () =>
                                        {
                                            ability.Use(ActingUnit, this, TryGetTargets(ability));
                                            ClearMenus();
                                            state = BattleState.BeginAction;
                                            isPickingTarget = false;
                                        };

                                        fakeMenu.onCancel = () =>
                                        {
                                            foreach (var menu in menus)
                                            {
                                                menu.visible = true;
                                            }
                                            isPickingTarget = false;
                                            RemoveMenu(fakeMenu);
                                        };

                                        fakeMenu.onChangeOption = () =>
                                        {
                                            selectedTarget += fakeMenu.selectedOption;
                                        };

                                        foreach(var menu in menus)
                                        {
                                            menu.visible = false;
                                        }

                                        AddMenu(fakeMenu);
                                        attacks.active = false;
                                    });
                            }

                            attacks.onCancel = () =>
                            {
                                RemoveMenu(attacks);
                            };

                            submenuTest.active = false;
                        }
                        );

                    submenuTest.onCancel = () =>
                    {
                        RemoveMenu(submenuTest);
                    };

                    AddMenu(submenuTest);

                    
                    playerMenu.active = false;
                    //subMenus
                });
                playerMenu.AddOption("Item", () => { });
                playerMenu.AddOption("Guard", () =>
                {
                    RemoveMenu(playerMenu);
                    playerMenu = null;
                    state = BattleState.BeginAction;
                }
                );

                playerMenu.onCancel = () =>
                {
                    playerMenu.selectedOption = playerMenu.Count-1;
                };

                menus.Add(playerMenu);
            }

            State = BattleState.CheckInput;
        }

        public void BeginAction()
        {

            State = BattleState.DoAction;
        }

        public void DoAction()
        {
            State = BattleState.VictoryCheck;
        }

        public void TurnProgression()
        {
            if (++actingUnit >= units.Count)
                actingUnit = 0;

            turnCount++;

            State = BattleState.BeginTurn;
        }

        public void CheckInput()
        {
            if(ActingUnit.ai != null)
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
                // end Battle;
                battleEnded = true;
                state = BattleState.Victory;
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

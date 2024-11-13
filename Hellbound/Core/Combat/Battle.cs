using HellTrail.Core.Combat.Abilities;
using HellTrail.Core.Combat.Scripting;
using HellTrail.Core.Combat.Sequencer;
using HellTrail.Core.Combat.Status;
using HellTrail.Core.DialogueSystem;
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

        public Ability lastUsedAbility;

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

        public List<Script> scripts = [];

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
                unit.stats.speed += battle.rand.Next(1, 10) * 0.01f;
            }
            battle.units.AddRange(enemies);
            battle.unitsNoSpeedSort.AddRange(battle.units);
            battle.units = [.. battle.units.OrderByDescending(x => x.stats.speed)];

            battle.winConditions.Add(x => !x.units.Any(x => !x.Downed && x.team == Team.Enemy));

            SoundEngine.StartMusic("SMT", true);

            int troll = 0;
            foreach (Unit unit in battle.units)
            {
                unit.scale = Vector2.One;
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
            Script triedToHitThePeas = new()
            {
                condition = (b) =>
                {
                    Unit unit = b.unitsHitLastRound.FirstOrDefault(x => x.name == "Peas");
                    return unit != null && unit.stats.HP == unit.stats.MaxHP && b.state == BattleState.VictoryCheck && b.units.Count(x => !x.Downed && x.team == Team.Enemy) == 1;
                },
                action = (b) =>
                {
                    var page1Portrait = new Portrait("EndLife", new FrameData(0, 0, 32, 32))
                    {
                        scale = new Vector2(-16, 16)
                    };
                    var page3Portrait = new Portrait("EndLife", new FrameData(0, 32, 32, 32))
                    {
                        scale = new Vector2(16, 16)
                    };
                    var page4Portrait = new Portrait("EndLife", new FrameData(0, 128, 32, 32))
                    {
                        scale = new Vector2(-16, 16),
                    };
                    var page5Portrait = new Portrait("EndLife", new FrameData(0, 32, 32, 32))
                    {
                        scale = new Vector2(-16, 16),
                        rotation = MathHelper.PiOver2
                    };
                    Dialogue dialogue = new();
                    DialoguePage page = new()
                    {
                        portrait = page1Portrait,
                        title = GlobalPlayer.ActiveParty[0].name,
                        text = "Да ему вообще поебать на наши атаки.."//"It's completely impervious to our attacks.."
                    };
                    DialoguePage page2 = new()
                    {
                        title = "Peas",
                        text = "(Pea noises)"
                    }; 
                    DialoguePage page3 = new()
                    {
                        portrait = page3Portrait,
                        title = GlobalPlayer.ActiveParty[0].name,
                        text = "Видимо, есть только один единственный выход.."//"There's only one thing that could work.."
                    }; 
                    DialoguePage page4 = new()
                    {
                        portrait = page4Portrait,
                        title = GlobalPlayer.ActiveParty[0].name,
                        text = "В ПИЗДУ ЭТУ ЖИЗНЬ!",//"GOODBYE CRUEL WORLD",
                        onPageEnd = (_) =>
                        {
                            SoundEngine.PlaySound("GunShot");
                            SoundEngine.PlaySound("Death");
                            GlobalPlayer.ActiveParty[0].stats.HP = 0;
                            GlobalPlayer.ActiveParty[0].shake = 0.36f;
                        }
                    };
                    DialoguePage page5 = new()
                    {
                        portrait = page5Portrait,
                        title = GlobalPlayer.ActiveParty[0].name,
                        text = "(ded)"
                    };
                    //dialogue.pages.AddRange([page, page2, page3]);
                    dialogue.pages.AddRange([page, page3, page4, page5]);
                    var disturb = new Disturb()
                    {
                        hpCost = GlobalPlayer.ActiveParty[0].stats.HP - 1,
                        spCost = 0
                    };

                    GlobalPlayer.ActiveParty[0].ClearEffects();
                    GlobalPlayer.ActiveParty[0].abilities.Clear();
                    GlobalPlayer.ActiveParty[0].abilities.Add(disturb);
                    b.weaknessHitLastRound = true;
                    UIManager.dialogueUI.dialogues.Add(dialogue);
                }
            };

            battle.scripts.Add(triedToHitThePeas);

            return battle;
        }

        public void Update()
        {
            fieldAnimations.RemoveAll(x => x.finished);

            damageNumbers.RemoveAll(x => x.timeLeft <= 0);

            foreach(Script script in scripts)
            {
                script.TryRunScript(this);
            }

            scripts.RemoveAll(x => x.activated);


            if (nextStateDelay > 0 || UIManager.dialogueUI.visible)
            {
                nextStateDelay--;
                return;
            }

            foreach (SpriteAnimation animation in fieldAnimations)
            {
                animation.Update();
            }

            foreach (DamageNumber number in damageNumbers)
            {
                number.Update();
            }

            foreach (Unit unit in units)
            {
                unit.UpdateVisuals();

                if (unit.Downed)
                    unit.statusEffects.Clear();
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
                case BattleState.Victory:
                    PlayVictory();
                    break;
            }
        }

        public void PlayVictory()
        {
        }

        public void DrawField(SpriteBatch spriteBatch)
        {
            if (bg != null)
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
                    anim.Draw(spriteBatch, unit.scale);
                } else
                {
                    spriteBatch.Draw(Assets.Textures[unit.sprite], new Vector2((int)(position.X), (int)(position.Y)), null, clr * unit.opacity, 0f, new Vector2(16), unit.scale, SpriteEffects.None, unit.depth);
                    //Renderer.DrawRect(spriteBatch, unit.position-unit.size*0.5f, unit.size, 1, Color.Orange * 0.25f);
                }

                foreach (StatusEffect effect in unit.statusEffects)
                {
                    effect.UpdateVisuals(spriteBatch, unit, this);
                }
            }


            if (ActingUnit != null)
            {
                Color clr = ActingUnit.team == Team.Player ? ActingUnit.ai != null ? Color.Yellow : Color.Lime : Color.Red;
                spriteBatch.Draw(Assets.Textures["Arrow"], new Vector2((int)(ActingUnit.position.X), (int)(ActingUnit.position.Y) - 24 + (float)Math.Cos(Main.totalTime)), null, clr * ActingUnit.opacity, 0f, new Vector2(5, 3.5f), new Vector2((float)Math.Sin(Main.totalTime * 0.75f), 1f), SpriteEffects.None, 1f);
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
                Vector2 adjPos = unit.position * 4;
                Vector2 orig = Assets.SmallFont.MeasureString($"[HP:{unit.stats.HP}]") * 0.5f;
                Vector2 finalPos = new Vector2((int)adjPos.X, (int)(adjPos.Y + 64));
                spriteBatch.DrawBorderedString(Assets.SmallFont, $"[HP:{unit.stats.HP}]", finalPos, Color.White, Color.Black, 0f, orig, Vector2.One, SpriteEffects.None, 0);

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
                            Color clr = option.color != Color.White ? option.color : menu.selectedOption == i ? Color.White : Color.Gray;
                            if (menu.selectedOption == i && menu.active)
                            {
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

                var skill = ActingUnit.abilities[option];
                UIManager.combatUI.skillDescription.text = skill.Description;
                UIManager.combatUI.skillCost.text = $"{(skill.hpCost > 0 ? $"Consumes {skill.hpCost} HP" : "")}{(skill.spCost > 0 ? $"Consumes {skill.spCost} SP" : "")}";

                AddMenu(attacks);

                foreach (var ability in ActingUnit.abilities)
                {
                    var attackOption = attacks.AddOption(ability.Name,
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

                                Ability skill = ActingUnit.abilities[option];

                                UIManager.combatUI.skillDescription.text = skill.Description;
                                UIManager.combatUI.skillCost.text = $"{(skill.hpCost > 0 ? $"Consumes {skill.hpCost} HP" : "")}{(skill.spCost > 0 ? $"Consumes {skill.spCost} SP" : "")}";
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
                            UIManager.combatUI.skillCost.text = "";
                        });

                    if (!ability.CanCast(ActingUnit))
                    {
                        attackOption.canSelect = false;
                        attackOption.color = Color.DarkSlateGray;
                    }
                }
                
                attacks.onChangeOption = () =>
                {
                    int option = attacks.selectedOption >= ActingUnit.abilities.Count ? 0 : attacks.selectedOption < 0 ? ActingUnit.abilities.Count - 1 : attacks.selectedOption;

                    var skill = ActingUnit.abilities[option];
                    UIManager.combatUI.skillDescription.text = skill.Description;
                    UIManager.combatUI.skillCost.text = $"{(skill.hpCost > 0 ? $"Consumes {skill.hpCost} HP" : "")}{(skill.spCost > 0 ? $"Consumes {skill.spCost} SP" : "")}";
                };

                attacks.onCancel = () =>
                {
                    UIManager.combatUI.skillDescription.text = "";
                    UIManager.combatUI.skillCost.text = "";
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
                int expValue = 0;
                foreach (var unit in units)
                {
                    if(unit.team == Team.Enemy)
                    {
                        expValue += unit.stats.value;
                    }

                    Sequence seq = new Sequence(this)
                    {
                        active = true
                    };

                    seq.Add(new SetActorAnimation(unit, "Victory"));

                    this.sequences.Add(seq);
                }

                foreach(Unit unit in GlobalPlayer.ActiveParty)
                {
                    unit.stats.EXP += expValue;
                    unit.TryLevelUp();
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

        public Sequence CreateSequence()
        {
            Sequence seq = new Sequence(this);
            sequences.Add(seq);
            return seq;
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

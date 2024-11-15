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
    public class Battle : IGameState
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
            if(Input.PressedKey([Keys.E, Keys.Enter]))
            {
                SoundEngine.StartMusic("SMT", true);
                GameStateManager.SetState(GameState.Overworld, new SliceTransition(Renderer.SaveFrame()));
                Main.instance.GetGameState().GetCamera().centre = GlobalPlayer.ActiveParty[0].position;
                Main.instance.battle = null;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawField(spriteBatch);
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

        public Camera GetCamera() => CameraManager.combatCamera;
    }
}

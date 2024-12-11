using HellTrail.Core.Combat.Abilities;
using HellTrail.Core.Combat.Items;
using HellTrail.Core.Combat.Scripting;
using HellTrail.Core.Combat.Sequencer;
using HellTrail.Core.Combat.Status;
using HellTrail.Core.DialogueSystem;
using HellTrail.Core.UI;
using HellTrail.Core.UI.Elements;
using HellTrail.Extensions;
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

namespace HellTrail.Core.Combat
{
    public class Battle : IGameState
    {
        int lastItemIndex, lastAbilityIndex;

        public int turnCount;

        public int _actingUnit;

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

        public List<Func<Battle, bool>> winConditions;

        public List<Menu> menus;

        public List<Menu> menusToRemove;

        public List<Menu> menusToAdd;

        public List<Sequence> sequences;

        public List<SpriteAnimation> fieldAnimations;

        public List<DamageNumber> damageNumbers;

        public List<Unit> playerParty;

        public List<Unit> unitsHitLastRound;

        public List<Script> scripts;


        public Action OnBattleEnd;

        public ICanTarget TargetingWith { get; private set; }

        public Battle() 
        {
            rand = new Random();
            winConditions = [];
            menus = [];
            menusToRemove = [];
            menusToAdd = [];
            sequences = [];
            fieldAnimations = [];
            damageNumbers = [];
            scripts = [];
            units = [];
            unitsNoSpeedSort = [];
            unitsHitLastRound = [];
            State = BattleState.BeginTurn;
            bg = new BattleBackground("TestBG");
            GetCamera().zoom = 4f;
            GetCamera().centre = new Vector2(160, 90);
        }

        public static Battle Create(List<Unit> enemies, List<Unit> trialCharacters = null)
        {
            SoundEngine.StartMusic("BossMusic", true);
            SoundEngine.PlaySound("Begin", 0.5f);

            Battle battle = new()
            {
                _actingUnit = 0
            };
            battle.SetUnits(enemies, trialCharacters);

            battle.winConditions.Add(x => !x.units.Any(x => !x.Downed && x.team == Team.Enemy)); 
            
            return battle;
        }

        public void SetUnits(List<Unit> enemies, List<Unit> trialCharacters = null)
        {
            units.Clear();
            unitsNoSpeedSort.Clear(); 
            if (trialCharacters != null)
            {
                playerParty = trialCharacters;
                units.AddRange(trialCharacters);
            } else
            {
                playerParty = GlobalPlayer.ActiveParty;
                units.AddRange(GlobalPlayer.ActiveParty);
            }

            foreach (Unit unit in enemies)
            {
                unit.team = Team.Enemy;
                unit.Stats.speed += rand.Next(1, 10) * 0.01f;
            }
            
            units.AddRange(enemies);

            unitsNoSpeedSort.AddRange(units);
            units = [.. units.OrderByDescending(x => x.Stats.speed)];

            int troll = 0;
            foreach (Unit unit in units)
            {
                unit.position = unit.BattleStation - new Vector2(200 * (unit.BattleStation.X > 160 ? -1 : 1), 0);
                Sequence sequence = new(this)
                {
                    active = true
                };
                int baseTime = unit.team == Team.Enemy ? 90 : 60;
                sequence.Add(new DelaySequence(baseTime + troll));
                sequence.Add(new MoveActorSequence(unit, unit.BattleStation, 0.22f));

                troll += 6;
                this.sequences.Add(sequence);
            }

            UIManager.combatUI.CreatePortraits(this);
        }

        public void Update()
        {
            foreach (var menu in menusToRemove)
            {
                menus.Remove(menu);
            }

            menusToRemove.Clear();
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
                animation.Update(null);
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
                case BattleState.Loss:
                    DoLoss();
                    break;
            }
        }

        bool didLoss;
        int timer;
        bool showedRestartOptions;
        public void DoLoss()
        {
            if(!didLoss)
            {
                didLoss = true;
                var skillIssue = new UIPanel();
                skillIssue.size = new Vector2(Renderer.UIPreferedWidth, Renderer.UIPreferedHeight);

                var wowYouSuck = new UIBorderedText("Damn.. You actually suck..");
                wowYouSuck.SetPosition(new Vector2(Renderer.UIPreferedWidth, Renderer.UIPreferedHeight) * 0.5f - wowYouSuck.font.MeasureString(wowYouSuck.text) * 0.5f);
                wowYouSuck.color = Color.White * 0;
                wowYouSuck.borderColor = Color.White * 0;
                skillIssue.outlineColor = Color.White * 0f;
                skillIssue.fillColor = Color.White * 0f;


                var wowYouSuck2 = new UIBorderedText("Have you tried, like, opening your eyes maybe?");
                wowYouSuck2.SetPosition(new Vector2(Renderer.UIPreferedWidth, Renderer.UIPreferedHeight) * 0.5f - wowYouSuck.font.MeasureString(wowYouSuck2.text) * 0.5f + new Vector2(0, 40));
                wowYouSuck2.color = Color.White * 0;
                wowYouSuck2.borderColor = Color.White * 0;

                skillIssue.Append(wowYouSuck);
                skillIssue.Append(wowYouSuck2);

                skillIssue.onUpdate = (sender) =>
                {
                    if (++timer >= 120)
                    {
                        wowYouSuck2.color = Color.Lerp(wowYouSuck2.color, Color.White, 0.026f);
                        wowYouSuck2.borderColor = Color.Lerp(wowYouSuck2.borderColor, Color.Black, 0.026f);
                    }

                    if(timer >= 240)
                    {
                        if (!showedRestartOptions)
                        {
                            UIScrollableMenu options = new UIScrollableMenu(3, ["Restart from zone entrance", "Retry battle", "Quit q_q"]);
                            options.drawArrows = false;
                            options.panelColor = Color.Transparent;
                            options.borderColor = Color.Transparent;
                            options.SetPosition(new Vector2(Renderer.UIPreferedWidth, Renderer.UIPreferedHeight) * 0.5f - wowYouSuck.font.MeasureString(wowYouSuck2.text) * 0.5f + new Vector2(0, 80));

                            UIManager.combatUI.Append(options);
                            showedRestartOptions = true;
                        }
                    }

                    wowYouSuck.color = Color.Lerp(wowYouSuck.color, Color.White, 0.026f);
                    wowYouSuck.borderColor = Color.Lerp(wowYouSuck.borderColor, Color.Black, 0.026f);
                    skillIssue.fillColor = Color.Lerp(skillIssue.fillColor, Color.Red * 0.65f, 0.026f);
                };

                UIManager.combatUI.Append(skillIssue);
            }
        }

        bool showedVictory;
        int showedVictoryTimer;

        public void PlayVictory()
        {
            if(!showedVictory)
            {
                var victoryDeclaration = new UIAnimatedPanel(new Vector2(400, 40), UIAnimatedPanel.AnimationStyle.Horizontal);
                victoryDeclaration.id = "victory";
                victoryDeclaration.SetPosition(Renderer.UIPreferedWidth * 0.5f - 200, 80);
                victoryDeclaration.openSpeed = 0.25f;
                var victoryText = new UIBorderedText("VICTORY!");
                victoryText.color = Color.Gold;

                victoryText.SetPosition(200 - victoryText.font.MeasureString(victoryText.text).X * 0.5f, 8);

                victoryDeclaration.Append(victoryText);

                victoryDeclaration.onUpdate = (sender) =>
                {
                    if (++showedVictoryTimer >= 600 || Input.PressedKey([Keys.E, Keys.Enter]))
                        victoryDeclaration.isClosed = true;
                };

                UIManager.combatUI.Append(victoryDeclaration);
                showedVictory = true;
            }

            if(Input.PressedKey([Keys.E, Keys.Enter]) && UIManager.combatUI.levelUpElements.Count <= 0)
            {
                UIManager.combatUI.Disown("victory");
                GameStateManager.SetState(GameState.Overworld, new SliceTransition(Renderer.SaveFrame()));
                OnBattleEnd?.Invoke();
                Main.instance.battle = null;
                playerParty = null;
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
                spriteBatch.Draw(Assets.GetTexture(bg.texture), Vector2.Zero, bg.color);
            }

            foreach (Unit unit in units)
            {
                Color clr = unit.Downed ? Color.Crimson : Color.White;
                Vector2 position = unit.position + new Vector2(unit.shake * Main.rand.Next(2) % 2 == 0 ? 1 : -1, 0f);
                if (unit.animations.TryGetValue(unit.currentAnimation, out var anim))
                {
                    anim.position = position;

                    anim.Draw(spriteBatch, unit.scale * anim.scale);

                } else
                {
                    spriteBatch.Draw(Assets.GetTexture(unit.sprite), new Vector2((int)(position.X), (int)(position.Y)), null, clr * unit.opacity, 0f, new Vector2(16), unit.scale, SpriteEffects.None, unit.depth);
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
                spriteBatch.Draw(Assets.GetTexture("Arrow"), new Vector2((int)(ActingUnit.position.X), (int)(ActingUnit.position.Y) - 24 + (float)Math.Cos(Main.totalTime)), null, clr * ActingUnit.opacity, 0f, new Vector2(5, 3.5f), new Vector2((float)Math.Sin(Main.totalTime * 0.75f), 1f), SpriteEffects.None, 1f);
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

            if (!weaknessHitLastRound)
            {
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
            weaknessHitLastRound = false;
        }

        private void HandleMenus()
        {
            var playerMenu = new UIScrollableMenu(3, "Attack", "Item", "Guard");
            playerMenu.openSpeed = 0.25f;

            playerMenu.SetPosition(64, Renderer.UIPreferedHeight * 0.5f - playerMenu.targetSize.Y * 0.5f);

            playerMenu.onSelectOption = (sender) =>
                {
                    playerMenu.focused = false;
                    switch (playerMenu.CurrentOption)
                    {
                        case "Attack":
                            string[] attackNames = ActingUnit.abilities.Select(x => x.Name).ToArray();
                            int[] unusable = ActingUnit.abilities.Where(x => !x.CanCast(ActingUnit)).Select(x=> ActingUnit.abilities.IndexOf(x)).ToArray();

                            var attacks = new UIScrollableMenu(8, attackNames)
                            {
                                Padding = 32
                            };
                            attacks.openSpeed = 0.25f;
                            attacks.SetPosition(playerMenu.GetPosition() + new Vector2(playerMenu.targetSize.X, -attacks.targetSize.Y * 0.25f));
                            attacks.unavailableOptions.AddRange(unusable);

                            attacks.onUnavailableSelectOption = (_) => { SoundEngine.PlaySound("MeepMerp"); };

                            var attackDescriptionPanel = new UIAnimatedPanel(new Vector2(400, 140));
                            attackDescriptionPanel.openSpeed = 0.35f;
                            attackDescriptionPanel.SetPosition(attacks.GetPosition() + new Vector2(attacks.targetSize.X + 10, attacks.targetSize.Y * 0.25f));
                            var attackDescriptionText = new UIBorderedText("");

                            attackDescriptionText.SetPosition(8);

                            var attackCost = new UIBorderedText("");
                            attackCost.SetPosition(8, 100);
                            attacks.ExtraSpacing = 8;
                            attackDescriptionPanel.Append(attackCost);

                            attacks.onUpdate = (sender) =>
                            {
                                if (Input.PressedKey(Keys.Q) && attacks.focused)
                                {
                                    attackDescriptionPanel.isClosed = attacks.closed = true;
                                }
                            };

                            UIPicture[] pic = new UIPicture[8];

                            for (int i = 0; i < 8; i++)
                            {
                                pic[i] = new UIPicture("ElementsIconsFramed");
                                pic[i].SetPosition(8, 10 + 32 * i + 4 * i);
                                attacks.Append(pic[i]);
                            }

                            attacks.onSelectOption = (sender) =>
                            {
                                attacks.focused = false;
                                attacks.suspended = true;
                                attackDescriptionPanel.suspended = true;
                                var ability = ActingUnit.abilities[attacks.currentSelectedOption];
                                TargetingWith = ability;
                                isPickingTarget = true;

                                var fakeMenu = new UIScrollableMenu(1, "");

                                fakeMenu.Visible = false;

                                fakeMenu.onScrollOption = (_)
                                =>
                                {
                                    selectedTarget += fakeMenu.currentSelectedOption;
                                };

                                fakeMenu.onSelectOption = (_) =>
                                {
                                    fakeMenu.closed = true;
                                    isPickingTarget = false;
                                    ability.Use(ActingUnit, this, TryGetTargets(ability));
                                    state = BattleState.DoAction;
                                    playerMenu.closed = true;
                                    attacks.closed = true;
                                    attackDescriptionPanel.isClosed = true;
                                    ActingUnit.lastAbilityIndex = attacks.currentSelectedOption;
                                };

                                fakeMenu.openSpeed = 1f;

                                fakeMenu.onUpdate = (sender) =>
                                {
                                    if (Input.PressedKey([Keys.Escape, Keys.Q]) && (sender as UIScrollableMenu).focused)
                                    {
                                        isPickingTarget = false;
                                        TargetingWith = null;
                                        fakeMenu.closed = true;
                                    }

                                    if (Input.PressedKey(Keys.D))
                                        selectedTarget += 3;

                                    if (Input.PressedKey(Keys.A))
                                        selectedTarget -= 3;
                                };

                                fakeMenu.onLoseParent = (_) =>
                                {
                                    attacks.focused = true;
                                    attacks.suspended = false;
                                    attackDescriptionPanel.suspended = false;
                                };

                                UIManager.combatUI.Append(fakeMenu);
                            };

                            attacks.onLoseParent = (sender) => { playerMenu.focused = true; };


                            attacks.onScrollOption = (sender) =>
                            {
                            };

                            attacks.onChangeOption = (sender) =>
                            {
                                for (int i = 0; i < pic.Length; i++)
                                {
                                    if (pic[i] == null)
                                        continue;

                                    var ableism = ActingUnit.abilities.Count > attacks.OptionWindowMin + i ? ActingUnit.abilities[attacks.OptionWindowMin + i] : null;
                                    if (ableism != null)
                                    {
                                        pic[i].frames = [new FrameData(0, 32 * (int)ableism.elementalType, 32, 32)];
                                    } else
                                    {
                                        pic[i].frames = [new(0, 32 * 9, 32, 32)];
                                    }
                                }

                                var ability = ActingUnit.abilities[attacks.currentSelectedOption];
                                attackDescriptionText.text = ability.Description;

                                string cost = "Cost: ";
                                if (ability.hpCost > 0)
                                    cost += $"{ability.hpCost} HP ";

                                if (ability.spCost > 0)
                                    cost += $"{ability.spCost} SP\n";

                                if (ability.spCost <= 0 && ability.hpCost <= 0)
                                    cost = "";
                                attackCost.text = cost;
                            };

                            if(ActingUnit.lastAbilityIndex < attacks.options.Count)
                            {
                                attacks.currentSelectedOption = ActingUnit.lastAbilityIndex;
                                attacks.onChangeOption?.Invoke(attacks);
                            }


                            attackDescriptionPanel.Append(attackDescriptionText);

                            UIManager.combatUI.Append(attackDescriptionPanel);
                            UIManager.combatUI.Append(attacks);
                            break;
                        case "Item":
                            OpenInventory(playerMenu);
                            break;
                        case "Guard":
                            playerMenu.closed = true;
                            ActingUnit.AddEffect(new GuardingEffect());
                            state = BattleState.DoAction;
                            break;
                    }
                };


            UIManager.combatUI.Append(playerMenu);
        }

        private void OpenInventory(UIScrollableMenu playerMenu)
        {
            List<string> items = [];

            foreach (var item in GlobalPlayer.Inventory)
            {
                items.Add($"{item.count} x {item.name}");
            }

            var InventoryMenu = new UIScrollableMenu(12, items.ToArray())
            {
                id = "inventoryMenu"
            };

            var uiPicture = new UIPicture("", new FrameData());
            uiPicture.SetPosition(300 - 16, 32);

            var descriptionPanel = new UIAnimatedPanel(new Vector2(600, 368), UIAnimatedPanel.AnimationStyle.FourWay);
            descriptionPanel.SetPosition(180 + InventoryMenu.targetSize.X, 160);
            descriptionPanel.openSpeed = 0.35f;
            UIBorderedText description = new("", 40);

            descriptionPanel.Append(description);
            descriptionPanel.Append(uiPicture);
            description.SetPosition(16, 160);

            InventoryMenu.onUpdate = (sender) =>
            {
                if (Input.PressedKey([Keys.Escape, Keys.Q]) && (sender as UIScrollableMenu).focused)
                {
                    InventoryMenu.focused = false;
                    InventoryMenu.closed = true;
                    descriptionPanel.isClosed = true;
                }
            };
            InventoryMenu.onSelectOption = (sender) =>
            {
                var item = GlobalPlayer.Inventory[InventoryMenu.currentSelectedOption];

                switch (item.canTarget)
                {
                    case ValidTargets.World:
                        item.Use(playerParty[0], null, playerParty);
                        break;
                    case ValidTargets.Enemy:

                        TargetingWith = item;

                        InventoryMenu.focused = false;
                        InventoryMenu.suspended = true;
                        descriptionPanel.suspended = true;

                        isPickingTarget = true;

                        var fakeMenu = new UIScrollableMenu(1, "");

                        fakeMenu.Visible = false;

                        fakeMenu.onScrollOption = (_)
                        =>
                        {
                            selectedTarget += fakeMenu.currentSelectedOption;
                        };

                        fakeMenu.onSelectOption = (_) =>
                        {
                            fakeMenu.closed = true;
                            InventoryMenu.openSpeed = 1f;
                            descriptionPanel.openSpeed = 1f;
                            CloseInventory(InventoryMenu, descriptionPanel);
                            isPickingTarget = false;
                            UIManager.combatUI.skillDescription.text = "";
                            UIManager.combatUI.skillCost.text = "";
                            ClearMenus();
                            item.Use(ActingUnit, this, TryGetTargets(item));
                            state = BattleState.DoAction;
                            playerMenu.closed = true;
                            ActingUnit.lastItemIndex = InventoryMenu.currentSelectedOption;
                        };

                        fakeMenu.openSpeed = 1f;

                        fakeMenu.onUpdate = (sender) =>
                        {
                            if (Input.PressedKey([Keys.Escape, Keys.Q]) && (sender as UIScrollableMenu).focused)
                            {
                                isPickingTarget = false;
                                TargetingWith = null;
                                fakeMenu.closed = true;
                            }

                            if (Input.PressedKey(Keys.D))
                                selectedTarget += 3;

                            if (Input.PressedKey(Keys.A))
                                selectedTarget -= 3;
                        };

                        fakeMenu.onLoseParent = (_) =>
                        {
                            ExposedTargets.Clear();
                            InventoryMenu.focused = true;
                            InventoryMenu.suspended = false;
                            descriptionPanel.suspended = false;
                        };

                        UIManager.combatUI.Append(fakeMenu);

                        break;
                    case ValidTargets.DownedAlly:
                    case ValidTargets.Ally:
                        InventoryMenu.focused = false;
                        if (item.aoe)
                        {
                            var useOnAll = new UIScrollableMenu(2, "Cancel", "Use");
                            useOnAll.openSpeed = 0.35f;
                            useOnAll.onSelectOption = (sender) =>
                            {
                                switch (useOnAll.CurrentOption)
                                {
                                    case "Cancel":
                                        useOnAll.closed = true;
                                        break;
                                    case "Use":
                                        useOnAll.closed = true;
                                        item.Use(playerParty[0], null, playerParty);
                                        CloseInventory(InventoryMenu, descriptionPanel);
                                        playerMenu.closed = true;
                                        ActingUnit.lastItemIndex = InventoryMenu.currentSelectedOption;
                                        break;
                                }
                            };

                            useOnAll.onLoseParent = (sender) =>
                            {
                                InventoryMenu.focused = true;
                                InventoryMenu.focused = true;
                                var oldCount = InventoryMenu.options.Count;
                                InventoryMenu.options = GlobalPlayer.Inventory.Select(item => $"{item.count} x {item.name}").ToList();

                                if (InventoryMenu.options.Count != oldCount)
                                {
                                    InventoryMenu.currentSelectedOption = InventoryMenu.options.Count - 1;
                                    InventoryMenu.onChangeOption?.Invoke(InventoryMenu);
                                }
                            };

                            InventoryMenu.Append(useOnAll);
                            useOnAll.SetPosition(InventoryMenu.targetSize.X * 0.5f - useOnAll.targetSize.X * 0.5f, InventoryMenu.targetSize.Y * 0.5f - useOnAll.targetSize.Y * 0.5f);

                            break;
                        }
                        List<string> allyNames = playerParty.Select(x => x.name).ToList();

                        var allySelect = new UIScrollableMenu(4, [.. allyNames])
                        {
                            openSpeed = 0.25f,
                            onLoseParent = (sender) =>
                            {
                                InventoryMenu.focused = true;
                                var oldCount = InventoryMenu.options.Count;
                                InventoryMenu.options = GlobalPlayer.Inventory.Select(item => $"{item.count} x {item.name}").ToList();

                                if (InventoryMenu.options.Count != oldCount)
                                {
                                    InventoryMenu.currentSelectedOption = InventoryMenu.options.Count - 1;
                                    InventoryMenu.onChangeOption?.Invoke(InventoryMenu);
                                }
                            }
                        };

                        var selectText = new UIBorderedText("Use on..");
                        selectText.color = new Color(57, 255, 20);
                        allySelect.drawArrows = false;
                        allySelect.Append(selectText);
                        selectText.SetPosition(20, -14);

                        allySelect.onUpdate = (sender) =>
                        {
                            if (Input.PressedKey([Keys.Escape, Keys.Q]) && (sender as UIScrollableMenu).focused)
                                allySelect.closed = true;
                        };

                        allySelect.onSelectOption = (sender) =>
                        {
                            var item = GlobalPlayer.Inventory[InventoryMenu.currentSelectedOption];
                            var target = playerParty[allySelect.currentSelectedOption];
                            if ((!target.Downed && item.canTarget == ValidTargets.Ally) || (target.Downed && item.canTarget == ValidTargets.DownedAlly))
                            {
                                ActingUnit.lastItemIndex = InventoryMenu.currentSelectedOption;
                                playerMenu.closed = true;
                                CloseInventory(InventoryMenu, descriptionPanel);
                                item.Use(target, null, [target]);
                            } else
                                SoundEngine.PlaySound("MeepMerp");
                            allySelect.closed = true;

                        };
                        allySelect.SetPosition(InventoryMenu.targetSize.X * 0.5f - allySelect.targetSize.X * 0.5f, InventoryMenu.targetSize.Y * 0.5f - allySelect.targetSize.Y * 0.5f);

                        allySelect.onChangeOption?.Invoke(allySelect);

                        InventoryMenu.Append(allySelect);
                        break;
                }
                
            };

            InventoryMenu.onChangeOption = (sender) =>
            {
                if (GlobalPlayer.Inventory.Count > 0)
                {
                    var item = GlobalPlayer.Inventory[InventoryMenu.currentSelectedOption];
                    if (!string.IsNullOrWhiteSpace(item.icon))
                    {
                        uiPicture.frames = item.frames;
                        uiPicture.textureName = item.icon;
                        uiPicture.scale = item.iconScale;
                        uiPicture.rotation = item.iconRotation;
                        uiPicture.origin = item.iconOrigin;

                        item.onViewed?.Invoke(item);

                        uiPicture.SetPosition(descriptionPanel.size.X * 0.5f, 64);
                    } else
                    {
                        uiPicture.frames = null;
                        uiPicture.textureName = "";
                    }
                    description.text = item.description;
                } else
                {
                    uiPicture.frames = null;
                    uiPicture.textureName = "";
                    uiPicture.scale = Vector2.One;
                    uiPicture.origin = Vector2.Zero;
                    uiPicture.origin = Vector2.Zero;
                    description.text = "Holy shit this lack of items is kinda\ndepressing.";
                }
            };

            InventoryMenu.onLoseParent += (sender) =>
            {
                isPickingTarget = false;
                TargetingWith = null;
                playerMenu.focused = true;
            };

            InventoryMenu.openSpeed = 0.35f;

            InventoryMenu.SetPosition(160);

            if (ActingUnit.lastItemIndex < InventoryMenu.options.Count)
            {
                InventoryMenu.currentSelectedOption = ActingUnit.lastItemIndex;
                InventoryMenu.onChangeOption?.Invoke(InventoryMenu);
            }

            UIManager.combatUI.Append(InventoryMenu);
            UIManager.combatUI.Append(descriptionPanel);
        }

        private void CloseInventory(UIScrollableMenu InventoryMenu, UIAnimatedPanel descriptionPanel, bool doSequence = true)
        {
            InventoryMenu.closed = true;
            descriptionPanel.isClosed = true;

            menus.Clear();

            if (doSequence)
            {
                Sequence seq = CreateSequence(true);
                seq.SetAnimation(ActingUnit, "Cast");
                seq.Delay(60);
            }

            state = BattleState.DoAction;
        }

        private static void SetSkillDesc(Ability skill)
        {
            UIManager.combatUI.skillDescription.text = skill.Description.Splice(24);
            UIManager.combatUI.skillCost.text = $"{(skill.hpCost > 0 ? $"Consumes {skill.hpCost} HP" : "")}{(skill.spCost > 0 ? $"Consumes {skill.spCost} SP" : "")}";
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
                UIManager.combatUI.beganActing = true;
            }

            State = BattleState.CheckInput;
        }

        public void DoAction()
        {
            ExposedTargets.Clear();
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

            if(!weaknessHitLastRound)
            foreach (StatusEffect effect in ActingUnit.statusEffects)
            {
                effect.OnTurnEnd(ActingUnit, this);
            }

            turnCount++;
            State = BattleState.BeginTurn;
            ActingUnit.depth = 0.01f;

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
                return;
            }

            unitsHitLastRound.Clear();
            if (++_actingUnit >= units.Count)
                _actingUnit = 0;

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
            if (winConditions.All(x=> x?.Invoke(this) == true))
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
                        expValue += unit.Stats.value;
                    }

                    Sequence seq = new(this)
                    {
                        active = true
                    };

                    seq.Add(new SetActorAnimation(unit, "Victory"));

                    this.sequences.Add(seq);
                }

                bool anyLevelUps = false;
                foreach(Unit unit in GlobalPlayer.ActiveParty)
                {
                    unit.Stats.EXP += expValue;
                    unit.ClearEffects();


                    bool leveledUp = unit.TryLevelUp();

                    if (!anyLevelUps)
                        anyLevelUps = leveledUp;
                }

                if(anyLevelUps)
                {
                    Sequence delayLevelUps = CreateSequence();
                    delayLevelUps.Delay(180);
                    delayLevelUps.CustomAction(() => { UIManager.combatUI.showLevelUps = true; });
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

        private List<Unit> _exposedTargets = [];

        public List<Unit> ExposedTargets
        { 
            get => _exposedTargets;
        }

        public List<Unit> TryGetTargets(ICanTarget getTargetFor)
        {
            ExposedTargets.Clear();
            Func<Unit, bool> selector = x => ActingUnit.team != x.team && !x.Downed;

            switch (getTargetFor.CanTarget())
            {
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
            {
                ExposedTargets.Clear();
                return [];
            }

            if (getTargetFor.AoE())
            {
                ExposedTargets.AddRange(targets);
                return targets;
            }

            if (Menu.mouseEnabled)
            {
                var tryMouse = targets.FirstOrDefault(x => x.ContainsMouse(-x.size * 0.5f));
                if (tryMouse != null)
                    selectedTarget = targets.IndexOf(tryMouse);
            }

            if (selectedTarget < 0)
                selectedTarget = targets.Count - 1;

            if (selectedTarget > targets.Count - 1)
                selectedTarget = 0;

            ExposedTargets.Add(targets[selectedTarget]);
            return [targets[selectedTarget]];
        }

        public Sequence CreateSequence(bool startActive = false)
        {
            Sequence seq = new(this);
            seq.active = startActive;
            sequences.Add(seq);
            return seq;
        }

        public Unit ActingUnit => units[_actingUnit];

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

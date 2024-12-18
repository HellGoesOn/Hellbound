using Casull.Core.Combat;
using Casull.Core.UI;
using Casull.Core.UI.Elements;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Casull.Core.Overworld
{
    public class OverworldUIState : UIState
    {
        //public UIBorderedText debugText;

        int _heldTimer;
        float _repeatRate;

        public UIScrollableMenu optionMenu;
        UIAnimatedPanel blackBarTop;
        UIAnimatedPanel blackBarBot;
        public UIBorderedText interactText;

        public float interactTextOpacityTarget = 0.0f;

        public int lastTarget;

        public List<UIBorderedText> notificationTexts = [];
        List<OverworldNotification> notifications = [];
        public int NotifCount => notifications.Count;
        List<UIBorderedText> notificationQueue = [];
        Queue<OverworldNotification> startedNotificationQueue = [];
        UIBorderedText zoneName;
        internal int showNameTimer;

        public OverworldUIState()
        {
            zoneName = new UIBorderedText("");
            zoneName.SetPosition(Renderer.ScreenMiddle);
            zoneName.SetFont(Assets.CombatMenuFont);

            Append(zoneName);
            //debugText = new("");
            //Append(debugText);

            for (int i = 0; i < 6; i++) {
                var newText = new UIBorderedText("Testing");
                newText.SetPosition(16, Renderer.UIPreferedHeight - 64 - newText.font.MeasureString("Y").Y*6 + newText.font.MeasureString("Y").Y *i);
                newText.opacity = 0.0f;
                notificationTexts.Add(newText);
                notificationQueue.Add(newText);
                Append(notificationTexts[i]);
            }

            interactText = new UIBorderedText("Press <ffff00/[E]> to Interact");
            interactText.SetPosition(Renderer.ScreenMiddle + new Vector2(0, 200)-(interactText.font.MeasureString("Press [E] to Interact") * 0.5f));
            interactText.opacity = 0.0f;

            Append(interactText);


            blackBarTop = new UIAnimatedPanel(new Vector2(Renderer.UIPreferedWidth, 160));
            blackBarTop.SetPosition(0, -80);
            blackBarTop.borderColor = Color.Black;
            blackBarTop.panelColor = Color.Black;
            blackBarTop.suspended = true;


            blackBarBot = new UIAnimatedPanel(new Vector2(Renderer.UIPreferedWidth, 160));
            blackBarBot.SetPosition(0, Renderer.UIPreferedHeight - 80);
            blackBarBot.borderColor = Color.Black;
            blackBarBot.panelColor = Color.Black;
            blackBarBot.suspended = true;

            Append(blackBarTop);
            Append(blackBarBot);
        }

        public void Notify(string text, Color color, int timeLeft)
        {
            if(GameStateManager.State == GameState.Overworld)
                notifications.Add(new(text, color, timeLeft));
        }

        public void SetBlackBars(bool value)
        {
            blackBarTop.suspended = blackBarBot.suspended = !value;
        }

        public override void Update()
        {
            if (!string.IsNullOrWhiteSpace(Main.instance.ActiveWorld.zoneName)) {
                zoneName.text = Main.instance.ActiveWorld.zoneName;
                zoneName.SetPosition(Renderer.ScreenMiddle - zoneName.size * 0.5f);
            }

            if(showNameTimer > 0) {
                if (zoneName.opacity < 1.0f)
                    zoneName.opacity += 0.05f;

                showNameTimer--;
            }
            else if(zoneName.opacity >0) {
                zoneName.opacity -= 0.05f;
            }

            if (notifications.Count > 0) {
                var notif = notifications[0];
                if (notificationQueue.Count() > 0 && !notif.started) {
                    var text = notificationQueue[0];
                    text.text = notif.message;
                    text.color = notif.color;
                    text.opacity = 1.0f;
                    notif.started = true;
                    notif.index = notificationTexts.IndexOf(text);
                    startedNotificationQueue.Enqueue(notif);
                    notifications.RemoveAt(0);
                    notificationQueue.RemoveAt(0);
                }
            }

            if (startedNotificationQueue.Count > 0 && --startedNotificationQueue.Peek().timeLeft <= 0) {
                var started =
                startedNotificationQueue.Dequeue();
                notificationQueue.Add(notificationTexts[started.index]);
                notificationTexts[started.index].opacity = 0.0f;
            }

            //UIManager.Debug($"{notifications.Count}; {notificationQueue.Count};");

            if (interactText.opacity != interactTextOpacityTarget) {
                interactText.opacity += 0.1f * Math.Sign((interactTextOpacityTarget - interactText.opacity));

                if (Math.Abs(interactTextOpacityTarget - interactText.opacity) <= 0.1f) {
                    interactText.opacity = interactTextOpacityTarget;
                }
            }

            base.Update();

            if (Input.PressedKey(Keys.Escape) && optionMenu == null && Main.instance.GetGameState() is World && World.cutscenes.Count <= 0) {
                Main.instance.ActiveWorld.paused = true;

                blackBarTop.suspended = blackBarBot.suspended = false;

                UIPanel darkening = new();
                darkening.fillColor = Color.Black * 0.5f;
                darkening.outlineColor = Color.White * 0f;
                darkening.size = new Vector2(Renderer.UIPreferedWidth, Renderer.UIPreferedHeight);

                string[] mainOptions = ["Continue", "View Party", "Inventory", "Settings", "Quit"];

                if(UIManager.combatUI.tutorialProgress < 1)
                    mainOptions = ["Continue", "Settings", "Quit"];

                bool angry = Main.instance.spiritsAngered;

                if (angry)
                    mainOptions = ["Continue"];

                optionMenu = new UIScrollableMenu(angry ? 1 : mainOptions.Length, mainOptions) {
                    drawArrows = false,
                    openSpeed = 0.15f,
                    onSelectOption = (sender)
                    => {
                        var optionMenuBox = (UIScrollableMenu)sender;

                        switch (optionMenuBox.CurrentOption) {
                            case "Continue":
                                optionMenuBox.closed = true;
                                optionMenu = null;
                                break;
                            case "Inventory":
                                optionMenuBox.focused = false;

                                List<string> items = [];

                                foreach (var item in GlobalPlayer.Inventory) {
                                    items.Add($"{item.count} x {item.name}");
                                }

                                var InventoryMenu = new UIScrollableMenu(12, items.ToArray()) {
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

                                InventoryMenu.onUpdate = (sender) => {
                                    if (Input.PressedKey([Keys.Escape, Keys.Q]) && (sender as UIScrollableMenu).focused) {
                                        InventoryMenu.focused = false;
                                        InventoryMenu.closed = true;
                                        descriptionPanel.isClosed = true;
                                    }
                                };

                                InventoryMenu.onSelectOption = (sender) => {
                                    var item = GlobalPlayer.Inventory[InventoryMenu.currentSelectedOption];

                                    if (item.canUseOutOfBattle) {
                                        switch (item.canTarget) {
                                            case ValidTargets.World:
                                                item.Use(GlobalPlayer.ActiveParty[0], null, GlobalPlayer.ActiveParty);
                                                break;
                                            case ValidTargets.DownedAlly:
                                            case ValidTargets.Ally:
                                                InventoryMenu.focused = false;
                                                if (item.aoe) {
                                                    var useOnAll = new UIScrollableMenu(2, "Cancel", "Use");
                                                    useOnAll.openSpeed = 0.35f;
                                                    useOnAll.onSelectOption = (sender) => {
                                                        switch (useOnAll.CurrentOption) {
                                                            case "Cancel":
                                                                useOnAll.closed = true;
                                                                break;
                                                            case "Use":
                                                                useOnAll.closed = true;
                                                                item.Use(GlobalPlayer.ActiveParty[0], null, GlobalPlayer.ActiveParty);

                                                                break;
                                                        }
                                                    };

                                                    useOnAll.onLoseParent = (sender) => {
                                                        InventoryMenu.focused = true;
                                                        lastTarget = (sender as UIScrollableMenu).currentSelectedOption;
                                                        InventoryMenu.focused = true;
                                                        var oldCount = InventoryMenu.options.Count;
                                                        InventoryMenu.options = GlobalPlayer.Inventory.Select(item => $"{item.count} x {item.name}").ToList();

                                                        if (InventoryMenu.options.Count != oldCount) {
                                                            InventoryMenu.currentSelectedOption = InventoryMenu.options.Count - 1;
                                                            InventoryMenu.onChangeOption?.Invoke(InventoryMenu);
                                                        }
                                                    };

                                                    InventoryMenu.Append(useOnAll);
                                                    useOnAll.SetPosition(InventoryMenu.targetSize.X * 0.5f - useOnAll.targetSize.X * 0.5f, InventoryMenu.targetSize.Y * 0.5f - useOnAll.targetSize.Y * 0.5f);

                                                    break;
                                                }
                                                List<string> allyNames = GlobalPlayer.ActiveParty.Select(x => x.name).ToList();

                                                var allySelect = new UIScrollableMenu(4, [.. allyNames]) {
                                                    openSpeed = 0.25f,
                                                    onLoseParent = (sender) => {
                                                        lastTarget = (sender as UIScrollableMenu).currentSelectedOption;
                                                        InventoryMenu.focused = true;
                                                        var oldCount = InventoryMenu.options.Count;
                                                        InventoryMenu.options = GlobalPlayer.Inventory.Select(item => $"{item.count} x {item.name}").ToList();

                                                        if (InventoryMenu.options.Count != oldCount) {
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

                                                allySelect.onUpdate = (sender) => {
                                                    if (Input.PressedKey([Keys.Escape, Keys.Q]) && (sender as UIScrollableMenu).focused)
                                                        allySelect.closed = true;
                                                };

                                                allySelect.onSelectOption = (sender) => {
                                                    var item = GlobalPlayer.Inventory[InventoryMenu.currentSelectedOption];
                                                    var target = GlobalPlayer.ActiveParty[allySelect.currentSelectedOption];
                                                    if ((!target.Downed && item.canTarget == ValidTargets.Ally) || (target.Downed && item.canTarget == ValidTargets.DownedAlly)) {
                                                        item.Use(target, null, [target]);
                                                    }
                                                    else
                                                        SoundEngine.PlaySound("MeepMerp");
                                                    allySelect.closed = true;

                                                };
                                                allySelect.SetPosition(InventoryMenu.targetSize.X * 0.5f - allySelect.targetSize.X * 0.5f, InventoryMenu.targetSize.Y * 0.5f - allySelect.targetSize.Y * 0.5f);

                                                allySelect.currentSelectedOption = Math.Clamp(lastTarget, 0, GlobalPlayer.ActiveParty.Count - 1);
                                                allySelect.onChangeOption?.Invoke(allySelect);

                                                InventoryMenu.Append(allySelect);
                                                break;
                                        }
                                    }
                                };

                                InventoryMenu.onChangeOption = (sender) => {
                                    if (GlobalPlayer.Inventory.Count > 0) {
                                        var item = GlobalPlayer.Inventory[InventoryMenu.currentSelectedOption];
                                        if (!string.IsNullOrWhiteSpace(item.icon)) {
                                            uiPicture.frames = item.frames;
                                            uiPicture.textureName = item.icon;
                                            uiPicture.scale = item.iconScale;
                                            uiPicture.Rotation = item.iconRotation;
                                            uiPicture.origin = item.iconOrigin;

                                            item.onViewed?.Invoke(item);

                                            uiPicture.SetPosition(descriptionPanel.size.X * 0.5f, 64);
                                        }
                                        else {
                                            uiPicture.frames = null;
                                            uiPicture.textureName = "";
                                        }
                                        description.text = item.Description;
                                    }
                                    else {
                                        uiPicture.frames = null;
                                        uiPicture.textureName = "";
                                        uiPicture.scale = Vector2.One;
                                        uiPicture.origin = Vector2.Zero;
                                        uiPicture.origin = Vector2.Zero;
                                        description.text = "Holy shit this lack of items is kinda\ndepressing.";
                                    }
                                };

                                InventoryMenu.onLoseParent += (sender) => {
                                    optionMenuBox.focused = true;
                                };

                                InventoryMenu.openSpeed = 0.35f;

                                InventoryMenu.SetPosition(160);

                                optionMenu.parent.Append(InventoryMenu);
                                optionMenu.parent.Append(descriptionPanel);

                                break;
                            case "View Party":
                                optionMenuBox.focused = false;
                                string[] partyMemberNames = GlobalPlayer.ActiveParty.Select(x => x.name).ToArray();


                                var partyMemberMenu = new UIScrollableMenu(4, partyMemberNames);
                                partyMemberMenu.SetPosition(200, 240);
                                partyMemberMenu.openSpeed = 0.25f;


                                partyMemberMenu.onLoseParent += (sender) => {
                                    optionMenuBox.focused = true;
                                };

                                var memberPanel = new UIAnimatedPanel(new Vector2(600, 240), UIAnimatedPanel.AnimationStyle.FourWay);
                                memberPanel.openSpeed = 0.25f;
                                memberPanel.SetPosition(partyMemberMenu.GetPosition() + new Vector2(160, -120));

                                var portrait = new UICombatPortrait("", 0, 0);
                                portrait.SetPosition(16, 32);
                                memberPanel.Append(portrait);
                                partyMemberMenu.onUpdate = (sender) => {
                                    if (Input.PressedKey([Keys.Escape, Keys.Q]) && partyMemberMenu.focused) {
                                        memberPanel.isClosed = true;
                                        partyMemberMenu.closed = true;
                                    }

                                    var target = GlobalPlayer.ActiveParty[partyMemberMenu.currentSelectedOption];

                                    portrait.SetValues(target.Stats.HP, target.Stats.SP, target.Stats.MaxHP, target.Stats.MaxSP);
                                };

                                partyMemberMenu.onSelectOption = (sender) => {
                                    partyMemberMenu.focused = false;
                                    var unit = GlobalPlayer.ActiveParty[partyMemberMenu.currentSelectedOption];
                                    string[] attackNames = unit.abilities.Select(x => x.name).ToArray();
                                    int[] unusable = unit.abilities.Where(x => !x.canUseOutOfCombat).Select(x => unit.abilities.IndexOf(x)).ToArray();

                                    var attacks = new UIScrollableMenu(4, attackNames) {
                                        Padding = 32
                                    };
                                    attacks.openSpeed = 0.25f;
                                    attacks.SetPosition(memberPanel.GetPosition() + new Vector2(0, memberPanel.targetSize.Y));
                                    attacks.unavailableOptions.AddRange(unusable);
                                    attacks.notAvailableColor = Color.DarkSlateGray;
                                    attacks.onUnavailableSelectOption = (_) => { SoundEngine.PlaySound("MeepMerp"); };

                                    attacks.onSelectOption = (sender) => {
                                        List<string> allyNames = GlobalPlayer.ActiveParty.Select(x => x.name).ToList();
                                        attacks.focused = false;
                                        var allySelect = new UIScrollableMenu(4, [.. allyNames]) {
                                            openSpeed = 0.25f,
                                            onLoseParent = (sender) => {
                                                lastTarget = (sender as UIScrollableMenu).currentSelectedOption;
                                                attacks.focused = true;
                                            }
                                        };

                                        var selectText = new UIBorderedText("Use on..");
                                        selectText.color = new Color(57, 255, 20);
                                        allySelect.drawArrows = false;
                                        allySelect.Append(selectText);
                                        selectText.SetPosition(20, -14);

                                        allySelect.onUpdate = (sender) => {
                                            if (Input.PressedKey([Keys.Escape, Keys.Q]) && (sender as UIScrollableMenu).focused)
                                                allySelect.closed = true;
                                        };

                                        allySelect.onSelectOption = (sender) => {
                                            var item = GlobalPlayer.ActiveParty[partyMemberMenu.currentSelectedOption].abilities[attacks.currentSelectedOption];
                                            var target = GlobalPlayer.ActiveParty[allySelect.currentSelectedOption];
                                            var caster = GlobalPlayer.ActiveParty[partyMemberMenu.currentSelectedOption];
                                            if ((!target.Downed && item.canTarget == ValidTargets.Ally) || (target.Downed && item.canTarget == ValidTargets.DownedAlly)) {
                                                item.Use(caster, null, [target]);
                                            }
                                            else
                                                SoundEngine.PlaySound("MeepMerp");
                                            allySelect.closed = true;

                                        };
                                        allySelect.SetPosition(attacks.targetSize.X * 0.5f - allySelect.targetSize.X * 0.5f, attacks.targetSize.Y * 0.5f - allySelect.targetSize.Y * 0.5f);

                                        allySelect.currentSelectedOption = Math.Clamp(lastTarget, 0, GlobalPlayer.ActiveParty.Count - 1);
                                        allySelect.onChangeOption?.Invoke(allySelect);

                                        attacks.Append(allySelect);
                                    };

                                    var attackDescriptionPanel = new UIAnimatedPanel(new Vector2(384, attacks.targetSize.Y + 32));
                                    attackDescriptionPanel.openSpeed = 0.35f;
                                    attackDescriptionPanel.SetPosition(attacks.GetPosition() + new Vector2(attacks.targetSize.X, 0));
                                    var attackDescriptionText = new UIBorderedText("");

                                    attackDescriptionText.SetPosition(8);

                                    var attackCost = new UIBorderedText("");
                                    attackCost.SetPosition(8, 140);
                                    attacks.ExtraSpacing = 8;
                                    attackDescriptionPanel.Append(attackCost);

                                    attacks.onUpdate = (sender) => {
                                        if (Input.PressedKey(Keys.Q) && attacks.focused) {
                                            attackDescriptionPanel.isClosed = attacks.closed = true;
                                        }
                                    };

                                    UIPicture[] pic = new UIPicture[8];

                                    for (int i = 0; i < attacks.OptionWindowMax; i++) {
                                        pic[i] = new UIPicture("ElementsIconsFramed");
                                        pic[i].SetPosition(8, 10 + 32 * i + 4 * i);
                                        attacks.Append(pic[i]);
                                    }

                                    attacks.onLoseParent = (sender) => 
                                    {
                                        partyMemberMenu.focused = true;
                                    };

                                    attacks.onChangeOption = (sender) => {
                                        for (int i = 0; i < pic.Length; i++) {
                                            if (pic[i] == null)
                                                continue;

                                            var ableism = unit.abilities.Count > attacks.OptionWindowMin + i ? unit.abilities[attacks.OptionWindowMin + i] : null;
                                            if (ableism != null) {
                                                pic[i].frames = [new FrameData(0, 32 * (int)ableism.elementalType, 32, 32)];
                                            }
                                            else {
                                                pic[i].frames = [new(0, 32 * 9, 32, 32)];
                                            }
                                        }

                                        var ability = unit.abilities[attacks.currentSelectedOption];
                                        attackDescriptionText.text = ability.Description;

                                        string cost = "Cost: ";
                                        if (ability.hpCost > 0)
                                            cost += ability.GetHpCostString + " ";

                                        if (ability.spCost > 0)
                                            cost += ability.GetSpCostString;

                                        if (ability.spCost <= 0 && ability.hpCost <= 0)
                                            cost = "";
                                        attackCost.text = cost;
                                    };

                                    if (unit.lastAbilityIndex < attacks.options.Count) {
                                        attacks.currentSelectedOption = unit.lastAbilityIndex;
                                        attacks.onChangeOption?.Invoke(attacks);
                                    }


                                    attackDescriptionPanel.Append(attackDescriptionText);

                                    Append(attackDescriptionPanel);
                                    Append(attacks);
                                };

                                //var memberName = new UIBorderedText("");
                                //memberName.SetPosition(16);
                                //memberPanel.Append(memberName);


                                //var memberStats = new UIBorderedText("");
                                //memberStats.SetPosition(120, 16);
                                //memberPanel.Append(memberStats);

                                var xxx = 130;
                                var yyy = 120;

                                var nextSkill = new UIBorderedText("");
                                nextSkill.SetPosition(xxx-10, 64);
                                memberPanel.Append(nextSkill);

                                var memberLevel = new UIBorderedText("");
                                memberLevel.SetPosition(120, 16);
                                memberPanel.Append(memberLevel);
                                var requiredEXP = new UIBorderedText("");
                                requiredEXP.SetPosition(memberPanel.targetSize.X, 120);
                                memberPanel.Append(requiredEXP);

                                var expBar = new UIProgressBar(new Vector2(memberPanel.targetSize.X - 130, 6), 0);
                                expBar.fillColor = Color.Yellow;
                                expBar.bgColor = Color.DarkGoldenrod;
                                expBar.bgSize = 4;
                                expBar.SetPosition(120, 48);
                                memberPanel.Append(expBar);

                                var magicStatText = new UIBorderedText("Magic");
                                memberPanel.Append(magicStatText);
                                magicStatText.SetPosition(xxx, yyy);

                                var magicStatBar = new UIProgressBar(new Vector2(100, magicStatText.font.MeasureString("Y").Y - 8), 0);
                                magicStatBar.SetPosition(240, yyy);
                                magicStatBar.fillColor = Color.Yellow;
                                magicStatBar.maxValue = 99;
                                memberPanel.Append(magicStatBar);

                                var magicStatValueText = new UIBorderedText("");
                                magicStatValueText.SetPosition(240 + magicStatBar.size.X, yyy+34);
                                memberPanel.Append(magicStatValueText);

                                var strStatText = new UIBorderedText("Strength");
                                memberPanel.Append(strStatText);
                                strStatText.SetPosition(xxx, yyy + 34);

                                var strStatBar = new UIProgressBar(new Vector2(100, magicStatText.font.MeasureString("Y").Y - 8), 0);
                                strStatBar.SetPosition(240, yyy + 36);
                                strStatBar.fillColor = Color.Yellow;
                                strStatBar.maxValue = 99;
                                memberPanel.Append(strStatBar);

                                var strStatValueText = new UIBorderedText("");
                                strStatValueText.SetPosition(240 + strStatBar.size.X, yyy + 34);
                                memberPanel.Append(strStatValueText);

                                var spdStatText = new UIBorderedText("Speed");
                                memberPanel.Append(spdStatText);
                                spdStatText.SetPosition(xxx, yyy + 70);

                                var spdStatBar = new UIProgressBar(new Vector2(100, magicStatText.font.MeasureString("Y").Y - 8), 0);
                                spdStatBar.SetPosition(240, yyy + 74);
                                spdStatBar.fillColor = Color.Yellow;
                                spdStatBar.maxValue = 99;
                                memberPanel.Append(spdStatBar);

                                var spdStatValueText = new UIBorderedText("");
                                spdStatValueText.SetPosition(240 + spdStatBar.size.X, yyy + 74);
                                memberPanel.Append(spdStatValueText);

                                var affinities = new UIBorderedText("Affinities");
                                memberPanel.Append(affinities);
                                affinities.SetPosition(new Vector2(420, yyy));

                                UIPicture[] affinityTypes = new UIPicture[7];
                                UIPicture[] elementalIcons = new UIPicture[7];
                                for (int i = 0; i < 7; i++) {
                                    var affinityElementPic = new UIPicture("AffinityTypeFramed", [new(0, 0, 32, 32)]);
                                    affinityElementPic.SetPosition(380 + i * 32 + 4 * i - 32, yyy + 64);
                                    affinityElementPic.scale = new Vector2(1);
                                    memberPanel.Append(affinityElementPic);
                                    affinityTypes[i] = affinityElementPic;
                                }

                                for (int i = 0; i < 7; i++) {
                                    var loadedIcons = "ElementsIconsFramed";
                                    var affinityElementPic = new UIPicture(loadedIcons, [new(0, 32 * i, 32, 32)]);
                                    affinityElementPic.SetPosition(380 + i * 32 + 4 * i - 32, yyy + 32);
                                    affinityElementPic.scale = new Vector2(1);
                                    memberPanel.Append(affinityElementPic);
                                    elementalIcons[i] = affinityElementPic;
                                }


                                partyMemberMenu.onChangeOption = (sender) => {
                                    var selectedMember = GlobalPlayer.ActiveParty[partyMemberMenu.currentSelectedOption];

                                    string nextSkillText = "Next Skill: -";
                                    if (selectedMember.learnableAbilities.Count > 0)
                                        nextSkillText = $"Next Skill: {selectedMember.learnableAbilities[0].abilityToLearn.name} at LVL {selectedMember.learnableAbilities[0].requiredLevel}";
                                    nextSkill.text = nextSkillText;

                                    portrait.SetValues(selectedMember.Stats.HP, selectedMember.Stats.SP, selectedMember.Stats.MaxHP, selectedMember.Stats.MaxSP);
                                    portrait.picture.textureName = selectedMember.portrait;
                                    portrait.picture.frames = [new FrameData(0, 0, 32, 32)];
                                    portrait.scale = new Vector2(3);
                                    portrait.lerpSpeed = 4;
                                    //memberName.text = selectedMember.name;
                                    //memberStats.text = selectedMember.Stats.ListStats();
                                    portrait.picture.tint = Color.White;
                                    memberLevel.text = $"Level {selectedMember.Stats.level} | EXP {selectedMember.Stats.EXP}..";
                                    requiredEXP.text = $"..{selectedMember.Stats.toNextLevel}";
                                    requiredEXP.SetPosition(memberPanel.targetSize.X - requiredEXP.font.MeasureString(requiredEXP.text).X - 8, 17);

                                    expBar.value = selectedMember.Stats.EXP;
                                    expBar.maxValue = selectedMember.Stats.toNextLevel;

                                    magicStatBar.value = selectedMember.Stats.magic;
                                    magicStatValueText.text = $"{(int)selectedMember.Stats.magic}";
                                    magicStatValueText.SetPosition(240 + magicStatBar.size.X - magicStatValueText.font.MeasureString(magicStatValueText.text).X, yyy);

                                    strStatBar.value = selectedMember.Stats.strength;
                                    strStatValueText.text = $"{(int)selectedMember.Stats.strength}";
                                    strStatValueText.SetPosition(240 + strStatBar.size.X - strStatValueText.font.MeasureString(strStatValueText.text).X, yyy+34);

                                    spdStatBar.value = selectedMember.Stats.speed;
                                    spdStatValueText.text = $"{(int)selectedMember.Stats.speed}";
                                    spdStatValueText.SetPosition(240 + strStatBar.size.X - strStatValueText.font.MeasureString(spdStatValueText.text).X, yyy+74);


                                    for (int i = 0; i < 7; i++) {

                                        var getValue = selectedMember.resistances[(ElementalType)i];

                                        FrameData[] newFrames = [new FrameData(0, 0, 32, 32)];

                                        if (getValue < 0)
                                            newFrames = [new(0, 32, 32, 32)];
                                        if (getValue > 0 && getValue < 1.0f)
                                            newFrames = [new(0, 64, 32, 32)];
                                        if (getValue == 1.0f)
                                            newFrames = [new(0, 96, 32, 32)];
                                        if (getValue > 1.0f)
                                            newFrames = [new(0, 128, 32, 32)];

                                        var loadedIcons = "ElementsIconsFramed";
                                        if (i == 0 && (Main.rand.Next(25) == 0 || (getValue > 1.0f && Main.rand.Next(5) == 0)))
                                            loadedIcons = "ElementsIconsFramed_EasterEgg";

                                        elementalIcons[i].textureName = loadedIcons;

                                        affinityTypes[i].frames = newFrames;
                                    }

                                    if (selectedMember.Downed) {
                                        portrait.picture.tint = Color.DarkGray;
                                    }
                                };

                                partyMemberMenu.onChangeOption?.Invoke(partyMemberMenu);

                                sender.parent.Append(partyMemberMenu);
                                sender.parent.Append(memberPanel);
                                break;
                            case "Settings":
                                optionMenu.focused = false;

                                var settings = new UIScrollableMenu(4, ["Very biiiiiiiiiiiiiiiiiiiig text", "Music Volume", "Resolution", "Back"]);
                                settings.openSpeed = 0.25f;
                                settings.onChangeOption = (sender) => {
                                    settings.options[0] = $"Game Volume: {Math.Round(GameOptions.GeneralVolume * 100)}%";
                                    settings.options[1] = $"Music Volume: {Math.Round(GameOptions.MusicVolume * 100)}%";
                                    settings.options[2] = $"Resolution: {GameOptions.ScreenWidth}x{GameOptions.ScreenHeight}";
                                };

                                settings.SetPosition(Renderer.UIPreferedWidth * 0.5f - settings.targetSize.X * 0.5f, Renderer.UIPreferedHeight * 0.5f - settings.targetSize.Y * 0.5f);

                                settings.onSelectOption = (sender) => {
                                    switch (settings.CurrentOption) {
                                        case "Back":
                                            settings.closed = true; break;
                                    }
                                };

                                settings.onLoseParent = (sender) => { optionMenu.focused = true; };

                                settings.onUpdate = (sender) => {
                                    if (Input.HeldKey(Keys.A) || Input.HeldKey(Keys.D)) {
                                        _heldTimer++;
                                        if (_heldTimer >= _repeatRate && _repeatRate > 5)
                                            _repeatRate--;
                                    }
                                    else {
                                        _repeatRate = 15;
                                        _heldTimer = 0;
                                    }

                                    settings.options[0] = $"Game Volume: {Math.Round(GameOptions.GeneralVolume * 100)}%";
                                    settings.options[1] = $"Music Volume: {Math.Round(GameOptions.MusicVolume * 100)}%";
                                    settings.options[2] = $"Resolution: {GameOptions.ScreenWidth}x{GameOptions.ScreenHeight}";
                                    if (Input.PressedKey([Keys.Escape, Keys.Q]))
                                        settings.closed = true;

                                    if (Input.PressedKey(Keys.D) || (Input.HeldKey(Keys.D) && _heldTimer >= _repeatRate)) {
                                        _heldTimer = 0;
                                        switch (settings.currentSelectedOption) {
                                            case 0:
                                                GameOptions.GeneralVolume += 0.01f;
                                                break;
                                            case 1:
                                                GameOptions.MusicVolume += 0.01f;
                                                break;
                                            case 2:
                                                GameOptions.ResolutionMultiplier += 1;
                                                Main.instance.UpdateResolution();
                                                break;
                                        }
                                    }

                                    if (Input.PressedKey(Keys.A) || (Input.HeldKey(Keys.A) && _heldTimer >= _repeatRate)) {
                                        _heldTimer = 0;
                                        switch (settings.currentSelectedOption) {
                                            case 0:
                                                GameOptions.GeneralVolume -= 0.01f;
                                                break;
                                            case 1:
                                                GameOptions.MusicVolume -= 0.01f;
                                                break;
                                            case 2:
                                                if (GameOptions.ResolutionMultiplier - 1 >= 1) {
                                                    GameOptions.ResolutionMultiplier -= 1;
                                                    Main.instance.UpdateResolution();
                                                }
                                                break;
                                        }
                                    }

                                    GameOptions.GeneralVolume = Math.Clamp(GameOptions.GeneralVolume, 0, 1.0f);
                                    GameOptions.MusicVolume = Math.Clamp(GameOptions.MusicVolume, 0, 1.0f);
                                };

                                Append(settings);

                                break;
                            case "Quit":
                                GlobalPlayer.SaveProgress();

                                optionMenuBox.focused = false;

                                string[] options = ["No", "Yes"];

                                //Main.angerCounter++;

                                if (Main.instance.spiritsAngered) {
                                    optionMenuBox.panelColor = Color.DarkRed;
                                    if (SoundEngine.IsMusicPlaying) {
                                        SoundEngine.StopMusic(doNotRestart: true);
                                        options = ["Is this really that interesting to you?"];
                                    }
                                    else {
                                        options = ["You didn't want to quit before, did something change hmm?"];
                                    }
                                }

                                var confirm = new UIScrollableMenu(Main.instance.spiritsAngered ? 1 : 2, options);
                                confirm.drawArrows = false;
                                confirm.SetPosition(optionMenuBox.targetSize * 0.5f - confirm.targetSize * 0.5f);
                                confirm.openSpeed = 0.25f;
                                sender.Append(confirm);
                                confirm.onSelectOption = (_)
                                => {
                                    if (confirm.currentSelectedOption == 1) {
                                        optionMenu.closed = true;
                                        optionMenu.onLoseParent += (sender) => {
                                            GameStateManager.SetState(GameState.MainMenu, new BlackFadeInFadeOut(Renderer.SaveFrame(true)));
                                        };
                                    }
                                    else {
                                        confirm.closed = true;
                                    }
                                };
                                confirm.onUpdate = (_) => {
                                    if (Input.PressedKey([Keys.Escape, Keys.Q]))
                                        confirm.closed = true;
                                };

                                confirm.onLoseParent = (_) => { optionMenuBox.focused = true; };
                                break;
                        }


                    }
                };

                if (Main.instance.spiritsAngered)
                    optionMenu.panelColor = Color.DarkRed;

                var navigation = new UIAnimatedPanel(new Vector2(764, 60), UIAnimatedPanel.AnimationStyle.Horizontal);

                navigation.SetPosition(Renderer.UIPreferedWidth * 0.5f - navigation.targetSize.X * 0.5f, Renderer.UIPreferedHeight - 120);
                navigation.openSpeed = 0.25f;
                navigation.Append(new UIBorderedText("ESC./Q -> Close/Cancel, W/S -> Move, E -> Select/Confirm").SetPosition(16));
                navigation.SetFont(Assets.DefaultFont);

                optionMenu.onLoseParent += (sender) => {
                    navigation.isClosed = true;
                    //Main.angerCounter = 0;
                    Main.instance.ActiveWorld.paused = false;
                    sender.parent.Disown(darkening);
                    optionMenu = null;
                    blackBarTop.suspended = true;
                    blackBarBot.suspended = true;
                };

                optionMenu.onUpdate = (sender) => {
                    if (Input.PressedKey([Keys.Escape, Keys.Q]) && optionMenu != null && optionMenu.focused) {
                        navigation.isClosed = true;
                        optionMenu.closed = true;
                    }
                };

                optionMenu.SetPosition(Renderer.UIPreferedWidth * 0.5f - optionMenu.targetSize.X * 0.5f, Renderer.UIPreferedHeight * 0.5f - optionMenu.targetSize.Y * 0.5f);
                Append(darkening);
                Append(navigation);
                Append(optionMenu);
            }
        }
    }
}

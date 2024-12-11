using HellTrail.Core.Combat;
using HellTrail.Core.Combat.Items;
using HellTrail.Core.DialogueSystem;
using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
using HellTrail.Core.UI.Elements;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Overworld
{
    public class OverworldUIState : UIState
    {
        //public UIBorderedText debugText;

        public UIScrollableMenu optionMenu;

        public int lastTarget;
        
        public OverworldUIState() 
        {
            //debugText = new("");
            //Append(debugText);
        }

        public override void Update()
        {
            base.Update();

            if (Input.PressedKey(Keys.Escape) && optionMenu == null && Main.instance.GetGameState() is World)
            {
                Main.instance.ActiveWorld.paused = true;

                UIAnimatedPanel blackBarTop = new UIAnimatedPanel(new Vector2(Renderer.UIPreferedWidth, 160));
                blackBarTop.SetPosition(0, -80);
                blackBarTop.borderColor = Color.Black;
                blackBarTop.panelColor = Color.Black;


                UIAnimatedPanel blackBarBot = new UIAnimatedPanel(new Vector2(Renderer.UIPreferedWidth, 160));
                blackBarBot.SetPosition(0, Renderer.UIPreferedHeight - 80);
                blackBarBot.borderColor = Color.Black;
                blackBarBot.panelColor = Color.Black;

                Append(blackBarTop);
                Append(blackBarBot);

                UIPanel darkening = new();
                darkening.fillColor = Color.Black * 0.5f;
                darkening.outlineColor = Color.White * 0f;
                darkening.size = new Vector2(Renderer.UIPreferedWidth, Renderer.UIPreferedHeight);

                string[] mainOptions = ["Continue", "View Party", "Inventory", "Settings", "Quit"];

                bool angry = Main.instance.spiritsAngered;

                if (angry)
                    mainOptions = ["Continue"];

                optionMenu = new UIScrollableMenu(angry ? 1 : 5, mainOptions)
                {
                    drawArrows = false,
                    openSpeed = 0.15f,
                    onSelectOption = (sender)
                    =>
                    {
                        var optionMenuBox = (UIScrollableMenu)sender;

                        switch (optionMenuBox.CurrentOption)
                        {
                            case "Continue":
                                optionMenuBox.closed = true;
                                optionMenu = null;
                                break;
                            case "Inventory":
                                optionMenuBox.focused = false;

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

                                    if (item.canUseOutOfBattle)
                                    {
                                        switch (item.canTarget)
                                        {
                                            case ValidTargets.World:
                                                item.Use(GlobalPlayer.ActiveParty[0], null, GlobalPlayer.ActiveParty);
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
                                                                item.Use(GlobalPlayer.ActiveParty[0], null, GlobalPlayer.ActiveParty);

                                                                break;
                                                        }
                                                    };

                                                    useOnAll.onLoseParent = (sender) =>
                                                    {
                                                        InventoryMenu.focused = true; 
                                                        lastTarget = (sender as UIScrollableMenu).currentSelectedOption;
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
                                                List<string> allyNames = GlobalPlayer.ActiveParty.Select(x => x.name).ToList();

                                                var allySelect = new UIScrollableMenu(4, [.. allyNames])
                                                {
                                                    openSpeed = 0.25f,
                                                    onLoseParent = (sender) =>
                                                    {
                                                        lastTarget = (sender as UIScrollableMenu).currentSelectedOption;
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
                                                    var target = GlobalPlayer.ActiveParty[allySelect.currentSelectedOption];
                                                    if ((!target.Downed && item.canTarget == ValidTargets.Ally) || (target.Downed && item.canTarget == ValidTargets.DownedAlly))
                                                    {
                                                        item.Use(target, null, [target]);
                                                    } else
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
                                partyMemberMenu.SetPosition(160, 240);
                                partyMemberMenu.openSpeed = 0.25f;

                                partyMemberMenu.onLoseParent += (sender) =>
                                {
                                    optionMenuBox.focused = true;
                                };

                                var memberPanel = new UIAnimatedPanel(new Vector2(600, 400), UIAnimatedPanel.AnimationStyle.FourWay);
                                memberPanel.openSpeed = 0.25f;
                                memberPanel.SetPosition(partyMemberMenu.GetPosition() + new Vector2(200, -120));

                                var portrait = new UICombatPortrait("", 0, 0);
                                portrait.SetPosition(16, 32);
                                memberPanel.Append(portrait);
                                partyMemberMenu.onUpdate = (sender) =>
                                {
                                    if (Input.PressedKey([Keys.Escape, Keys.Q]))
                                    {
                                        memberPanel.isClosed = true;
                                        partyMemberMenu.closed = true;
                                    }
                                };



                                //var memberName = new UIBorderedText("");
                                //memberName.SetPosition(16);
                                //memberPanel.Append(memberName);


                                //var memberStats = new UIBorderedText("");
                                //memberStats.SetPosition(120, 16);
                                //memberPanel.Append(memberStats);

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
                                var xxx = 130;
                                magicStatText.SetPosition(xxx, 64);

                                var magicStatBar = new UIProgressBar(new Vector2(210, magicStatText.font.MeasureString("Y").Y - 8), 0);
                                magicStatBar.SetPosition(240, 70);
                                magicStatBar.fillColor = Color.Yellow;
                                magicStatBar.maxValue = 99;
                                memberPanel.Append(magicStatBar);

                                var magicStatValueText = new UIBorderedText("");
                                magicStatValueText.SetPosition(240 + magicStatBar.size.X, 64);
                                memberPanel.Append(magicStatValueText);

                                var strStatText = new UIBorderedText("Strength");
                                memberPanel.Append(strStatText);
                                strStatText.SetPosition(xxx, 104);

                                var strStatBar = new UIProgressBar(new Vector2(210, magicStatText.font.MeasureString("Y").Y - 8), 0);
                                strStatBar.SetPosition(240, 110);
                                strStatBar.fillColor = Color.Yellow;
                                strStatBar.maxValue = 99;
                                memberPanel.Append(strStatBar);

                                var strStatValueText = new UIBorderedText("");
                                strStatValueText.SetPosition(240 + strStatBar.size.X, 104);
                                memberPanel.Append(strStatValueText);

                                var spdStatText = new UIBorderedText("Speed");
                                memberPanel.Append(spdStatText);
                                spdStatText.SetPosition(xxx, 144);

                                var spdStatBar = new UIProgressBar(new Vector2(210, magicStatText.font.MeasureString("Y").Y - 8), 0);
                                spdStatBar.SetPosition(240, 150);
                                spdStatBar.fillColor = Color.Yellow;
                                spdStatBar.maxValue = 99;
                                memberPanel.Append(spdStatBar);

                                var spdStatValueText = new UIBorderedText("");
                                spdStatValueText.SetPosition(240 + spdStatBar.size.X, 144);
                                memberPanel.Append(spdStatValueText);

                                var affinities = new UIBorderedText("Affinities");
                                memberPanel.Append(affinities);
                                affinities.SetPosition(new Vector2(xxx, 184));

                                UIPicture[] affinityTypes = new UIPicture[7];
                                UIPicture[] elementalIcons = new UIPicture[7];
                                for (int i = 0; i < 7; i++)
                                {
                                    var affinityElementPic = new UIPicture("AffinityTypeFramed", [new(0, 0, 32, 32)]);
                                    affinityElementPic.SetPosition(xxx + i * 64 + 4 * i - 64, 224 + 64);
                                    affinityElementPic.scale = new Vector2(2);
                                    memberPanel.Append(affinityElementPic);
                                    affinityTypes[i] = affinityElementPic;
                                }

                                for (int i = 0; i < 7; i++)
                                {
                                    var loadedIcons = "ElementsIconsFramed";
                                    var affinityElementPic = new UIPicture(loadedIcons, [new(0, 32 * i, 32, 32)]);
                                    affinityElementPic.SetPosition(xxx + i * 64 + 4 * i - 64, 224);
                                    affinityElementPic.scale = new Vector2(2);
                                    memberPanel.Append(affinityElementPic);
                                    elementalIcons[i] = affinityElementPic;
                                }


                                partyMemberMenu.onChangeOption = (sender) =>
                                {
                                    var selectedMember = GlobalPlayer.ActiveParty[partyMemberMenu.currentSelectedOption];
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
                                    magicStatValueText.text = $"{selectedMember.Stats.magic}";
                                    magicStatValueText.SetPosition(240 + magicStatBar.size.X - magicStatValueText.font.MeasureString(magicStatValueText.text).X, 64);

                                    strStatBar.value = selectedMember.Stats.strength;
                                    strStatValueText.text = $"{selectedMember.Stats.strength}";
                                    strStatValueText.SetPosition(240 + strStatBar.size.X - strStatValueText.font.MeasureString(strStatValueText.text).X, 104);

                                    spdStatBar.value = selectedMember.Stats.speed;
                                    spdStatValueText.text = $"{selectedMember.Stats.speed}";
                                    spdStatValueText.SetPosition(240 + strStatBar.size.X - strStatValueText.font.MeasureString(strStatValueText.text).X, 144);


                                    for(int i = 0; i < 7; i++)
                                    {

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

                                    if (selectedMember.Downed)
                                    {
                                        portrait.picture.tint = Color.DarkGray;
                                    }
                                };

                                partyMemberMenu.onChangeOption?.Invoke(partyMemberMenu);

                                sender.parent.Append(partyMemberMenu);
                                sender.parent.Append(memberPanel);
                                break;
                            case "Quit":
                                optionMenuBox.focused = false;

                                string[] options = ["No", "Yes"];

                                //Main.angerCounter++;

                                if (Main.instance.spiritsAngered)
                                {
                                    optionMenuBox.panelColor = Color.DarkRed;
                                    if (SoundEngine.IsMusicPlaying)
                                    {
                                        SoundEngine.StopMusic(doNotRestart: true);
                                        options = ["Is this really that interesting to you?"];
                                    } else
                                    {
                                        options = ["You didn't want to quit before, did something change hmm?"];
                                    }
                                }

                                var confirm = new UIScrollableMenu(Main.instance.spiritsAngered ? 1 : 2, options);
                                confirm.drawArrows = false;
                                confirm.SetPosition(optionMenuBox.targetSize * 0.5f - confirm.targetSize * 0.5f);
                                confirm.openSpeed = 0.25f;
                                sender.Append(confirm);
                                confirm.onSelectOption = (_)
                                =>
                                {
                                    if (confirm.currentSelectedOption == 1)
                                        Main.instance.Exit();
                                    else
                                    {
                                        confirm.closed = true;
                                    }
                                };
                                confirm.onUpdate = (_) =>
                                {
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

                optionMenu.onLoseParent += (sender) =>
                {
                    navigation.isClosed = true;
                    //Main.angerCounter = 0;
                    Main.instance.ActiveWorld.paused = false;
                    sender.parent.Disown(darkening);
                    optionMenu = null;
                    blackBarTop.isClosed = true;
                    blackBarBot.isClosed = true;
                };

                optionMenu.onUpdate = (sender) =>
                {
                    if (Input.PressedKey([Keys.Escape, Keys.Q]) && optionMenu != null && optionMenu.focused)
                    {
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

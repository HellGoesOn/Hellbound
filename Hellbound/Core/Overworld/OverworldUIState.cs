using HellTrail.Core.Combat;
using HellTrail.Core.Combat.Items;
using HellTrail.Core.DialogueSystem;
using HellTrail.Core.ECS;
using HellTrail.Core.UI;
using HellTrail.Core.UI.Elements;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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

                UIPanel darkening = new UIPanel();
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
                                UIBorderedText description = new UIBorderedText("", 40);

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
                                        switch (item.validTargets)
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
                                                        switch(useOnAll.CurrentOption)
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
                                                    if ((!target.Downed && item.validTargets == ValidTargets.Ally) ||(target.Downed && item.validTargets == ValidTargets.DownedAlly))
                                                        item.Use(target, null, [target]);
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

                                var portrait = new UIPicture("", []);
                                portrait.SetPosition(16, 48);
                                memberPanel.Append(portrait);
                                partyMemberMenu.onUpdate = (sender) =>
                                {
                                    if (Input.PressedKey([Keys.Escape, Keys.Q]))
                                    {
                                        memberPanel.isClosed = true;
                                        partyMemberMenu.closed = true;
                                    }
                                };

                                var restInPeace = new UIPicture("DeadFrame", new FrameData(0, 0, 32, 32));
                                restInPeace.SetPosition(16, 48);
                                restInPeace.scale = new Vector2(3);
                                memberPanel.Append(restInPeace);



                                var memberName = new UIBorderedText("");
                                memberName.SetPosition(16);
                                memberPanel.Append(memberName);


                                var memberStats = new UIBorderedText("");
                                memberStats.SetPosition(16, 148);
                                memberPanel.Append(memberStats);

                                partyMemberMenu.onChangeOption = (sender) =>
                                {
                                    var selectedMember = GlobalPlayer.ActiveParty[partyMemberMenu.currentSelectedOption];
                                    portrait.textureName = selectedMember.portrait;
                                    portrait.frames = [new FrameData(0, 0, 32, 32)];
                                    portrait.scale = new Vector2(3);
                                    memberName.text = selectedMember.name;
                                    memberStats.text = selectedMember.stats.ListStats();
                                    portrait.tint = Color.White;
                                    restInPeace.tint = Color.White * 0;

                                    if(selectedMember.Downed)
                                    {
                                        portrait.tint = Color.DarkGray;
                                        restInPeace.tint = Color.White;
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

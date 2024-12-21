using Casull.Core;
using Casull.Core.Combat;
using Casull.Core.ECS;
using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Casull.Core.UI;
using Casull.Core.UI.Elements;
using Casull.Render;
using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Casull
{
    public class MainMenu : IGameState
    {
        bool init;
        float positionY = 320;

        float progress = -8f;

        internal bool started;
        internal bool newGame;
        bool startFlag;

        int transitionDelay;

        Context context;
        Systems systems;

        Entity bladeRunner;
        Entity dog;
        List<Entity> dooDads = [];

        Color groundColor = new Color(58, 32, 24);

        public MainMenu()
        {
            context = new Context();
            systems = new Systems();
            GetCamera().centre = new Vector2(160, 90);
            GetCamera().Zoom = 4f;

            bladeRunner = context.CopyFrom(EntitySaver.Load("\\Content\\Prefabs\\", "Dumbass"));
            systems.AddSystem(new NewAnimationSystem(context));
            systems.AddSystem(new DrawSystem(context));
            systems.AddSystem(new MoveSystem(context));
            systems.AddSystem(new DDDSystem(context));


            // doodads/

            for (int i = 0; i < 25; i++) {
                var sunflowerDoodad = context.CopyFrom(EntitySaver.Load("\\Content\\Prefabs\\", "BobbingSunflower"));
                sunflowerDoodad.AddComponent(new TextureComponent("Sunflower_Idle", new Vector2(24, 48)) {
                    solidColor = true,
                });
                sunflowerDoodad.AddComponent(new Velocity(-1, 0));
                sunflowerDoodad.AddComponent(new Transform(-60, 0));

                dooDads.Add(sunflowerDoodad);
            }

            var haus = context.Create();
            haus.AddComponent(new Transform(-128, 0));
            haus.AddComponent(new Velocity(-1, 0));
            haus.AddComponent(new TextureComponent("Haus", new Vector2(64, 128)) {
                solidColor = true
            });

            dooDads.Add(haus);

            // characters
            bladeRunner.AddComponent(new Transform(160, 120) {
                layer = 20
            });
            var anim = bladeRunner.GetComponent<NewAnimationComponent>();
            anim.currentAnimation = "Run";
            bladeRunner.GetComponent<TextureComponent>().color = Color.DarkSeaGreen;
            bladeRunner.GetComponent<TextureComponent>().solidColor = true;

            dog = context.Create();
            dog.AddComponent(new TextureComponent("WhatDaDogDoin2", new Vector2(16, 16), Vector2.One));

            dog.AddComponent(new Transform(130, 0) {
                layer = 20
            });

            dog.GetComponent<TextureComponent>().color = Color.DarkSeaGreen;
            dog.GetComponent<TextureComponent>().solidColor = true;
            dog.AddComponent(new NewAnimationComponent("Dog", "Run", new Dictionary<string, Animation>()
            { {"Run", new(0, 3) } }, [new (0, 0, 32, 32, 5), new(0, 32, 32, 32, 5), new(0, 64, 32, 32, 5), new(0, 96, 32, 32, 5)]));

            MainMenuUI mainMenuUI = new(this) {
                id = "MainMenu"
            };

            UIManager.UIStates.Add(mainMenuUI);

            UIManager.overworldUI.showNameTimer = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Renderer.Draw(Assets.GetTexture("Pixel"), Vector2.Zero, new Rectangle(0, 0, 320, 180), Color.Lerp(Color.Black, Color.Orange, progress), 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            systems.Draw(context, spriteBatch);
            Renderer.Draw(Assets.GetTexture("Pixel"), new Vector2(0, positionY), new Rectangle(0, 0, 320, 1024), Color.Lerp(Color.DarkSlateGray, groundColor, progress), 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 200f);

        }

        public Camera GetCamera()
        {
            return CameraManager.mainMenuCamera;
        }

        public void Update()
        {
            if (progress < 1f)
                progress += 0.025f;

            if (!init) {
                init = true;
                SoundEngine.StartMusic("KickBack", true);
            }

            if (startFlag) {
                ++transitionDelay;
                if (positionY > 0 && transitionDelay >= 180)
                    positionY -= 5f;
#if DEBUG
                if(transitionDelay >= 1) {
#else
                    if (transitionDelay >= 240) {
#endif
                    var state = UIManager.GetStateByName("MainMenu");
                    (state as MainMenuUI).mainMenu = null;
                    UIManager.UIStates.Remove(state);
                    SoundEngine.StartMusic("ChangingSeasons", true);
                    GameStateManager.SetState(GameState.Overworld, new BlackFadeInFadeOut(Renderer.SaveFrame()));


                    UIManager.combatUI.tutorialProgress = 0;
                    Main.lastTransitionPosition = Vector2.Zero;
                    Main.currentZone = "Forest3";
                    World.flags.Clear();

                    GlobalPlayer.Init();
                    GlobalPlayer.LoadProgress(Main.saveSlot);

                    World.InitTriggers();
                    if (newGame) {
                        World.triggers.Clear();
                        World.cutscenes.Clear();
                        World.InitNightmare();
                        Main.instance.ActiveWorld = World.LoadFromFile("\\Content\\Scenes\\", "Nightmare");
                    }
                    else
                        Main.instance.ActiveWorld = World.LoadFromFile("\\Content\\Scenes\\", Main.currentZone);

                    if (Main.lastTransitionPosition == Vector2.Zero && !newGame) {
                        var plr = Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("Player"));
                        if(plr != null) {
                            Main.lastTransitionPosition = plr.GetComponent<Transform>().position;
                        }
                    }
                    else {
                        var plr = Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x!= null && x.GetComponent<Tags>().Has("Player"));

                        if (plr != null)
                            plr.GetComponent<Transform>().position = Main.lastTransitionPosition;
                    }

                }
            }

            if (started) {
                if (!startFlag) {
                    startFlag = true;
                    dog.AddComponent(new Velocity(new Vector2(2f, 0)));
                    bladeRunner.AddComponent(new Velocity(new Vector2(2f, 0)));
                }
            }

            if (positionY > 120) {
                positionY -= 0.5f;
            }

            foreach(var doodad in dooDads) {
                var tc = doodad.GetComponent<Transform>();
                var tex = doodad.GetComponent<TextureComponent>();
                var velocity = doodad.GetComponent<Velocity>();

                if(positionY > 120) {
                    tc.position.Y = positionY;
                }
                
                if (tc.position.X < -360 && Main.rand.Next(50) == 0) {
                    tc.position.X = 400;
                    tc.position.Y = positionY + 1 + (float)Math.Sin(Main.totalTime);
                    velocity.X = -Main.rand.Next(5, 30) * 0.1f;

                    tex.scale = Main.rand.Next(2) == 0 ? new Vector2(-1, 1) : Vector2.One;
                }


                if (started)
                    velocity.X = 0;

                tex.color = Color.Lerp(Color.DarkSlateGray, groundColor, progress);
            }

            var runForestRun = bladeRunner.GetComponent<Transform>();
            runForestRun.position.Y = positionY - 12;
            runForestRun.position.X += startFlag ? 0.0f : (float)Math.Sin(Main.totalTime) * 0.25f;

            var dogRun = dog.GetComponent<Transform>();
            dogRun.position.Y = positionY - 8;
            dogRun.position.X += (startFlag ? 0 : (float)Math.Sin(Main.totalTime - 20) * 0.35f);

            systems.Execute(context);
            dog.GetComponent<TextureComponent>().color =
            bladeRunner.GetComponent<TextureComponent>().color = Color.Lerp(Color.DarkGray, Color.SaddleBrown, progress);
        }
    }

    public class MainMenuUI : UIState
    {
        int _heldTimer;
        float _repeatRate;
        public MainMenu mainMenu;

        public UIScrollableMenu menu;

        UIPicture[] logoLetters;

        public MainMenuUI(MainMenu mainMenu)
        {
            logoLetters = new UIPicture[6];
            int[] frames = { 0, 32, 64, 96, 128, 128 };
            for (int i = 0; i < 6; i++) {
                logoLetters[i] = new UIPicture("Logo", new FrameData(0, frames[i], 32, 32));
                logoLetters[i].SetPosition(Renderer.ScreenMiddle.X - 48 * 6 + 96 * i, 120);
                logoLetters[i].scale = new Vector2(3);
                logoLetters[i].origin = new Vector2(16);
                logoLetters[i].tint = Color.Transparent;

                Append(logoLetters[i]);
            }

            
            this.mainMenu = mainMenu;

            menu = new UIScrollableMenu(4, ["Continue", "New Game", "Settings", "Quit"]);
            menu.SetPosition(new Vector2(0, Renderer.PreferedHeight * 0.5f - menu.targetSize.Y * 0.5f));
            menu.openSpeed = 1f;
            menu.panelColor = Color.Transparent;
            menu.borderColor = Color.Transparent;
            menu.opacity = -2.0f;
            menu.drawArrows = false; 
            
            string[] slots = ["Slot 1", "Slot 2", "Slot 3"];

            for (int i = 0; i < 3; i++) {

                if (!File.Exists(Environment.CurrentDirectory + $"\\SaveData\\Save{i}.sdt"))
                    slots[i] += " - Empty";
                else {
                    string txt = File.ReadAllText(Environment.CurrentDirectory + $"\\SaveData\\Save{i}.sdt");

                    var entries = txt.Split("}" + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    string[] characters = entries[2].Substring(8 + Environment.NewLine.Length).Split(Environment.NewLine);
                    var matches = Regex.Matches(characters[0], @"\[(.+?)\]");

                    slots[i] += " - " + matches[1].Groups[1].Value.Replace("\"", "");
                }
            }


            var demo = new UIBorderedText("-Demo Version-");
            demo.SetPosition(Renderer.ScreenMiddle.X - demo.font.MeasureString(demo.text).X * 0.5f - 32, Renderer.ScreenMiddle.Y - 180);
            demo.color = Color.Transparent;
            demo.borderColor = Color.Transparent;
            demo.onUpdate = (sender) => {
                demo.color = Color.White * menu.opacity;
                demo.borderColor = Color.Black * menu.opacity;
            };
            Append(demo);
            var help = new UIBorderedText("W/A/S/D - Navigate. E/Enter - Select. Q/ESC. - Back/Cancel");
            help.SetPosition(Renderer.ScreenMiddle.X - help.font.MeasureString(help.text).X * 0.5f, Renderer.ScreenMiddle.Y + 300);
            help.color = Color.Transparent;
            help.borderColor = Color.Transparent;
            Append(help);
            help.onUpdate = (sender) => {
                help.color = Color.White * menu.opacity;
                help.borderColor = Color.Black * menu.opacity;
            };
            menu.onSelectOption = (sender) => {
                switch (menu.CurrentOption) {
                    case "Quit":
                        Main.instance.Exit();
                        break;
                    case "New Game":
                        menu.focused = false;
                        var selectSlotMenu = new UIScrollableMenu(3, slots);
                        selectSlotMenu.SetPosition(Renderer.ScreenMiddle - selectSlotMenu.targetSize * 0.5f);
                        selectSlotMenu.openSpeed = 0.25f;

                        selectSlotMenu.onLoseParent = (sender) => {
                            menu.focused = true;
                        };

                        selectSlotMenu.onUpdate = (sender) => {
                            if (Input.PressedKey([Keys.Escape, Keys.Q]) && selectSlotMenu.focused)
                                selectSlotMenu.closed = true;
                        };
                        selectSlotMenu.onSelectOption = (sender) => {
                            selectSlotMenu.focused = false;
                            selectSlotMenu.suspended = true;
                            var animPanel = new UIAnimatedPanel(new Vector2(400, 200), UIAnimatedPanel.AnimationStyle.FourWay);
                            animPanel.borderColor = Color.Transparent;
                            animPanel.panelColor = Color.Black * 0.35f;
                            animPanel.SetPosition(Renderer.ScreenMiddle - animPanel.targetSize * 0.5f);
                            animPanel.openSpeed = 0.35f;


                            var nameTextBox = new UITextBox();
                            nameTextBox.maxCharacters = 20;
                            nameTextBox.onTextSubmit = (sender) => {
                                if (!string.IsNullOrWhiteSpace(nameTextBox.myText)) {
                                    mainMenu.started = true;
                                    mainMenu.newGame = true;
                                    if (File.Exists(Environment.CurrentDirectory + $"\\SaveData\\Save{selectSlotMenu.currentSelectedOption}.sdt"))
                                        File.Delete(Environment.CurrentDirectory + $"\\SaveData\\Save{selectSlotMenu.currentSelectedOption}.sdt");
                                    Main.saveSlot = selectSlotMenu.currentSelectedOption;
                                    Main.newSlotName = (sender as UITextBox).myText;
                                    animPanel.isClosed = true;
                                    selectSlotMenu.closed = true;
                                }
                                else
                                    nameTextBox.Click();
                            };
                            nameTextBox.boxBorderColor = Color.Transparent;
                            nameTextBox.boxInnerColor = Color.Transparent;
                            nameTextBox.size.X = 200;
                            nameTextBox.myText = "";
                            nameTextBox.SetPosition(animPanel.targetSize * 0.5f - nameTextBox.size *0.5f);
                            animPanel.Append(nameTextBox);

                            animPanel.onUpdate = (sender) => {
                                if(!nameTextBox.IsEditing)
                                    nameTextBox.Click();
                                if (Input.PressedKey([Keys.Escape])) {
                                    selectSlotMenu.focused = true;
                                    selectSlotMenu.suspended = false;
                                    animPanel.isClosed = true;
                                }
                            };

                            var writeName = new UIBorderedText("What's your name?");
                            writeName.SetPosition(animPanel.targetSize.X * 0.5f - writeName.size.X * 0.5f, 16);

                            var controls = new UIBorderedText("[Enter] Submit | [ESC.] Cancel");
                            controls.SetPosition(animPanel.targetSize.X * 0.5f - controls.size.X * 0.5f, animPanel.targetSize.Y - controls.size.Y - 6);

                            animPanel.Append(writeName);
                            animPanel.Append(controls);


                            Append(animPanel);
                            //Main.saveSlot = selectSlotMenu.currentSelectedOption;
                            //mainMenu.started = true;

                            //if (File.Exists(Environment.CurrentDirectory + $"\\SaveData\\Save{selectSlotMenu.currentSelectedOption}.sdt"))
                            //    File.Delete(Environment.CurrentDirectory + $"\\SaveData\\Save{selectSlotMenu.currentSelectedOption}.sdt");
                        };


                        Append(selectSlotMenu);
                        break;
                    case "Continue":
                        //mainMenu.started = true;
                        menu.focused = false; 
                        
                        selectSlotMenu = new UIScrollableMenu(3, slots);
                        selectSlotMenu.SetPosition(Renderer.ScreenMiddle - selectSlotMenu.targetSize * 0.5f);
                        selectSlotMenu.openSpeed = 0.25f;

                        selectSlotMenu.onSelectOption = (sender) => {
                            if(!File.Exists(Environment.CurrentDirectory + $"\\SaveData\\Save{selectSlotMenu.currentSelectedOption}.sdt")) {
                                selectSlotMenu.focused = false;
                                selectSlotMenu.suspended = true;
                                var animPanel = new UIAnimatedPanel(new Vector2(400, 200), UIAnimatedPanel.AnimationStyle.FourWay);
                                animPanel.borderColor = Color.Transparent;
                                animPanel.panelColor = Color.Black * 0.35f;
                                animPanel.SetPosition(Renderer.ScreenMiddle - animPanel.targetSize * 0.5f);
                                animPanel.openSpeed = 0.35f;


                                var nameTextBox = new UITextBox();
                                nameTextBox.maxCharacters = 20;
                                nameTextBox.onTextSubmit = (sender) => {
                                    if (!string.IsNullOrWhiteSpace(nameTextBox.myText)) {
                                        mainMenu.started = true;
                                        mainMenu.newGame = true;
                                        Main.saveSlot = selectSlotMenu.currentSelectedOption;
                                        Main.newSlotName = (sender as UITextBox).myText;
                                        animPanel.isClosed = true;
                                        selectSlotMenu.closed = true;
                                    }
                                    else
                                        nameTextBox.Click();
                                };
                                nameTextBox.boxBorderColor = Color.Transparent;
                                nameTextBox.boxInnerColor = Color.Transparent;
                                nameTextBox.size.X = 200;
                                nameTextBox.myText = "";
                                nameTextBox.SetPosition(animPanel.targetSize * 0.5f - nameTextBox.size * 0.5f);
                                animPanel.Append(nameTextBox);

                                animPanel.onUpdate = (sender) => {
                                    if (!nameTextBox.IsEditing)
                                        nameTextBox.Click();
                                    if (Input.PressedKey([Keys.Escape])) {
                                        selectSlotMenu.focused = true;
                                        selectSlotMenu.suspended = false;
                                        animPanel.isClosed = true;
                                    }
                                };

                                var writeName = new UIBorderedText("What's your name?");
                                writeName.SetPosition(animPanel.targetSize.X * 0.5f - writeName.size.X * 0.5f, 16);

                                var controls = new UIBorderedText("[Enter] Submit | [ESC.] Cancel");
                                controls.SetPosition(animPanel.targetSize.X * 0.5f - controls.size.X * 0.5f, animPanel.targetSize.Y - controls.size.Y - 6);

                                animPanel.Append(writeName);
                                animPanel.Append(controls);


                                Append(animPanel);
                            }
                            else {
                                Main.saveSlot = selectSlotMenu.currentSelectedOption;
                                mainMenu.started = true;
                                selectSlotMenu.closed = true;
                            }
                        };

                        selectSlotMenu.onUpdate = (sender) => {
                            if (Input.PressedKey([Keys.Escape, Keys.Q]) && selectSlotMenu.focused)
                                selectSlotMenu.closed = true;
                        };
                        selectSlotMenu.onLoseParent = (sender) => {
                            menu.focused = true;
                        };

                        Append(selectSlotMenu);

                        break;
                    case "Settings":
                        menu.focused = false;

                        var settings = new UIScrollableMenu(4, ["Very biiiiiiiiiiiiiiiiiiiig text", "Music Volume", "Resolution", "Back"]);
                        settings.openSpeed = 1f;
                        settings.panelColor = Color.Transparent;
                        settings.borderColor = Color.Transparent;
                        settings.drawArrows = false;
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

                        settings.onLoseParent = (sender) => { menu.focused = true; };

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
                }
            };

            Append(menu);
        }

        Color[] colors = {
            Color.Red,
            Color.Green,
            Color.Blue,
        };

        int nextIndex;

        public override void Update()
        {
            base.Update();
            for (int i = 0; i < 6; i++) {
                logoLetters[i].tint = Color.Lerp(logoLetters[i].tint, colors[nextIndex], 0.07f) * menu.opacity;
                logoLetters[i].Rotation = -(float)Math.Sin(Main.totalTime) * 0.05f;
                logoLetters[i].SetPosition(logoLetters[i].GetPosition() + new Vector2(0, (float)Math.Cos(Main.totalTime - i)));
            }

            if (Main.rand.Next(25) == 0)
                nextIndex = Main.rand.Next(3);

            if (menu.opacity < 1.0f && !mainMenu.started) {

                menu.opacity += 0.005f;
            }
            else if (mainMenu.started) {
                menu.opacity -= 0.05f;
            }
        }
    }
}

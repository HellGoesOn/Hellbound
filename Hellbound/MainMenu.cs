using HellTrail.Core;
using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
using HellTrail.Core.UI.Elements;
using HellTrail.Extensions;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail
{
    public class MainMenu : IGameState
    {
        bool init;
        float positionY = 320;

        internal bool started;
        bool startFlag;

        int transitionDelay;

        Context context;
        Systems systems;

        Entity bladeRunner;
        Entity dog;
        public MainMenu()
        {
            context = new Context();
            systems = new Systems();
            GetCamera().centre = new Vector2(160, 90);
            GetCamera().zoom = 4f;

            bladeRunner = context.CopyFrom(EntitySaver.Load("\\Content\\Prefabs\\", "Dumbass"));
            systems.AddSystem(new NewAnimationSystem(context));
            systems.AddSystem(new DrawSystem(context));
            systems.AddSystem(new MoveSystem(context));

            bladeRunner.AddComponent(new Transform(160, 120));
            var anim = bladeRunner.GetComponent<NewAnimationComponent>();
            anim.currentAnimation = "Run";
            bladeRunner.GetComponent<TextureComponent>().color = Color.DarkSeaGreen;
            bladeRunner.GetComponent<TextureComponent>().solidColor = true;

            dog = context.Create();
            dog.AddComponent(new TextureComponent("WhatDaDogDoin2", new Vector2(16, 16), Vector2.One));

            dog.AddComponent(new Transform(130, 0));

            dog.GetComponent<TextureComponent>().color = Color.DarkSeaGreen;
            dog.GetComponent<TextureComponent>().solidColor = true; ;

            MainMenuUI mainMenuUI = new(this)
            {
                id = "MainMenu"
            };

            UIManager.UIStates.Add(mainMenuUI);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Renderer.Draw(Assets.GetTexture("Pixel"), Vector2.Zero, new Rectangle(0, 0, 320, 180), Color.Orange, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            Renderer.Draw(Assets.GetTexture("Pixel"), new Vector2(0, positionY), new Rectangle(0, 0, 320, 1024), new Color(58, 32, 24), 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

            systems.Draw(context, spriteBatch);
        }

        public Camera GetCamera()
        {
            return CameraManager.mainMenuCamera;
        }

        public void Update()
        {
            if(!init)
            {
                init = true;
                SoundEngine.StartMusic("KickBack", true);
            }

            if(startFlag)
            {
                ++transitionDelay;
                if (positionY > 0 && transitionDelay >= 180)
                positionY -= 5f;
                if (transitionDelay >= 240)
                {
                    var state = UIManager.GetStateByName("MainMenu");
                    (state as MainMenuUI).mainMenu = null;
                    UIManager.UIStates.Remove(state);
                    SoundEngine.StartMusic("ChangingSeasons", true);
                    GameStateManager.SetState(GameState.Overworld, new BlackFadeInFadeOut(Renderer.SaveFrame()));
                }
            }

            if(started)
            {
                if (!startFlag)
                {
                    startFlag = true;
                    dog.AddComponent(new Velocity(new Vector2(2f, 0)));
                    bladeRunner.AddComponent(new Velocity(new Vector2(2f, 0)));
                }
            }

            if (positionY > 120)
            {
                positionY -= 0.5f;
            }
            var runForestRun = bladeRunner.GetComponent<Transform>();
            runForestRun.position.Y = positionY -12;
            runForestRun.position.X += (float)Math.Sin(Main.totalTime) * (startFlag ? 1.0f : 0.25f);

            var dogRun = dog.GetComponent<Transform>();
            dogRun.position.Y = positionY - 10 - (float)Math.Sin(Main.totalTime);
            dogRun.position.X += (float)Math.Sin(Main.totalTime) * (startFlag ? 2.0f : 1f);

            systems.Execute(context);
            dog.GetComponent<TextureComponent>().color =
            bladeRunner.GetComponent<TextureComponent>().color = Color.SaddleBrown;
        }
    }

    public class MainMenuUI : UIState
    {
        int _heldTimer;
        float _repeatRate;
        public MainMenu mainMenu;

        public UIScrollableMenu menu;

        public MainMenuUI(MainMenu mainMenu)
        {

            this.mainMenu = mainMenu;

            menu = new UIScrollableMenu(3, ["Start", "Settings", "Quit"]);
            menu.SetPosition(new Vector2(0, Renderer.PreferedHeight * 0.5f - menu.targetSize.Y * 0.5f));
            menu.openSpeed = 1f;
            menu.panelColor = Color.Transparent;
            menu.borderColor = Color.Transparent;
            menu.opacity = -2.0f;
            menu.drawArrows = false;

            var help = new UIBorderedText("W/A/S/D - Navigate. E/Enter - Select. Q/ESC. - Back/Cancel");
            help.SetPosition(Renderer.ScreenMiddle.X - help.font.MeasureString(help.text).X * 0.5f, Renderer.ScreenMiddle.Y + 300);
            help.color = Color.Transparent;
            help.borderColor = Color.Transparent;
            Append(help);
            help.onUpdate = (sender) =>
            {
                help.color = Color.White * menu.opacity;
                help.borderColor = Color.Black * menu.opacity;
            };
            menu.onSelectOption = (sender) =>
            {
                switch(menu.CurrentOption)
                {
                    case "Quit":
                        Main.instance.Exit();
                        break;
                    case "Start":
                        mainMenu.started = true;
                        break;
                    case "Settings":
                        menu.focused = false;

                        var settings = new UIScrollableMenu(4, ["Very biiiiiiiiiiiiiiiiiiiig text", "Music Volume", "Resolution", "Back"]);
                        settings.openSpeed = 1f;
                        settings.panelColor = Color.Transparent;
                        settings.borderColor = Color.Transparent;
                        settings.drawArrows = false;
                        settings.onChangeOption = (sender) =>
                        {
                            settings.options[0] = $"Game Volume: {Math.Round(GameOptions.GeneralVolume * 100)}%";
                            settings.options[1] = $"Music Volume: {Math.Round(GameOptions.MusicVolume * 100)}%";
                            settings.options[2] = $"Resolution: {GameOptions.ScreenWidth}x{GameOptions.ScreenHeight}";
                        };

                        settings.SetPosition(Renderer.UIPreferedWidth * 0.5f - settings.targetSize.X * 0.5f, Renderer.UIPreferedHeight * 0.5f - settings.targetSize.Y * 0.5f);

                        settings.onSelectOption = (sender) =>
                        {
                            switch (settings.CurrentOption)
                            {
                                case "Back":
                                    settings.closed = true; break;
                            }
                        };

                        settings.onLoseParent = (sender) => { menu.focused = true; };

                        settings.onUpdate = (sender) =>
                        {
                            if (Input.HeldKey(Keys.A) || Input.HeldKey(Keys.D))
                            {
                                _heldTimer++;
                                if (_heldTimer >= _repeatRate && _repeatRate > 5)
                                    _repeatRate--;
                            } else
                            {
                                _repeatRate = 15;
                                _heldTimer = 0;
                            }

                            settings.options[0] = $"Game Volume: {Math.Round(GameOptions.GeneralVolume * 100)}%";
                            settings.options[1] = $"Music Volume: {Math.Round(GameOptions.MusicVolume * 100)}%";
                            settings.options[2] = $"Resolution: {GameOptions.ScreenWidth}x{GameOptions.ScreenHeight}";
                            if (Input.PressedKey([Keys.Escape, Keys.Q]))
                                settings.closed = true;

                            if (Input.PressedKey(Keys.D) || (Input.HeldKey(Keys.D) && _heldTimer >= _repeatRate))
                            {
                                _heldTimer = 0;
                                switch (settings.currentSelectedOption)
                                {
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

                            if (Input.PressedKey(Keys.A) || (Input.HeldKey(Keys.A) && _heldTimer >= _repeatRate))
                            {
                                _heldTimer = 0;
                                switch (settings.currentSelectedOption)
                                {
                                    case 0:
                                        GameOptions.GeneralVolume -= 0.01f;
                                        break;
                                    case 1:
                                        GameOptions.MusicVolume -= 0.01f;
                                        break;
                                    case 2:
                                        if (GameOptions.ResolutionMultiplier - 1 >= 1)
                                        {
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

        public override void Update()
        {
            base.Update();

            if(menu.opacity < 1.0f && !mainMenu.started)
            {
                menu.opacity += 0.005f;
            }   
            else
            {
                menu.opacity -= 0.05f;
            }
        }
    }
}

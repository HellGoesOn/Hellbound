using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellTrail.Render;
using Microsoft.Xna.Framework.Input;
using HellTrail.Core.Combat;
using HellTrail.Core;
using HellTrail.Core.UI;
using HellTrail.Core.Combat.Abilities;
using Treeline.Core.Graphics;
using HellTrail.Core.Combat.Abilities.Fire;
using HellTrail.Core.Combat.AI;
using HellTrail.Core.Overworld;
using HellTrail.Core.DialogueSystem;
using HellTrail.Core.Combat.Scripting;
using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;

namespace HellTrail
{
    public class Main : Game
    {
        internal SpriteBatch spriteBatch;
        internal GraphicsDeviceManager gdm;
        internal static Main instance;
        internal static double totalTime;
        internal static Random rand;

        public Battle battle;
        public World activeWorld;

        public Context prefabContext;

        internal List<Transition> transitions = [];

        public Main()
        {
            prefabContext = new Context();
            instance = this;
            gdm = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = true;
            this.Window.Title = "Hell Trail";
        }

        protected override void Initialize()
        {
            /* This is a nice place to start up the engine, after
             * loading configuration stuff in the constructor
             */
            rand = new Random();
            base.Initialize();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameOptions.ResolutionMultiplier = 4;
            gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
            gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
            gdm.ApplyChanges();
            GameStateManager.State = GameState.Overworld;
            SoundEngine.StartMusic("ChangingSeasons", true);

            //GetGameState().GetCamera().centre = GlobalPlayer.ActiveParty[0].position;

            var e = prefabContext.Create();
            e.AddComponent(new TextureComponent("Slime3")
            {
                origin = new Vector2(16),
                scale = new Vector2(1)
            });
            e.AddComponent(new Transform(0, 0));
            e.AddComponent(new Obesity(-0.02f));
            e.AddComponent(new Velocity(0, 0));
            e.AddComponent(new CreateBattleOnContact(["Slime", "Slime"], null));
            e.AddComponent(new CollisionBox(32, 32));
        }

        protected override void LoadContent()
        {
            // Load textures, sounds, and so on in here...
            base.LoadContent();
            Assets.Load(this);
            Renderer.Load(GraphicsDevice);
            CameraManager.Initialize();
            UIManager.Init();
            UnitDefinitions.DefineUnits();
            GlobalPlayer.Init();
            ParticleManager.Initialize();
            Context.InitializeAll();

            activeWorld = new World();

            Dialogue dialogue = Dialogue.Create();
            UIManager.dialogueUI.dialoguePanel.Position = new Vector2(32, Renderer.UIPreferedHeight * 0.5f);
            DialoguePage[] pages =
                {
                new()
            {
                fillColor = Color.Transparent,
                borderColor = Color.Transparent,
                text = "In this world, the events to unfold, people to meet and places to visit are entirely fictional. All similarities to your own world are entirely coincidental."
            },
                new()
                {
                fillColor = Color.Transparent,
                borderColor = Color.Transparent,
                    text = "You have been warned.",
                    textColor = Color.Crimson,
                    onPageEnd = (_) =>
                    {
                        _.CurrentPage.textColor = Color.Transparent;
                        UIManager.dialogueUI.dialoguePanel.Position = new Vector2(32, Renderer.UIPreferedHeight - 180 - 16);
                    }
                }
        };
            dialogue.pages.AddRange(pages);
        }

        internal void StartBattle(bool onStone = false)
        {
            if(battle != null)
            {
                battle = null;
            }
            GameStateManager.SetState(GameState.Combat, new TrippingBalls(Renderer.SaveFrame()));
            List<Unit> list = [];
            var slimeList = activeWorld.context.entities.Where(x => x != null && x.enabled && x.HasComponent<TextureComponent>() && x.GetComponent<TextureComponent>().textureName == "Slime3");
            for (int i = 0; i < slimeList.Count(); i++)
            {
                Unit slime = UnitDefinitions.Get("Slime");
                slime.BattleStation = new Vector2(220 + i * 8 + ((i / 3) * 24), 60 + i * 32 - (i / 3 * 86));
                list.Add(slime);
            }

            Unit peas = new Unit()
            {
                sprite = "Peas",
                name = "Peas",
                //ai = new BasicAI(),
            };
            peas.resistances = new ElementalResistances(1f, 1f, 1f, 1f, 1f, 0f);
            peas.BattleStation = new Vector2(90, 90);

            Vector2 pos = GlobalPlayer.ActiveParty[0].position;

            int x = Math.Max(0, (int)pos.X) / TileMap.TILE_SIZE;
            int y = Math.Max(0, (int)pos.Y) / TileMap.TILE_SIZE;


            battle = Battle.Create(list);
            var endBattle = () =>
            {
                foreach (var item in slimeList)
                {
                    activeWorld.context.Destroy(item);
                }
            };
            battle.OnBattleEnd = endBattle;
            if (onStone)
            {
                //list.Add(peas);
                battle.SetUnits(list, [peas]);
                Script triedToHitThePeas = new()
                {
                    condition = (b) =>
                    {
                        Unit unit = b.unitsHitLastRound.FirstOrDefault(x => x.name == "Peas");
                        return unit != null && unit.stats.HP == unit.stats.MaxHP && b.State == BattleState.VictoryCheck;
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
                        Dialogue dialogue = Dialogue.Create();
                        DialoguePage page = new()
                        {
                            portraits = [page1Portrait],
                            title = battle.playerParty[0].name,
                            text = "It's completely impervious to our attacks.."
                        };
                        DialoguePage page2 = new()
                        {
                            title = "Peas",
                            text = "(Pea noises)"
                        };
                        DialoguePage page3 = new()
                        {
                            portraits = [page3Portrait],
                            title = battle.playerParty[0].name,
                            text = "There's only one thing that could work.."
                        };
                        dialogue.pages.AddRange([page, page2, page3]);
                        var disturb = new Disturb()
                        {
                            hpCost = battle.playerParty[0].stats.HP - 1,
                            spCost = 0,
                            canTarget = ValidTargets.All
                        };

                        battle.playerParty[0].ClearEffects();
                        battle.playerParty[0].abilities.Clear();
                        battle.playerParty[0].abilities.Add(disturb);
                        b.weaknessHitLastRound = true;
                    }
                };

                battle.scripts.Add(triedToHitThePeas);
                battle.bg = new BattleBackground("TestBG2");
            }
        }

        protected override void UnloadContent()
        {
            // Clean up after yourself!
            base.UnloadContent();

            Assets.Unload();
            Renderer.Unload();
            Context.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            // Run game logic in here. Do NOT render anything here!
            base.Update(gameTime);
            totalTime += gameTime.ElapsedGameTime.TotalMilliseconds * 0.01f;
            GetGameState()?.Update();
            if(Input.PressedKey(Keys.F1))
            {
                GameStateManager.SetState(GameState.Combat, new TrippingBalls(Renderer.SaveFrame()));
                StartBattle();
            }
            
            if(Input.PressedKey(Keys.F2))
            {
                GameState state = GameStateManager.State == GameState.Combat ? GameState.Overworld : GameState.Combat;
                GameStateManager.SetState(state);
            }
            
            if(Input.PressedKey(Keys.F3))
            {
                GameState state = GameStateManager.State == GameState.Overworld ? GameState.MainMenu : GameState.Overworld;
                GameStateManager.SetState(state);
            }

            if (Input.PressedKey(Keys.OemPlus))
            {
                GameOptions.ResolutionMultiplier++;
                gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
                gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
                gdm.ApplyChanges();
            }
            if (Input.PressedKey(Keys.OemMinus))
            {
                GameOptions.ResolutionMultiplier--;
                gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
                gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
                gdm.ApplyChanges();
            }

            UIManager.Update();
            Input.Update();
            SoundEngine.Update();
            CameraManager.Update();
            ParticleManager.Update();

            foreach (Transition transition in transitions)
            {
                transition.Update();

                if (transition.finished)
                    transition.target.Dispose();
            }

            transitions.RemoveAll(x => x.finished);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Render stuff in here. Do NOT run game logic in here!

            if (GetGameState() is World world)
            {
                if (world.tileMap.needsUpdate)
                    world.tileMap.BakeMap();
            }

            GraphicsDevice.SetRenderTarget(Renderer.WorldTarget);
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);

            IGameState state = GetGameState();

            if (state != null)
            {
                Renderer.StartSpriteBatch(spriteBatch);
                state?.Draw(spriteBatch);
                ParticleManager.Draw(spriteBatch);
                //Vector2 mousePos = Input.MousePosition;
                //spriteBatch.Draw(AssetManager.Textures["Elements2"], new Vector2((int)(mousePos.X), (int)(mousePos.Y)), Color.White);
                //CombatSystem.DrawInWorld(spriteBatch);
                //spriteBatch.Draw(AssetManager.Textures["Slime3"], new Vector2((int)(mousePos.X + 12), (int)(mousePos.Y+24)), Color.White);
                //spriteBatch.Draw(AssetManager.Textures["PeasToDisturb3"], new Vector2((int)(mousePos.X + 16), (int)(mousePos.Y-12)), Color.White);
                spriteBatch.End();
            }
            GraphicsDevice.SetRenderTarget(Renderer.UITarget);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            UIManager.Draw(spriteBatch);
            //spriteBatch.Draw(AssetManager.Textures["Pixel"], new Vector2(110, 586), new Rectangle(0, 0, 66, 20), Color.Black, 0f, Vector2.Zero, new Vector2(4), SpriteEffects.None, 0f);
            //spriteBatch.Draw(AssetManager.Textures["Pixel"], new Vector2(114, 590), new Rectangle(0, 0, 64, 18), Color.Gray, 0f, Vector2.Zero, new Vector2(4), SpriteEffects.None, 0f);
            //spriteBatch.Draw(AssetManager.Textures["Elements2"], new Vector2(400, 590), null, Color.White, 0f, Vector2.Zero, new Vector2(4), SpriteEffects.None, 0f);
            //spriteBatch.DrawString(AssetManager.DefaultFont, $"Tis ur haus?\n{Input.MousePosition}", new Vector2(130, 602), Color.Black);
            //spriteBatch.DrawString(AssetManager.DefaultFont, $"Tis ur haus?\n{Input.MousePosition}", new Vector2(128, 600), Color.White);
            //spriteBatch.Draw(AssetManager.Textures["Cursor3"], new Vector2(30+(float)Math.Cos(totalTime*panic)*drunkness, 600 + (float)Math.Sin(totalTime * panic) * drunkness + cursorPos), null, Color.Lerp(Color.White, Color.Gray, (float)Math.Cos(totalTime)), 0f, Vector2.Zero, new Vector2(4), SpriteEffects.None, 1f);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(Renderer.MainTarget);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(Renderer.WorldTarget, new Rectangle(0, 0, Renderer.UIPreferedWidth, Renderer.UIPreferedHeight), Color.White);
            spriteBatch.Draw(Renderer.UITarget, new Rectangle(0, 0, Renderer.UIPreferedWidth, Renderer.UIPreferedHeight), Color.White);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(Renderer.MainTarget, new Rectangle(0, 0, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight), Color.White);

            foreach(Transition transition in transitions)
            {
                transition.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

        public IGameState GetGameState()
        {
            switch(GameStateManager.State)
            {
                case GameState.Overworld:
                    return activeWorld;
                case GameState.Combat:
                    return battle;
                case GameState.MainMenu:
                    return null;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

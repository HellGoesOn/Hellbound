﻿using Microsoft.Xna.Framework;
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
using Microsoft.Xna.Framework.Input.Touch;

namespace HellTrail
{
    public class Main : Game
    {
        internal SpriteBatch spriteBatch;
        internal GraphicsDeviceManager gdm;
        internal static Main instance;
        internal static double totalTime;
        internal static Random rand;
        internal bool spiritsAngered;
        internal static int angerCounter;

        public Battle battle;
        private World activeWorld;

        public World ActiveWorld
        {
            get => activeWorld;
            set
            {
                activeWorld = value;
                UIManager.RelaunchEditor();
            }
        }

        public Context prefabContext;

        internal List<Transition> transitions = [];

        internal Dictionary<string, World> worlds = [];

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

            var dict = new Dictionary<string, Animation>
            {
                { "Idle", new Animation(0, 10) }
            };
            var test = new Transform(69, 69);
            string crime = ComponentIO.New_Serialize(test);

            IComponent component = ComponentIO.New_Deserialize(crime);
            //GetGameState().GetCamera().centre = GlobalPlayer.ActiveParty[0].position;

            var slime = prefabContext.Create();
            slime.AddComponent(new TextureComponent("Slime3")
            {
                origin = new Vector2(16),
                scale = new Vector2(1)
            });
            slime.AddComponent(new Transform(0, 0));
            slime.AddComponent(new Obesity(-0.02f));
            slime.AddComponent(new Velocity(0, 0));
            slime.AddComponent(new CreateBattleOnContact("TestBG", ["Slime", "Slime"], null));
            slime.AddComponent(new CollisionBox(16, 10, new Vector2(8, -4)));

            var torch = prefabContext.Create();
            torch.AddComponent(new TextureComponent("Torch")
            {
                origin = new Vector2(6),
                scale = new Vector2(1)
            });
            torch.AddComponent(new Transform(0, 0));
            torch.AddComponent(new Velocity(0, 0));
        }

        protected override void LoadContent()
        {
            // Load textures, sounds, and so on in here...
            base.LoadContent();
            Assets.Load(this);
            TileMap.Init();
            Renderer.Load(GraphicsDevice);
            CameraManager.Initialize();
            Context.InitializeAll();
            activeWorld = new World();
            UIManager.Init();
            UnitDefinitions.DefineUnits();
            GlobalPlayer.Init();
            ParticleManager.Initialize();

            Dialogue dialogue = Dialogue.Create();
            UIManager.dialogueUI.dialoguePanel.SetPosition(new Vector2(32, Renderer.UIPreferedHeight * 0.5f));
            DialoguePage[] pages =
                {
                new()
            {
                fillColor = Color.Transparent,
                borderColor = Color.Transparent,
                textColor = Color.Cyan,
                text = "(This 'Hell' guy just keeps going on..)",
                onPageEnd = (_) =>
                {
                    UIManager.dialogueUI.dialoguePanel.SetPosition(new Vector2(32, Renderer.UIPreferedHeight - 180 - 16));
                }
            }
        };
            dialogue.pages.AddRange(pages);

            ActiveWorld = World.LoadFromFile("\\Content\\Scenes\\", "Hills3");
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

            int x = Math.Max(0, (int)pos.X) / DisplayTileLayer.TILE_SIZE;
            int y = Math.Max(0, (int)pos.Y) / DisplayTileLayer.TILE_SIZE;


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

            //if (angerCounter >= 15)
            //    spiritsAngered = true;

            totalTime += gameTime.ElapsedGameTime.TotalMilliseconds * 0.01f;
            GetGameState()?.Update();
            if(Input.PressedKey(Keys.F1))
            {
                GameStateManager.SetState(GameState.Combat, new TrippingBalls(Renderer.SaveFrame()));
                StartBattle();
            }

            if (Input.PressedKey(Keys.F3))
            {
                GameOptions.ResolutionMultiplier++;
                gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
                gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
                gdm.ApplyChanges();
            }
            if (Input.PressedKey(Keys.F4))
            {
                GameOptions.ResolutionMultiplier--;
                gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
                gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
                gdm.ApplyChanges();
            }

            GlobalPlayer.Update();
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

            base.Draw(gameTime);

            IGameState state = GetGameState();

            GraphicsDevice.SetRenderTarget(Renderer.WorldTarget);
            Renderer.StartSpriteBatch(spriteBatch, stencil: DepthStencilState.DepthRead);
            GetGameState()?.Draw(spriteBatch);
            spriteBatch.End();
            Renderer.DoRender(spriteBatch);
            Renderer.StartSpriteBatch(spriteBatch);
            ParticleManager.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(Renderer.UITarget);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            UIManager.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone); 
            spriteBatch.Draw(Renderer.WorldTarget, new Rectangle(0, 0, (int)GameOptions.ScreenWidth, (int)GameOptions.ScreenHeight), Color.White);
            spriteBatch.Draw(Renderer.UITarget, new Rectangle(0, 0, (int)GameOptions.ScreenWidth, (int)GameOptions.ScreenHeight), Color.White);


            foreach (Transition transition in transitions)
            {
                transition.Draw(spriteBatch);
            }
            spriteBatch.End();


            //if (GetGameState() is World world)
            //{
            //    //if (world.tileMap.needsUpdate)
            //    //    world.tileMap.BakeMap();

            //    if(!world.tileMap.didBakeTexture)
            //    {
            //        world.tileMap.Draw(spriteBatch);
            //    }
            //}
            //GraphicsDevice.SetRenderTarget(null);

            //GraphicsDevice.Clear(Color.Black);
            //base.Draw(gameTime);

            //IGameState state = GetGameState();
            //if (state != null)
            //{
            //    state.Draw(spriteBatch);
            //    Renderer.RenderLayers(spriteBatch, Renderer.WorldTarget);
            //    Renderer.StartSpriteBatch(spriteBatch);
            //    ParticleManager.Draw(spriteBatch);
            //    spriteBatch.End();
            //}
            //GraphicsDevice.SetRenderTarget(Renderer.UITarget);
            //GraphicsDevice.Clear(Color.Transparent);
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            //UIManager.Draw(spriteBatch);
            //spriteBatch.End();

            //GraphicsDevice.SetRenderTarget(Renderer.MainTarget);
            //spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            //spriteBatch.Draw(Renderer.WorldTarget, new Rectangle(0, 0, Renderer.UIPreferedWidth, Renderer.UIPreferedHeight), Color.White);
            //spriteBatch.Draw(Renderer.UITarget, new Rectangle(0, 0, Renderer.UIPreferedWidth, Renderer.UIPreferedHeight), Color.White);
            //spriteBatch.End();
            //GraphicsDevice.SetRenderTarget(null);
            //spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            //spriteBatch.Draw(Renderer.MainTarget, new Rectangle(0, 0, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight), Color.White);

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

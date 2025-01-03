using Casull.Core;
using Casull.Core.Combat;
using Casull.Core.Combat.Abilities;
using Casull.Core.Combat.Scripting;
using Casull.Core.Combat.Sequencer;
using Casull.Core.DialogueSystem;
using Casull.Core.ECS;
using Casull.Core.ECS.Components;
using Casull.Core.Graphics;
using Casull.Core.Overworld;
using Casull.Core.UI;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Globalization;

namespace Casull
{
    /// <summary>
    /// If you are seeing this code, something is very fucking wrong with you.
    /// And with me too, you are welcome here.
    /// </summary>
    public class Main : Game
    {
        internal SpriteBatch spriteBatch;
        internal GraphicsDeviceManager gdm;
        internal static Main instance;
        internal static double totalTime;
        internal static Random rand;
        internal bool spiritsAngered;
        internal static int angerCounter;
        internal static string currentZone = "Forest3";
        internal static Vector2 lastTransitionPosition;

        public Battle battle;
        private World activeWorld;

        public MainMenu mainMenu;

        internal static int saveSlot = 0;
        internal static string newSlotName = "";

        public List<Sequence> outOfBoundsSequences = [];

        public World ActiveWorld {
            get => activeWorld;
            set {
                activeWorld = value;
#if DEBUG
                if(activeWorld != null)
                UIManager.RelaunchEditor(activeWorld.zoneName);
#endif
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
            this.Window.Title = "Casull";
        }

        protected unsafe override void Initialize()
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
            SoundEngine.StartMusic("ChangingSeasons", true);

            //var dict = new Dictionary<string, Animation>
            //{
            //    { "Idle", new Animation(0, 10) }
            //};
            //var test = new Transform(69, 69);
            //string crime = ComponentIO.New_Serialize(test);

            //IComponent component = ComponentIO.New_Deserialize(crime);
            //GetGameState().GetCamera().centre = GlobalPlayer.ActiveParty[0].position;

            mainMenu = new MainMenu();


            if (File.Exists(Environment.CurrentDirectory + "\\config.cfg")) {
                var oldCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                var options = File.ReadAllText(Environment.CurrentDirectory + "\\config.cfg").Split(Environment.NewLine);

                if (options.Length >= 1 && int.TryParse(options[0], out var x)) {
                    GameOptions.ResolutionMultiplier = x;
                    UpdateResolution();
                }
                if (options.Length >= 2 && float.TryParse(options[1], out var y)) {
                    GameOptions.GeneralVolume = Math.Clamp(y, 0.0f, 1.0f);
                }
                if (options.Length >= 3 && float.TryParse(options[2], out var z)) {
                    GameOptions.MusicVolume = Math.Clamp(z, 0.0f, 1.0f);
                }

                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
            GameStateManager.SetState(GameState.MainMenu, new BlackFadeInFadeOut(Renderer.SaveFrame()));
            GlobalPlayer.Init();
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
            ParticleManager.Initialize();

            //    Dialogue dialogue = Dialogue.Create();
            //    UIManager.dialogueUI.dialoguePanel.SetPosition(new Vector2(32, Renderer.UIPreferedHeight * 0.5f));
            //    DialoguePage[] pages =
            //        [
            //        new()
            //    {
            //        fillColor = Color.Transparent,
            //        borderColor = Color.Transparent,
            //        textColor = Color.Cyan,
            //        text = "(This 'Hell' guy just keeps going on..)",
            //        onPageEnd = (_) =>
            //        {
            //            UIManager.dialogueUI.dialoguePanel.SetPosition(new Vector2(32, Renderer.UIPreferedHeight - 180 - 16));
            //        }
            //    }
            //];
            //    dialogue.pages.AddRange(pages);

            ActiveWorld = World.LoadFromFile("\\Content\\Scenes\\", "Forest3");

            lastTransitionPosition = ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x.GetComponent<Tags>().Has("Player")).GetComponent<Transform>().position;
        }

        internal void StartBattle(bool onStone = false)
        {
            if (battle != null) {
                battle = null;
            }
            GameStateManager.SetState(GameState.Combat, new TrippingBalls(Renderer.SaveFrame()));
            List<Unit> list = [];
            var slimeList = activeWorld.context.GetAllEntities().Where(x => x != null && x.enabled && x.HasComponent<TextureComponent>() && x.GetComponent<TextureComponent>().textureName == "Slime3");
            for (int i = 0; i < slimeList.Count(); i++) {
                Unit slime = UnitDefinitions.Get("Slime");
                slime.BattleStation = new Vector2(220 + i * 8 + ((i / 3) * 24), 60 + i * 32 - (i / 3 * 86));
                list.Add(slime);
            }

            Unit peas = new() {
                sprite = "Peas",
                name = "Peas",
                resistances = new ElementalResistances(1f, 1f, 1f, 1f, 1f, 0f),
                BattleStation = new Vector2(90, 90)
            };

            Vector2 pos = GlobalPlayer.ActiveParty[0].position;

            int x = Math.Max(0, (int)pos.X) / DisplayTileLayer.TILE_SIZE;
            int y = Math.Max(0, (int)pos.Y) / DisplayTileLayer.TILE_SIZE;


            battle = Battle.Create(list);
            var endBattle = () => {
                foreach (var item in slimeList) {
                    activeWorld.context.Destroy(item);
                }
            };
            battle.OnBattleEnd = endBattle;
            if (onStone) {
                //list.Add(peas);
                battle.SetUnits(list, [peas]);
                Script triedToHitThePeas = new() {
                    condition = (b) => {
                        Unit unit = b.unitsHitLastRound.FirstOrDefault(x => x.name == "Peas");
                        return unit != null && unit.Stats.HP == unit.Stats.MaxHP && b.State == BattleState.VictoryCheck;
                    },
                    action = (b) => {
                        var page1Portrait = new Portrait("EndLife", new FrameData(0, 0, 32, 32)) {
                            scale = new Vector2(-16, 16)
                        };
                        var page3Portrait = new Portrait("EndLife", new FrameData(0, 32, 32, 32)) {
                            scale = new Vector2(16, 16)
                        };
                        Dialogue dialogue = Dialogue.Create();
                        DialoguePage page = new() {
                            portraits = [page1Portrait],
                            title = battle.playerParty[0].name,
                            text = "It's completely impervious to our attacks.."
                        };
                        DialoguePage page2 = new() {
                            title = "Peas",
                            text = "(Pea noises)"
                        };
                        DialoguePage page3 = new() {
                            portraits = [page3Portrait],
                            title = battle.playerParty[0].name,
                            text = "There's only one thing that could work.."
                        };
                        dialogue.pages.AddRange([page, page2, page3]);
                        var disturb = new Disturb() {
                            hpCost = battle.playerParty[0].Stats.HP - 1,
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

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            File.WriteAllText(Environment.CurrentDirectory + "\\config.cfg", $"{GameOptions.ResolutionMultiplier}{Environment.NewLine}{GameOptions.GeneralVolume}{Environment.NewLine}{GameOptions.MusicVolume}");
        }

        public void UpdateResolution()
        {
            gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
            gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
            gdm.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            // Run game logic in here. Do NOT render anything here!
            base.Update(gameTime);

            //if (angerCounter >= 15)
            //    spiritsAngered = true;

            if (World.cutscenes.Count > 0)
                World.cutscenes[0].Update();

            World.cutscenes.RemoveAll(x => x.Finished);

            foreach (var seq in outOfBoundsSequences)
                seq.Update();

            outOfBoundsSequences.RemoveAll(x => x.isFinished);

            totalTime += gameTime.ElapsedGameTime.TotalMilliseconds * 0.01f;
            GetGameState()?.Update();
            if (Input.PressedKey(Keys.F1)) {
                GameStateManager.SetState(GameState.Combat, new TrippingBalls(Renderer.SaveFrame()));
                StartBattle();
            }

#if DEBUG
            if (Input.PressedKey(Keys.F3)) {
                GameOptions.ResolutionMultiplier++;
                gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
                gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
                gdm.ApplyChanges();
            }
            if (Input.PressedKey(Keys.F4)) {
                GameOptions.ResolutionMultiplier--;
                gdm.PreferredBackBufferWidth = GameOptions.ScreenWidth;
                gdm.PreferredBackBufferHeight = GameOptions.ScreenHeight;
                gdm.ApplyChanges();
            }
#endif
            GlobalPlayer.Update();
            UIManager.Update();
            Input.Update();
            SoundEngine.Update();
            CameraManager.Update();
            ParticleManager.Update();

            foreach (Transition transition in transitions) {
                transition.Update();

                if (transition.finished)
                    transition.target.Dispose();
            }

            transitions.RemoveAll(x => x.finished);

            GameStateManager.Update();
        }

        bool onceAndNeverAgain;

        protected override void Draw(GameTime gameTime)
        {
            // Render stuff in here. Do NOT run game logic in here!

            base.Draw(gameTime);

            if (!onceAndNeverAgain) {
                GraphicsDevice.Clear(Color.Black);
                onceAndNeverAgain = true;
            }

            GraphicsDevice.SetRenderTarget(Renderer.WorldTarget);
            Renderer.StartSpriteBatch(spriteBatch, state: SamplerState.PointWrap, stencil: DepthStencilState.DepthRead);
            GetGameState()?.Draw(spriteBatch);
            spriteBatch.End();
            Renderer.DoRender(spriteBatch);
            Renderer.StartSpriteBatch(spriteBatch);
            ParticleManager.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(Renderer.UITarget);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            UIManager.Draw(spriteBatch);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(Renderer.WorldTarget, new Rectangle(0, 0, (int)GameOptions.ScreenWidth, (int)GameOptions.ScreenHeight), Color.White);
            spriteBatch.Draw(Renderer.UITarget, new Rectangle(0, 0, (int)GameOptions.ScreenWidth, (int)GameOptions.ScreenHeight), Color.White);


            foreach (Transition transition in transitions) {
                transition.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

        public IGameState GetGameState()
        {
            return GameStateManager.State switch {
                GameState.Overworld => activeWorld,
                GameState.Combat => battle,
                GameState.MainMenu => mainMenu,
                _ => throw new NotImplementedException(),
            };
        }
    }
}

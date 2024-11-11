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

namespace HellTrail
{
    public class Main : Game
    {
        SpriteBatch spriteBatch;
        internal GraphicsDeviceManager gdm;
        internal static Main instance;
        internal static double totalTime;
        float panic = 1f;
        float drunkness = 1f;
        float cursorPos = 0f;
        internal static Random rand;

        public Battle battle;

        public Main()
        {
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
        }

        protected override void LoadContent()
        {
            // Load textures, sounds, and so on in here...
            base.LoadContent();
            Assets.Load(this);
            Renderer.Load(GraphicsDevice);
            CameraManager.Initialize();
            UIManager.Init();
            GlobalPlayer.Init();
            ParticleManager.Initialize();
            StartBattle();
        }

        private void StartBattle()
        {
            foreach(Unit unit in GlobalPlayer.ActiveParty)
            {
                unit.HP = unit.MaxHP;
                unit.opacity = 1f;
            }
            List<Unit> list = [];
            for (int i = 0; i < 5; i++)
            {
                Unit slime = new()
                {
                    name = "Slime",
                    ai = new BasicAI()
                };
                slime.resistances[ElementalType.Phys] = 0.20f;
                slime.resistances[ElementalType.Fire] = -0.5f;
                slime.abilities.Add(new Bite());
                slime.abilities.Add(new Agi());
                slime.abilities.Add(new Disturb());
                slime.BattleStation = new Vector2(220 + i * 8 + ((i / 3) * 24), 60 + i * 32 - (i / 3 * 86));
                list.Add(slime);
            }

            battle = Battle.Create(list);
        }

        protected override void UnloadContent()
        {
            // Clean up after yourself!
            base.UnloadContent();

            Assets.Unload();
            Renderer.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            // Run game logic in here. Do NOT render anything here!
            base.Update(gameTime);
            totalTime += gameTime.ElapsedGameTime.TotalMilliseconds * 0.01f;
            panic = 1f;
            drunkness = 1f;
            battle?.Update();

            if(Input.PressedKey(Keys.F1))
            {
                StartBattle();
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

        }

        protected override void Draw(GameTime gameTime)
        {
            // Render stuff in here. Do NOT run game logic in here!
            GraphicsDevice.SetRenderTarget(Renderer.MainTarget);
            GraphicsDevice.Clear(Color.DarkOliveGreen);
            base.Draw(gameTime);

            Renderer.StartSpriteBatch(spriteBatch);
            battle?.DrawField(spriteBatch);
            ParticleManager.Draw(spriteBatch);
            //Vector2 mousePos = Input.MousePosition;
            //spriteBatch.Draw(AssetManager.Textures["Elements2"], new Vector2((int)(mousePos.X), (int)(mousePos.Y)), Color.White);
            //CombatSystem.DrawInWorld(spriteBatch);
            //spriteBatch.Draw(AssetManager.Textures["Slime3"], new Vector2((int)(mousePos.X + 12), (int)(mousePos.Y+24)), Color.White);
            //spriteBatch.Draw(AssetManager.Textures["PeasToDisturb3"], new Vector2((int)(mousePos.X + 16), (int)(mousePos.Y-12)), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(Renderer.UITarget);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            battle?.DrawUI(spriteBatch);
            UIManager.Draw(spriteBatch);
            //spriteBatch.Draw(AssetManager.Textures["Pixel"], new Vector2(110, 586), new Rectangle(0, 0, 66, 20), Color.Black, 0f, Vector2.Zero, new Vector2(4), SpriteEffects.None, 0f);
            //spriteBatch.Draw(AssetManager.Textures["Pixel"], new Vector2(114, 590), new Rectangle(0, 0, 64, 18), Color.Gray, 0f, Vector2.Zero, new Vector2(4), SpriteEffects.None, 0f);
            //spriteBatch.Draw(AssetManager.Textures["Elements2"], new Vector2(400, 590), null, Color.White, 0f, Vector2.Zero, new Vector2(4), SpriteEffects.None, 0f);
            //spriteBatch.DrawString(AssetManager.DefaultFont, $"Tis ur haus?\n{Input.MousePosition}", new Vector2(130, 602), Color.Black);
            //spriteBatch.DrawString(AssetManager.DefaultFont, $"Tis ur haus?\n{Input.MousePosition}", new Vector2(128, 600), Color.White);
            //spriteBatch.Draw(AssetManager.Textures["Cursor3"], new Vector2(30+(float)Math.Cos(totalTime*panic)*drunkness, 600 + (float)Math.Sin(totalTime * panic) * drunkness + cursorPos), null, Color.Lerp(Color.White, Color.Gray, (float)Math.Cos(totalTime)), 0f, Vector2.Zero, new Vector2(4), SpriteEffects.None, 1f);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(Renderer.MainTarget, new Rectangle(0, 0, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight), Color.White);
            spriteBatch.Draw(Renderer.UITarget, new Rectangle(0, 0, gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
        }
    }
}

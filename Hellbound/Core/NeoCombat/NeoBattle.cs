using HellTrail.Core.Combat;
using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
using HellTrail.Extensions;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.NeoCombat
{
    public class NeoBattle : IGameState
    {
        public Context context;

        public Systems systems;

        private State _state;

        private readonly Vector2[] myEnemiesPositions;

        private readonly Vector2[] myAlliesPositions;

        private Vector2 pointOfInterest;
        private float targetZoom;

        private BattleBackground bg;
        private BattleBackground bg2;

        public NeoBattle()
        {
            bg = new("VisualTest");
            bg2 = new("TestBG");
            myEnemiesPositions = [new Vector2(180, 120), new Vector2(220, 120)];
            myAlliesPositions = [new Vector2(104, 135)];

            _state = State.Intro;
            context = new Context();

            pointOfInterest = new Vector2(160, 90);

            systems = new Systems();

            systems.AddSystem(new NewAnimationSystem(context));
            systems.AddSystem(new DrawSystem(context));
            //systems.AddSystem(new MoveCameraSystem(context));
            systems.AddSystem(new ReadPlayerInputSystem(context));
            systems.AddSystem(new MoveSystem(context)); 
            GetCamera().speed = 0.1f;
            targetZoom = GetCamera().zoom;
        }

        public static NeoBattle Create(List<Entity> allies, List<Entity> enemies)
        {
            var battle = new NeoBattle();
            int i = 0;
            foreach(var ally in allies)
            {
                var e = battle.context.CopyFrom(ally);
                var position = battle.myAlliesPositions[i];
                e.AddComponent(new Transform(position.X, position.Y));
                i++;
            }

            i = 0;
            foreach(var enemy in enemies)
            {

                var e = battle.context.CopyFrom(enemy);
                var position = battle.myEnemiesPositions[i];
                e.AddComponent(new Transform(position.X, position.Y));
                i++;
            }

            return battle;
        }

        public void Update()
        {
            systems.Execute(context);

            switch(_state)
            {
                case State.Intro:
                    break;
                case State.TurnBegin:
                    break;
                case State.CheckInput:
                    break;
                case State.Action:
                    break;
                    case State.TurnEnd:
                    break;
                case State.VictoryCheck:
                    break;
                case State.ProgressTurn:
                    break;
            }

            var cam = GetCamera();

            if (Input.LMBClicked)
            {
                targetZoom = 2f;
                pointOfInterest = Input.MousePosition;
            }
            if (Input.RMBClicked)
            {
                targetZoom = 1f;
                pointOfInterest = new Vector2(160, 90);
            }
            cam.centre += (pointOfInterest - cam.centre) * cam.speed;

            cam.zoom = MathHelper.Lerp(cam.zoom, targetZoom, cam.speed);

            cam.centre.X = Math.Clamp(cam.centre.X, -cam.view.Width*0.5f, cam.view.Width* 1.5f);
            cam.centre.Y = Math.Clamp(cam.centre.Y, -cam.view.Height * 0.5f, cam.view.Height * 0.5f );

            UIManager.Debug((cam.view.Height * 0.5f * cam.zoom).ToString());
            UIManager.Debug((cam.centre).ToString());
            UIManager.Debug((pointOfInterest).ToString());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Main.instance.GraphicsDevice.Clear(new Color(147, 245, 249));

            Renderer.Draw(Assets.GetTexture(bg2.texture), new Vector2(-320, 0), new Rectangle(0, 0, 960, 180), bg.color * bg.opacity, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            //Renderer.Draw(Assets.GetTexture("Space"), new Vector2(-320, -180), new Rectangle(0, 0, 960, 180), bg.color * bg.opacity, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            //Renderer.Draw(Assets.GetTexture(bg.texture), new Vector2(40, 22.5f), null, bg.color * bg.opacity, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

            Renderer.Draw(Assets.GetTexture("Arrow"), pointOfInterest.ToInt(), null, Color.White, 0f, new Vector2(5, 3f), new Vector2((float)Math.Sin(Main.totalTime), 1f), SpriteEffects.None, 1000f);

            systems.Draw(context, spriteBatch);
        }

        public Camera GetCamera()
        {
            return CameraManager.neoCombatCamera;
        }

        public enum State
        {
            Intro,
            TurnBegin,
            CheckInput,
            Action,
            TurnEnd,
            VictoryCheck,
            ProgressTurn
        }
    }

    public class NeoBattleScene
    {

    }
}

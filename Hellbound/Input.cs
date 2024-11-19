using HellTrail.Core;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail
{
    public partial class Input
    {
        static MouseState _oldMBState;

        static KeyboardState _oldKBState;

        public static void Update()
        {
            //_oldPos = ScaledMousePos;
            _oldMBState = Mouse.GetState();
            _oldKBState = Keyboard.GetState();
        }

        public static bool IsWindowActive => Main.instance.IsActive;

        public static Vector2 Adjustment
            {
            get
            {
                var mx = Main.instance.gdm.PreferredBackBufferWidth / Renderer.PreferedWidth;
                var my = Main.instance.gdm.PreferredBackBufferHeight / Renderer.PreferedHeight;
                return new Vector2(mx, my);
            }
            }

        public static Vector2 MousePosition
        {
            get
            {
                MouseState state = Mouse.GetState();
                var adjusted = new Vector2(state.X, state.Y) / Adjustment;
                var cam = Main.instance.GetGameState().GetCamera();
                Vector2 v = (adjusted + cam.Position) / cam.zoom;
                return v;
            }
        }

        public static Vector2 UIMousePosition
        {
            get
            {
                MouseState state = Mouse.GetState();
                float mx = Main.instance.gdm.PreferredBackBufferWidth / (float)Renderer.UIPreferedWidth;
                float my = Main.instance.gdm.PreferredBackBufferHeight / (float)Renderer.UIPreferedHeight;
                var adjusted = new Vector2(state.X, state.Y) / new Vector2(mx, my); // GameOptions.Resolution); 
                //var cam = CameraManager.GetCamera;
                //Vector2 v = (adjusted + cam.Position) / cam.zoom;
                return adjusted;
            }
        }

        public static bool PressedKey(Keys key) => Keyboard.GetState().IsKeyDown(key) && _oldKBState.IsKeyUp(key) && IsWindowActive;

        public static bool PressedKey(Keys[] key) => key.Any(PressedKey);

        public static bool HeldKey(Keys key) => Keyboard.GetState().IsKeyDown(key) && IsWindowActive;

        public static bool ReleasedKey(Keys key) => Keyboard.GetState().IsKeyUp(key) && _oldKBState.IsKeyDown(key) && IsWindowActive;

        public static bool LMBClicked => Mouse.GetState().LeftButton == ButtonState.Pressed && _oldMBState.LeftButton == ButtonState.Released && IsWindowActive;

        public static bool LMBReleased => Mouse.GetState().LeftButton == ButtonState.Released && _oldMBState.LeftButton == ButtonState.Pressed && IsWindowActive;

        public static bool LMBHeld => Mouse.GetState().LeftButton == ButtonState.Pressed && IsWindowActive;

        public static bool RMBClicked => Mouse.GetState().RightButton == ButtonState.Pressed && _oldMBState.RightButton == ButtonState.Released && IsWindowActive;

        public static bool RMBReleased => Mouse.GetState().RightButton == ButtonState.Released && _oldMBState.RightButton == ButtonState.Pressed && IsWindowActive;

        public static bool RMBHeld => Mouse.GetState().RightButton == ButtonState.Pressed && IsWindowActive;
    }
}

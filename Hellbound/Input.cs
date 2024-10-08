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

        public static Vector2 MousePosition
        {
            get
            {
                MouseState state = Mouse.GetState();
                var mx = Main.instance.gdm.PreferredBackBufferWidth / Renderer.PreferedWidth;
                var my = Main.instance.gdm.PreferredBackBufferHeight / Renderer.PreferedHeight;
                var adjusted = new Vector2(state.X, state.Y) / new Vector2(mx, my);
                //var cam = CameraManager.GetCamera;
                //Vector2 v = (adjusted + cam.Position) / cam.zoom;
                return adjusted;
            }
        }

        public static bool PressedKey(Keys key) => Keyboard.GetState().IsKeyDown(key) && _oldKBState.IsKeyUp(key);

        public static bool HeldKey(Keys key) => Keyboard.GetState().IsKeyDown(key);

        public static bool ReleasedKey(Keys key) => Keyboard.GetState().IsKeyUp(key) && _oldKBState.IsKeyDown(key);

        public static bool LMBClicked => Mouse.GetState().LeftButton == ButtonState.Pressed && _oldMBState.LeftButton == ButtonState.Released;

        public static bool LMBReleased => Mouse.GetState().LeftButton == ButtonState.Released && _oldMBState.LeftButton == ButtonState.Pressed;

        public static bool LMBHeld => Mouse.GetState().LeftButton == ButtonState.Pressed;
    }
}

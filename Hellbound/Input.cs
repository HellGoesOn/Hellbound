using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Casull
{
    public partial class Input
    {
        static MouseState _oldMBState;

        static KeyboardState _oldKBState;

        public static KeyEventHandler OnKeyPressed;
        public static KeyEventHandler OnKeyHeld;
        public static KeyEventHandler OnKeyReleased;

        public static MouseEventHandler OnMousePressed;
        public static MouseEventHandler OnMouseHeld;
        public static MouseEventHandler OnMouseReleased;
        public static MouseWheelEventHandler OnMouseWheel;

        public static bool isTyping;

        public static void Update()
        {
            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            Keys[] oldKeys = _oldKBState.GetPressedKeys();
            MouseState state = Mouse.GetState();

            if (Main.instance.IsActive) {
                HandleMouse(state);

                for (int i = 0; i < keys.Length; i++) {
                    if (oldKeys.Length <= 0 || !oldKeys.Contains(keys[i]))
                        OnKeyPressed?.Invoke(keys[i]);
                }

                for (int i = 0; i < keys.Length; i++) {
                    OnKeyHeld?.Invoke(keys[i]);
                }

                for (int i = 0; i < oldKeys.Length; i++) {
                    if (keys.Length <= 0 || !keys.Contains(oldKeys[i]))
                        OnKeyReleased?.Invoke(oldKeys[i]);
                }
            }

            //_oldPos = ScaledMousePos;
            _oldMBState = Mouse.GetState();
            _oldKBState = Keyboard.GetState();
        }

        private static void HandleMouse(MouseState state)
        {
            if (state.ScrollWheelValue != _oldMBState.ScrollWheelValue) {
                OnMouseWheel?.Invoke(state.ScrollWheelValue, _oldMBState.ScrollWheelValue);
            }

            if (state.LeftButton == ButtonState.Pressed && _oldMBState.LeftButton == ButtonState.Released) {
                OnMousePressed?.Invoke(MouseButton.Left);
            }

            if (state.RightButton == ButtonState.Pressed && _oldMBState.RightButton == ButtonState.Released) {
                OnMousePressed?.Invoke(MouseButton.Right);
            }

            if (state.MiddleButton == ButtonState.Pressed && _oldMBState.MiddleButton == ButtonState.Released) {
                OnMousePressed?.Invoke(MouseButton.Middle);
            }

            if (state.LeftButton == ButtonState.Pressed) {
                OnMouseHeld?.Invoke(MouseButton.Left);
            }

            if (state.RightButton == ButtonState.Pressed) {
                OnMouseHeld?.Invoke(MouseButton.Right);
            }

            if (state.MiddleButton == ButtonState.Pressed) {
                OnMouseHeld?.Invoke(MouseButton.Middle);
            }

            if (state.LeftButton == ButtonState.Released && _oldMBState.LeftButton == ButtonState.Pressed) {
                OnMouseReleased?.Invoke(MouseButton.Left);
            }

            if (state.RightButton == ButtonState.Released && _oldMBState.RightButton == ButtonState.Pressed) {
                OnMouseReleased?.Invoke(MouseButton.Right);
            }

            if (state.MiddleButton == ButtonState.Released && _oldMBState.MiddleButton == ButtonState.Pressed) {
                OnMouseReleased?.Invoke(MouseButton.Middle);
            }
        }

        public static bool IsWindowActive => Main.instance.IsActive;

        public static Vector2 Adjustment {
            get {
                var mx = Main.instance.gdm.PreferredBackBufferWidth / Renderer.PreferedWidth;
                var my = Main.instance.gdm.PreferredBackBufferHeight / Renderer.PreferedHeight;
                return new Vector2(mx, my);
            }
        }

        public static Vector2 MousePosition {
            get {
                MouseState state = Mouse.GetState();
                var adjusted = new Vector2(state.X, state.Y);
                var gameState = Main.instance.GetGameState();
                var cam = gameState.GetCamera();
                Vector2 v = Vector2.Transform(adjusted, cam.Inverse);
                return v;
            }
        }

        public static Vector2 UIMousePosition {
            get {
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

    public delegate void MouseEventHandler(MouseButton state);
    public delegate void MouseWheelEventHandler(int newValue, int oldValue);

    public delegate void KeyEventHandler(Keys key);

    public enum MouseButton
    {
        Left,
        Middle,
        Right
    }
}

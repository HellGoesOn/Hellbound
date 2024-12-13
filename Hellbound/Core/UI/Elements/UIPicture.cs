using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.UI.Elements
{
    public class UIPicture : UIElement
    {
        public int currentFrame;
        public string textureName;
        public Vector2 origin;
        public Color tint;
        public FrameData[] frames;
        public UIPicture(string textureName, params FrameData[] frames)
        {
            tint = Color.White;
            currentFrame = 0;
            this.textureName = textureName;
            this.frames = frames;
            scale = Vector2.One;
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            if (frames == null || frames.Length == 0 || string.IsNullOrWhiteSpace(textureName))
                return;

            spriteBatch.Draw(Assets.GetTexture(textureName), GetPosition(), frames[currentFrame].AsRect, tint, Rotation, origin, scale, SpriteEffects.None, 1);
        }
    }
}

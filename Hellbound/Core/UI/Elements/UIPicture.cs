using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.Elements
{
    public class UIPicture : UIElement
    {
        public int currentFrame;
        public float rotation;
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

            spriteBatch.Draw(Assets.GetTexture(textureName), GetPosition(), frames[currentFrame].AsRect, tint, rotation, origin, scale, SpriteEffects.None, 1);
        }
    }
}

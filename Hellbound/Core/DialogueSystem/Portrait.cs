using Casull.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.DialogueSystem
{
    public class UIPortrait : UIElement
    {
        public List<Portrait> portrait;

        public UIPortrait(List<Portrait> defaultPortrait)
        {
            portrait = defaultPortrait;
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            if (portrait != null && portrait.Count > 0) {
                foreach (Portrait portrait in portrait)
                    spriteBatch.Draw(portrait.texture, this.GetPosition() + portrait.offset, portrait.frame.AsRect, portrait.tint, portrait.rotation, portrait.origin, portrait.scale, SpriteEffects.None, 0);
            }
        }
    }

    public class Portrait
    {
        public float rotation;
        public Vector2 offset;
        public Vector2 origin;
        public Vector2 scale;
        public Color tint;
        public FrameData frame;
        public Texture2D texture;

        public Portrait(string texture, FrameData frame)
        {
            rotation = 0f;
            tint = Color.White;
            this.texture = Assets.GetTexture(texture);
            this.frame = frame;
            scale = Vector2.One;
            origin = new Vector2(frame.width, frame.height) * 0.5f;
        }
    }


}

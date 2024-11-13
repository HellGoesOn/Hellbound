using HellTrail.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.DialogueSystem
{
    public class UIPortrait : UIElement
    {
        public Portrait portrait;

        public UIPortrait(Portrait defaultPortrait)
        {
            portrait = defaultPortrait;
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            if (portrait != null)
            {
                spriteBatch.Draw(portrait.texture, this.Position + portrait.offset, portrait.frame.AsRect, portrait.tint, portrait.rotation, portrait.origin, portrait.scale, SpriteEffects.None, 0);
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
            this.texture = Assets.Textures[texture];
            this.frame = frame;
            scale = Vector2.One;
            origin = new Vector2(frame.width, frame.height) * 0.5f;
        }
    }


}

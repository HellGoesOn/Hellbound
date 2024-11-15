using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Render
{
    public class Transition
    {
        public RenderTarget2D target;
        public float scale = 1.0f;
        public float swirl = 0.0f;
        public bool finished;
        public Transition(RenderTarget2D target) 
        {
            scale = 1.0f;
            this.target = target;
        }

        public virtual void Update()
        {
            if (scale <= 0.0f)
            {
                finished = true;
                return;
            }
            scale -= 0.025f;
            swirl += 0.032f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!target.IsDisposed && target != null)
                DoDraw(spriteBatch);
        }

        public virtual void DoDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(target, new Vector2(target.Width, target.Height) * 0.5f, null, Color.White, swirl, new Vector2(target.Width, target.Height) * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }

    public class TrippingBalls : Transition
    {
        public float opacity;
        public TrippingBalls(RenderTarget2D target) : base(target)
        {
            opacity = 1.2f;
            this.target = target;
        }

        public override void Update()
        {
            if(opacity <= 0.8f && scale <= 2)
            {
                scale += 0.005f;
            }

            if (opacity <= 0.0f)
                finished = true;

            opacity -= 0.02f;
        }

        public override void DoDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(target, new Vector2(target.Width, target.Height) * 0.5f, null, Color.White * opacity * 1.15f, 0, new Vector2(target.Width, target.Height) * 0.5f, scale * 1.15f, SpriteEffects.None, 0f);

            spriteBatch.Draw(target, new Vector2(target.Width, target.Height) * 0.5f, null, Color.White * opacity, 0, new Vector2(target.Width, target.Height) * 0.5f, scale, SpriteEffects.None, 0f);

        }
    }

    public class SliceTransition : Transition
    {
        public float moveAmount = 0.0f;
        public SliceTransition(RenderTarget2D target) : base(target)
        {
            this.target = target;
        }

        public override void Update()
        {
            if (moveAmount >= target.Width)
            {
                finished = true;
                return;
            }

            moveAmount += 0.32f;
            moveAmount *= 1.16f;
        }

        public override void DoDraw(SpriteBatch spriteBatch)
        {
            int width = target.Width;
            int height = target.Height;
            Vector2 size = new Vector2(width, height);
            spriteBatch.Draw(target, new Vector2(moveAmount, 0), new Rectangle(0, 0, width, (int)(height * 0.5f)), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(target, new Vector2(-moveAmount, height / 2), new Rectangle(0, (int)(height * 0.5f), width, (int)(height * 0.5f)), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        }
    }
}

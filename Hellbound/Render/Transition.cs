using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Render
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
            if (scale <= 0.0f) {
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

    public class BlackFadeInFadeOut : Transition
    {
        float fadeIn;
        float fadeOut;
        public float speed = 0.05f;
        public BlackFadeInFadeOut(RenderTarget2D target) : base(target)
        {
        }

        public override void Update()
        {
            if (fadeOut < 1.0f)
                fadeOut += speed;
            else if (fadeIn < 1.0f)
                fadeIn += speed;
            else
                finished = true;
        }

        public override void DoDraw(SpriteBatch spriteBatch)
        {
            Color clr = Color.Lerp(Color.White, Color.Black, fadeOut);
            if (fadeOut <= 1.0f)
                spriteBatch.Draw(target, Vector2.Zero, clr);
            else
                spriteBatch.Draw(Assets.GetTexture("Pixel"), new Rectangle(0, 0, target.Width, target.Height), Color.Black * (1.0f - fadeIn));
        }
    }

    public class ZoneTransition : Transition
    {
        Vector2 endPosition;
        Vector2 currentPosition;
        public Vector2 startPosition;
        public Vector2 direction;
        public float speed = 100f;
        public bool coveredScreen;

        public float progress = 1.05f;

        public ZoneTransition(RenderTarget2D target, Vector2 direction) : base(target)
        {
            this.direction = direction;

            //if(float.Sign(direction.X) == 1)
            //{
            //    currentPosition = startPosition = new Vector2(-target.Width * 2, 0);
            //    endPosition = new Vector2(-startPosition.X, 0);
            //}

            //if(float.Sign(direction.X) == -1)
            //{
            //    currentPosition = startPosition = new Vector2(target.Width * 2, 0);
            //    endPosition = new Vector2(-startPosition.X, 0);
            //}

            //if(direction.Y < 0)
            //{
            //    startPosition = new Vector2(0, target.Width * 2f);
            //}
            //else if(direction.Y > 0)
            //{
            //    startPosition = new Vector2(0, -target.Width * 2f);
            //}
            //else if(direction.X < 0)
            //{
            //    startPosition = new Vector2(target.Width * 2, 0);
            //}
            //else
            //{
            //    startPosition = new Vector2(-target.Width * 2f, 0);
            //}

        }

        public override void Update()
        {
            //currentPosition += direction * speed;

            //if (Vector2.Distance(currentPosition, Vector2.Zero) <= speed)
            //    coveredScreen = true;

            //if(Vector2.Distance(currentPosition, endPosition) <= speed * 0.5f)
            //{
            //    finished = true;
            //}

            progress -= 0.05f;

            if (progress <= 0.0f)
                finished = true;
        }

        public override void DoDraw(SpriteBatch spriteBatch)
        {
            //if(!coveredScreen)

            var scale = direction.X != 0 ? new Vector2(progress, 1) : new Vector2(1, progress);

            var origin = new Vector2(target.Width, target.Height) * 0.5f;

            if (direction.X > 0) {
                origin = new Vector2(target.Width, 0);
            }
            if (direction.X < 0) {
                origin = new Vector2(0, 0);
            }
            if (direction.Y > 0) {
                origin = new Vector2(0, target.Height);
            }

            if (direction.Y < 0) {
                origin = new Vector2(0, -target.Height);
            }



            spriteBatch.Draw(target, Vector2.Zero + origin, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Assets.GetTexture("Pixel"), currentPosition, new Rectangle(0, 0, (int)(target.Width * 2f), (int)(target.Width * 2f)), Color.Black);
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
            if (opacity <= 0.8f && scale <= 2) {
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
            if (moveAmount >= target.Width) {
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
            Vector2 size = new(width, height);
            spriteBatch.Draw(target, new Vector2(moveAmount, 0), new Rectangle(0, 0, width, (int)(height * 0.5f)), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(target, new Vector2(-moveAmount, height / 2), new Rectangle(0, (int)(height * 0.5f), width, (int)(height * 0.5f)), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        }
    }
}

using HellTrail;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Treeline.Core.Graphics
{
    public class Particle
    {
        public bool castShadow;
        public bool diesToGravity;
        public float height;
        public float rotation;
        public float weight;
        public float degradeSpeed;
        public float dissapateSpeed;
        public Vector3 position;
        public Vector3 velocity;
        public Color color;
        public Color endColor;
        public int timeLeft;
        public int delay;
        public Vector2 scale;

        public Particle(Vector3 position, Vector3 velocity)
        {
            weight = 0.001f;
            timeLeft = 120;
            color = Color.White;
            endColor = Color.Black;
            this.position = position;
            this.velocity = velocity;
            scale = Vector2.One;
            degradeSpeed = 0.025f;
            dissapateSpeed = 0.02f;
            delay = 0;
        }

        public void SetDefaults()
        {
            weight = 0.001f;
            timeLeft = 120;
            color = Color.White;
            endColor = Color.Black;
            scale = Vector2.One;
            degradeSpeed = 0.025f;
            dissapateSpeed = 0.02f;
            delay = 0;
        }

        public virtual void Update()
        {
            if (delay > 0)
            {
                delay--;
                return;
            }
            this.position += this.velocity;
            this.velocity.Z += this.weight;
            if (this.position.Z > 0)
                this.position.Z = 0;

            color = Color.Lerp(color, endColor, degradeSpeed);
            color = color * (1.0f - dissapateSpeed);
            this.timeLeft--;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (delay > 0)
                return;

            Texture2D texture = Assets.Textures["Pixel"];
            Vector2 position = new Vector2(this.position.X, this.position.Y + this.position.Z);

            spriteBatch.Draw(texture, position, null, color, rotation, new Vector2(0.5f), scale, SpriteEffects.None, 1.0f);

            if (castShadow)
            {
                spriteBatch.Draw(texture, new Vector2(this.position.X, this.position.Y), null, Color.Black * 0.15f, rotation, new Vector2(0.5f), scale, SpriteEffects.None, 1.0f);
            }
        }
    }
}

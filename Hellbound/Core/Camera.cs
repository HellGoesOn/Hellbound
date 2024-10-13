using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core
{
    public class Camera
    {
        public Viewport view;
        public Vector2 centre;
        public Matrix transform;
        public float zoom;
        public float speed;
        public float rotation;
        // public Entity Following;

        public Camera(Viewport viewport)
        {
            speed = 0.1f;
            zoom = 1f;
            this.view = viewport;
        }

        /*public void SetFollowing(Entity e)
        {
            Following = e;
        }*/

        public void Update()
        {
            /*if (Following.Active && Following.World != null) {
                var pos = Following.GetComponent<TransformComponent>().Position;
                var finalPos = new Vector2((int)pos.X, (int)pos.Y);
                centre += (finalPos * zoom - centre) * speed;
            }*/

            transform = Matrix.CreateScale(new Vector3(zoom, zoom, 0))
                * Matrix.CreateTranslation(-centre.X, -centre.Y, 0)
                * Matrix.CreateRotationZ(rotation)
                * Matrix.CreateTranslation(new Vector3(view.Width * 0.5f, view.Height * 0.5f, 0f));

#if DEBUG
            /*
            if (Input.HeldKey(Keys.A))
                centre.X -= speed;
            if (Input.HeldKey(Keys.D))
                centre.X += speed;
            if (Input.HeldKey(Keys.W))
                centre.Y -= speed;
            if (Input.HeldKey(Keys.S))
                centre.Y += speed;
            if (Input.HeldKey(Keys.LeftShift))
                zoom += 0.02f;
            if (Input.HeldKey(Keys.Space))
                zoom -= 0.02f;
            if (Input.HeldKey(Keys.LeftControl))
                rotation += 0.02f;
            if (Input.HeldKey(Keys.LeftAlt))
                rotation -= 0.02f;*/

            if (zoom < 0.1f)
                zoom = 0.1f;
#endif
        }

        public Vector2 Position => centre - new Vector2(view.Width, view.Height) * 0.5f;
    }
}

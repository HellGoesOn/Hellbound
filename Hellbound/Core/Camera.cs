using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellTrail.Extensions;

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

        public Camera(Viewport viewport)
        {
            speed = 0.1f;
            zoom = 1f;
            this.view = viewport;
        }

        public void Update()
        {
            transform =
                Matrix.CreateTranslation(-(int)centre.X, -(int)centre.Y, 0)
                * Matrix.CreateRotationZ(rotation)
                * Matrix.CreateScale(new Vector3(zoom, zoom, 1))
                * Matrix.CreateTranslation(new Vector3(view.Width * 0.5f, view.Height * 0.5f, 0f));
        }

        public void Clamp(Vector2 min, Vector2 max)
        {
            Vector2 vec = new Vector2(view.Width, view.Height) / zoom;
            centre = Vector2.Clamp(centre, min+vec * 0.5f, max - vec*0.5f);
        }

        public Matrix Inverse => Matrix.Invert(transform);

        public Vector2 Position => centre - new Vector2(view.Width, view.Height) * 0.5f;
    }
}

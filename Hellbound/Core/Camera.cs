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

            transform =
                Matrix.CreateTranslation(-(int)MathF.Floor(centre.X), -(int)MathF.Floor(centre.Y), 0)
                * Matrix.CreateRotationZ(rotation)
                * Matrix.CreateScale(new Vector3(zoom, zoom, 1))
                * Matrix.CreateTranslation(new Vector3(view.Width * 0.5f, view.Height * 0.5f, 0f));
        }

        public void Clamp(Vector2 min, Vector2 max)
        {
            var topLeft = centre - new Vector2(view.Width, view.Height) * 0.5f;
            var bottomRight = centre + new Vector2(view.Width, view.Height) * 0.5f;

            var cameraTopLeftWorld = Vector2.Transform(topLeft, Inverse);
            var cameraBottomRightWorld = Vector2.Transform(bottomRight, Inverse);

            var width = cameraBottomRightWorld.X - cameraTopLeftWorld.X;
            var height = cameraBottomRightWorld.Y - cameraTopLeftWorld.Y;

            var bounds = new Rectangle((int)cameraTopLeftWorld.X, (int)cameraBottomRightWorld.Y, (int)width, (int)height);

            if (bounds.X < min.X) bounds.X = (int)min.X;
            if(bounds.Y < min.Y) bounds.Y = (int)min.Y;
            if(bounds.Right > max.Y) bounds.Y = (int)max.Y - bounds.Width;
            if (bounds.Bottom > max.Y) bounds.Y = (int)max.Y - bounds.Height;

            var boundsCenter = new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height) * 0.5f;
            var cameraCenterPosition = Vector2.Transform(boundsCenter, transform);
            centre = cameraCenterPosition;
        }

        public Matrix Inverse => Matrix.Invert(transform);

        public Vector2 Position => centre - new Vector2(view.Width, view.Height) * 0.5f;
    }
}

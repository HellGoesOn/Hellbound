using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core
{
    public class Camera
    {
        public Viewport view;
        public Vector2 centre;
        public Matrix transform;
        private float zoom;
        public float speed;
        public float rotation;

        private float zoomTarget;

        public Camera(Viewport viewport)
        {
            speed = 0.1f;
            Zoom = 1f;
            this.view = viewport;
        }

        public void Update()
        {
            if(zoom != zoomTarget) {
                zoom += speed * Math.Sign((zoomTarget - zoom));

                if(Math.Abs(zoomTarget - zoom) <= speed) {
                    zoom = zoomTarget;
                }
            }

            transform =
                Matrix.CreateTranslation(-(int)centre.X, -(int)centre.Y, 0)
                * Matrix.CreateRotationZ(rotation)
                * Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))
                * Matrix.CreateTranslation(new Vector3(view.Width * 0.5f, view.Height * 0.5f, 0f));
        }

        public void SetZoomTarget(float zoom)
        {
            zoomTarget = zoom;
        }

        public void Clamp(Vector2 min, Vector2 max)
        {
            Vector2 vec = new Vector2(view.Width, view.Height) / Zoom;
            centre = Vector2.Clamp(centre, min + vec * 0.5f, max - vec * 0.5f);
        }

        public Matrix Inverse => Matrix.Invert(transform);

        public Vector2 Position => centre - new Vector2(view.Width, view.Height) * 0.5f;

        public float Zoom { get => zoom; set => zoomTarget = zoom = value; }
    }
}

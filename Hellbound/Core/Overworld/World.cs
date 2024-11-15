using HellTrail.Core.Combat;
using HellTrail.Core.Combat.Scripting;
using HellTrail.Core.Combat.Status;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Overworld
{
    public class World : IGameState
    {
        public List<Trigger> triggers = [];

        public Texture2D mapTexture;

        public TileMap tileMap;

        public List<Unit> units = [];

        public World() 
        {
            tileMap = new TileMap(20, 20);
            GetCamera().centre = new Vector2(tileMap.width, tileMap.height) * 32 * 0.5f;
            units.AddRange(GlobalPlayer.ActiveParty);
        }

        public void Update()
        {
            foreach (var trigger in triggers)
            {
                trigger.TryRunScript(this);
            }

            foreach (Unit unit in units)
            {
                unit.UpdateVisuals();
            }

                triggers.RemoveAll(x => x.activated);

            Camera cam = GetCamera();
            if (Input.HeldKey(Keys.A))
                cam.centre.X -= cam.speed;
            if (Input.HeldKey(Keys.D))
                cam.centre.X += cam.speed;
            if (Input.HeldKey(Keys.W))
                cam.centre.Y -= cam.speed;
            if (Input.HeldKey(Keys.S))
                cam.centre.Y += cam.speed;
            /*
            if (Input.HeldKey(Keys.LeftShift))
                cam.zoom += 0.02f;
            if (Input.HeldKey(Keys.Space))
                cam.zoom -= 0.02f;
            if (Input.HeldKey(Keys.LeftControl))
                cam.rotation += 0.02f;
            if (Input.HeldKey(Keys.LeftAlt))
                cam.rotation -= 0.02f;*/
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            mapTexture = tileMap.texture;

            if (mapTexture != null)
            {
                spriteBatch.Draw(mapTexture, Vector2.Zero, Color.White);
            }

            foreach (Unit unit in units)
            {
                Color clr = unit.Downed ? Color.Crimson : Color.White;
                Vector2 position = unit.position + new Vector2(unit.shake * Main.rand.Next(2) % 2 == 0 ? 1 : -1, 0f);
                if (unit.animations.TryGetValue(unit.currentAnimation, out var anim))
                {
                    anim.position = position;
                    anim.Draw(spriteBatch, unit.scale);
                } else
                {
                    spriteBatch.Draw(Assets.Textures[unit.sprite], new Vector2((int)(position.X), (int)(position.Y)), null, clr * unit.opacity, 0f, new Vector2(16), unit.scale, SpriteEffects.None, unit.depth);
                    //Renderer.DrawRect(spriteBatch, unit.position-unit.size*0.5f, unit.size, 1, Color.Orange * 0.25f);
                }
            }
        }

        public Camera GetCamera() => CameraManager.overworldCamera;
    }

    public class Actor
    {
        public List<SpriteAnimation> animations = [];

        public Actor()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}

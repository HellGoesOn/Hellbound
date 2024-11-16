using HellTrail.Core.Combat;
using HellTrail.Core.Combat.Scripting;
using HellTrail.Core.Combat.Status;
using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
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
        public Context context;

        public Systems systems;

        public List<Trigger> triggers = [];

        public Texture2D mapTexture;

        public TileMap tileMap;

        public List<Unit> units = [];

        public World() 
        {
            systems = new Systems();
            context = new Context(1000);
            tileMap = new TileMap(20, 20);
            GetCamera().centre = new Vector2(tileMap.width, tileMap.height) * TileMap.TILE_SIZE * 0.5f;
            units.AddRange(GlobalPlayer.ActiveParty);

            systems.AddSystem(new DrawSystem());
            systems.AddSystem(new MoveSystem());
        }

        public void Update()
        {
            var debugText = UIManager.GetStateByName("debugState").GetElementById("debugText") as UIBorderedText;
            debugText.text = $"EC={context.entityCount}, CC={Context._maxComponents}";
            
            foreach (var trigger in triggers)
            {
                trigger.TryRunScript(this);
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

            systems.Execute(context);

            if (Input.HeldKey(Keys.Delete))
            {
                int id = context.LastActiveEntity;
                if(id >= 0)
                context.Destroy(id);
            }

            if (Input.LMBClicked)
            {
                int x = tileMap.width;
                int y = tileMap.height;
                tileMap.ChangeTile(1, (int)Input.MousePosition.X / TileMap.TILE_SIZE, (int)Input.MousePosition.Y / TileMap.TILE_SIZE);

            }

            if (Input.RMBClicked)
            {
                if(context.entityCount < context.entities.Length)
                {
                    var e = context.Create();
                    e.AddComponent(new TextureComponent("Slime3")
                    {
                        origin = new Vector2(16)
                    });
                    var xx = Main.rand.Next(-100, 101);
                    var yy = Main.rand.Next(-100, 101);
                    var off = new Vector2(xx, yy);
                    e.AddComponent(new Transform(new Vector2((int)Input.MousePosition.X, (int)Input.MousePosition.Y)));
                }    
            }

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

            systems.Draw(context, spriteBatch);
        }

        public Camera GetCamera() => CameraManager.overworldCamera;
    }
}

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

        public World() 
        {
            systems = new Systems();
            context = new Context(500);
            tileMap = new TileMap(30, 30);
            GetCamera().centre = new Vector2(tileMap.width, tileMap.height) * TileMap.TILE_SIZE * 0.5f;

            systems.AddSystem(new ObesitySystem(context));
            systems.AddSystem(new MoveSystem(context));
            systems.AddSystem(new DrawSystem(context));
            //systems.AddSystem(new DrawBoxSystem(context));
            systems.AddSystem(new MoveCameraSystem(context));
            systems.AddSystem(new ReadPlayerInputSystem(context));
            systems.AddSystem(new BoxCollisionSystem(context));
            GetCamera().speed = 0.12f;
        }

        public void Update()
        {
            systems.Execute(context);
            var debugText = UIManager.GetStateByName("debugState").GetElementById("debugText") as UIBorderedText;
            debugText.text = $"EC={context.entityCount}, CC={Context._maxComponents}\n";
            
            foreach (var trigger in triggers)
            {
                trigger.TryRunScript(this);
            }

            triggers.RemoveAll(x => x.activated);

            Camera cam = GetCamera();
            
            if (Input.HeldKey(Keys.A))
                cam.centre.X -= cam.speed * 16;
            if (Input.HeldKey(Keys.D))
                cam.centre.X += cam.speed * 16;
            if (Input.HeldKey(Keys.W))
                cam.centre.Y -= cam.speed * 16;
            if (Input.HeldKey(Keys.S))
                cam.centre.Y += cam.speed * 16;

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
                        origin = new Vector2(16),
                        scale = new Vector2(1)
                    });
                    var xx = Main.rand.Next(-100, 101);
                    var yy = Main.rand.Next(-100, 101);
                    var off = new Vector2(xx, yy);
                    e.AddComponent(new Transform(Input.MousePosition));
                    e.AddComponent(new ShawtyObese(-0.02f));
                    e.AddComponent(new Velocity(xx * 0.005f, yy * 0.005f));
                    e.AddComponent(new CollisionBox(32, 32)
                    {
                        onCollide = (me, other) =>
                        {
                            if (other.HasComponent<PlayerMarker>())
                            {
                                Vector2 pos = me.GetComponent<Transform>().position;

                                int x = Math.Max(0, (int)pos.X) / TileMap.TILE_SIZE;
                                int y = Math.Max(0, (int)pos.Y) / TileMap.TILE_SIZE;
                                Main.instance.StartBattle(tileMap[x, y] == 1);
                                me.GetComponent<CollisionBox>().onCollide = null;
                            }
                        }
                    });
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

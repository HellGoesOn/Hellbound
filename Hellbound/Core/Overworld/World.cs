using HellTrail.Core.Combat;
using HellTrail.Core.Combat.Scripting;
using HellTrail.Core.Combat.Status;
using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
using HellTrail.Extensions;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public World(int entityLimit = 1000, int width = 30, int height = 30) 
        {
            systems = new Systems();
            context = new Context(entityLimit);
            tileMap = new TileMap(width, height);
            GetCamera().centre = new Vector2(tileMap.width, tileMap.height) * TileMap.TILE_SIZE * 0.5f;

            systems.AddSystem(new DrawSystem(context));
            systems.AddSystem(new MoveSystem(context));
            systems.AddSystem(new ObesitySystem(context));
            systems.AddSystem(new DrawBoxSystem(context));
            systems.AddSystem(new BoxCollisionSystem(context));
            systems.AddSystem(new CreateBattleOnContactSystem(context));
            systems.AddSystem(new MoveCameraSystem(context));
            systems.AddSystem(new ReadPlayerInputSystem(context));
            systems.AddSystem(new ClearCollisionMarkerSystem(context));
            systems.AddSystem(new ParticleEmissionSystem(context));
            systems.AddSystem(new DrawTransformBoxSystem(context));
            GetCamera().speed = 0.12f;
        }

        public void Update()
        {
            systems.Execute(context);
            var debugText = UIManager.GetStateByName("debugState").GetElementById("debugText") as UIBorderedText;
            debugText.text = $"EC={context.entityCount}, UIE={UIManager.hoveredElement}\n";
            
            foreach (var trigger in triggers)
            {
                trigger.TryRunScript(this);
            }

            triggers.RemoveAll(x => x.activated);

            if (Input.RMBClicked)
            {
                if(context.entityCount < context.entities.Length)
                {
                    var entity = context.CopyFrom(Main.instance.prefabContext.entities[1]);
                    entity.AddComponent(new Transform(Input.MousePosition));
                    var xx = Main.rand.Next(-100, 101);
                    var yy = Main.rand.Next(-100, 101);
                    entity.AddComponent(new CollisionBox(6, 12, new Vector2(12, 6)));
                    entity.AddComponent(new ParticleEmitter
                        (1,
                        25,
                        -15,
                        15,
                        -60,
                        -10,
                        new Vector2(0.01f, 0.01f),
                        [Vector2.One * 2, Vector2.One],
                        [Color.Yellow, Color.Red, Color.Orange, Color.Crimson, Color.Black],
                        new Vector2(-1, -7),
                        new Vector2(2),
                        false));

                    entity = context.CopyFrom(Main.instance.prefabContext.entities[1]);
                    entity.RemoveComponent<TextureComponent>();
                    entity.AddComponent(new Transform(Input.MousePosition));
                    entity.AddComponent(new CollisionBox(6, 12, new Vector2(6)));
                    xx = Main.rand.Next(-100, 101);
                    yy = Main.rand.Next(-100, 101);
                    entity.AddComponent(new ParticleEmitter
                        (1,
                        15,
                        -15,
                        15,
                        -60,
                        -10,
                        new Vector2(0.01f, 0.01f),
                        [Vector2.One * 2, Vector2.One],
                        [Color.Yellow, Color.Red, Color.Orange, Color.Crimson, Color.Black],
                        new Vector2(-1, -7),
                        new Vector2(2),
                        true));
                    //entity.AddComponent(new TestComponent(13, [12, 14, 16], [Vector2.One, Vector2.Zero, Vector2.UnitX], 17));
                    //entity.AddComponent(new Velocity(xx * 0.005f, yy * 0.005f));
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

        public void SaveFile(string path)
        {
            string savePath = GameOptions.WorldDirectory;
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            for (int i = 0; i < tileMap.height; i++)
            {
                for (int j = 0; j < tileMap.width; j++)
                {
                    sb.Append(tileMap.tiles[j, i] + " ");
                }
                sb.Append("]");
                sb.AppendLine("");
                
                if(i != tileMap.height-1)
                    sb.Append("[ ");
            }

            sb.AppendLine();
            List<Entity> entities = context.entities.Where(x => x != null && x.enabled).ToList();
            for(int i = 0; i < entities.Count; i++)
            {
                sb.Append(Entity.Serialize(entities[i]));
                if (i != entities.Count - 1)
                    sb.Append(Environment.NewLine);
            }
            File.WriteAllText(savePath + $"\\{path}.scn", sb.ToString());
        }

        public void LoadFromFile(string path)
        {
            string finalPath = GameOptions.WorldDirectory + $"{path}.scn";

            string text = File.ReadAllText(finalPath);

            string tileText = Regex.Match(text, @$".*\]{Environment.NewLine}{Environment.NewLine}", RegexOptions.Singleline).Value;

            string[] strings = tileText.Split("\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            string[] numbers = Regex.Replace(strings[0], "[\\[\\]]", "").Trim().Split(" ");

            tileMap = new TileMap(numbers.Length, strings.Length);

            if(numbers.Length > tileMap.width)
                throw new Exception("Attempting to load map too big for the current world");

            for (int i = 0; i < strings.Length; i++)
            {
                numbers = Regex.Replace(strings[i], "[\\[\\]]", "").Trim().Split(" ");

                if (strings.Length > tileMap.height)
                    throw new Exception("Attempting to load map too big for the current world");

                for (int j = 0; j < numbers.Length; j++)
                {
                    tileMap[j, i] = int.Parse(numbers[j]);
                }
            }

            tileMap.BakeMap();

            context.Armaggedon();

            // TO-DO: move to different class

            if(!string.IsNullOrWhiteSpace(text.Substring(tileText.Length)))
            Entity.DeserializeAll(text.Substring(tileText.Length), context);
        }

        public Camera GetCamera() => CameraManager.overworldCamera;
    }
}

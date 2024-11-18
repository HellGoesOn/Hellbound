using HellTrail.Core.Combat;
using HellTrail.Core.Combat.Scripting;
using HellTrail.Core.Combat.Status;
using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
using HellTrail.Extensions;
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

        public World() 
        {
            systems = new Systems();
            context = new Context(500);
            tileMap = new TileMap(30, 30);
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
                    var entity = context.CopyFrom(Main.instance.prefabContext.entities[0]);
                    entity.AddComponent(new Transform(Input.MousePosition));
                    var xx = Main.rand.Next(-100, 101);
                    var yy = Main.rand.Next(-100, 101);
                    //entity.AddComponent(new Velocity(xx * 0.005f, yy * 0.005f));
                }    
            }

            if(Input.PressedKey(Keys.D9))
            {
                SaveTile("BaseScene");
            }
            if(Input.PressedKey(Keys.D8))
            {
                LoadFromFile("BaseScene");
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

        public void SaveTile(string path)
        {
            string savePath = Environment.CurrentDirectory +"\\Worlds";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            for (int i = 0; i < tileMap.width; i++)
            {
                for (int j = 0; j < tileMap.height; j++)
                {
                    sb.Append(tileMap.tiles[i, j] + " ");
                }
                sb.Append("]");
                sb.AppendLine("");
                
                if(i != tileMap.width-1)
                    sb.Append("[ ");
            }

            sb.AppendLine();
            List<Entity> entities = context.entities.Where(x => x != null && x.enabled).ToList();
            for(int i =0; i < entities.Count; i++)
            {
                Entity e = entities[i];
                sb.AppendLine($"Entity_{e.id}{Environment.NewLine}\t{{");
                foreach(IComponent c in e.GetAllComponents())
                {
                    sb.AppendLine($"\t\t{ComponentIO.SerializeComponent(c)}");
                }
                sb.Append($"\t}} ;{(i == entities.Count-1 ? "" : Environment.NewLine)}");
            }
            File.WriteAllText(savePath + $"\\{path}.scn", sb.ToString());
        }

        public void LoadFromFile(string path)
        {
            string finalPath = Environment.CurrentDirectory + $"\\Worlds\\{path}.scn";

            string text = File.ReadAllText(finalPath);

            string tileText = Regex.Match(text, @$".*\]{Environment.NewLine}{Environment.NewLine}", RegexOptions.Singleline).Value;

            string[] strings = tileText.Split("\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if(strings.Length > tileMap.width)
                throw new Exception("Attempting to load map too big for the current world");

            for (int i = 0; i < strings.Length; i++)
            {
                string[] numbers = Regex.Replace(strings[i], "[\\[\\]]", "").Trim().Split(" ");

                if (numbers.Length > tileMap.height)
                    throw new Exception("Attempting to load map too big for the current world");

                for (int j = 0; j < numbers.Length; j++)
                {
                    tileMap[i, j] = int.Parse(numbers[j]);
                }
            }

            tileMap.BakeMap();

            context.Armaggedon();

            string[] entities = Regex.Split(text.Substring(tileText.Length), @$" ;{Environment.NewLine}", RegexOptions.Singleline);
            for(int i = 0; i < entities.Length;i++)
            {
                string preSplit = Regex.Match(entities[i], @"{(.*)}", RegexOptions.Singleline).Groups[1].Value.Trim();
                string[] components = preSplit.Split($";{Environment.NewLine}");

                Entity e = context.Create();

                for (int componentIndex = 0; componentIndex < components.Length; componentIndex++)
                {
                    e.AddComponent(ComponentIO.DeserializeComponent(components[componentIndex]));
                }
            }

            bool shit = true;
        }

        public Camera GetCamera() => CameraManager.overworldCamera;
    }
}

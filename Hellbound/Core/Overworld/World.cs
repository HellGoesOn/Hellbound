﻿using HellTrail.Core.Combat;
using HellTrail.Core.Combat.Scripting;
using HellTrail.Core.Combat.Status;
using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
using HellTrail.Core.UI.Elements;
using HellTrail.Extensions;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HellTrail.Core.Overworld
{
    public class World : IGameState
    {
        public bool paused;

        public Context context;

        public Systems systems;

        public List<Trigger> triggers = [];

        public Texture2D mapTexture;

        //public TileMap tileMap;

        public TileMap tileMap;

        public World(int entityLimit = 1000, int width = 30, int height = 30) 
        {
            systems = new Systems();
            context = new Context(entityLimit);
            //tileMap = new TileMap(width, height);
            tileMap = new TileMap(width, height);
            GetCamera().centre = new Vector2(tileMap.width, tileMap.height) * DisplayTileLayer.TILE_SIZE * 0.5f;

            systems.AddSystem(new ReadPlayerInputSystem(context));
            systems.AddSystem(new MoveSystem(context));
            systems.AddSystem(new TileCollisionSystem(context));
            systems.AddSystem(new ObesitySystem(context));
            systems.AddSystem(new BoxCollisionSystem(context));
            systems.AddSystem(new CreateBattleOnContactSystem(context));
            systems.AddSystem(new ClearCollisionMarkerSystem(context));
            systems.AddSystem(new ParticleEmissionSystem(context));
            systems.AddSystem(new FollowerSystem(context));
            systems.AddSystem(new MoveCameraSystem(context));
            systems.AddSystem(new DrawBoxSystem(context));
            systems.AddSystem(new DrawSystem(context));
            systems.AddSystem(new DrawAnimationSystem(context));
            systems.AddSystem(new DrawTransformBoxSystem(context));
            GetCamera().speed = 0.1f;
        }

        public void Update()
        {
            if(!paused)
            systems.Execute(context);
            //var debugText = UIManager.GetStateByName("debugState").GetElementById("debugText") as UIBorderedText;
            //debugText.text = $"EC={context.entityCount}, UIE={UIManager.hoveredElement}\n";
            
            foreach (var trigger in triggers)
            {
                if(!paused)
                    trigger.TryRunScript(this);
            }

            triggers.RemoveAll(x => x.activated);

            if(Input.PressedKey(Keys.F7))
            {
                Stream stream = File.OpenWrite(GameOptions.WorldDirectory + "World.png");
                Renderer.MainTarget.SaveAsPng(stream, (int)GameOptions.ScreenWidth, (int)GameOptions.ScreenHeight);
                stream.Close();
                stream.Dispose();
            }

            //if(Input.LMBHeld)
            //{
            //    int x = (int)Input.MousePosition.X / NewTileMap.TILE_SIZE;
            //    int y = (int)Input.MousePosition.Y / NewTileMap.TILE_SIZE;
            //    tileMap.SetTile(NewerTileMap.TileDefinitions["DarkGrass"], x, y);
            //}

            //if (Input.RMBHeld)
            //{
            //    int x = (int)Input.MousePosition.X / NewTileMap.TILE_SIZE;
            //    int y = (int)Input.MousePosition.Y / NewTileMap.TILE_SIZE;
            //    tileMap.SetTile(NewerTileMap.TileDefinitions["Path"], x, y);
            //}

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
            //mapTexture = tileMap.texture;
            tileMap.Draw(spriteBatch);
            systems.Draw(context, spriteBatch);
            //if (mapTexture != null)
            //{
            //    //spriteBatch.Draw(mapTexture, Vector2.Zero, Color.White);
            //}
        }

        public void SaveFile(string path, string name)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + path))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + path);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[{context.entities.Length}]");
            sb.Append("[ ");
            for (int i = 0; i < tileMap.height; i++)
            {
                for (int j = 0; j < tileMap.width; j++)
                {
                    sb.Append(tileMap.GetTile(j, i) + " ");
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
            File.WriteAllText(Environment.CurrentDirectory + path + $"\\{name}.scn", sb.ToString());
        }

        public static World LoadFromFile(string path, string name)
        {
            string finalPath = Environment.CurrentDirectory + path + $"{name}.scn";

            string text = File.ReadAllText(finalPath);
            string entityCT = Regex.Match(text, @"\[.*\]").Value;
            int entityCount = int.Parse(Regex.Replace(entityCT, @"[\[\]]", ""));

            World world = new World(entityCount);
            string tileText = Regex.Match(text.Substring(entityCT.Length), @$".*\]{Environment.NewLine}{Environment.NewLine}", RegexOptions.Singleline).Value;

            string[] strings = tileText.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            string[] numbers = Regex.Replace(strings[0], "[\\[\\]]", "").Trim().Split(" ");

            world.tileMap = new TileMap(numbers.Length, strings.Length);

            if(numbers.Length > world.tileMap.width)
                throw new Exception("Attempting to load map too big for the current world");

            for (int i = 0; i < strings.Length; i++)
            {
                numbers = Regex.Replace(strings[i], "[\\[\\]]", "").Trim().Split(" ");

                if (strings.Length > world.tileMap.height)
                    throw new Exception("Attempting to load map too big for the current world");

                for (int j = 0; j < numbers.Length; j++)
                {
                    world.tileMap.SetTile(TileMap.GetById(int.Parse(numbers[j])), j, i);
                }
            }

            //world.newTileMap.BakeMap();

            world.context.Armaggedon();

            // TO-DO: move to different class

            if(!string.IsNullOrWhiteSpace(text.Substring(entityCT.Length + tileText.Length)))
                Entity.DeserializeAll(text.Substring(entityCT.Length + tileText.Length), world.context);

            return world;
        }

        public Camera GetCamera() => CameraManager.overworldCamera;
    }
}

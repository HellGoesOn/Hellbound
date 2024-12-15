using Casull.Core.ECS;
using Casull.Core.UI;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Text.RegularExpressions;

namespace Casull.Core.Overworld
{
    public partial class World : IGameState
    {
        public bool paused;

        public Context context;

        public Systems systems;

        public static List<Trigger> triggers = [];

        public static List<string> flags = [];

        public static List<Cutscene> cutscenes = [];

        public static void RaiseFlag(string flag)
        {
            if(!flags.Contains(flag))
                flags.Add(flag);
        }

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
            systems.AddSystem(new DDDSystem(context));
            systems.AddSystem(new ShitCollisionSystem(context));
            systems.AddSystem(new TripWireSystem(context));
            systems.AddSystem(new LoadingZoneSystem(context));
            systems.AddSystem(new CreateBattleOnContactSystem(context));
            systems.AddSystem(new ParticleEmissionSystem(context));
            systems.AddSystem(new FollowerSystem(context));
            systems.AddSystem(new MoveCameraSystem(context));
            systems.AddSystem(new DrawSystem(context));
            systems.AddSystem(new KillBasedOnFlagSystem(context));
            systems.AddSystem(new NewAnimationSystem(context));
            systems.AddSystem(new ClearCollisionMarkerSystem(context));

#if true

            systems.AddSystem(new DrawTransformBoxSystem(context));
            systems.AddSystem(new DrawBoxSystem(context));
#endif
            GetCamera().speed = 0.1f;
            GetCamera().Zoom = 4f;
        }

        public void Update()
        {

            if (!paused)
                systems.Execute(context);
            //var debugText = UIManager.GetStateByName("debugState").GetElementById("debugText") as UIBorderedText;
            //debugText.text = $"EC={context.entityCount}, UIE={UIManager.hoveredElement}\n";

            foreach (var trigger in triggers) {
                if (!paused)
                    trigger.TryRunScript(this);

                if (trigger.repeatadble && trigger.activated)
                    trigger.activated = false;
            }

            triggers.RemoveAll(x => x.activated && !x.repeatadble);

            if (Input.PressedKey(Keys.F7)) {
                Stream stream = File.OpenWrite(GameOptions.WorldDirectory + "World.png");
                Renderer.MainTarget.SaveAsPng(stream, (int)GameOptions.ScreenWidth, (int)GameOptions.ScreenHeight);
                stream.Close();
                stream.Dispose();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            tileMap.Draw(spriteBatch);
            systems.Draw(context, spriteBatch);
        }

        public static void StartCutscene(Cutscene cutscene)
        {
            UIManager.overworldUI.SetBlackBars(true);
            cutscenes.Add(cutscene);
        }

        public void SaveFile(string path, string name)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + path)) {
                Directory.CreateDirectory(Environment.CurrentDirectory + path);
            }

            StringBuilder sb = new();
            sb.AppendLine($"[{context.GetAllEntities().Length}]");
            sb.Append("[ ");
            for (int i = 0; i < tileMap.height; i++) {
                for (int j = 0; j < tileMap.width; j++) {
                    sb.Append(tileMap.GetTile(j, i) + " ");
                }
                sb.Append(']');
                sb.AppendLine("");

                if (i != tileMap.height - 1)
                    sb.Append("[ ");
            }

            sb.AppendLine("Elevation");

            sb.Append("[ ");
            for (int i = 0; i < tileMap.ElevationMap.GetLength(0); i++) {
                for (int j = 0; j < tileMap.ElevationMap.GetLength(1); j++) {
                    sb.Append(tileMap.GetTileElevation(j, i) + " ");
                }
                sb.Append(']');
                sb.AppendLine("");

                if (i != tileMap.height - 1)
                    sb.Append("[ ");
            }

            sb.AppendLine();

            List<Entity> entities = context.GetAllEntities().Where(x => x != null && x.enabled).ToList();
            for (int i = 0; i < entities.Count; i++) {
                sb.Append(Entity.Serialize(entities[i]));
                if (i != entities.Count - 1)
                    sb.Append(Environment.NewLine);
            }
            File.WriteAllText(Environment.CurrentDirectory + path + $"\\{name}.scn", sb.ToString());
        }

        public static World LoadFromFile(string path, string name)
        {
            string finalPath = Environment.CurrentDirectory + path + $"\\{name}.scn";

            string text = File.ReadAllText(finalPath);
            string entityCT = Regex.Match(text, @"\[.*\]").Value;
            int entityCount = int.Parse(Regex.Replace(entityCT, @"[\[\]]", ""));

            World world = new(entityCount);
            string tileText = Regex.Match(text[entityCT.Length..], @$"(.*)Elevation{Environment.NewLine}", RegexOptions.Singleline).Groups[1].Value;

            if (string.IsNullOrEmpty(tileText)) {
                tileText = Regex.Match(text[entityCT.Length..], @$".*\]{Environment.NewLine}{Environment.NewLine}", RegexOptions.Singleline).Value;
            }
            string tileElevationText = "";

            int dolbaebEbaniyNaRazrabeChtobOnSdohBlyat = $"Elevation{Environment.NewLine}".Length;

            string[] columnsElevation = [];
            if (text.Contains("Elevation")) {
                tileElevationText = Regex.Match(text[(entityCT.Length + tileText.Length + dolbaebEbaniyNaRazrabeChtobOnSdohBlyat)..], @$".*\]{Environment.NewLine}{Environment.NewLine}", RegexOptions.Singleline).Value;
                columnsElevation = tileElevationText.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            }
            string[] columns = tileText.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            string[] rows = Regex.Replace(columns[0], "[\\[\\]]", "").Trim().Split(" ");

            world.tileMap = new TileMap(rows.Length, columns.Length);

            if (rows.Length > world.tileMap.width)
                throw new Exception("Attempting to load map too big for the current world");

            for (int i = 0; i < columns.Length; i++) {
                rows = Regex.Replace(columns[i], "[\\[\\]]", "").Trim().Split(" ");

                if (columns.Length > world.tileMap.height)
                    throw new Exception("Attempting to load map too big for the current world");

                for (int j = 0; j < rows.Length; j++) {
                    world.tileMap.SetTile(TileMap.GetById(int.Parse(rows[j])), j, i);
                }
            }

            for (int i = 0; i < columnsElevation.Length; i++) {
                var rowsElevation = Regex.Replace(columnsElevation[i], "[\\[\\]]", "").Trim().Split(" ");

                for (int j = 0; j < rowsElevation.Length; j++) {
                    world.tileMap.SetTileElevation(int.Parse(rowsElevation[j]), j, i);
                }
            }

            //world.newTileMap.BakeMap();

            world.context.Armaggedon();

            // TO-DO: move to different class

            var entityDefinitions = text.Substring(entityCT.Length + tileText.Length + tileElevationText.Length + dolbaebEbaniyNaRazrabeChtobOnSdohBlyat);
            if (!string.IsNullOrWhiteSpace(entityDefinitions))
                Entity.DeserializeAll(entityDefinitions, world.context);

            Main.currentZone = name;

            return world;
        }

        public Camera GetCamera() => CameraManager.overworldCamera;

        public static Trigger AddTrigger(string Name)
        {
            var trigger = new Trigger(Name);
            triggers.Add(trigger);
            return trigger;
        }
    }
}

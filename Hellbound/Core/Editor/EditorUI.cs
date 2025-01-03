using Casull.Core.ECS;
using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Casull.Core.UI;
using Casull.Core.UI.Elements;
using Casull.Extensions;
using Casull.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text.RegularExpressions;

namespace Casull.Core.Editor
{
    public class EditorUI : UIState
    {
        public bool panelShown;
        public int selectedPanel;
        public int lastPanel;
        private int selectedTile = 1;
        private int selectedHeight = 0;
        public UIPanel[] panels;
        string currentScene = "EmptyScene";
        Vector2 camAnchor;

        private bool subscribedEvent;

        public const int PANEL_WIDTH = 800;
        public const int PANEL_HEIGHT = 300;

        Entity prefabEntity;
        public EditorUI(string zoneName = "")
        {
            panels = new UIPanel[6];
            for (int i = 0; i < panels.Length; i++) {
                panels[i] = new() {
                    size = new Vector2(PANEL_WIDTH, PANEL_HEIGHT),
                };

                panels[i].SetPosition(new Vector2(Renderer.UIPreferedWidth * 0.5f - PANEL_WIDTH * 0.5f, Renderer.UIPreferedHeight));

                UIPanel numPanel = new() {
                    id = $"numPanel{i}",
                    size = new Vector2(32),
                    capturesMouse = true
                };
                numPanel.SetPosition(new Vector2(Renderer.PreferedWidth - numPanel.size.X * 6 + numPanel.size.X * i, 0));

                numPanel.onClick = (sender) => {
                    lastPanel = selectedPanel;
                    selectedPanel = int.Parse(Regex.Replace(sender.id, "[^0-9]", ""));
                    TogglePanel();
                };

                UIBorderedText numText = new($"{i + 1}") {
                    origin = Assets.Arial.MeasureString($"{i + 1}") * 0.5f
                };
                numText.SetPosition(16);

                numPanel.Append(numText);
                Append(numPanel);

                Append(panels[i]);
            }

            UIBorderedText entityInspectorTitle = new("Entity Inspector") {
            };
            entityInspectorTitle.SetPosition(32, 12);
            UIBorderedText systemInspectorTitle = new("System Inspector") {
            };
            systemInspectorTitle.SetPosition(64, 12);

            panels[0].Append(entityInspectorTitle);

            panels[0].Append(new UICheckBox("Inspect Entities") {
                id = "startInspectEntities",
                drawsPanel = true,
                onClick = (sender) => {
                    if ((sender as UICheckBox).isChecked) {
                        Input.OnMousePressed += InspectEntity;
                    }
                    else {
                        Input.OnMousePressed -= InspectEntity;
                    }
                }

            }.SetPosition(16));

            UITextBox prefabPath = new();
            prefabPath.size.X = 240;
            prefabPath.maxCharacters = 500;
            prefabPath.myText = "\\Content\\Prefabs\\";
            prefabPath.SetFont(Assets.Arial);
            prefabPath.SetPosition(16, 60);

            UIBorderedText loadPrefabText = new("");

            UITextBox prefabName = new();
            prefabName.size.X = 240;
            prefabName.maxCharacters = 500;
            prefabName.myText = "Base";
            prefabName.SetFont(Assets.Arial);
            prefabName.SetPosition(256, 60);
            prefabName.onTextSubmit = (sender) => {
                try {
                    Input.OnMousePressed -= CreatePrefab;
                    Input.OnMouseHeld -= DragEntity;
                    Input.OnMouseReleased -= ReleaseEntity;
                    prefabEntity = EntitySaver.Load(prefabPath.myText, prefabName.myText);
                    Input.OnMousePressed += CreatePrefab;
                    if (prefabEntity != null) {
                        loadPrefabText.text = $"Loaded {prefabName.myText} prefab. Left Click to place. Right Click to deactivate";
                        loadPrefabText.color = Color.Lime;
                    }
                    else {
                        loadPrefabText.text = $"Entity prefab not found!";
                        loadPrefabText.color = Color.Crimson;
                    }
                }
                catch {
                    Input.OnMousePressed -= CreatePrefab;
                    Input.OnMouseHeld -= DragEntity;
                    Input.OnMouseReleased -= ReleaseEntity;
                    loadPrefabText.text = "ERROR: Prefab not found!";
                    loadPrefabText.color = Color.Red;
                }
            };


            loadPrefabText.SetPosition(16, 100);

            panels[0].Append(loadPrefabText);
            panels[0].Append(prefabName);
            panels[0].Append(prefabPath);

            panels[1].Append(systemInspectorTitle);

            World con = Main.instance.ActiveWorld;
            int off = 0;
            foreach (var system in con.systems.GetAll()) {
                var type = system.GetType();
                //if (system is IExecute && system is not IDraw)
                //{
                //    con.systems.ToggleSystem(type);
                //}

                UICheckBox box = new($"Toggle {type.Name}") {
                    isChecked = con.systems.IsEnabled(type)
                };
                box.SetPosition(16 + off, 48);
                box.color = box.isChecked ? Color.Lime : Color.Red;
                box.onClick = (sender) => {
                    var b = (sender as UICheckBox);
                    b.color = b.isChecked ? Color.Lime : Color.Red;
                    con.systems.ToggleSystem(type);
                };
                panels[1].Append(box);
                off += 18;
            }

            UICheckBox toggleAll = new("Toggle All") {
                drawsPanel = true,
                onClick = (sender) => {
                    foreach (UICheckBox checkBox in (sender.parent as UIElement).children.Where(x => x is UICheckBox).Cast<UICheckBox>()) {
                        if (checkBox == sender)
                            continue;

                        if (checkBox.id != "enableAll")
                            checkBox.Click();
                        else
                            checkBox.isChecked = false;
                    }
                }
            };
            toggleAll.SetPosition(new Vector2(30, 16));

            UICheckBox enableALl = new("Enable All") {
                id = "enableAll",
                drawsPanel = true,
                onClick = (sender) => {
                    foreach (UICheckBox checkBox in (sender.parent as UIElement).children.Where(x => x is UICheckBox).Cast<UICheckBox>()) {
                        if (checkBox == sender || checkBox == toggleAll)
                            continue;

                        if (!checkBox.isChecked)
                            checkBox.Click();
                    }
                }
            };
            enableALl.SetPosition(12, 16);

            panels[1].Append(toggleAll);
            panels[1].Append(enableALl);

            panels[2].Append(new UIBorderedText("Tile Placer") {
            }.SetPosition(32, 12));

            UITextBox tileName = new UITextBox() {
                myText = "Stone",
            maxCharacters = 255
            };
            tileName.size = new Vector2(200, 40);
            tileName.onTextSubmit = (sender) => {
                var myText = tileName.myText;
                var key = TileMap.TileDefinitions.Keys.FirstOrDefault(x => x.ToLower().Contains(tileName.myText.ToLower()));

                if (key != default) {
                    tileName.myText = key;
                    selectedTile = TileMap.TileDefinitions[key].id;
                }
                else {
                    tileName.myText = TileMap.TileDefinitions.Keys.Where(x => TileMap.TileDefinitions[x].id == selectedTile).FirstOrDefault();
                }


            };

            UITextBox tileElevationText = new UITextBox() {
                myText = "0",
                maxCharacters = 10
            };

            tileElevationText.size = new Vector2(40, 40);
            tileElevationText.onTextSubmit = (sender) => {

                if (int.TryParse(tileElevationText.myText, out var height)) {
                    selectedHeight = height;
                }
                else {
                    tileElevationText.myText = selectedHeight.ToString();
                }
            };

            tileName.SetPosition(32, 64);
            tileElevationText.SetPosition(32 + tileName.size.X + 4, 64);
            panels[2].Append(tileName);
            panels[2].Append(tileElevationText);

            UICheckBox isPlacingTiles = new("Activate tile placement") {
                panelColor = Color.DarkBlue,
                drawsPanel = true,
                onClick = (UIElement sender) => {
                    selectedTile = TileMap.TileDefinitions[tileName.myText].id;
                    var element = panels[2].GetElementById("boxPlace") as UICheckBox;
                    element.isChecked = false;
                    Input.OnMousePressed -= OnStartDrawingTiles;
                    Input.OnMouseReleased -= OnEndDrawingTiles;

                    if ((sender as UICheckBox).isChecked) {
                        Input.OnMouseHeld += PlaceTile;
                    }
                    else {
                        Input.OnMouseHeld -= PlaceTile;
                    }
                }
            };
            isPlacingTiles.SetPosition(32, 40);

            UICheckBox boxPlace = new("Activate box placement") {
                id = "boxPlace",
                panelColor = Color.DarkBlue,
                drawsPanel = true,
                onClick = (sender) => {
                    selectedTile = TileMap.TileDefinitions[tileName.myText].id;
                    isPlacingTiles.isChecked = false;
                    Input.OnMouseHeld -= PlaceTile;
                    if ((sender as UICheckBox).isChecked) {
                        Input.OnMousePressed += OnStartDrawingTiles;
                        Input.OnMouseReleased += OnEndDrawingTiles;
                    }
                    else {
                        Input.OnMousePressed -= OnStartDrawingTiles;
                        Input.OnMouseReleased -= OnEndDrawingTiles;
                    }
                }
            };
            boxPlace.SetPosition(32 + isPlacingTiles.size.X + 2, 40);

            UICheckBox showGrid = new("Show tile grid") {
                panelColor = Color.DarkBlue,
                drawsPanel = true,
                onClick = (sender) => {
                    Main.instance.ActiveWorld.tileMap.showGrid = !Main.instance.ActiveWorld.tileMap.showGrid;
                }
            };
            showGrid.SetPosition(32 + isPlacingTiles.size.X + 4 + boxPlace.size.X, 40);

            panels[2].Append(isPlacingTiles);
            panels[2].Append(showGrid);
            panels[2].Append(boxPlace);

            //LoadScenes();

            UITextBox scenePath = new();
            scenePath.size.X = 240;
            scenePath.maxCharacters = 500;
            scenePath.myText = "\\Content\\Scenes\\";
            scenePath.SetFont(Assets.Arial);
            scenePath.SetPosition(16, 60);

            UITextBox sceneName = new();
            sceneName.size.X = 240;
            sceneName.maxCharacters = 500;
            sceneName.myText = "BaseScene";
            sceneName.SetFont(Assets.Arial);
            sceneName.SetPosition(256, 60);

            var zoneNameText = new UIBorderedText("Zone Name:");
            zoneNameText.SetPosition(12, 12);
            panels[5].Append(zoneNameText);

            var textboxZoneNameBox = new UITextBox() {
                maxCharacters = 500,
                myText = zoneName,
            };
            textboxZoneNameBox.size.X = 200;
            textboxZoneNameBox.onTextSubmit = (sender) => {
                Main.instance.ActiveWorld.zoneName = textboxZoneNameBox.myText;
            };
            textboxZoneNameBox.SetPosition(12 + zoneNameText.size.X, 4);
            panels[5].Append(textboxZoneNameBox);

            var newText = new UIBorderedText("Load Scene") {
                size = Assets.Arial.MeasureString("Load File"),
                capturesMouse = true,
                onClick = (sender) => {
                    try {
                        currentScene = Path.GetFileNameWithoutExtension(sender.id);
                        Main.instance.ActiveWorld = World.LoadFromFile(scenePath.myText, sceneName.myText);
                    }
                    catch { }
                },
                onMouseEnter = (sender) => {
                    (sender as UIBorderedText).color = Color.Yellow;
                },
                onMouseLeave = (sender) => {
                    (sender as UIBorderedText).color = Color.White;
                }
            };
            panels[5].Append(new UIBorderedText("Save Scene") {
                capturesMouse = true,
                size = Assets.Arial.MeasureString("Save Scene"),
                onClick = (sender) => {
                    try {
                        World con = Main.instance.ActiveWorld;
                        con.SaveFile(scenePath.myText, sceneName.myText);
                    }
                    catch { }
                },
                onMouseEnter = (sender) => {
                    (sender as UIBorderedText).color = Color.Yellow;
                },
                onMouseLeave = (sender) => {
                    (sender as UIBorderedText).color = Color.White;
                }
            }.SetPosition(16, 160));

            newText.SetPosition(16, 200);

            panels[5].Append(scenePath);
            panels[5].Append(sceneName);
            panels[5].Append(newText);

            foreach (UIPanel panel in panels) {
                panel.SetFont(Assets.Arial);
            }
        }

        Vector2 tileBoxBasePosition = new Vector2(-1);
        private void OnStartDrawingTiles(MouseButton button)
        {
            if (button == MouseButton.Left && UIManager.hoveredElement == null) {
                tileBoxBasePosition = Input.MousePosition / DisplayTileLayer.TILE_SIZE;
                tileBoxBasePosition = tileBoxBasePosition.Floor();
            }
        }

        private void OnEndDrawingTiles(MouseButton button)
        {
            if (button == MouseButton.Left && UIManager.hoveredElement == null) {

                var tileMap = Main.instance.ActiveWorld.tileMap;
                var startPosition = tileBoxBasePosition;
                var endPosition = (Input.MousePosition / DisplayTileLayer.TILE_SIZE).Floor() + new Vector2(1) - startPosition;

                var realStart = Vector2.Clamp(Vector2.Min(startPosition, startPosition+endPosition), Vector2.Zero, new Vector2(tileMap.width, tileMap.height));
                var realEnd = Vector2.Clamp(Vector2.Max(startPosition, startPosition+endPosition), Vector2.Zero, new Vector2(tileMap.width, tileMap.height));

                for(int i = (int)realStart.Y; i < (int)realEnd.Y; i++) {
                    for (int j = (int)realStart.X; j < (int)realEnd.X; j++) {
                        tileMap.SetTile(TileMap.GetById(selectedTile), j, i);
                        tileMap.SetTileElevation(selectedHeight, j, i);
                    }
                }


                tileBoxBasePosition = new Vector2(-1);
            }
        }

        //private void LoadScenes()
        //{
        //    foreach (UIElement element in panels[5].children.Where(x => x.id.Contains(".scn")))
        //    {
        //        panels[5].DisownById(element.id);
        //    }

        //    string[] files = Directory.GetFiles(GameOptions.WorldDirectory, "*.scn", SearchOption.AllDirectories);
        //    for (int i = 0; i < files.Length; i++)
        //    {
        //        string fileNoExtension = Path.GetFileNameWithoutExtension(files[i]);
        //        var newText = new UIBorderedText(fileNoExtension)
        //        {
        //            id = files[i],
        //            size = Assets.Arial.MeasureString(fileNoExtension),
        //            capturesMouse = true,
        //            onClick = (sender) =>
        //            {
        //                currentScene = Path.GetFileNameWithoutExtension(sender.id);
        //                Main.instance.ActiveWorld = World.LoadFromFile("\\Content\\Scenes\\", Path.GetFileNameWithoutExtension(sender.id));
        //            }
        //        };

        //        newText.SetPosition(16 + (16 + (int)Assets.Arial.MeasureString($"BaseScene{i}").X) * (int)(i / 6), 48 + 32 * i - ((32 * 6) * (int)(i / 6)));

        //        panels[5].Append(newText);
        //    }
        //}

        public override void Update()
        {
            CheckSubscriptions();

            base.Update();
        }

        Entity draggedEntity;

        private void DragEntity(MouseButton button)
        {
            if (draggedEntity != null)
                draggedEntity.GetComponent<Transform>().position = Input.MousePosition;
        }

        private void ReleaseEntity(MouseButton button)
        {
            draggedEntity = null;
            Input.OnMouseReleased -= ReleaseEntity;
            Input.OnMouseHeld -= DragEntity;
        }

        private void CreatePrefab(MouseButton button)
        {
            if (UIManager.hoveredElement != null || prefabEntity == null)
                return;

            if (button == MouseButton.Left) {
                var entity = Main.instance.ActiveWorld.context.CopyFrom(prefabEntity);
                draggedEntity = entity;
                draggedEntity.GetComponent<Transform>().position = Input.MousePosition;
                Input.OnMouseHeld += DragEntity;
                Input.OnMouseReleased += ReleaseEntity;
            }

            if (button == MouseButton.Right) {
                Input.OnMousePressed -= CreatePrefab;
            }
        }

        public void CheckSubscriptions()
        {
            if (active && !subscribedEvent) {
                Input.OnKeyPressed += PressedKey;
                Input.OnMouseWheel += ScrollWheel;
                Input.OnMouseHeld += OnMiddleMouse;
                Input.OnMousePressed += AnchorCam;
                subscribedEvent = true;
            }
            else if (!active && subscribedEvent) {
                Input.OnMouseWheel -= ScrollWheel;
                Input.OnKeyPressed -= PressedKey;
                Input.OnMousePressed -= AnchorCam;
                Input.OnMouseHeld -= OnMiddleMouse;
                Input.OnMousePressed -= CreatePrefab;
                Input.OnMouseHeld -= DragEntity;
                Input.OnMouseReleased -= ReleaseEntity;
                Input.OnMousePressed -= PlaceTile;
                Input.OnMousePressed -= OnStartDrawingTiles;
                Input.OnMouseReleased -= OnEndDrawingTiles;
                subscribedEvent = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //Renderer.DrawBorderedString(spriteBatch, Assets.Arial, $"{camAnchor}", Input.UIMousePosition, Color.White, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);

            if(tileBoxBasePosition != new Vector2(-1)) {
                var startPosition = tileBoxBasePosition * DisplayTileLayer.TILE_SIZE;
                var endPosition = ((Input.MousePosition / DisplayTileLayer.TILE_SIZE).Floor() - tileBoxBasePosition) * DisplayTileLayer.TILE_SIZE;

                var size = new Vector2(32) + endPosition;
                Renderer.DrawRectToWorld(startPosition, size, Color.Red * 0.25f, float.MaxValue);
            }
        }

        private void AnchorCam(MouseButton button)
        {
            if (button == MouseButton.Middle) {
                camAnchor = Input.MousePosition;
            }
        }

        private void OnMiddleMouse(MouseButton button)
        {
            if (button == MouseButton.Middle) {
                IGameState con = Main.instance.GetGameState();
                Camera cam = con.GetCamera();
                cam.centre += (camAnchor - Input.MousePosition);
                cam.centre = cam.centre.ToInt();
            }
        }

        readonly Group<Entity> group = Main.instance.ActiveWorld.context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform)));
        private void InspectEntity(MouseButton button)
        {
            if (UIManager.hoveredElement != null)
                return;

            for (int i = 0; i < group.Count; i++) {
                Entity e = group[i];
                Transform transform = e.GetComponent<Transform>();
                var mpos = Input.MousePosition;
                if (mpos.X >= transform.position.X - 4 && mpos.X <= transform.position.X + 4
                    && mpos.Y >= transform.position.Y - 4 && mpos.Y <= transform.position.Y + 4) {

                    if (button == MouseButton.Left) {
                        var elem = new UIEntityInspectionPanel(e);
                        Append(elem);
                    }
                    if (button == MouseButton.Right) {
                        draggedEntity = e;
                        Input.OnMouseHeld += DragEntity;
                        Input.OnMouseReleased += ReleaseEntity;
                    }
                }
            }
        }

        private void PlaceTile(MouseButton button)
        {
            if (UIManager.hoveredElement != null)
                return;
            IGameState con = Main.instance.GetGameState();
            if (button == MouseButton.Left) {
                World w = (con as World);
                w.tileMap.SetTile(TileMap.GetById(selectedTile), (int)Input.MousePosition.X / DisplayTileLayer.TILE_SIZE, (int)Input.MousePosition.Y / DisplayTileLayer.TILE_SIZE);
                w.tileMap.SetTileElevation(selectedHeight, (int)Input.MousePosition.X / DisplayTileLayer.TILE_SIZE, (int)Input.MousePosition.Y / DisplayTileLayer.TILE_SIZE);
            }
        }

        private void ScrollWheel(int newValue, int oldValue)
        {
            IGameState con = Main.instance.ActiveWorld;
            if (newValue > oldValue) {
                //con.GetCamera().centre += con.GetCamera().centre * 0.1f;
                con.GetCamera().Zoom += 0.1f;
            }
            else {
                //con.GetCamera().centre -= con.GetCamera().centre * 0.1f;
                con.GetCamera().Zoom -= 0.1f;
            }
        }

        private void PressedKey(Keys key)
        {
            if (Input.isTyping)
                return;

            IGameState con = Main.instance.GetGameState();

            if (key == Keys.R) {
                con.GetCamera().Zoom = 4f;
            }

            if (Input.HeldKey(Keys.LeftControl) && key >= Keys.D1 && key <= Keys.D1 + panels.Length - 1) {
                lastPanel = selectedPanel;
                selectedPanel = Convert.ToInt32(Regex.Replace((key - 1).ToString(), "[^0-9]", ""));
                TogglePanel();
            }
        }

        private void TogglePanel()
        {
            panels[lastPanel].SetPosition(new Vector2(Renderer.UIPreferedWidth * 0.5f - PANEL_WIDTH * 0.5f, Renderer.UIPreferedHeight));
            panels[selectedPanel].SetPosition(panels[selectedPanel].GetPosition() - new Vector2(0, PANEL_HEIGHT));

            if (lastPanel == selectedPanel) {
                panelShown = !panelShown;

                if (!panelShown) {
                    panels[selectedPanel].SetPosition(new Vector2(Renderer.UIPreferedWidth * 0.5f - PANEL_WIDTH * 0.5f, Renderer.UIPreferedHeight));
                }
            }
        }
    }
}

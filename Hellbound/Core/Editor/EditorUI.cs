using HellTrail.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using HellTrail.Render;
using Microsoft.Xna.Framework.Input;
using System.Text.RegularExpressions;
using HellTrail.Core.Overworld;
using HellTrail.Core.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;
using HellTrail.Extensions;

namespace HellTrail.Core.Editor
{
    public class EditorUI : UIState
    {
        public bool panelShown;
        public int selectedPanel;
        public int lastPanel;
        private int selectedTile = 1;
        public UIPanel[] panels;
        string currentScene = "EmptyScene";
        Vector2 camAnchor;

        private bool subscribedEvent;

        public const int PANEL_WIDTH = 800;
        public const int PANEL_HEIGHT = 300;

        Entity prefabEntity;
        public EditorUI()
        {
            panels = new UIPanel[6];
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i] = new()
                {
                    size = new Vector2(PANEL_WIDTH, PANEL_HEIGHT),
                };

                panels[i].SetPosition(new Vector2(Renderer.UIPreferedWidth * 0.5f - PANEL_WIDTH * 0.5f, Renderer.UIPreferedHeight));

                UIPanel numPanel = new UIPanel()
                {
                    id = $"numPanel{i}",
                    size = new Vector2(32),
                    capturesMouse = true
                };
                numPanel.SetPosition(new Vector2(34 + 34 * i, Renderer.UIPreferedHeight - 34));

                numPanel.onClick = (sender) =>
                {
                    lastPanel = selectedPanel;
                    selectedPanel = int.Parse(Regex.Replace(sender.id, "[^0-9]", ""));
                    TogglePanel();
                };

                UIBorderedText numText = new UIBorderedText($"{i + 1}")
                {
                    origin = Assets.Arial.MeasureString($"{i + 1}") * 0.5f
                };
                numText.SetPosition(16);

                numPanel.Append(numText);
                Append(numPanel);

                Append(panels[i]);
            }

            UIBorderedText entityInspectorTitle = new UIBorderedText("Entity Inspector")
            {
            };
            entityInspectorTitle.SetPosition(32, 12);
            UIBorderedText systemInspectorTitle = new UIBorderedText("System Inspector")
            {
            };
            systemInspectorTitle.SetPosition(64, 12);

            panels[0].Append(entityInspectorTitle);

            panels[0].Append(new UICheckBox("Inspect Entities")
            {
                id = "startInspectEntities",
                drawsPanel = true,
                onClick = (sender) =>
                {
                    if((sender as UICheckBox).isChecked)
                    {
                        Input.OnMousePressed += InspectEntity;
                    }
                    else
                    {
                        Input.OnMousePressed -= InspectEntity;
                    }
                }

            }.SetPosition(16));

            UITextBox prefabPath = new UITextBox();
            prefabPath.size.X = 240;
            prefabPath.maxCharacters = 500;
            prefabPath.myText = "\\Content\\Prefabs\\";
            prefabPath.SetFont(Assets.Arial);
            prefabPath.SetPosition(16, 60);

            UIBorderedText loadPrefabText = new UIBorderedText("");

            UITextBox prefabName = new UITextBox();
            prefabName.size.X = 240;
            prefabName.maxCharacters = 500;
            prefabName.myText = "Base";
            prefabName.SetFont(Assets.Arial);
            prefabName.SetPosition(256, 60);
            prefabName.onTextSubmit = (sender) =>
            {
                try
                {
                    Input.OnMousePressed -= CreatePrefab;
                    Input.OnMouseHeld -= DragEntity;
                    Input.OnMouseReleased -= ReleaseEntity;
                    prefabEntity = EntitySaver.Load(prefabPath.myText, prefabName.myText);
                    Input.OnMousePressed += CreatePrefab;
                    loadPrefabText.text = $"Loaded {prefabName.myText} prefab. Left Click to place. Right Click to deactivate";
                    loadPrefabText.color = Color.Lime;
                }
                catch
                {
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
            foreach (var system in con.systems.GetAll())
            {
                var type = system.GetType();
                if (system is IExecute && system is not IDraw)
                {
                    con.systems.ToggleSystem(type);
                }

                UICheckBox box = new($"Toggle {type.Name}")
                {
                    isChecked = con.systems.IsEnabled(type)
                };
                box.SetPosition(16 + off, 48);
                box.color = box.isChecked ? Color.Lime : Color.Red;
                box.onClick = (sender) =>
                {
                    var b = (sender as UICheckBox);
                    b.color = b.isChecked ? Color.Lime : Color.Red;
                    con.systems.ToggleSystem(type);
                };
                panels[1].Append(box);
                off += 18;
            }

            UICheckBox toggleAll = new UICheckBox("Toggle All")
            {
                drawsPanel = true,
                onClick = (sender) =>
                {
                    foreach (UICheckBox checkBox in (sender.parent as UIElement).children.Where(x => x is UICheckBox))
                    {
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

            UICheckBox enableALl = new UICheckBox("Enable All")
            {
                id = "enableAll",
                drawsPanel = true,
                onClick = (sender) =>
                {
                    foreach (UICheckBox checkBox in (sender.parent as UIElement).children.Where(x => x is UICheckBox))
                    {
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

            panels[2].Append(new UIBorderedText("Tile Placer")
            {
            }.SetPosition(32, 12));

            UICheckBox isPlacingTiles = new UICheckBox("Activate tile placement")
            {
                panelColor = Color.DarkBlue,
                drawsPanel = true,
                onClick = (sender) =>
                {

                    if ((sender as UICheckBox).isChecked)
                    {
                        Input.OnMouseHeld += PlaceTile;
                    } else
                    {
                        Input.OnMouseHeld -= PlaceTile;
                    }
                }
            };
            isPlacingTiles.SetPosition(12, 16);

            for (int i = 0; i < NewerTileMap.TileDefinitions.Count; i++)
            {
                string text = NewerTileMap.TileDefinitions.Keys.ToList()[i];
                UIBorderedText tileText = new UIBorderedText(text)
                {
                    id = $"tileId={NewerTileMap.TileDefinitions[text].id}",
                    size = Assets.Arial.MeasureString(text),
                    onClick = (sender) =>
                    {
                        selectedTile = int.Parse(Regex.Replace(sender.id, "[^0-9]", ""));
                    },
                    capturesMouse = true
                };
                tileText.SetPosition(new Vector2(16, 12 + Assets.Arial.MeasureString(text).Y + Assets.Arial.MeasureString(text).Y * i));
                panels[2].Append(tileText);
            }
            panels[2].Append(isPlacingTiles);

            panels[5].Append(new UIBorderedText("Scenes")
            {
            }.SetPosition(16, 12));
            //LoadScenes();

            UITextBox scenePath = new UITextBox();
            scenePath.size.X = 240;
            scenePath.maxCharacters = 500;
            scenePath.myText = "\\Content\\Scenes\\";
            scenePath.SetFont(Assets.Arial);
            scenePath.SetPosition(16, 60);

            UITextBox sceneName = new UITextBox();
            sceneName.size.X = 240;
            sceneName.maxCharacters = 500;
            sceneName.myText = "BaseScene";
            sceneName.SetFont(Assets.Arial);
            sceneName.SetPosition(256, 60);

            var newText = new UIBorderedText("Load Scene")
            {
                size = Assets.Arial.MeasureString("Load File"),
                capturesMouse = true,
                onClick = (sender) =>
                {
                    try
                    {
                        currentScene = Path.GetFileNameWithoutExtension(sender.id);
                        Main.instance.ActiveWorld = World.LoadFromFile(scenePath.myText, sceneName.myText);
                    }
                    catch { }
                },
                onMouseEnter = (sender) =>
                {
                    (sender as UIBorderedText).color = Color.Yellow;
                },
                onMouseLeave = (sender) =>
                {
                    (sender as UIBorderedText).color = Color.White;
                }
            };
            panels[5].Append(new UIBorderedText("Save Scene")
            {
                capturesMouse = true,
                size = Assets.Arial.MeasureString("Save Scene"),
                onClick = (sender) =>
                {
                    try
                    {
                        World con = Main.instance.ActiveWorld;
                        con.SaveFile(scenePath.myText, sceneName.myText);
                    }
                    catch { }
                },
                onMouseEnter = (sender) =>
                {
                    (sender as UIBorderedText).color = Color.Yellow;
                },
                onMouseLeave = (sender) =>
                {
                    (sender as UIBorderedText).color = Color.White;
                }
            }.SetPosition(16, 160));

            newText.SetPosition(16, 200);

            panels[5].Append(scenePath);
            panels[5].Append(sceneName);
            panels[5].Append(newText);

            foreach (UIPanel panel in panels)
            {
                panel.SetFont(Assets.Arial);
            }
        }

        private void LoadScenes()
        {
            foreach (UIElement element in panels[5].children.Where(x => x.id.Contains(".scn")))
            {
                panels[5].DisownById(element.id);
            }

            string[] files = Directory.GetFiles(GameOptions.WorldDirectory, "*.scn", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string fileNoExtension = Path.GetFileNameWithoutExtension(files[i]);
                var newText = new UIBorderedText(fileNoExtension)
                {
                    id = files[i],
                    size = Assets.Arial.MeasureString(fileNoExtension),
                    capturesMouse = true,
                    onClick = (sender) =>
                    {
                        currentScene = Path.GetFileNameWithoutExtension(sender.id);
                        Main.instance.ActiveWorld = World.LoadFromFile("\\Content\\Scenes\\", Path.GetFileNameWithoutExtension(sender.id));
                    }
                };

                newText.SetPosition(16 + (16 + (int)Assets.Arial.MeasureString($"BaseScene{i}").X) * (int)(i / 6), 48 + 32 * i - ((32 * 6) * (int)(i / 6)));

                panels[5].Append(newText);
            }
        }

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
            if (UIManager.hoveredElement != null)
                return;

            if (button == MouseButton.Left)
            {
                var entity = Main.instance.ActiveWorld.context.CopyFrom(prefabEntity);
                draggedEntity = entity;
                draggedEntity.GetComponent<Transform>().position = Input.MousePosition;
                Input.OnMouseHeld += DragEntity;
                Input.OnMouseReleased += ReleaseEntity;
            }

            if (button == MouseButton.Right)
            {
                Input.OnMousePressed -= CreatePrefab;
            }
        }

        public void CheckSubscriptions()
        {
            if (active && !subscribedEvent)
            {
                Input.OnKeyPressed += PressedKey;
                Input.OnMouseWheel += ScrollWheel;
                Input.OnMouseHeld += OnMiddleMouse;
                Input.OnMousePressed += AnchorCam;
                subscribedEvent = true;
            } else if (!active && subscribedEvent)
            {
                Input.OnMouseWheel -= ScrollWheel;
                Input.OnKeyPressed -= PressedKey;
                Input.OnMousePressed -= AnchorCam;
                Input.OnMouseHeld -= OnMiddleMouse;
                Input.OnMousePressed -= CreatePrefab; 
                Input.OnMouseHeld -= DragEntity;
                Input.OnMouseReleased -= ReleaseEntity;
                subscribedEvent = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //Renderer.DrawBorderedString(spriteBatch, Assets.Arial, $"{camAnchor}", Input.UIMousePosition, Color.White, Color.Black, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
        }

        private void AnchorCam(MouseButton button)
        {
            if(button == MouseButton.Middle)
            {
                IGameState con = Main.instance.GetGameState();
                Camera cam = con.GetCamera();
                camAnchor = Input.MousePosition;
            }
        }

        private void OnMiddleMouse(MouseButton button)
        {
            if(button == MouseButton.Middle)
            {
                IGameState con = Main.instance.GetGameState();
                Camera cam = con.GetCamera();
                cam.centre += (camAnchor-Input.MousePosition)*cam.zoom;
                cam.centre = cam.centre.ToInt();
            }
        }

        List<UIElement> _elementsToDisown = [];

        Group<Entity> group = Main.instance.ActiveWorld.context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform)));
        private void InspectEntity(MouseButton button)
        {
            if (UIManager.hoveredElement != null)
                return;
            
            for(int i = 0; i < group.Count; i++)
            {
                Entity e = group[i];
                Transform transform = e.GetComponent<Transform>();
                var mpos = Input.MousePosition;
                if (mpos.X >= transform.position.X - 4 && mpos.X <= transform.position.X + 4
                    && mpos.Y >= transform.position.Y - 4 && mpos.Y <= transform.position.Y + 4)
                {

                    if (button == MouseButton.Left)
                    {
                        var elem = new UIEntityInspectionPanel(e);
                        Append(elem);
                    }
                    if (button == MouseButton.Right)
                    {
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
            if (button == MouseButton.Left)
            {
                World w = (con as World);
                int x = w.tileMap.width;
                int y = w.tileMap.height;
                w.tileMap.SetTile(NewerTileMap.GetById(selectedTile), (int)Input.MousePosition.X / DisplayTileLayer.TILE_SIZE, (int)Input.MousePosition.Y / DisplayTileLayer.TILE_SIZE);
            }
        }

        private void ScrollWheel(int newValue, int oldValue)
        {
            IGameState con = Main.instance.ActiveWorld;
            if (newValue > oldValue)
            {
                con.GetCamera().centre += con.GetCamera().centre * 0.1f;
                con.GetCamera().zoom += 0.1f;
            }
            else
            {
                con.GetCamera().centre -= con.GetCamera().centre * 0.1f;
                con.GetCamera().zoom -= 0.1f;
            }
        }

        private void PressedKey(Keys key)
        {
            if (Input.isTyping)
                return;

            IGameState con = Main.instance.GetGameState();

            if (key == Keys.R)
            {
                con.GetCamera().zoom = 1f;
            }

            if (Input.HeldKey(Keys.LeftControl) && key >= Keys.D1 && key <= Keys.D1 + panels.Length-1)
            {
                lastPanel = selectedPanel;
                selectedPanel = Convert.ToInt32(Regex.Replace((key - 1).ToString(), "[^0-9]", ""));
                TogglePanel();
            }
        }

        private void TogglePanel()
        {
            panels[lastPanel].SetPosition(new Vector2(Renderer.UIPreferedWidth * 0.5f - PANEL_WIDTH * 0.5f, Renderer.UIPreferedHeight));
            panels[selectedPanel].SetPosition(panels[selectedPanel].GetPosition() - new Vector2(0, PANEL_HEIGHT));

            if (lastPanel == selectedPanel)
            {
                panelShown = !panelShown;

                if (!panelShown)
                {
                    panels[selectedPanel].SetPosition(new Vector2(Renderer.UIPreferedWidth * 0.5f - PANEL_WIDTH * 0.5f, Renderer.UIPreferedHeight));
                }
            }
        }
    }
}

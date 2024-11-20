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

namespace HellTrail.Core.Editor
{
    public class EditorUI : UIState
    {
        public bool panelShown;
        public int selectedPanel;
        public int lastPanel;
        private int selectedTile;
        public UIPanel[] panels;

        Entity inspectedEntity;
        string currentScene = "";
        Vector2 camAnchor;

        private bool subscribedEvent;

        public const int PANEL_WIDTH = 800;
        public const int PANEL_HEIGHT = 300;

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

            panels[0].Append(new UIBorderedText("")
            {
                id = "successful",
            }.SetPosition(96 + Assets.Arial.MeasureString("Inspect Entities").X + Assets.Arial.MeasureString("Apply changes").X, 16));

            panels[0].Append(new UIBorderedText("Apply changes")
            {
                id = "trySerialize",
                size = Assets.Arial.MeasureString("Apply changes"),
                capturesMouse = true,
                onClick = (sender) =>
                {
                    try
                    {
                        Context con = Main.instance.activeWorld.context;
                        var entityText = (panels[0].GetElementById("inspectedEntity") as UIBorderedText);
                        string[] text = entityText.text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < text.Length; i++) 
                        {
                            inspectedEntity.AddComponent(ComponentIO.DeserializeComponent(text[i]));
                        }

                        if (inspectedEntity != null)
                        {
                            var yes = (panels[0].GetElementById("successful") as UIBorderedText);
                            yes.text = "Successful!";
                            yes.color = Color.Lime;
                        }
                    }
                    catch
                    {
                        var no = (panels[0].GetElementById("successful") as UIBorderedText);
                        no.text = "ERROR";
                        no.color = Color.Red;
                    }
                }
            }.SetPosition(48 + Assets.Arial.MeasureString("Inspect Entities").X, 12));

            panels[1].Append(systemInspectorTitle);

            World con = Main.instance.activeWorld;
            int off = 0;
            foreach (var system in con.systems.GetAll())
            {
                var type = system.GetType();
                if (system is IExecute)
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

            for (int i = 0; i < TileMap.Tiles.Count; i++)
            {
                string text = TileMap.Tiles.Keys.ToList()[i];
                UIBorderedText tileText = new UIBorderedText(text)
                {
                    id = $"tileId={i}",
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

            panels[5].Append(new UIBorderedText("Save Scene")
            {
                capturesMouse = true,
                size = Assets.Arial.MeasureString("Save Scene"),
                onClick = (sender) =>
                {
                    if (!string.IsNullOrEmpty(currentScene))
                    {
                        World con = Main.instance.activeWorld;
                        con.SaveFile(currentScene);
                        LoadScenes();
                    }
                }
            }.SetPosition(16, panels[5].size.Y - 32));

            panels[5].Append(new UIBorderedText("Save as New Scene")
            {
                capturesMouse = true,
                size = Assets.Arial.MeasureString("Save as New Scene"),
                onClick = (sender) =>
                {
                    if (!string.IsNullOrEmpty(currentScene))
                    {
                        int count = (sender.parent as UIElement).children.Count(x => x.id.Contains(".scn"));
                        World con = Main.instance.activeWorld;
                        con.SaveFile($"NewScene{count}");
                        LoadScenes();
                    }
                }
            }.SetPosition(48 + Assets.Arial.MeasureString("Save Scene").X, panels[5].size.Y - 32));
            LoadScenes();

            foreach(UIPanel panel in panels)
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
                        Main.instance.activeWorld.LoadFromFile(Path.GetFileNameWithoutExtension(sender.id));
                    }
                };

                newText.SetPosition(16 + (16 + (int)Assets.Arial.MeasureString($"BaseScene{i}").X) * (int)(i / 6), 48 + 32 * i - ((32 * 6) * (int)(i / 6)));

                panels[5].Append(newText);
            }
        }

        public override void Update()
        {
            if(active && !subscribedEvent)
            {
                Input.OnKeyPressed += PressedKey;
                Input.OnMouseWheel += ScrollWheel;
                Input.OnMouseHeld += OnMiddleMouse;
                Input.OnMousePressed += AnchorCam;
                subscribedEvent = true;
            } else if(!active && subscribedEvent)
            {
                Input.OnMouseWheel -= ScrollWheel;
                Input.OnKeyPressed -= PressedKey;
                Input.OnMousePressed -= AnchorCam;
                subscribedEvent = false;
            }

            base.Update();
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
            }
        }

        List<UIElement> _elementsToDisown = [];

        Group<Entity> group = Main.instance.activeWorld.context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform)));
        private void InspectEntity(MouseButton button)
        {
            if (button != MouseButton.Left || UIManager.hoveredElement != null)
                return;
            
            for(int i = 0; i < group.Count; i++)
            {
                Entity e = group[i];
                Transform transform = e.GetComponent<Transform>();
                var mpos = Input.MousePosition;
                if (mpos.X >= transform.position.X - 4 && mpos.X <= transform.position.X + 4
                    && mpos.Y >= transform.position.Y - 4 && mpos.Y <= transform.position.Y + 4)
                {
                    inspectedEntity = e;
                    
                    Append(new UIEntityInspectionPanel(e));
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
                w.tileMap.ChangeTile(selectedTile, (int)Input.MousePosition.X / TileMap.TILE_SIZE, (int)Input.MousePosition.Y / TileMap.TILE_SIZE);
            }
        }

        private void ScrollWheel(int newValue, int oldValue)
        {
            IGameState con = Main.instance.activeWorld;
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

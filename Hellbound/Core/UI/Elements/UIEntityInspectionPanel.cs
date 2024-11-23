using HellTrail.Core.ECS;
using HellTrail.Core.ECS.Components;
using HellTrail.Extensions;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.Elements
{
    public class UIEntityInspectionPanel : UIElement
    {
        Entity _inspectedEnity;
        UIPanel panel;
        UIComponentChanger changer;
        UIComponentSelector componentSelector;
        public UIEntityInspectionPanel(Entity entity)
        {
            font = Assets.Arial;
            _inspectedEnity = entity;
            entity.OnDestroy += (_) =>
            {
                if(parent != null)
                    parent.Disown(this);
                _inspectedEnity = null;
                entity.OnComponentAdded -= UpdateEntity;
                entity.OnComponentRemoved -= UpdateEntity;
            };

            entity.OnComponentAdded += UpdateEntity;
            entity.OnComponentRemoved += UpdateEntity;

            components = _inspectedEnity.GetAllComponents();
            texts = new UIText[components.Length]; 
            onLoseParent = (_) =>
            {
                entity.OnComponentAdded -= UpdateEntity;
                entity.OnComponentRemoved -= UpdateEntity;
                _inspectedEnity = null;
            };

            UIPanel draggablePanel = new UIDraggablePanel()
            {
                id = "inspectedEntityPanel",
                font = Assets.Arial,
                size = new Vector2(600, 32)
            };
            draggablePanel.SetPosition(new Vector2(64, Renderer.UIPreferedHeight * 0.5f) - new Vector2(0, 300));
            var entityText = new UIText($"Entity_{_inspectedEnity.id}")
            {
                id="entityText"
            };
            entityText.SetPosition(96, 6);
            draggablePanel.Append(entityText);

            panel = new UIPanel()
            {
                fillColor = Color.DarkSlateGray,
                size = new Vector2(600, 600)
            };
            panel.SetPosition(0, 34);

            UpdateEntity(_inspectedEnity, null);

            UIWindowButton btn = new(WindowButtonType.XMark, "Close", Color.Red)
            {
                scale = Vector2.One * 2,
                onClick = (sender) =>
                {
                    parent.Disown(this);
                }
            };
            btn.SetPosition(600 - 32, 0);

            UIWindowButton btnAll = new(WindowButtonType.XMark, "Close All ", Color.Orange)
            {
                scale = Vector2.One * 2,
                onClick = (sender) =>
                {
                    foreach (UIElement element in parent.Children)
                    {
                        if(element is UIEntityInspectionPanel && element.parent != null)
                        {
                            element.parent.Disown(element);
                        }
                    }
                }
            };
            btnAll.SetPosition(600 - 64, 0);

            UIWindowButton killEntity = new UIWindowButton(WindowButtonType.XMark, "Destroy entity", Color.Crimson)
            {
                drawsPanel = true,
                scale = Vector2.One * 2,
                onClick = (sender) =>
                {
                    Main.instance.ActiveWorld.context.Destroy(_inspectedEnity);
                }
            };
            killEntity.SetPosition(0, 0);

            UIWindowButton addComponent = new UIWindowButton(WindowButtonType.Wrench, "Add Component")
            {
                drawsPanel = true,
                scale = Vector2.One * 2,
                onClick = (sender) =>
                {
                    if(componentSelector != null)
                        Disown(componentSelector);
                    Append(componentSelector = new UIComponentSelector(entity));
                    componentSelector.SetPosition(draggablePanel.GetPosition() + new Vector2(68, 40));
                }
            };
            addComponent.SetPosition(32, 0);

            UIWindowButton saveEntity = new UIWindowButton(WindowButtonType.CheckMark, "Save to prefab")
            {
                color = Color.Lime,
                drawsPanel = true,
                scale = Vector2.One * 2,
                onClick = (sender) =>
                {
                    Append(new UISaveFileElement<Entity>(new EntitySaver(_inspectedEnity), "\\Content\\Prefabs\\"));
                }
            };
            saveEntity.SetPosition(64, 0);


            draggablePanel.Append(panel);
            draggablePanel.Append(btn);
            draggablePanel.Append(btnAll);
            draggablePanel.Append(killEntity);
            draggablePanel.Append(addComponent);
            draggablePanel.Append(saveEntity);
            this.Append(draggablePanel);
        }

        IComponent[] components;
        UIText[] texts;

        public void UpdateEntity(Entity e, IComponent component)
        {
            foreach (UIText text in texts)
            {
                panel.Disown(text);
            }

            components = e.GetAllComponents();

            texts = new UIText[components.Length];

            Vector2 accumulatedOffset = Vector2.Zero;
            for (int i = 0; i < components.Length; i++)
            {
                texts[i] = new UIText(ComponentIO.SerializeComponent(components[i]), 40)
                {
                    font = Assets.Arial
                };

                panel.Append(texts[i]);
                UIWindowButton btn = new UIWindowButton(WindowButtonType.XMark, " Delete", Color.Red)
                {
                    drawsPanel = true,
                    id = $"{i}"
                };

                UIWindowButton btnChange = new UIWindowButton(WindowButtonType.Wrench, " Change component", Color.Yellow)
                {
                    drawsPanel = true,
                    id = $"{i}"
                };
                Vector2 measurement = font.MeasureString(components[i].GetType().Name);

                Vector2 off = Vector2.Zero;

                if (i > 0)
                {
                    accumulatedOffset.Y += texts[i-1].size.Y;
                }

                Vector2 pos = new Vector2(34, accumulatedOffset.Y + 4);
                texts[i].SetPosition(pos);
                btn.SetPosition(-32, 2);
                btnChange.SetPosition(-16, 2);
                btnChange.onClick = (sender) =>
                {
                    if (changer != null)
                        Disown(changer);

                    changer = new UIComponentChanger(components[int.Parse(sender.id)], sender.GetPosition() + new Vector2(68, 0));
                    changer.SetFont(Assets.Arial);
                    Append(changer);
                };
                btn.onClick = (sender) =>
                {
                    if(_inspectedEnity != null)
                        _inspectedEnity.RemoveComponent(components[int.Parse(sender.id)].GetType());
                };
                    
                texts[i].Append(btnChange);
                if (components[i] is not Transform)
                    texts[i].Append(btn);
            }

            panel.size.Y = accumulatedOffset.Y + texts[texts.Length-1].size.Y + 12;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            for (int i = 0; i < components.Length; i++)
            {
                texts[i].text = ComponentIO.SerializeComponent(components[i]);
            }

            //(UIManager.GetStateByName("debugState").GetElementById("debugText") as UIBorderedText).text = $"Children:{this.children.Count}, Components:{components.Length}";
        }
    }
}

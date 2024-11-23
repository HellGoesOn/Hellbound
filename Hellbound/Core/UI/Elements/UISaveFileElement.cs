using HellTrail.Core.ECS;
using HellTrail.Core.Editor;
using HellTrail.Core.Overworld;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.UI.Elements
{
    public class UISaveFileElement<T> : UIElement
    {
        IFileHandler<T> fileHandler;
        public UISaveFileElement(IFileHandler<T> handler, string defaultPath)
        {
            onLoseParent = (sender) =>
            {
                fileHandler = null;
            };
            font = Assets.Arial;
            fileHandler = handler;
            UITextBox fileName = new UITextBox();
            fileName.size.X = 280;
            fileName.clearOnBeginTyping = true;
            fileName.myText = "Enter file name";
            fileName.maxCharacters = 500;

            UITextBox filePath = new UITextBox();
            filePath.size.X = 280;
            filePath.myText = defaultPath;
            filePath.maxCharacters = 500;

            UIPanel panel = new UIPanel()
            {
                size = new Vector2(300, 200)
            };
            panel.Append(fileName);

            UIBorderedText accept = new UIBorderedText("Save")
            {
                capturesMouse = true,
                size = font.MeasureString("Save"),
                onMouseEnter = (sender) =>
                {
                    (sender as UIBorderedText).color = Color.Yellow;
                },
                onMouseLeave = (sender) =>
                {
                    (sender as UIBorderedText).color = Color.White;
                }

            };
            UIBorderedText cancel = new UIBorderedText("Cancel")
            {
                capturesMouse = true,
                size = font.MeasureString("Cancel"),
                onMouseEnter = (sender) =>
                {
                    (sender as UIBorderedText).color = Color.Yellow;
                },
                onMouseLeave = (sender) =>
                {
                    (sender as UIBorderedText).color = Color.White;
                }
            };

            accept.SetPosition(16, panel.size.Y - 16 - accept.size.Y);
            accept.onClick = (sender) =>
            {
                fileHandler.Save(filePath.myText, fileName.myText);
                parent.Disown(this);
            };
            cancel.SetPosition(panel.size.X - 16 - cancel.size.X, panel.size.Y - 16 - cancel.size.Y);
            cancel.onClick = (sender) =>
            {
                parent.Disown(this);
            };

            filePath.SetPosition(10, 10);
            fileName.SetPosition(10, filePath.GetPosition().Y + filePath.size.Y + 4);
            Append(panel);

            panel.Append(cancel);
            panel.Append(accept);
            panel.Append(filePath);

            SetPosition(Renderer.UIPreferedWidth * 0.5f - 150, Renderer.UIPreferedHeight * 0.5f - 50);
            SetFont(font);
        }
    }

    public interface IFileHandler<T>
    {
        void Save(string path, string name);
        static abstract T Load(string path, string name);
    }

    public class WorldSaver : IFileHandler<World>
    {
        World world;
        public WorldSaver(World world) { this.world = world; }

        public void Save(string path, string name)
        {
            world.SaveFile(path, name);
        }

        public static World Load(string path, string name)
        {
            return World.LoadFromFile(path, name);
        }
    }

    public class EntitySaver : IFileHandler<Entity>
    {
        Entity e;
        public EntitySaver(Entity e)
        {
            this.e = e;
        }

        public void Save(string path, string name)
        {
            if(!Directory.Exists(Environment.CurrentDirectory + path))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + path);
            }

            File.WriteAllText(Environment.CurrentDirectory + path + name + ".fab", Entity.Serialize(e));
        }

        public static Entity Load(string path, string name)
        {
            return Entity.Deserialize(File.ReadAllText(Environment.CurrentDirectory + path + name + ".fab"));
        }
    }
}

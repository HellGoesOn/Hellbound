using HellTrail.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public class Menu
    {
        private int oldOption;
        public int selectedOption;
        public int sideStep = 1;
        public bool active;
        public bool visible = true;

        public bool allowMouseClicks = true;

        public List<MenuOption> items = [];

        public Vector2 position;

        public Action onSelectOption;
        public Action onCancel;
        public Action onChangeOption;
        public Menu parentMenu;

        public MenuOption this[int index] => items[index];

        public static bool mouseEnabled = false;

        public void Update()
        {
            if (!active)
                return;

            if(Input.PressedKey(Keys.S))
            {
                oldOption = selectedOption;
                ++selectedOption;
                onChangeOption?.Invoke();
                if (selectedOption >= items.Count)
                    selectedOption = 0;

                onChangeOption?.Invoke();
            }

            if (Input.PressedKey(Keys.W))
            {
                oldOption = selectedOption;
                --selectedOption;
                onChangeOption?.Invoke();
                if (selectedOption < 0)
                    selectedOption = items.Count - 1;

            }

            if (Input.PressedKey(Keys.D))
            {
                oldOption = selectedOption;
                selectedOption += sideStep;
                onChangeOption?.Invoke();
                if (selectedOption >= items.Count)
                    selectedOption = 0;

                onChangeOption?.Invoke();
            }

            if (Input.PressedKey(Keys.A))
            {
                oldOption = selectedOption;
                selectedOption -= sideStep;
                onChangeOption?.Invoke();
                if (selectedOption < 0)
                    selectedOption = items.Count - 1;

            }

            if (Input.PressedKey(Keys.E) || (Input.LMBClicked && mouseEnabled))
            {
                onSelectOption?.Invoke();
                if (items.Count > 0)
                    items[selectedOption].onConfirmOption?.Invoke();
            }

            if (mouseEnabled)
            {
                var tryMouse = items.FirstOrDefault(x => OptionContainsMouse(x));
                if (tryMouse != null)
                {
                    selectedOption = tryMouse.index;

                    if (selectedOption != oldOption)
                    {
                        onChangeOption?.Invoke();
                        oldOption = selectedOption;
                    }
                }
            }

            if (items.Count <= 0)
                selectedOption = 0;

            if (Input.PressedKey(Keys.Q) || (Input.RMBClicked && mouseEnabled))
            {
                onCancel?.Invoke();
                if (parentMenu != null)
                    parentMenu.active = true;
            }
        }

        public List<string> OptionNames
        {
            get
            {
                List<string> names = [];
                foreach (var item in items)
                {
                    names.Add(item.name);
                }

                return names;
            }
        }

        public MenuOption AddOption(string name, Action action)
        {
            MenuOption option = new()
            {
                index = items.Count,
                name = name,
                onConfirmOption = action
            };

            items.Add(option);

            return option;
        }

        public int Count => items.Count;

        public Vector2 GetSize => Assets.CombatMenuFont.MeasureString(OptionNames.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur));


        public bool OptionContainsMouse(MenuOption option)
        {
            int height = (int)(GetSize.Y);
            var rect = new Rectangle((int)position.X, (int)position.Y + height * option.index, (int)GetSize.X+4, height);
            var mousePoint = new Point((int)Input.MousePosition.X * 4, (int)Input.MousePosition.Y *4);

            return rect.Contains(mousePoint);
        }

        public class MenuOption 
        {
            public int index;
            public float scale;
            public float opacity;
            public string name = "???";
            public Color color = Color.White;
            public Action onConfirmOption;
        }
    }
}

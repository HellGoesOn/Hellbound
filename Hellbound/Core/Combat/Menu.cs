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
        public int selectedOption;
        public int sideStep = 1;
        public bool active;
        public bool visible = true;

        public List<MenuOption> items = [];

        public Vector2 position;

        public Action? onSelectOption;
        public Action? onCancel;
        public Action? onChangeOption;
        public Menu? parentMenu;

        public MenuOption this[int index] => items[index];

        public void Update()
        {
            if (!active)
                return;
            if(Input.PressedKey(Keys.S))
            {
                ++selectedOption;
                onChangeOption?.Invoke();
                if (selectedOption >= items.Count)
                    selectedOption = 0;

                onChangeOption?.Invoke();
            }

            if (Input.PressedKey(Keys.W))
            {
                --selectedOption;
                onChangeOption?.Invoke();
                if (selectedOption < 0)
                    selectedOption = items.Count - 1;

            }

            if (Input.PressedKey(Keys.D))
            {
                selectedOption += sideStep;
                onChangeOption?.Invoke();
                if (selectedOption >= items.Count)
                    selectedOption = 0;

                onChangeOption?.Invoke();
            }

            if (Input.PressedKey(Keys.A))
            {
                selectedOption -= sideStep;
                onChangeOption?.Invoke();
                if (selectedOption < 0)
                    selectedOption = items.Count - 1;

            }

            if (Input.PressedKey(Keys.E))
            {
                onSelectOption?.Invoke();
                if (items.Count > 0)
                    items[selectedOption].onConfirmOption?.Invoke();
            }


            if (items.Count <= 0)
                selectedOption = 0;

            if (Input.PressedKey(Keys.Q))
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
                name = name,
                onConfirmOption = action
            };

            items.Add(option);

            return option;
        }

        public int Count => items.Count;

        public Vector2 GetSize => AssetManager.DefaultFont.MeasureString(OptionNames.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur));

        public class MenuOption 
        {
            public float scale;
            public float opacity;
            public string name = "???";
            public Color color = Color.White;
            public Action? onConfirmOption;
        }
    }
}

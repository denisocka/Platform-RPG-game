using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    static public class ColorItem
    {
        public static readonly Color Junk = Color.Gray;
        public static readonly Color Common = Color.White;
        public static readonly Color Uncommon = Color.Green;
        public static readonly Color Rare = Color.DodgerBlue;
        public static readonly Color Epic = Color.MediumPurple;
        public static readonly Color Legendary = Color.Gold;
        public static readonly Color Mythic = Color.Red;
    }

    public class ItemDefinition
    {
        public string Name;
        public Image Icon;
        public int MaxCount = 16;
        public Color color = ColorItem.Junk;


        public ItemDefinition(string name, Image icon)
        {
            Name = name;
            Icon = icon;
        }
    }

    public static class DefaultItems
    {
        static Image source = Properties.Resources.items;
        static Image source2 = Properties.Resources.items2;

        public static readonly ItemDefinition copper_ingot = new ItemDefinition("Медный слиток", ImageCreator.CreateImage(source, 128, 64, 64, 64)) { color = ColorItem.Common };
        public static readonly ItemDefinition iron_ingot = new ItemDefinition("Железный слиток", ImageCreator.CreateImage(source, 64, 64, 64, 64)) { color = ColorItem.Uncommon };
        public static readonly ItemDefinition golden_ingot = new ItemDefinition("Золотой слиток", ImageCreator.CreateImage(source, 192, 64, 64, 64)) { color = ColorItem.Rare };

        public static readonly ItemDefinition copper_ore = new ItemDefinition("Медная руда", ImageCreator.CreateImage(source, 128, 64, 64, 64)) { color = ColorItem.Junk };
        public static readonly ItemDefinition iron_ore = new ItemDefinition("Железная руда", ImageCreator.CreateImage(source, 64, 64, 64, 64)) { color = ColorItem.Common };
        public static readonly ItemDefinition golden_ore = new ItemDefinition("Золотая руда", ImageCreator.CreateImage(source, 192, 64, 64, 64)) { color = ColorItem.Uncommon };

        public static readonly ItemDefinition anvil = new ItemDefinition("Наковальня", ImageCreator.CreateImage(source, 256, 0, 64, 64)) { color = ColorItem.Rare };
        public static readonly ItemDefinition arrow = new ItemDefinition("Стрела", ImageCreator.CreateImage(source, 256, 256, 64, 64)) { color = ColorItem.Common };
        public static readonly ItemDefinition feather = new ItemDefinition("Перо", ImageCreator.CreateImage(source, 512, 320, 64, 64)) { color = ColorItem.Junk };

        public static readonly ItemDefinition raw_lether = new ItemDefinition("Необработанная кожа", ImageCreator.CreateImage(source, 0, 320, 64, 64)) { color = ColorItem.Junk };
        public static readonly ItemDefinition lether = new ItemDefinition("Кожа", ImageCreator.CreateImage(source, 448, 320, 64, 64)) { color = ColorItem.Common };
        public static readonly ItemDefinition textile = new ItemDefinition("Ткань", ImageCreator.CreateImage(source, 128, 320, 64, 64)) { color = ColorItem.Uncommon };
        public static readonly ItemDefinition quiver_of_arrows = new ItemDefinition("Колчан стрел", ImageCreator.CreateImage(source, 320, 256, 64, 64)) { color = ColorItem.Uncommon };
        public static readonly ItemDefinition log = new ItemDefinition("Дерево", ImageCreator.CreateImage(source, 512, 384, 64, 64)) { color = ColorItem.Common };

        public static readonly ItemDefinition leave = new ItemDefinition("Листок", ImageCreator.CreateImage(source, 448, 0, 64, 64)) { color = ColorItem.Common };

        public static readonly ItemDefinition eye_of_orc = new ItemDefinition("Глаз орка", ImageCreator.CreateImage(source, 512, 320, 64, 64)) { color = ColorItem.Uncommon };






        public static readonly ItemDefinition fire_powder = new ItemDefinition("Огненный порошок", ImageCreator.CreateImage(source, 384, 320, 64, 64)) { color = ColorItem.Rare};
        public static readonly ItemDefinition amulet_of_life = new ItemDefinition("Амулет жизни", ImageCreator.CreateImage(source, 192, 128, 64, 64)) { color = ColorItem.Epic};    


    }

    public class SimpleItem : Item
    {
        public override Item Clone()
        {
            return new SimpleItem(Definition, X, Y)
            {
                map = this.map,
            };
        }

        public ItemDefinition Definition { get; }

        public SimpleItem(ItemDefinition def, double x = 0, double y = 0) : base(x, y)
        {
            Definition = def;
            name = def.Name;
            image = def.Icon;
            MaxCount = def.MaxCount;
            colorName = def.color;
        }
    }
}

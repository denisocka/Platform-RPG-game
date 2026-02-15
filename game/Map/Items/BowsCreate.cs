using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public static class Bows
    {
        static Image source = Properties.Resources.items;
        static Image source2 = Properties.Resources.items2;
        static Image source3 = Properties.Resources.item3;

        public static readonly BowDefinition Default_bow = new BowDefinition("Лук", ImageCreator.CreateImage(source, 128, 256, 64, 64))
        {
            damage = 8,
            color = ColorItem.Common,
            maxStretch = 60
        };

        public static readonly BowDefinition Hunting_bow = new BowDefinition("Охотничий лук", ImageCreator.CreateImage(source, 128, 256, 64, 64))
        {
            damage = 18,
            CreteChance = 10,
            color = ColorItem.Uncommon,
            maxStretch = 50
        };

        public static readonly BowDefinition Bow_of_orc = new BowDefinition("Лук орка", ImageCreator.CreateImage(source2, 768, 7104, 64, 64))
        {
            damage = 14,
            CreteChance = 30,
            color = ColorItem.Uncommon,
            maxStretch = 35,
            Vampirism = 10
        };

        //=================================================
        
        public static readonly BowDefinition Crossbow = new BowDefinition("Арбалет", ImageCreator.CreateImage(source3, 1344, 64, 64, 64))
        {
            damage = 38,
            CreteChance = 30,
            color = ColorItem.Common,
            maxStretch = 140
        };

        public static readonly BowDefinition Bow_of_killer_orcs = new BowDefinition("Лук убийцы орков", ImageCreator.CreateImage(source2, 128, 6784, 64, 64))
        {
            damage = 24,
            CreteChance = 10,
            color = ColorItem.Rare,
            maxStretch = 50,
            Vampirism = 10,
        };

        //=================================================================

        public static readonly BowDefinition Crossbow_Ice = new BowDefinition("Ледяной арбалет", ImageCreator.CreateImage(source3, 1344, 448, 64, 64))
        {
            damage = 68,
            CreteChance = 30,
            color = ColorItem.Epic,
            maxStretch = 100
        };

        //=====================================

        public static readonly BowDefinition Demons_bow = new BowDefinition("Демоничнский лук", ImageCreator.CreateImage(source, 192, 256, 64, 64))
        {
            damage = 64,
            CreteChance = 30,
            color = ColorItem.Legendary,
            maxStretch = 50,
            Vampirism = 10,
        };

        public static readonly BowDefinition Bow_of_taras = new BowDefinition("Лук Тараса", ImageCreator.CreateImage(source3, 1280, 448, 64, 64))
        {
            damage = 58,
            CreteChance = 10,
            color = ColorItem.Mythic,
            maxStretch = 35,
            Vampirism = 10,
        };

        

        

    }

    public class BowDefinition
    {

        public int maxStretch = 70;
        public string Name;
        public Image Icon;
        public Color color = Color.LightGray;
        public int damage = 8;
        public double CreteChance = 0;
        public double Vampirism = 0;
        public BowDefinition(string name, Image icon)
        {
            Name = name;
            Icon = icon;
        }
    }

    public class Bow : Item
    {
        int Damage;
        public double CreteChance;
        public double Vampirism;

        public int maxStretch;
        public int strech;
        public double GetDamage()
        {
            if (strech >= maxStretch)
                return Damage;
            return (double)strech / maxStretch * Damage;
        }


        public override Item Clone()
        {
            return new Bow(Definition, X, Y)
            {
                map = this.map,
            };
        }

        public BowDefinition Definition { get; }
        public Bow(BowDefinition def, double x = 0, double y = 0) : base(x, y)
        {
            Definition = def;
            name = def.Name;
            image = def.Icon;
            MaxCount = 1;
            colorName = def.color;
            Damage = def.damage;
            CreteChance = def.CreteChance;
            Vampirism = def.Vampirism;
            maxStretch = def.maxStretch;
        }
    }
}

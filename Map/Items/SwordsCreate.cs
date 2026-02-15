using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class Sword : Item
    {
        public int CoolDown;
        public int BaseCoolDown;
        public int Damage;
        public double CreteChance;
        public double Vampirism;

        public override Item Clone()
        {
            return new Sword(Definition, X, Y)
            {
                map = this.map,
            };
        }

        public SwordDefinition Definition { get; }

        public Sword(SwordDefinition def, double x = 0, double y = 0) : base(x, y)
        {
            BaseCoolDown = def.CoolDown;
            Definition = def;
            name = def.Name;
            image = def.Icon;
            MaxCount = 1;
            colorName = def.color;
            Damage = def.damage;
            CreteChance = def.CreteChance;
            Vampirism = def.Vampirism;
        }

        public void SetBaseCoolDown()
        {
            CoolDown = BaseCoolDown;
        }

        public override void Update()
        {
            base.Update();

            if (CoolDown > 0)
                CoolDown--;
            
        }
    }

    public static class Swords
    {
        static Image source = Properties.Resources.items;
        static Image source3 = Properties.Resources.item3;


        public static readonly SwordDefinition Stone_sword = new SwordDefinition("Каменный меч", ImageCreator.CreateImage(source, 320, 192, 64, 64))
        {
            damage = 4,
            color = ColorItem.Common,
            CoolDown = 40
        };
        public static readonly SwordDefinition Iron_sword = new SwordDefinition("Железный меч", ImageCreator.CreateImage(source, 384, 192, 64, 64))
        {
            damage = 8,
            color = ColorItem.Uncommon,
            CreteChance = 10,
            CoolDown = 40
        };
        
        public static readonly SwordDefinition Spear = new SwordDefinition("Копье орка", ImageCreator.CreateImage(source, 512, 192, 64, 64))
        {
            damage = 14,
            color = ColorItem.Rare,
            CreteChance = 30,
            CoolDown = 30
        };
        
        //=============================

        public static readonly SwordDefinition Spear_of_zulus = new SwordDefinition("Копье Зулуса", ImageCreator.CreateImage(source, 576, 192, 64, 64))
        {
            damage = 18,
            color = ColorItem.Epic,
            CreteChance = 50,
            CoolDown = 30
        };

        public static readonly SwordDefinition Golden_sword = new SwordDefinition("Золотой меч", ImageCreator.CreateImage(source, 448, 192, 64, 64))
        {
            damage = 12,
            color = ColorItem.Rare,
            CreteChance = 10,
            Vampirism = 20,
            CoolDown = 35
        };

        //==========================================

        public static readonly SwordDefinition trident_of_dezmont = new SwordDefinition("Безупречный трезубец Дезмонта", ImageCreator.CreateImage(source3, 448, 1728, 64, 64))
        {
            damage = 52,
            color = ColorItem.Legendary,
            CreteChance = 10,
            Vampirism = 10,
            CoolDown = 40
        };

        public static readonly SwordDefinition Staff = new SwordDefinition("Силовой посох", ImageCreator.CreateImage(source, 0, 256, 64, 64))
        {
            damage = 22,
            color = ColorItem.Epic,
            CreteChance = 10,
            Vampirism = 30,
            CoolDown = 35
        };

        public static readonly SwordDefinition Rapire = new SwordDefinition("Железная рапира", ImageCreator.CreateImage(source3, 0, 1088, 64, 64))
        {
            damage = 16,
            color = ColorItem.Epic,
            CreteChance = 0,
            Vampirism = 10,
            CoolDown = 20,
        };

        //===================================================================

        public static readonly SwordDefinition Staff_of_nobius = new SwordDefinition("Посох владыки Нобиуса", ImageCreator.CreateImage(source, 64, 256, 64, 64))
        {
            damage = 68,
            color = ColorItem.Mythic,
            CreteChance = 20,
            Vampirism = 30,
            CoolDown = 30
        };
    }

    public class SwordDefinition
    {
        public int CoolDown = 30;
        public string Name;
        public Image Icon;
        public Color color = Color.LightGray;
        public int damage = 4;
        public double CreteChance = 0;
        public double Vampirism = 0;
        public SwordDefinition(string name, Image icon)
        {
            Name = name;
            Icon = icon;
        }
    }

    

   
}

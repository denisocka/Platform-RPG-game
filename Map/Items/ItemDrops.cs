using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class ItemsDropMeneger
    {
        static Random rnd = new Random();

        Entity mob;
        Map map;
        bool IsDrop = false;

        List<ItemsDropPack> itemPaks = new List<ItemsDropPack>();
        public ItemsDropMeneger(Entity mob, Map map)
        {
            this.mob = mob;
            this.map = map; 
        }

        public void Add(ItemsDropPack pack)
        {
            itemPaks.Add(pack);
        }

        public void Droped()
        {
            List<Item> itemsDproped = new List<Item>();
            foreach (var pack  in itemPaks)
            {
                itemsDproped.AddRange(pack.Droped());
            }
            foreach (var item in itemsDproped)
            {
                var vx = rnd.Next(-8, 8);
                var vy = rnd.Next(-2, -10);
                item.IsInWorld = true;
                map.Items.CreateItem(item, vx, vy);
            }
        }

        public void Update()
        {
            if (!IsDrop && !mob.IsAlive)
            {
                IsDrop = true;
                Droped();
            }
        }
    }

    public class ItemDrop
    {
        public Item item;
        public double chance;
        public int count;
        
        public ItemDrop(Item item, double Chance, int Count = 1)
        {
            this.item = item;
            this.chance = Chance;
            this.count = Count;
        }
    }

    public class ItemsDropPack
    {
        

        static Random rnd = new Random();
        List<ItemDrop> items;

        public ItemsDropPack(List<ItemDrop> items)
        {
            this.items = items;
        }

        public void Add(ItemDrop item)
        {
            items.Add(item);
        }

        public List<Item> Droped()
        {
            List<Item> itemsDroped = new List<Item>();
            foreach (ItemDrop item in items)
            {
                if (rnd.NextDouble() * 100 <=item.chance)
                {
                    int count = rnd.Next(1,item.count);
                    for (int i = 0; i < count; i++)
                        itemsDroped.Add(item.item);
                }
                    
            }

            return itemsDroped;
        }

        static public class Packs
        {
            static readonly public ItemsDropPack DefaultPack = new ItemsDropPack(new List<ItemDrop>
            {
                
            });

        }

    }
}

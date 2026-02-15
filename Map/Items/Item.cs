using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace game
{
    public class ItemMeneger : IEnumerable<Item>
    {
        public IEnumerator<Item> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        Map map { set; get; }

        public int Count()
        {
            return items.Count;
        }

        public ItemMeneger(Map map)
        {
            this.map = map;
        }
        public ItemMeneger() { }

        List<Item> items = new List<Item>();

        public Item this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }

        public void Add(Item item)
        {
            items.Add(item);
            item.map = map;
        }

        public void Inertion(Item item,double vx, double vy)
        {
            item.Vx = vx;
            item.Vy = vy;
        }

        public void CreateItem(Item item, double vx, double vy = -10)
        {
            Add(item);
            Inertion(item, vx, vy);
        }

        public void Draw(Graphics g, Point cameraOffSet)
        {
                foreach (var item in items)
                    item.Draw(g, cameraOffSet);
        }

        public void Update()
        {
                foreach (var item in items)
                    item.Update();

            for (int i = 0; i < items.Count; i++)
                if (!items[i].IsAlive)
                    items.RemoveAt(i);
        }
    }

    public abstract class Item
    {
        public int MaxCount = 1;
        public Image image;
        public double X, Y;
        public double Vx, Vy;
        public String name;
        public Color colorName = Color.White;
        public bool IsAlive = true;
        public bool IsInWorld = true;

        public Map map;

        public Size Size = new Size(48,48);

        public void Delete()
        {
            IsAlive = false;
        }

        public Item(double x=0, double y=0)
        {
            X = x; Y = y;
        }

        public abstract Item Clone();

        public void OnUse(Player player) { }

        public virtual void Draw(Graphics g, Point cameraOffSet)
        {
            g.DrawImage(image, new Rectangle((int)X + cameraOffSet.X, (int)Y + cameraOffSet.Y, Size.Width, Size.Height));
        }

        public virtual void Update()
        {
            if (IsInWorld && map != null)
            {
                Movement();
                Gravity();
            }
        }


        public (double newDX, double newDY) CanMove(double dx, double dy, double tolerance = 0.1)
        {
            double newDX = dx;
            double newDY = dy;

            foreach (var platform in map.platforms)
            {
                double px = platform.X;
                double py = platform.Y;
                double pw = platform.size.Width;
                double ph = platform.size.Height;

                if (Y + Size.Height - tolerance > py && Y + tolerance < py + ph)
                {
                    if (dx > 0 && X + Size.Width + dx > px && X + Size.Width <= px)
                    {
                        newDX = Math.Min(newDX, px - (X + Size.Width));
                        Vx = 0;
                    }
                    else if (dx < 0 && X + dx < px + pw && X >= px + pw)
                    {
                        newDX = Math.Max(newDX, (px + pw) - X);
                        Vx = 0;
                    }
                }
            }

            double tempX = X + newDX;

            foreach (var platform in map.platforms)
            {
                double px = platform.X;
                double py = platform.Y;
                double pw = platform.size.Width;
                double ph = platform.size.Height;

                if (tempX + Size.Width - tolerance > px && tempX + tolerance < px + pw)
                {
                    if (dy > 0)
                    {
                        if (Y + Size.Height + dy > py && Y + Size.Height <= py)
                        {
                            newDY = py - (Y + Size.Height);
                            Vy = 0;
                        }
                    }
                    else if (dy < 0)
                    {
                        if (Y + dy < py + ph && Y >= py + ph)
                        {
                            newDY = (py + ph) - Y;
                            Vy = 0;
                        }
                    }
                }
            }

            return (newDX, newDY);
        }

        void Move(double dx, double dy)
        {
            var v = CanMove(dx, dy);
            X += v.Item1;
            Y += v.Item2;
        }

        public void Movement()
        {
            double friction = 0.2;

            if (Vx > 0)
            {
                Vx -= friction;
                if (Vx < 0) Vx = 0;
            }
            else if (Vx < 0)
            {
                Vx += friction;
                if (Vx > 0) Vx = 0;
            }

            if (Vx != 0)
                Move(Vx, 0);
        }
        

        public void Gravity()
        {
            if (Vy != 0)
                Move(0, Vy);
            if (!IsOnPlatform())
                Vy += 1;
        }

        public bool IsOnPlatform(double tolerance = 1.0)
        {
            foreach (var platform in map.platforms)
            {
                double px = platform.X;
                double py = platform.Y;
                double pw = platform.size.Width;
                double ph = platform.size.Height;

                bool intersectsX = (X + Size.Width > px) && (X < px + pw);
                bool onTop = Math.Abs((Y + Size.Height) - py) <= tolerance;

                if (intersectsX && onTop)
                    return true;
            }
            return false;
        }

    }

   
}

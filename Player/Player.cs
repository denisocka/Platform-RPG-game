using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public enum Direction
    {
        Left, Right
    }

    public partial class Player
    {

        public double X, Y;
        public double Vx, Vy;

        int HealPoint;
        int BaseHealPoint = 20;
        int FeedPoint;

        double Speed = 10;

        public Direction dir = Direction.Left;

        public Size Size = new Size(60,90);

        public Invetory inv = new Invetory();

        public bool IsDPressed;
        public bool IsAPressed;
        public bool IsSpacePressed;
        public bool IsEPressed;
        public bool IsQPressed;

        public TextureManager Texture = new TextureManager();

        Map map;


        public Player(double x, double y, Map map)
        {
            X = x;
            Y = y;

            inv.AddItem(new Sword(Swords.Stone_sword));
            inv.AddItem(new Sword(Swords.Iron_sword));
            inv.AddItem(new Sword(Swords.Spear));
            inv.AddItem(new Sword(Swords.Spear_of_zulus));
            inv.AddItem(new Sword(Swords.Golden_sword));
            inv.AddItem(new Sword(Swords.trident_of_dezmont));
            inv.AddItem(new Sword(Swords.Staff));
            inv.AddItem(new Sword(Swords.Rapire));
            inv.AddItem(new Sword(Swords.Staff_of_nobius));


            inv.AddItem(new Bow(Bows.Default_bow));
            inv.AddItem(new Bow(Bows.Hunting_bow ));
            inv.AddItem(new Bow(Bows.Bow_of_orc));
            inv.AddItem(new Bow(Bows.Crossbow));
            inv.AddItem(new Bow(Bows.Bow_of_killer_orcs));
            inv.AddItem(new Bow(Bows.Crossbow_Ice));
            inv.AddItem(new Bow(Bows.Demons_bow));
            inv.AddItem(new Bow(Bows.Bow_of_taras));




            var sizeSprites = new Size(500, 500);

            Texture.AddSprite(Properties.Resources.walk, 8, 3, sizeSprites);

            Texture.AddSprite(Properties.Resources.attack, 6, 3, sizeSprites);
            Texture.AddSprite(Properties.Resources.attack2, 6, 3, sizeSprites);
            Texture.AddSprite(Properties.Resources.attack3, 9, 5, sizeSprites);

            Texture.AddSprite(Properties.Resources.dead, 4, 5, sizeSprites);

            Texture.AddImage(Properties.Resources.player);
            Texture.AddImage(ImageCreator.CreateImage(Properties.Resources.dead, new Point(300,0), new Size(100,100)));
            Texture.sizeImage = new Size(500, 500);
            this.map = map;

            HealPoint = BaseHealPoint;
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
            double acceleration = 1.2; 
            double friction = 1;   
            double maxSpeed = Speed; 

            if (IsDPressed)
            {
                dir = Direction.Right;
                if (Vx < maxSpeed)
                    Vx += acceleration;

                if (CanAnimationMove())
                    Texture.PlayingWhileDontStop(0);
            }
            else if (IsAPressed)
            {
                dir = Direction.Left;
                if (Vx > -maxSpeed)
                    Vx -= acceleration;

                if (CanAnimationMove())
                    Texture.PlayingWhileDontStop(0);
            }
            else
            {
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

                Texture.StopPlaying(0);
            }

            if (IsSpacePressed && IsOnPlatform())
            {
                Vy = -23; 
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

        private bool CanAnimationMove()
        {
            return (!(Texture.IsAnimation(1) || Texture.IsAnimation(2) || Texture.IsAnimation(3) || !IsOnPlatform()));
        }

        

        private void KeyPress()
        {
            if (IsEPressed)
            {
                dir = Direction.Right;
                Attack();
            }

            if (IsQPressed)
            {
                dir = Direction.Left;
                Attack();
            }
        }

        public void MousePress(MouseEventArgs e)
        {
            if (IsAlive)
                Attack();
        }

        

        public bool IsAlive = true;
        public void Dead()
        {
            if (IsAlive)
            {
                IsAlive = false;
                Texture.SetActiveImage(1);
                Texture.PlayAnimation(4);
            }
        }


        public void Draw(Graphics g, Point cameraOffSet)
        {

            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;


            var sizeImage = Texture.GetSize();

            if (dir == Direction.Right)
            {
                int x = (int)X + Size.Width / 2 - sizeImage.Width / 2 + cameraOffSet.X;
                int y = (int)Y + Size.Height / 2 - sizeImage.Height / 2  + cameraOffSet.Y;

                g.DrawImage(
                       Texture.GetImage(),
                       new Rectangle(
                           x, y + 10, sizeImage.Width, sizeImage.Height
                       )
                   );
            }

            if (dir == Direction.Left)
            {
                int x = (int)X + Size.Width / 2 + sizeImage.Width / 2 + cameraOffSet.X;
                int y = (int)Y + Size.Height / 2 - sizeImage.Height / 2 + cameraOffSet.Y;

                g.DrawImage(
                       Texture.GetImage(),
                       new RectangleF(
                           x, y + 10, -sizeImage.Width, sizeImage.Height
                       )
                   );
            }

            foreach (var arrow in arrows)
                arrow.Draw(g, cameraOffSet);

            inv.Draw(g, cameraOffSet, this);

            DrawAttack(g,cameraOffSet);
        }

        public void Update(Point cameraOffSet)
        {
            foreach (var arrow in arrows)
                arrow.Update(cameraOffSet);
            UpdateAttack();
            if (Texture.IsAnimation(0) && !IsOnPlatform())
                Texture.StopPlaying(0);

            inv.Update();
            Texture.Update();
            Gravity();
            if (IsAlive)
            {
                KeyPress();
                Movement();
            }

        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public abstract class Entity
    {
        public int SpeedAttack = 50;

        public int damage = 10;
        public double ChanceMiss = 0;
        Random rnd = new Random();
        public TextureManager texture;
        public Map map;
        public double X,Y;
        public double Vx,Vy;
        public Size Size = new Size(60, 90);
        public double Speed = 4;
        public bool isCanMove = true;
        public string Name;
        public Color NameColor;
        public Entity(int x, int y, Map map)
        {
            this.X = x;
            this.Y = y;
            this.map = map;
            health = baseHealth;
        }

        public int health;
        public int baseHealth = 20;

        public bool IsAlive = true;


        public void Dead()
        {
            if (IsAlive)
            {
                Vx = 0;
                texture.StopPlaying(1);
                IsAlive = false;
                texture.PlayAnimation(2);
                texture.SetActiveImage(1);
                DropItem();
            }
        }

        public virtual void DropItem()
        {
            map.Items.CreateItem(new SimpleItem(DefaultItems.amulet_of_life, X + Size.Width/2, Y + Size.Height/2), rnd.Next(-10, 10), rnd.Next(-15, -5));


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

        public void Move(double dx, double dy)
        {
            var v = CanMove(dx, dy);
            X += v.Item1;
            Y += v.Item2;
        }

        public Direction dir = Direction.Left;

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

        int AttackKD;

        public void Attack()
        {
            if (!texture.IsAnimation(0) && AttackKD == 0)
            {
                texture.PlayAnimation(0);
                AttackKD = SpeedAttack;

            }
        }

        static Font font = new Font("Arial", 12, FontStyle.Bold);

        public virtual void Draw(Graphics g, Point cameraOffSet)
        {

            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            var sizeImage = texture.GetSize();

            if (dir == Direction.Right)
            {
                int x = (int)X + Size.Width / 2 - sizeImage.Width / 2 + cameraOffSet.X;
                int y = (int)Y + Size.Height / 2 - sizeImage.Height / 2 + cameraOffSet.Y;

                g.DrawImage(
                       texture.GetImage(),
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
                       texture.GetImage(),
                       new RectangleF(
                           x, y + 10, -sizeImage.Width, sizeImage.Height
                       )
                   );
            }

            //Прорисовка хп
            Size HpSize = new Size(60, 15);
            if (health<baseHealth)
            {
                if (health > 0)
                {
                    g.FillRectangle(Brushes.DarkRed, new Rectangle((int)X + Size.Width / 2 - HpSize.Width / 2 + cameraOffSet.X, (int)Y + cameraOffSet.Y - 30, HpSize.Width, HpSize.Height));
                    g.FillRectangle(Brushes.Red, new Rectangle((int)X + Size.Width / 2 - HpSize.Width / 2 + cameraOffSet.X, (int)Y + cameraOffSet.Y - 30, (int)((double)Size.Width * health / baseHealth), HpSize.Height));
                }
            }

            //Прорисовка имени
            if (Name != null && IsAlive)
            {
                var sizeNameF = g.MeasureString(this.Name, font);
                Size sizeName = new Size();
                sizeName.Width = (int) sizeNameF.Width;
                sizeName.Height = (int) sizeNameF.Height;
                g.FillRectangle(Brushes.White, new Rectangle((int)X + Size.Width / 2 - sizeName.Width / 2 + cameraOffSet.X - 2, (int)Y + cameraOffSet.Y - 10 - 2, sizeName.Width + 4, sizeName.Height + 4));
                g.DrawString(Name, font, new SolidBrush(this.NameColor), new Point((int)X + Size.Width / 2 - sizeName.Width / 2 + cameraOffSet.X, (int)Y + cameraOffSet.Y - 10));

            }


        }

        private bool CanAnimationMove()
        {
            return (!(texture.IsAnimation(0)) && Vx != 0);
        }

        public virtual void Movement()
        {
            double lookAhead = 5;      
            double downCheck = 5;     

            Vx = (dir == Direction.Left ? -Speed : Speed);

            double checkX = dir == Direction.Left
                ? X - lookAhead
                : X + Size.Width + lookAhead;

            double checkY = Y + Size.Height + downCheck;

            bool hasGroundAhead = false;

            foreach (var platform in map.platforms)
            {
                double px = platform.X;
                double py = platform.Y;
                double pw = platform.size.Width;

                if (checkX >= px && checkX <= px + pw)
                {
                    if (Math.Abs(py - checkY) <= downCheck)
                    {
                        hasGroundAhead = true;
                        break;
                    }
                }
            }

            if (!hasGroundAhead)
            {
                dir = dir == Direction.Left ? Direction.Right : Direction.Left;
                Vx = 0;
                return;
            }

            double oldX = X;
            Move(Vx, 0);

            if (Math.Abs(X - oldX) < 0.01)
            {
                dir = dir == Direction.Left ? Direction.Right : Direction.Left;
                Vx = 0;
            }
        }

        public void Update()
        {
            if (AttackKD > 0)
                AttackKD--;
            if (CanAnimationMove())
                texture.PlayingWhileDontStop(1);

            if (Vx ==0)
                texture.StopPlaying(1);
            Gravity();
            texture.Update();
            if (IsAlive && isCanMove)
            {
                Movement();
            }
           
        }
    }

    class CloseCombatEnuty : Entity
    {

        public double AttackDistance = 70;

        static TextureManager BaseTexture;
        static CloseCombatEnuty()
        {
            var sizeSprites = new Size(500, 500);
            BaseTexture = new TextureManager();
            BaseTexture.sizeImage = sizeSprites;
            BaseTexture.AddImage(ImageCreator.CreateImage(Properties.Resources.orc_attack, 0, 0, 100, 100));
            BaseTexture.AddImage(ImageCreator.CreateImage(Properties.Resources.orc_dead, 300, 0, 100, 100));
            BaseTexture.AddSprite(Properties.Resources.orc_attack, 6, 5, sizeSprites);
            BaseTexture.AddSprite(Properties.Resources.orc_walk, 8, 5, sizeSprites);
            BaseTexture.AddSprite(Properties.Resources.orc_dead, 4, 5, sizeSprites);
        }
        CloseCombatMobsDifinition dif;
        public CloseCombatEnuty(CloseCombatMobsDifinition dif, int x, int y, Map map) : base(x, y, map)
        {
            this.dif = dif;

            Name = dif.Name;
            ChanceMiss = dif.ChanceMiss;

            damage = dif.damage;
            Speed = dif.speed;
            this.baseHealth = dif.hp;
            this.health = baseHealth;
            SpeedAttack = dif.SpeedAttack;
            AttackDistance = dif.DistanceAttack;
            isCanMove = dif.IsCanMove;
            NameColor = dif.color;

            if (texture == null)
                texture = BaseTexture.Clone();

        }

        public override void Movement()
        {
            double aggroRadius = 500;
            double lookAhead = 5;
            double downCheck = 5;

            var player = map.player;
            if (player == null)
            {
                base.Movement();
                return;
            }

            double dxToPlayer = (player.X + player.Size.Width / 2)
                              - (X + Size.Width / 2);

            double absDist = Math.Abs(dxToPlayer);

            if (absDist > aggroRadius)
            {
                base.Movement();
                return;
            }

            dir = dxToPlayer < 0 ? Direction.Left : Direction.Right;

            double checkX = dir == Direction.Left
                ? X - lookAhead
                : X + Size.Width + lookAhead;

            double checkY = Y + Size.Height + downCheck;

            bool hasGroundAhead = false;

            foreach (var platform in map.platforms)
            {
                double px = platform.X;
                double py = platform.Y;
                double pw = platform.size.Width;

                if (checkX >= px && checkX <= px + pw)
                {
                    if (Math.Abs(py - checkY) <= downCheck)
                    {
                        hasGroundAhead = true;
                        break;
                    }
                }
            }

            if (!hasGroundAhead)
            {
                Vx = 0;
                return;
            }

            if (absDist <= AttackDistance)
            {
                Vx = 0;
                Attack();
                return;
            }

            

            Vx = dir == Direction.Left ? -Speed : Speed;
            Move(Vx, 0);
        }

    }

    public class EntityMeneger : IEnumerable<Entity>
    {
        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        List<Entity> entities = new List<Entity> ();

        public void Add(Entity entity)
        {
            entities.Add(entity);

        }

        public void Draw(Graphics g, Point cameraOffSet)
        {
            foreach (var entity in entities)
            {
                if (entity.X + cameraOffSet.X < 2000 && entity.X + cameraOffSet.X > -500 && entity.Y + cameraOffSet.Y > -500 && entity.Y + cameraOffSet.Y < 1000)
                    entity.Draw(g, cameraOffSet);
            }
        }

        public void Update()
        {
            foreach (var entity in entities)
                entity.Update();
        }
    }
}

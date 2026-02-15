using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace game
{
    partial class Player
    {
        Random rnd = new Random();
        public List<Arrow> arrows = new List<Arrow>();
        public bool IsAttackBow;
        private void Attack()
        {
            if (!Texture.IsAnimation(1) && !Texture.IsAnimation(2) && !Texture.IsAnimation(3) && IsAlive && !inv.IsOpen)
            {
                if (inv.GetActiveItem() is Sword s)
                {
                    if (s.CoolDown != 0)
                        return;

                    if (Vy > 0)
                    {
                        AttackSwordInJump();
                        s.SetBaseCoolDown();
                        return;
                    }

                    AttackSword();
                    s.SetBaseCoolDown();
                }
                    
                if (inv.GetActiveItem() is Bow b )
                    AttackBow();

            }

        }

        void DrawAttack(Graphics g, Point cameraOffSet)
        {
            
        }
        

        bool IsAttacking = false;

        void Attacking()
        {

            if ((Texture.IsAnimation(1) || Texture.IsAnimation(2)) && IsAttacking)
                if (inv.GetActiveItem() is Sword pd)
                {
                    var sizeAttack = new Size(160, 100);
                    Rectangle rec = new Rectangle();

                    int newX = 0;
                    if (Texture.IsAnimation(1))
                    {
                        if (dir == Direction.Right)
                            newX = (int)X + Size.Width / 4 * 3;
                        else
                            newX = (int)X - sizeAttack.Width + Size.Width / 4 * 3;

                        rec = new Rectangle(newX, (int)Y + Size.Height / 2 - sizeAttack.Height / 2, sizeAttack.Width, sizeAttack.Height);
                    }


                    if (Texture.IsAnimation(2))
                    {
                        if (dir == Direction.Right)
                            newX = (int)X + Size.Width;
                        else
                            newX = (int)X - sizeAttack.Width;

                        rec = new Rectangle(newX, (int)Y + Size.Height - sizeAttack.Height / 2, sizeAttack.Width, sizeAttack.Height);

                    }


                    foreach (var mob in map.mobs)
                    {
                        if (!mob.IsAlive)
                            continue;

                        if (rec.X < mob.X + mob.Size.Width &&
                            rec.X + rec.Width > mob.X &&
                            rec.Y < mob.Y + mob.Size.Height &&
                            rec.Y + rec.Height > mob.Y)
                        {
                            GetDamage(
                                mob,
                                pd.Damage,
                                PercentSum(pd.CreteChance, 30),
                                pd.Vampirism
                            );

                            IsAttacking = false;
                            break;
                        }
                    }
                }
        }
        void AttackSword()
        {
            Texture.PlayAnimation(1);
            IsAttacking = true;
        }
        void AttackSwordInJump()
        {
            Texture.PlayAnimation(2);
            IsAttacking = true;
        }

        double PercentSum(double chance, double addChance)
        {
            return (100 - chance)*addChance/100 + chance;
        }

        public void GetDamage(Entity mob, int damage, double creteChance, double vampirism)
        {
            var color = Color.Red;
            bool isCrete = false;
            if (rnd.NextDouble() * 100 <= creteChance)
            {
                damage *= 2;
                color = Color.DarkRed;
                isCrete = true;
            }

            if (rnd.NextDouble() * 100 <= mob.ChanceMiss)
            {
                map.effects.Spawn(new SignPartical((int)mob.X + mob.Size.Width / 2, (int)mob.Y, "miss", Color.Gray));
                return;
            }

            if (mob.health - (int)damage > 0)
            {
                mob.health -= (int)damage;
                this.AddHeal((int)vampirism / 100 * damage);
                map.effects.Spawn(new SignPartical((int)mob.X + mob.Size.Width/2, (int)mob.Y, !isCrete ? damage.ToString(): "*" + damage.ToString() + "*", color));
            }
            else
            {
                map.effects.Spawn(new SignPartical((int)mob.X + mob.Size.Width / 2, (int)mob.Y, !isCrete ? mob.health.ToString() : "*" + mob.health.ToString() + "*", color));
                this.AddHeal((int)vampirism / 100 * mob.health);
                mob.health = 0;
                mob.Dead();
            }
        }


        void AttackBow()
        {
            if (inv.GetActiveItem() is Bow b)
                b.strech=0;
            IsAttackBow = true;
            Texture.PlayAnimation(3);
            
        }

        public void Shot(double damage)
        {
            arrows.Add(new Arrow(X, Y + Size.Height / 2, dir, damage,map));
        }

        public void MouseUp(MouseEventArgs e)
        {
            if (inv.GetActiveItem() is Bow b)
                if (IsAttackBow && b.strech > 35)
                {
                    IsAttackBow = false;
                    Texture.ResumeAnimation(3);
                    Shot(b.GetDamage());
                }
        }

        public void AddHeal(int heal)
        {
            var hp = this.HealPoint;
            if (hp + heal > this.BaseHealPoint)
                this.HealPoint = this.BaseHealPoint;
            else
                this.HealPoint += heal;
        }


        public void UpdateAttack()
        {
            Attacking();
            if (inv.GetActiveItem() is Bow b)
            {
                if (IsAttackBow)
                    b.strech++;
                if (b.strech == 35)
                    Texture.PauseAnimation(3);
                if (b.strech == b.maxStretch)
                {
                    Shot(b.GetDamage());
                    b.strech = 0;
                    IsAttackBow = false;
                    Texture.ResumeAnimation(3);
                }
            }
            else
            {
                Texture.StopPlaying(3);
                IsAttackBow = false;
            }

            for (int i = 0; i < arrows.Count; i++)
                if (!arrows[i].IsAlive)
                {
                    arrows.RemoveAt(i);
                    break;
                }


        }
    }

    public class Arrow
    {
        static Image icon = Properties.Resources.arrow;

        public double x, y;
        double speed = 40;
        Direction dir;

        double damage;

        Map map;

        public bool IsAlive = true;

        Size Size = new Size(60, 20);
        public Arrow(double x, double y, Direction dir, double Damage,Map Map)
        {
            this.dir = dir;
            this.x = x; 
            this.y = y;
            map = Map;
            damage = Damage;
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

                if (y + Size.Height - tolerance > py && y + tolerance < py + ph)
                {
                    if (dx > 0 && x + Size.Width + dx> px && x + Size.Width <= px)
                    {
                        newDX = Math.Min(newDX, px - (x + Size.Width) + 10);
                        speed = 0;
                    }
                    else if (dx < 0 && x + dx < px + pw && x >= px + pw)
                    {
                        newDX = Math.Max(newDX, (px + pw) - x + -10);
                        speed = 0;
                    }
                }
            }

            return (newDX, newDY);
        }

        void Move(double dx, double dy)
        {
            var v = CanMove(dx, dy);
            x += v.Item1;
            y += v.Item2;
        }

        public void Shot(Entity mob)
        {
            if (map.player.inv.GetActiveItem() is Bow b)
                map.player.GetDamage(mob, (int)damage, b.CreteChance, b.Vampirism);
            IsAlive = false;
        }

        public void CollisionEntity()
        {
            foreach (var mob in map.mobs)
            {
                if (mob.IsAlive)
                if (dir == Direction.Right && x + Size.Width > mob.X && x + Size.Width < mob.X + mob.Size.Width && y + Size.Height > mob.Y && y < mob.Y + mob.Size.Height)
                {
                    Shot(mob);
                    return;
                }
                else
                if (dir == Direction.Left && x < mob.X + mob.Size.Width && x > mob.X && y + Size.Height > mob.Y && y < mob.Y + mob.Size.Height)
                    {
                        Shot(mob);
                        return;
                    }
            }
        }

        public void Update(Point cameraOffSet)
        {
            if (speed != 0)
                if(dir == Direction.Left)
                    Move(-speed, 0);
                else
                    if (dir == Direction.Right)
                    Move(speed, 0);
            if (x + cameraOffSet.X > 2000 || x + cameraOffSet.X < -500 || y + cameraOffSet.Y > 1200 || y + cameraOffSet.Y < -500)
                IsAlive = false;
            CollisionEntity();

        }

        public void Draw(Graphics g, Point cameraOffset)
        {
            int X, Y;
            int width, height;
            if (dir == Direction.Left)
            {
                X = (int)x + Size.Width;
                Y = (int)y;
                width = -Size.Width;
                height = Size.Height;
            }
            else
            {
                X = (int)x;
                Y = (int)y;
                width = Size.Width;
                height = Size.Height;
            }
                g.DrawImage(icon, new Rectangle(X + cameraOffset.X,Y + cameraOffset.Y,width,height));
        }
    }
}

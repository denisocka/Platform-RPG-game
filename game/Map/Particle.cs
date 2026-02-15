using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class ParticleMenger
    {
        List<ParticleEffect> particalEffects;
        public ParticleMenger()
        {
            particalEffects = new List<ParticleEffect>();
        }

        public void Spawn(Particle partical,int count=1)
        {
            particalEffects.Add(partical.CreateEffect(count));
        }

        public void Draw(Graphics g, Point cameraOffSet)
        {
            foreach (var effect in particalEffects) 
                effect.Draw(g, cameraOffSet);
        }

        public void Update()
        {
            foreach (var effect in particalEffects)
                effect.Update();
            for (int i = 0; i < particalEffects.Count; i++)
                if (!particalEffects[i].IsAlive)
                {
                    particalEffects.RemoveAt(i);
                    return;
                }

        }

    }

    public class ParticleEffect
    {
        public bool IsAlive = true;
        public List<Particle> particals = new List<Particle>();
        int timer = 100;
        public ParticleEffect(int timer)
        {
            this.timer = timer;
        }

        public void Add(Particle particle)
        {
            particals.Add(particle);
        }

        public void Draw(Graphics g, Point cameraOffSet)
        {
            foreach (var part in particals) 
                part.Draw(g, cameraOffSet);
        }

        public void Update()
        {
            foreach (var partical in particals)
                partical.Update();
            timer--;
            if (timer <= 0)
                IsAlive = false;
        }
    }

    public abstract class Particle
    {
        protected double x,y;

        protected Size size = new Size(50,50);
        protected Image icon;

        public Particle(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public abstract ParticleEffect CreateEffect(int count);
        public virtual void Update() { }

        public virtual void Draw(Graphics g, Point cameraOffSet)
        {
            if (icon != null)
                g.DrawImage(icon, new Rectangle((int)x, (int)y, size.Width, size.Height));
        }
    }

    public class SignPartical : Particle
    {
        string text;
        static Font font = new Font("Arial", 12);
        Brush brush = Brushes.White; 

        public SignPartical(double x, double y, string text, Color color) :base(x,y)
        {
            this.text = text;
            brush = new SolidBrush(color);
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                var sizeString = g.MeasureString(this.text, font);
                size.Width = (int) sizeString.Width;
                size.Height = (int) sizeString.Height;
                this.x -= size.Width / 2;

            }
        }


        public override ParticleEffect CreateEffect(int count)
        {
            var particles = new ParticleEffect(40);
            particles.Add(this);
            return particles;

        }

        public override void Draw(Graphics g, Point cameraOffSet)
        {
            g.FillRectangle(Brushes.White, new Rectangle((int)x - 5 + cameraOffSet.X, (int)y - 5 + cameraOffSet.Y, size.Width + 10, size.Height + 10));
            g.DrawString(text, font, brush, (int)x + cameraOffSet.X, (int)y + cameraOffSet.Y);
        }

        public override void Update()
        {
            y-=2;
        }

    }

    public class Blood : Particle
    {
        public Blood(double x, double y) : base(x, y) { }
        public override ParticleEffect CreateEffect(int count)
        {
            return new ParticleEffect(count);
        }


    }
}

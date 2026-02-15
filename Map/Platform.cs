using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class PlatformsMeneger : IEnumerable<Platform>
    {
        public IEnumerator<Platform> GetEnumerator()
        {
            return platforms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        List<Platform> platforms;
        public PlatformsMeneger()
        {
            platforms = new List<Platform>();
        }

        public void Add(Platform platform)
        {
            platforms.Add(platform);
        }

        public void Draw(Graphics g, Point cameraOffSet)
        {
            foreach (Platform platform in platforms) 
                platform.Draw(g, cameraOffSet);
        }

        public void Update()
        {
            foreach(Platform platform in platforms) 
                platform.Update();
        }

    }

    public abstract class Platform
    {
        public double X, Y;

        public Size size = new Size(64,64);

        public TexturePlatform Texture = new TexturePlatform();

        public Platform(double x, double y)
        {
            X = x;
            Y = y;
        }

        public virtual void Draw(Graphics g, Point cameraOffSet)
        {
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            var image = Texture.GetImage();

            if (image == null)
                return;

            g.DrawImage(
                       image,
                       new Rectangle(
                           (int) X + cameraOffSet.X, (int)Y + cameraOffSet.Y, size.Width, size.Height
                       )
                   );
        }  

        public virtual void Update()
        {

        }

    }

    public class Grass : Platform
    {
        static TexturePlatform BaseTexture = new TexturePlatform();
        static Grass()
        {
            Image source = Properties.Resources.platforms;
            BaseTexture.Add(ImageCreator.CreateImage(source,0,0,24,24), "cornerLeft");
            BaseTexture.Add(ImageCreator.CreateImage(source, 24, 0, 24, 24), "up0");
            BaseTexture.Add(ImageCreator.CreateImage(source, 48, 0, 24, 24), "up1");
            BaseTexture.Add(ImageCreator.CreateImage(source, 72, 0, 24, 24), "cornerRight");
            BaseTexture.Add(ImageCreator.CreateImage(source, 0, 24, 24, 24), "left");
            BaseTexture.Add(ImageCreator.CreateImage(source, 24, 24, 24, 24), "mid");
            BaseTexture.Add(ImageCreator.CreateImage(source, 72, 24, 24, 24), "right");
            BaseTexture.Add(ImageCreator.CreateImage(source, 0, 72, 24, 24), "cornerLeftDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 72, 72, 24, 24), "cornerRightDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 24, 72, 24, 24), "down0");
            BaseTexture.Add(ImageCreator.CreateImage(source, 48, 72, 24, 24), "down1");
            BaseTexture.Add(ImageCreator.CreateImage(source, 240, 0, 24, 24), "only");
            BaseTexture.Add(ImageCreator.CreateImage(source, 144, 48, 24, 24), "onlyUp");
            BaseTexture.Add(ImageCreator.CreateImage(source, 120, 72, 24, 24), "onlyLeft");
            BaseTexture.Add(ImageCreator.CreateImage(source, 168, 72, 24, 24), "onlyRight");
            BaseTexture.Add(ImageCreator.CreateImage(source, 144, 120, 24, 24), "onlyDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 144, 96, 24, 24), "upDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 288, 192, 24, 24), "leftRight");
        }

        public Grass(double x, double y) : base(x,y)
        {
            Texture = BaseTexture.Clone();
            Texture.State = "up0";
        }

    }

    public class Stone : Platform
    {
        static TexturePlatform BaseTexture = new TexturePlatform();
        static Stone()
        {
            Image source = Properties.Resources.platforms;
            BaseTexture.Add(ImageCreator.CreateImage(source, 120, 0, 24, 24), "cornerLeft");
            BaseTexture.Add(ImageCreator.CreateImage(source, 144, 0, 24, 24), "up0");
            BaseTexture.Add(ImageCreator.CreateImage(source, 168, 0, 24, 24), "up1");
            BaseTexture.Add(ImageCreator.CreateImage(source, 192, 0, 24, 24), "cornerRight");
            BaseTexture.Add(ImageCreator.CreateImage(source, 0, 24, 24, 24), "left");
            BaseTexture.Add(ImageCreator.CreateImage(source, 24, 24, 24, 24), "mid");
            BaseTexture.Add(ImageCreator.CreateImage(source, 72, 24, 24, 24), "right");
            BaseTexture.Add(ImageCreator.CreateImage(source, 0, 72, 24, 24), "cornerLeftDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 72, 72, 24, 24), "cornerRightDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 24, 72, 24, 24), "down0");
            BaseTexture.Add(ImageCreator.CreateImage(source, 48, 72, 24, 24), "down1");
            BaseTexture.Add(ImageCreator.CreateImage(source, 288, 0, 24, 24), "only");
            BaseTexture.Add(ImageCreator.CreateImage(source, 240, 48, 24, 24), "onlyUp");
            BaseTexture.Add(ImageCreator.CreateImage(source, 216, 72, 24, 24), "onlyLeft");
            BaseTexture.Add(ImageCreator.CreateImage(source, 264, 72, 24, 24), "onlyRight");
            BaseTexture.Add(ImageCreator.CreateImage(source, 144, 120, 24, 24), "onlyDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 144, 96, 24, 24), "upDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 288, 240, 24, 24), "leftRight");
        }

        public Stone(double x, double y) : base(x, y)
        {
            Texture = BaseTexture.Clone();
            Texture.State = "up0";
        }
    }

    public class Dirt : Platform
    {
        static TexturePlatform BaseTexture = new TexturePlatform();
        static Dirt()
        {
            Image source = Properties.Resources.platforms;
            BaseTexture.Add(ImageCreator.CreateImage(source, 336, 0, 24, 24), "cornerLeft");
            BaseTexture.Add(ImageCreator.CreateImage(source, 360, 0, 24, 24), "up0");
            BaseTexture.Add(ImageCreator.CreateImage(source, 384, 0, 24, 24), "up1");
            BaseTexture.Add(ImageCreator.CreateImage(source, 408, 0, 24, 24), "cornerRight");
            BaseTexture.Add(ImageCreator.CreateImage(source, 336, 24, 24, 24), "left");
            BaseTexture.Add(ImageCreator.CreateImage(source, 360, 24, 24, 24), "mid");
            BaseTexture.Add(ImageCreator.CreateImage(source, 408, 24, 24, 24), "right");
            BaseTexture.Add(ImageCreator.CreateImage(source, 336, 72, 24, 24), "cornerLeftDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 408, 72, 24, 24), "cornerRightDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 360, 72, 24, 24), "down0");
            BaseTexture.Add(ImageCreator.CreateImage(source, 384, 72, 24, 24), "down1");
            BaseTexture.Add(ImageCreator.CreateImage(source, 360, 192, 24, 24), "only");
            BaseTexture.Add(ImageCreator.CreateImage(source, 360, 240, 24, 24), "onlyUp");
            BaseTexture.Add(ImageCreator.CreateImage(source, 384, 312, 24, 24), "onlyLeft");
            BaseTexture.Add(ImageCreator.CreateImage(source, 408, 312, 24, 24), "onlyRight");
            BaseTexture.Add(ImageCreator.CreateImage(source, 360, 264, 24, 24), "onlyDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 360, 120, 24, 24), "upDown");
            BaseTexture.Add(ImageCreator.CreateImage(source, 360, 192, 24, 24), "leftRight");
        }

        public Dirt(double x, double y) : base(x, y)
        {
            Texture = BaseTexture.Clone();
            Texture.State = "up0";
        }
    }

}

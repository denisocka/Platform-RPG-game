using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public static class ImageCreator
    {
        public static Image CreateImage(Image source, Point from, Size size)
        {
            Bitmap result = new Bitmap(size.Width, size.Height);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(
                    source,
                    new Rectangle(0, 0, size.Width, size.Height),
                    new Rectangle(from.X, from.Y, size.Width, size.Height),
                    GraphicsUnit.Pixel
                );
            }

            return result;
        }

        public static Image CreateImage(Image source, int x, int y , int width, int height)
        {
            Bitmap result = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(
                    source,
                    new Rectangle(0, 0, width, height),
                    new Rectangle(x, y, width, height),
                    GraphicsUnit.Pixel
                );
            }

            return result;
        }

        public static Image ResizeImage(Image source, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                g.DrawImage(source, 0, 0, width, height);
            }

            return result;
        }

        public static Bitmap StretchImage(Image original, int newWidth, int newHeight, bool pixelPerfect = true)
        {
            Bitmap stretched = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(stretched))
            {
                g.InterpolationMode = pixelPerfect
                                      ? InterpolationMode.NearestNeighbor
                                      : InterpolationMode.HighQualityBicubic;

                g.DrawImage(original, new Rectangle(0, 0, newWidth, newHeight));
            }

            return stretched;
        }

    }

    public class Sprite
    {
        List<Image> Frames;

        public Size size = new Size(500,500);

        int Speed;
        int Count;
        int score;

        int scoreSpeed;

        int countPlay;

        public bool IsSpritePlay = false;

        public bool IsPause = false;

        public Sprite()
        {
            Frames = new List<Image>();
        }

        public Sprite(Image source, int count, int speed= 5, Size size = default)
        {
            var resulr = new List<Image>();
            int xSize = source.Size.Width / count;
            Size sizeFrame = new Size(xSize, source.Size.Height);

            for (int j = 0; j < count; j++)
            {
                int x = j * xSize;

                resulr.Add(ImageCreator.CreateImage(source, new Point(x, 0), sizeFrame));
            }

            Frames = resulr;
            Speed = speed;
            Count = count;
            this.size = size;
        }

        public Sprite(List<Image> frames, int speed = 5, Size size = default)
        {
            Frames = frames;
            Speed = speed;
            Count = frames.Count;
        }

        public Image GetImage()
        {
            return Frames[score];
        }

        public void PlaySprite(int count = 1)
        {
            countPlay = count;
            IsSpritePlay = true;
            score = 0;
            scoreSpeed = 0;
        }

        public Sprite Clone()
        {
            Sprite sprite = new Sprite();
            sprite.Count = this.Count;
            sprite.Speed = this.Speed;
            sprite.score = 0;
            sprite.scoreSpeed = 0;
            sprite.countPlay = 0;

            List<Image> frames = new List<Image>();

            foreach (var frame in Frames)
                frames.Add(new Bitmap(frame));

            sprite.Frames = frames;

            return sprite;
        }

        public void Update()
        {
            if (!IsSpritePlay)
                return;

            if (IsPause)
                return;

            scoreSpeed++;

            if (scoreSpeed < Speed)
                return;

            scoreSpeed = 0;

            score++;

            if (score >= Count)
            {
                score = 0;

                if (countPlay != int.MaxValue)
                {
                    countPlay--;

                    if (countPlay <= 0)
                    {
                        IsSpritePlay = false;
                    }
                }
            }
        }
    }

    public class TextureManager
    {
        List<Sprite> sprites = new List<Sprite>();
        List<Image> images = new List<Image>();
        int ActiveImage = 0;

        public Size sizeImage = new Size(500, 500);

        public void SetSize(Size size)
        {
            sizeImage = size;
        }

        public TextureManager Clone()
        {
            var tm = new TextureManager();
            tm.sizeImage = sizeImage;

            tm.images = images
                .Select(i => (Image)new Bitmap(i))
                .ToList();
            tm.sprites = sprites.Select(s => s.Clone()).ToList();

            return tm;
        }

        public Image GetImage()
        {
            if (!sprites.Any(e => e.IsSpritePlay))
                return images[ActiveImage];
            else
                return
                    sprites.FirstOrDefault(e => e.IsSpritePlay).GetImage();
        }

        public void PauseAnimation(int number)
        {
            sprites[number].IsPause = true;
        }

        public void ResumeAnimation(int number)
        {
            sprites[number].IsPause = false;
        }

        public void SetActiveImage(int index)
        {
            if (index < images.Count)
                ActiveImage = index;
        }

        public Size GetSize()
        {
            if (!sprites.Any(e => e.IsSpritePlay))
                return sizeImage;
            else
                return
                    sprites.FirstOrDefault(e => e.IsSpritePlay).size;
        }

        public void PlayAnimation(int number, int count = 1)
        {
            sprites.ForEach(x => x.IsSpritePlay = false);
            sprites[number].PlaySprite(count);
        }

        public void PlayingWhileDontStop(int number)
        {
            if (!sprites[number].IsSpritePlay)
            {
                sprites.ForEach(x => x.IsSpritePlay = false);
                sprites[number].PlaySprite(int.MaxValue);
            }
        }

        public void StopPlaying(int number)
        {
            sprites[number].IsSpritePlay = false;
        }

        public void AddSprite(Sprite sprite)
        {
            sprites.Add(sprite);
        }

        public void AddSprite(Image source, int count, int speed = 5, Size size = default)
        {
            sprites.Add(new Sprite(source, count, speed,size));
        }

        public void AddSprite(List<Image> frames, int speed = 5, Size size = default)
        {
            sprites.Add(new Sprite(frames, speed));
        }

        public void AddImage(Image image)
        {
            images.Add(image);
        }

        public void Update()
        {
            sprites.ForEach(e => e.Update());
        }

        public bool IsAnimation(int number)
        {
            return sprites[number].IsSpritePlay;
        }

    }

    public class TexturePlatform
    {
        Dictionary<String, Image> textures;
        public String State = "";
        public TexturePlatform()
        {
            textures = new Dictionary<String, Image>();
        }

        public void Add(Image image, String name)
        {
            if (textures.ContainsKey(name))
                textures.Add(name, image);
            textures[name] = image;
        }

        public void Remove(String name)
        {
            textures.Remove(name);
        }

        public Image GetImage()
        {
            if (State != null && textures.ContainsKey(State))
                return textures[State];
            return null;
        }

        public Image GetImage(String name)
        {
            if (textures.ContainsKey(name))
                return textures[name];
            return null;
        }

        public TexturePlatform Clone()
        {
            var storage = new TexturePlatform();
            storage.textures = textures.ToDictionary();
            storage.State = State.ToString();
            return storage;
        }

        public void SetState(String name)
        {
            if (textures.ContainsKey(name))
                State = name;
        }

    }

} 
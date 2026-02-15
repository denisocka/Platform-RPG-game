using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace game
{
    public class Map
    {
        public PlatformsMeneger platforms = new PlatformsMeneger();
        Platform[,] platformsArr;
        public ItemMeneger Items = new ItemMeneger();

        Random rnd = new Random();

        public EntityMeneger mobs = new EntityMeneger();

        public Map() { }
        public Player player;
        public ParticleMenger effects = new ParticleMenger();


        public Map(char[,] map)
        {
            CreatePlatforms(map);
            SmoothPlatforms();

            Items = new ItemMeneger(this);
            mobs.Add(new CloseCombatEnuty(CloseCombatMobs.Orc_Glutton, 500, 200, this));
        }

        public void CreatePlatforms(char[,] map)
        {
            platformsArr = new Platform[map.GetLength(0),map.GetLength(1)];

            for (int i = 0; i< map.GetLength(0);i++) 
                for (int j = 0; j < map.GetLength(1);j++)
                    switch (map[i,j])
                    {
                        case 's':
                            var platform = new Grass(j * 64, i * 64);
                            platforms.Add(platform);
                            platformsArr[i, j] = platform;
                            break;
                        case 'g':
                            var platform1 = new Stone(j * 64, i * 64);
                            platforms.Add(platform1);
                            platformsArr[i, j] = platform1;
                            break;
                        case 'd':
                            var platform2 = new Dirt(j * 64, i * 64);
                            platforms.Add(platform2);
                            platformsArr[i, j] = platform2;
                            break;
                        case 'c':
                            this.mobs.Add(new Chest(j*64,i*64,this));
                            break;
                    }

        }

        public void SmoothPlatforms()
        {
            int rows = platformsArr.GetLength(0);
            int cols = platformsArr.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var p = platformsArr[y, x];
                    if (p == null) continue;

                    bool up = IsSameType((y - 1, x), (y - 1, x), (y - 1, x));
                    bool down = IsSameType((y + 1, x), (y + 1, x), (y + 1, x));
                    bool left = IsSameType((y, x - 1), (y, x - 1), (y, x - 1));
                    bool right = IsSameType((y, x + 1), (y, x + 1), (y, x + 1));

                    if (!left && !up && down && right) p.Texture.SetState("cornerLeft");
                    else if (!right && !up && down && left) p.Texture.SetState("cornerRight");
                    else if (up && right && down && left) p.Texture.SetState("mid");
                    else if (right && up && down && !left) p.Texture.SetState("left");
                    else if (left && up && down && !right) p.Texture.SetState("right");
                    else if (up && left && right && !down) p.Texture.SetState("down0");
                    else if (!down && !left && up && right) p.Texture.SetState("cornerLeftDown");
                    else if (!down && !right && left && up) p.Texture.SetState("cornerRightDown");

                    else if (!down && !right && !left && !up) p.Texture.SetState("only");
                    else if(down && !up & !left & !right) p.Texture.SetState("onlyUp");
                    else if (!down && !up & !left & right) p.Texture.SetState("onlyLeft");
                    else if (!down && !up & left & !right) p.Texture.SetState("onlyRight");
                    else if (!down && up & !left & !right) p.Texture.SetState("onlyDown");
                    else if (down && up & !left & !right) p.Texture.SetState("upDown");
                    else if (!down && !up & left & right) p.Texture.SetState("leftRight");


                }
            }
        }

        public bool IsSameType((int y, int x) from, params (int y, int x)[] to)
        {
            int rows = platformsArr.GetLength(0);
            int cols = platformsArr.GetLength(1);

            if (from.y < 0 || from.y >= rows || from.x < 0 || from.x >= cols) return false;
            if (platformsArr[from.y, from.x] == null) return false;

            foreach (var p in to)
            {
                if (p.y < 0 || p.y >= rows || p.x < 0 || p.x >= cols) return false;
                if (platformsArr[p.y, p.x] == null) return false;
            }

            return true;
        }

        

        public void CollisisonItems()
        {
            double X = player.X; 
            double Y = player.Y;

            for (int i=0; i< Items.Count(); i++)
            {
                var item = Items[i];
                double x = item.X;
                double y = item.Y;

                if (X + player.Size.Width > x &&  
                    X < x + item.Size.Width &&    
                    Y + player.Size.Height > y &&  
                    Y < y + item.Size.Height)
                {
                    player.inv.AddItem(item);
                    return;
                }

            }

        }

        public void Draw(Graphics g, Point cameraOffSet)
        {

            platforms.Draw(g, cameraOffSet);
            mobs.Draw(g, cameraOffSet);
            Items.Draw(g, cameraOffSet);
            effects.Draw(g, cameraOffSet);
        }

        public void Update()
        {
            mobs.Update();
            CollisisonItems();
            Items.Update();
            effects.Update();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    class Chest : Entity
    {
        static TextureManager Icon = new TextureManager();
        
        static Chest()
        {
            Icon.AddImage(ImageCreator.CreateImage(Properties.Resources.items2, 320, 0, 64, 64));
            Icon.SetSize(new Size(64, 64));
        }

        public Chest(int x, int y, Map map) : base(x, y, map)
        {
            Speed = 0;
            health= 20;
            texture = Icon;
            Size = new Size(64, 64);
        }

        public override void DropItem()
        {
            

        }
        

    }
}

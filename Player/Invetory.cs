using Microsoft.VisualBasic.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Reflection.Metadata.BlobBuilder;

namespace game
{
    public class Invetory : IEnumerable<Slot>
    {
        public bool IsShiftDown = false;
        public IEnumerator<Slot> GetEnumerator()
        {
            foreach (var slot in hotBar.slots)
                yield return slot;

            foreach (var slot in backPack.slots)
                yield return slot;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Size WinSize = new Size(1400, 768);

        public BackPack backPack = new BackPack();
        public HotBar hotBar = new HotBar();

        Image Icon = Properties.Resources.Inventory;

        Size size = new Size(854, 492);

        public bool IsOpen = false;

        public int CountSlots() => hotBar.slots.Count() + backPack.slots.GetLength(0) * backPack.slots.GetLength(1);

        public Slot this[int index]
        {
            get
            {
                return GetSlot(index);
            }
            set
            {
                var slot = GetSlot(index);
                slot = value;
            }
        }

        public void AddItem(Item item)
        {
            var newItem = item.Clone();
            Slot slot;

            slot = CanAddInSlot(newItem);
            if (slot != null)
            {
                slot.count++;
                item.Delete();
                return;
            }

            slot = GetFreeSlot();
            if (slot != null)
            {
                slot.item = newItem;
                newItem.IsInWorld = false;
                slot.count = 1;
                item.Delete();
            }
        }

        public bool AddItem(Item item, int index, int count = 1)
        {
            var slot = GetSlot(index);

            item.IsInWorld = false;
            if (slot.item != null)
                if (slot.item.name == item.name && slot.count + count <= slot.item.MaxCount)
                {
                    slot.count += count;
                    return true;
                }

            if (slot.item == null)
            {
                slot.item = item;
                slot.count = count;
                return true;
            }

            return false;

        }

        public Slot GetSlot(int index)
        {
            if (index >= 0 && index < 10)
                return hotBar.slots[index];
            if (index >= 10 && index < 34)
            {
                int j = (index - 10) % 6;
                int i = (index - 10) / 6;

                return backPack.slots[i,j];
            }
            return null;
        }

        public Slot IsItemHavePlayer(Item item)
        {
            foreach (var slot in hotBar.slots)
                if (slot.item != null)
                    if (slot.item.name == item.name)
                        return slot;
            return null;
        }

        public Slot CanAddInSlot(Item item)
        {
            foreach (var slot in hotBar.slots)
                if (slot.item != null)
                    if (slot.count < slot.item.MaxCount)
                        if (slot.item.name == item.name)
                            return slot;
            return null;
        }

        public Slot GetFreeSlot()
        {
            
            foreach(var Slot in hotBar.slots) 
                if (Slot.item == null)
                    return Slot;

            foreach (var Slot in backPack.slots)
                if (Slot.item == null)
                    return Slot;

            return null;
        }

        public void SlotSwap(int index1, int index2)
        {
            Slot slot1 = GetSlot(index1);
            Slot slot2 = GetSlot(index2);

            var item = slot1.item;
            var count = slot1.count;
            slot1.item = slot2.item;
            slot1.count = slot2.count;

            slot2.item = item;
            slot2.count = count;
        }

        public void SlotSwap(Slot slot1, Slot slot2)
        {
            var item = slot1.item;
            var count = slot1.count;
            slot1.item = slot2.item;
            slot1.count = slot2.count;

            slot2.item = item;
            slot2.count = count;
        }

        public Item GetActiveItem()
        {
            return hotBar.slots[hotBar.ActiveSlot].item;
        }


        public void Open()
        {
            IsOpen = true;
        }

        public void Close()
        {
            IsOpen = false;
        }

        Font font = new Font("Arial", 16);
        Brush brush = Brushes.White;

        public void Draw(Graphics g, Point cameraOffSet, Player player)
        {
            if (!IsOpen)
                hotBar.Draw(g, cameraOffSet, player);

            if (IsOpen)
            {
                g.DrawImage(Icon, new Rectangle(WinSize.Width/2 - size.Width/2,WinSize.Height/2 - size.Height/2 ,size.Width,size.Height));
                
                var SizePlayer = player.Texture.GetSize();
                g.DrawImage(player.Texture.GetImage(), new Rectangle(170,140,(int)(SizePlayer.Width * 1.1), (int)(SizePlayer.Height*1.1)));

                foreach (var slot in this)
                    if (slot.IsClick)
                        g.DrawImage(Slot.icon, new Rectangle(slot.x,slot.y,slot.size.Width, slot.size.Height) );

                hotBar.DrawInInventory(g);
                backPack.DrawInInventory(g);

                foreach (var slot in this)
                    if (slot.IsClick && slot.item != null)
                    {
                        Brush brush = new SolidBrush(slot.item.colorName);
                        SizeF size = g.MeasureString(slot.item.name, font);
                        g.DrawImage(Properties.Resources.sign, new Rectangle(slot.x - (int)size.Width / 2 + slot.size.Width / 2 - 5, slot.y - 10 - 5, (int)size.Width+ 5, (int)size.Height+5));
                        g.DrawString(slot.item.name, font, brush, new Point(slot.x - (int) size.Width/2 + slot.size.Width/2, slot.y - 10));

                    }

                if (itemMooving)
                {
                    g.DrawImage(ItemReserv.image, new Rectangle(MousePosition.X - pointClick.X, MousePosition.Y - pointClick.Y, 64, 64));
                }
            }
        }

        public Point MousePosition;


        Point pointClick;
        Slot SlotReserv;
        Item ItemReserv;
        int CountReserv;

        public void MouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.IsOpen)
            {
                if (IsShiftDown)
                    ItemShiftMove();

                if (!itemMooving)
                    SlotActiving(e);
                else
                if (!IsShiftDown)
                {
                    ItemMove(e);
                    return;
                }

                
            }
        }

        public void SlotActiving(MouseEventArgs e)
        {
            var slot = this.FirstOrDefault(x => x.IsClick);
            if (slot == null)
                return;
            if (slot.item == null)
                return;

            pointClick.X = (int)MousePosition.X - slot.x - 8;
            pointClick.Y = (int)MousePosition.Y - slot.y - 8;

            ItemReserv = slot.item.Clone();
            CountReserv = slot.count;
            SlotReserv = slot;

            slot.item = null;
            slot.count = 0;

            itemMooving = true;
            return;
        }

        public void ItemMove(MouseEventArgs e)
        {
            var slot = this.FirstOrDefault(x => x.IsClick);
            if (slot == null)
                return;
            if (slot.item != null)
                if (slot.item.name == ItemReserv.name)
                {
                    var countCanTake = slot.item.MaxCount - slot.count;
                    if (countCanTake < CountReserv)
                    {
                        slot.count += countCanTake;
                        CountReserv -= countCanTake;
                        return;
                    }
                    slot.count += CountReserv;
                    itemMooving = false;
                    return;
                }
            if (slot.item != null)
            {
                Item itemR = slot.item;
                int countR = slot.count;

                slot.item = ItemReserv;
                slot.count = CountReserv;

                ItemReserv = itemR;
                CountReserv = countR;
                return;
            }
            SlotSwap(slot, SlotReserv);
            slot.item = ItemReserv;
            slot.count = CountReserv;
            itemMooving = false;
        }

        public void ItemShiftMove()
        {
            var slot = this.FirstOrDefault(x => x.IsClick);
            if (slot == null) return;
            if (slot.item == null) 
                return;
            if (slot.index < 10)
            {
                for (int i = 10; i < this.CountSlots(); i++)
                {
                    if (GetSlot(i).item != null)
                        if (GetSlot(i).item.name == slot.item.name)
                        {
                            int maxAdd = GetSlot(i).item.MaxCount - GetSlot(i).count;
                            if (maxAdd > slot.count)
                            {
                                GetSlot(i).count += slot.count;
                                slot.item = null;
                                return;
                            }
                            else
                            {
                                GetSlot(i).count += maxAdd;
                                slot.count -= maxAdd;
                                if (slot.count <= 0)
                                {
                                    slot.item = null;
                                    return;
                                }
                            }
                        }
                }

                for (int i = 10;i < this.CountSlots();i++)
                    if (GetSlot(i).item == null)
                        SlotSwap(slot,GetSlot(i));
            }

            if (slot.index >= 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (GetSlot(i).item != null)
                        if (GetSlot(i).item.name == slot.item.name)
                        {
                            int maxAdd = GetSlot(i).item.MaxCount - GetSlot(i).count;
                            if (maxAdd > slot.count)
                            {
                                GetSlot(i).count += slot.count;
                                slot.item = null;
                                return;
                            }
                            else
                            {
                                GetSlot(i).count += maxAdd;
                                slot.count -= maxAdd;
                                if (slot.count <= 0)
                                {
                                    slot.item = null;
                                    return;
                                }
                            }
                        }
                }

                for (int i = 0; i < 10; i++)
                    if (GetSlot(i).item == null)
                        SlotSwap(slot, GetSlot(i));
            }

        }


        public void MouseUp(MouseEventArgs e)
        {

        }

        bool itemMooving = false;

        public void ItemMove()
        {
            

        }

        public void Update()
        {
            foreach (var slot in this)
            {
                slot.IsClick = slot.Contains(MousePosition);
                if (slot.item != null)
                    slot.item.Update();
            }
        }

    }

    public class Slot
    {
        public bool IsClick;
        public int index;
        public Size size = new Size(84,84);
        static public Image icon = Properties.Resources.SlotActive;
        public Item item;

        public int count;

        public int x, y;

        public bool Contains(Point mouse)
        {
            return mouse.X >= x + 5&&
                   mouse.Y >= y + 5 &&
                   mouse.X <= x + size.Width - 5 &&
                   mouse.Y <= y + size.Height -5 ;
        }

    }


    public class BackPack
    {
        public Size WinSize = new Size(1400, 768);

        public Slot[,] slots = new Slot[4, 6];
        public BackPack()
        {
            for (int i = 0; i < slots.GetLength(0); i++)
                for (int j = 0; j < slots.GetLength(1); j++)
                    slots[i, j] = new Slot()
                    {
                        x = 618 + j * (64 + 16),
                        y = i * (64 + 16) + 281,
                        index = 10 + j + i * 6
                    };

        }

        public bool AddItem(Item item, int x, int y,int count = 1)
        {
            var slot = slots[y,x];
            item.IsInWorld = false;
            if (slot.item != null)
                if (slot.item.name == item.name && slot.count  + count <= slot.item.MaxCount)
                {
                    slot.count += count;
                    return true;
                }

            if (slot.item == null)
            {
                slot.item = item;
                slot.count = count;
                return true;
            }

            return false;

        }

        Font font = new Font("Arial", 16);
        Brush brush = Brushes.White;

        public void DrawInInventory(Graphics g)
        {
            for (int i = 0;i < slots.GetLength(0); i++)
                for (int j = 0;j < slots.GetLength(1); j++)
                {
                    if (slots[i, j].item != null)
                    {
                        g.DrawImage(slots[i, j].item.image,
                                new Rectangle(628 + j * (64 + 16), i * (64 + 16) + 292, 64, 64));
                        SizeF sizeString = g.MeasureString(slots[i,j].count.ToString(), font);
                        if (slots[i, j].count > 1)
                            g.DrawString(slots[i, j].count.ToString(), font, brush, new Point(628 + j * (64 + 16) + 60 - (int)sizeString.Width / 2, i * (64 + 16) + 292 + 44));
                    }
                }
        }
    }

    public class HotBar
    {
        public int ActiveSlot;

        public Size WinSize = new Size(1400, 768);

        public Slot[] slots = new Slot[10];
        public HotBar()
        {
            for (int i = 0; i < slots.GetLength(0); i++)
                slots[i] = new Slot()
                {
                    x = WinSize.Width / 2 - size.Width / 2 + i * (64 + 16) + 25,
                    y = WinSize.Height / 2 - size.Height / 2 + 19,
                    index = i
                };

        }

        static Image icon = Properties.Resources.Inventory_Bar;

        int y = 16;
        Font font = new Font("Arial", 16);
        Brush brush = Brushes.White;
        Size size = new Size(854, 480);

        public bool AddItem(Item item, int x, int count = 1)
        {
            var slot = slots[x];
            item.IsInWorld = false;
            if (slot.item != null)
                if (slot.item.name == item.name && slot.count + count <= slot.item.MaxCount)
                {
                    slot.count += count;
                    return true;
                }

            if (slot.item == null)
            {
                slot.item = item;
                slot.count = count;
                return true;
            }

            return false;

        }

        public void DrawInInventory(Graphics g)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    g.DrawImage(slots[i].item.image,
                                new Rectangle(WinSize.Width / 2 - size.Width / 2 + i * (64 + 16) + 35, WinSize.Height / 2 - size.Height / 2 + 30, 64, 64));
                    SizeF sizeString = g.MeasureString(slots[i].count.ToString(), font);

                    if (slots[i].count > 1)

                        g.DrawString(slots[i].count.ToString(), font, brush, new Point(WinSize.Width / 2 - size.Width / 2 + 45 + i * (64 + 16) + 50 - (int)sizeString.Width / 2, WinSize.Height / 2 - size.Height / 2 + 30 + 44));
                }
            }
        }

        public void Draw(Graphics g, Point cameraOffSet, Player player)
        {
            if (player.Y + cameraOffSet.Y + player.Size.Height > 300)
                y = 16;
            if (player.Y + cameraOffSet.Y < 100)
                y = 620;

            using (var attributesBar = new System.Drawing.Imaging.ImageAttributes())
            using (var attributesItem = new System.Drawing.Imaging.ImageAttributes())
            using (var attriActiveSlot = new System.Drawing.Imaging.ImageAttributes())
            {
                var matrixBar = new System.Drawing.Imaging.ColorMatrix();
                matrixBar.Matrix33 = 0.6f; 
                attributesBar.SetColorMatrix(matrixBar, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                var matrixItem = new System.Drawing.Imaging.ColorMatrix();
                matrixItem.Matrix33 = 0.85f;
                attributesItem.SetColorMatrix(matrixItem, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                var matrixActive = new System.Drawing.Imaging.ColorMatrix();
                matrixActive.Matrix33 = 0.9f;
                attriActiveSlot.SetColorMatrix(matrixBar, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                g.DrawImage(icon, new Rectangle(29, y, 854, 134),
                            0, 0, icon.Width, icon.Height,
                            GraphicsUnit.Pixel,
                            attributesBar);

                if (slots[ActiveSlot].item != null)
                {
                    g.DrawImage(slots[ActiveSlot].item.image, new Rectangle(64 + ActiveSlot * (64 + 16), y + 34, 64, 64));

                    Brush brush = new SolidBrush(slots[ActiveSlot].item.colorName);
                    SizeF sizeSign = g.MeasureString(slots[ActiveSlot].item.name, font);

                    if (y == 16)
                    {
                        g.DrawImage(Properties.Resources.sign, new Rectangle(70 + ActiveSlot * (80) - (int)sizeSign.Width / 2 + 26 - 5, y + 130 - 5, (int)sizeSign.Width + 5, (int)sizeSign.Height + 5));
                        g.DrawString(slots[ActiveSlot].item.name, font, brush, new Point(64 + ActiveSlot * (80) + 5 - (int)sizeSign.Width / 2 + 26, y + 130));

                    }
                    else
                    {
                        g.DrawImage(Properties.Resources.sign, new Rectangle(70 + ActiveSlot * (80) - (int)sizeSign.Width / 2 + 26 - 5, y - 15 - 5, (int)sizeSign.Width + 5, (int)sizeSign.Height + 5));
                        g.DrawString(slots[ActiveSlot].item.name, font, brush, new Point(64 + ActiveSlot * (80) + 5 - (int)sizeSign.Width / 2 + 26, y + -15));
                    }

                }

                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].item != null)
                    {
                        if (i != ActiveSlot)
                        g.DrawImage(slots[i].item.image,
                                    new Rectangle(64 + i * (64 + 16), y + 34, 64, 64),
                                    0, 0, slots[i].item.image.Width, slots[i].item.image.Height,
                                    GraphicsUnit.Pixel,
                                    attributesItem);

                        SizeF sizeString = g.MeasureString(slots[i].count.ToString(), font);

                        if (slots[i].count > 1)
                            g.DrawString(slots[i].count.ToString(), font, brush, new Point(64 + i * (64 + 16) + 55 - (int)sizeString.Width / 2, y + 36  + 44));
                    }
                }
                Image iconActive = Properties.Resources.SlotActive;
                g.DrawImage(iconActive, new Rectangle(54 + ActiveSlot * (64 + 16), y + 25, 84, 84));
                


            }
        }


    }

}

using game;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Numerics;

namespace game
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer timer;

        public Size WinSize = new Size(1400, 768);

        private Random rnd = new Random();

        public Point cameraOffset;

        Bitmap scaledBackground1;
        Bitmap scaledBackground2;
        Bitmap scaledBackground3;
        Bitmap scaledBackground;

        Map map;
        Player Player;
        GameButton ButtonRestart;


        public Form1()
        {
            InitializeComponent();
            Initialize();
            InitializeButtonRestart();



            this.DoubleBuffered = true;
            this.KeyPreview = true;

            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;
            this.MouseMove += OnMouseMove;
            this.MouseWheel += Form_MouseWheel;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 10; 
            timer.Tick += Timer_Tick;

        }

        public void InitializeButtonRestart()
        {
            ButtonRestart = new GameButton()
            {
                Bounds = new Rectangle(this.ClientSize.Width / 2 - 200, this.ClientSize.Height/2 - 70, 400, 140),
                Icon = Properties.Resources.but_restart,
                OnClick = StartGame
            };
        }

        private void Initialize()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(WinSize.Width, WinSize.Height);
            this.Text = "игруха";

            scaledBackground = ImageCreator.StretchImage(Properties.Resources.background4, ClientSize.Width + 5, ClientSize.Height + 5);

            scaledBackground1 = ImageCreator.StretchImage(Properties.Resources.background, ClientSize.Width + 5, ClientSize.Height + 5);
            scaledBackground2 = ImageCreator.StretchImage(Properties.Resources.background1, ClientSize.Width + 5, ClientSize.Height + 5);
            scaledBackground3 = ImageCreator.StretchImage(Properties.Resources.background2, ClientSize.Width + 5, ClientSize.Height + 5);

            map = new Map(MapMaker.Map1());
            Player = new Player(this.ClientSize.Width/2 - 40, 485, map);

        }

        bool gameStarted = false;

        void StartGame()
        {
            map = new Map(MapMaker.Map1());
            Player = new Player(this.ClientSize.Width / 2 - 40, 485, map);
            map.player = Player;

            gameStarted = true;
            timer.Start();

            Player.inv.AddItem(new SimpleItem(DefaultItems.iron_ingot),21,2);
            Player.inv.AddItem(new SimpleItem(DefaultItems.iron_ingot), 33, 2);
            Player.inv.AddItem(new SimpleItem(DefaultItems.iron_ingot), 10, 2);
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {   
            if (Player != null)
                Player.inv.MousePosition = e.Location;
        }

        private void Form_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                Player.inv.hotBar.ActiveSlot--;
                if (Player.inv.hotBar.ActiveSlot < 0)
                    Player.inv.hotBar.ActiveSlot = 9;

            }
            else if (e.Delta < 0)
            {
                Player.inv.hotBar.ActiveSlot++;
                if (Player.inv.hotBar.ActiveSlot > 9)
                    Player.inv.hotBar.ActiveSlot = 0;

            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!gameStarted)
            {
                ButtonRestart.HandleMouseDown(e.Location);
            }
            Player.inv.MouseDown(e);
            Player.MousePress(e);
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            Player.MouseUp(e);
            Player.inv.MouseUp(e);
        }

        public void BackGroundDraw(Graphics g)
        {
            g.DrawImage(scaledBackground1, 0, 0);
            g.DrawImage(scaledBackground2, cameraOffset.X/4, 0);
            g.DrawImage(scaledBackground3, cameraOffset.X / 2, 0);

        }

        protected override void OnPaint(PaintEventArgs e)
        {

            var g = e.Graphics;

            

            //BackGroundDraw(g);
            g.DrawImage(scaledBackground, 0, 0);

            map.Draw(g, cameraOffset);
            Player.Draw(g, cameraOffset);

            if (!gameStarted)
                ButtonRestart.Draw(g);

            base.OnPaint(e);

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad1) map.mobs.Add(new CloseCombatEnuty(CloseCombatMobs.GetMob1Act(), 1800, 300, map));
            if (e.KeyCode == Keys.NumPad2) map.mobs.Add(new CloseCombatEnuty(CloseCombatMobs.GetMob2Act(), 1800, 300, map));
            if (e.KeyCode == Keys.NumPad3) map.mobs.Add(new CloseCombatEnuty(CloseCombatMobs.GetMob3Act(), 1800, 300, map));
            if (e.KeyCode == Keys.NumPad4) map.mobs.Add(new CloseCombatEnuty(CloseCombatMobs.Orc_Warchief, 1800, 300, map));
            if (e.KeyCode == Keys.NumPad5) map.mobs.Add(new CloseCombatEnuty(CloseCombatMobs.Orc_CorruptedChief, 1800, 300, map));
            if (e.KeyCode == Keys.NumPad6) map.mobs.Add(new CloseCombatEnuty(CloseCombatMobs.Orc_BloodOverlord, 1800, 300, map));

            if (e.KeyCode == Keys.W) { Player.inv.SlotSwap(1, 21); }
            if (e.KeyCode == Keys.Space) { }
            if (e.KeyCode == Keys.S) { }
            if (e.KeyCode == Keys.A) { Player.IsAPressed = true; }
            if (e.KeyCode == Keys.D) { Player.IsDPressed = true; }
            if (e.KeyCode == Keys.Q) { Player.IsQPressed = true; }
            if (e.KeyCode == Keys.E) { Player.IsEPressed = true; }
            if (e.KeyCode == Keys.Space) { Player.IsSpacePressed = true; }
            if (e.KeyCode == Keys.Tab) 
            {
                if (Player.inv.IsOpen)
                    Player.inv.Close();
                else
                    Player.inv.Open();
            }
            if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
                Player.inv.IsShiftDown = true;
            if (e.KeyCode == Keys.D1) { Player.inv.hotBar.ActiveSlot = 0; }
            if (e.KeyCode == Keys.D2) { Player.inv.hotBar.ActiveSlot = 1; }
            if (e.KeyCode == Keys.D3) { Player.inv.hotBar.ActiveSlot = 2; }
            if (e.KeyCode == Keys.D4) { Player.inv.hotBar.ActiveSlot = 3; }
            if (e.KeyCode == Keys.D5) { Player.inv.hotBar.ActiveSlot = 4; }
            if (e.KeyCode == Keys.D6) { Player.inv.hotBar.ActiveSlot = 5; }
            if (e.KeyCode == Keys.D7) { Player.inv.hotBar.ActiveSlot = 6; }
            if (e.KeyCode == Keys.D8) { Player.inv.hotBar.ActiveSlot = 7; }
            if (e.KeyCode == Keys.D9) { Player.inv.hotBar.ActiveSlot = 8; }
            if (e.KeyCode == Keys.D0) { Player.inv.hotBar.ActiveSlot = 9; }



        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { }
            if (e.KeyCode == Keys.S) {}
            if (e.KeyCode == Keys.A) { Player.IsAPressed = false; }
            if (e.KeyCode == Keys.D) { Player.IsDPressed = false; }
            if (e.KeyCode == Keys.Q) { Player.IsQPressed = false; }
            if (e.KeyCode == Keys.E) { Player.IsEPressed = false; }
            if (e.KeyCode == Keys.Space) { Player.IsSpacePressed = false; }
            if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
                Player.inv.IsShiftDown = false;
        }

        public void GameOver()
        {
            timer.Stop();
            gameStarted = false;
        }

        public void MoveCamera()
        {
            double screenMiddleX = this.ClientSize.Width / 2;
            double targetOffsetX = screenMiddleX - (Player.X + Player.Size.Width / 2);

            double smoothSpeed = 0.1;
            cameraOffset.X += (int)((targetOffsetX - cameraOffset.X) * smoothSpeed);

            //double screenMiddleY = this.ClientSize.Height / 3*2;
            //double targetOffsetY = screenMiddleY - (Player.Y + Player.Size.Height / 2);

            //cameraOffset.Y += (int)((targetOffsetY - cameraOffset.Y) * smoothSpeed);
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            if (Player.Y > this.ClientSize.Height + 100 || !Player.IsAlive)
            {
                Player.Dead();
                GameOver();
            }

            Player.inv.IsShiftDown = Control.ModifierKeys.HasFlag(Keys.Shift);


            MoveCamera();

            map.Update();
            Player.Update(cameraOffset);

            Invalidate();
        }
    }

}

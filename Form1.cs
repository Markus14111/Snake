using System;
using System.Drawing;
using System.Windows.Forms;
using Position = System.Tuple<int, int>;

namespace Snake
{
    public partial class Form1 : Form
    {
        private int CanvasSize = 600;
        private int Tile_Size = 30;
        private Position LastInput = Tuple.Create(1, 0);

        private Controll controll;
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;

            controll = new Controll(this);

            //set WindowSize
            this.Size = new Size(CanvasSize + 16, CanvasSize + 39);

            int[] test = new int[3];
            test[0] = 0;
            test[1] = 1;
            test[2] = 2;

            Array.Sort(test);

            for (int i = 0; i < 3; i++)
                Console.WriteLine(test[i]);
        }

        public Position GetLastInput()
        {
            return LastInput;
        }
        public void SetLastInput(Position LastInput)
        {
            this.LastInput = LastInput;
        }
        public int GetTileamount()
        {
            return (CanvasSize / Tile_Size);
        }

        //draws Snake and Food
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Load Snake Positions
            Position[] positions = controll.GetSnakePosition();
            //Draw Snake
            Color color;
            for (int i = 0; i < positions.Length; i++)
            {
                color = ColorTranslator.FromHtml(util.ColorGradientGreen[i * (util.ColorGradientGreen.Length / positions.Length)]);
                Brush brush = new SolidBrush(color);

                e.Graphics.FillRectangle(   brush,
                                            positions[i].Item1 * Tile_Size,
                                            positions[i].Item2 * Tile_Size,
                                            Tile_Size,
                                            Tile_Size);
            }

            //Draw Food
            e.Graphics.FillRectangle(Brushes.Crimson,
                                        controll.GetFoodPosition().Item1 * Tile_Size,
                                        controll.GetFoodPosition().Item2 * Tile_Size,
                                        Tile_Size,
                                        Tile_Size);
        }
        //store Last Input 
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
                LastInput = Tuple.Create(0, -1);
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                LastInput = Tuple.Create(-1, 0);
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
                LastInput = Tuple.Create(0, 1);
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                LastInput = Tuple.Create(1, 0);
        }
        //Closes Exe correctely 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

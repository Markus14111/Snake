using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Position = System.Tuple<int, int>;

namespace Snake
{
    public partial class Form1 : Form
    {
        private int CanvasSize = 600;
        private int Tile_Size = 30;
        private char LastInput = 'r';
        private int AllowJump = 10;

        private Controll controll;
        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;

            controll = new Controll(this);

            //set WindowSize
            this.Size = new Size(CanvasSize + 16, CanvasSize + 39);
        }



        public Position getLastInput()
        {
            Position direction = Tuple.Create(0, 0);

            if (LastInput == 'r')
                direction = Tuple.Create(1, 0);
            if (LastInput == 'l')
                direction = Tuple.Create(-1, 0);
            if (LastInput == 'u')
                direction = Tuple.Create(0, -1);
            if (LastInput == 'd')
                direction = Tuple.Create(0, 1);

            return direction;
        }        
        public int GetTileamount()
        {
            return (CanvasSize / Tile_Size);
        }
        public int GetAllowJump()
        {
            return AllowJump;
        }

        public void reset()
        {
            LastInput = 'r';
            controll.reset();
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
                //check BodyLength
                if (positions.Length < AllowJump)
                    color = ColorTranslator.FromHtml(util.ColorGradientBlue[i * (util.ColorGradientBlue.Length / positions.Length)]);
                else 
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
                LastInput = 'u';
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                LastInput = 'l';
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
                LastInput = 'd';
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                LastInput = 'r';
        }
        //Closes Exe correctely 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

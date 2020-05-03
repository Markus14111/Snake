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

        private System.Windows.Forms.Timer Game_Timer = new System.Windows.Forms.Timer();
              
        private Snake snake;
        private Food food;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;

            //set WindowSize
            this.Size = new Size(CanvasSize + 16, CanvasSize + 39);

            //Create Snake and food
            snake = new Snake(this);
            food = new Food(this, snake.GetPositions());

            //Initilaze Timer
            Game_Timer.Interval = (int)(1000/ 10);
            Game_Timer.Tick += new EventHandler(Tick);
            //start Timer
            Game_Timer.Enabled = true;
            Game_Timer.Start();

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
        public Position GetFoodposition()
        {
            return food.getPosition();
        }
        //starts new Game
        public void reset()
        {
            Game_Timer.Stop();
            util.wait(1000);
            snake = new Snake(this);
            food = new Food(this, snake.GetPositions());
            Refresh();
            util.wait(2000);
            LastInput = 'r';
            Game_Timer.Start();

        }
        //creates new food
        public void newFood()
        {
            food = new Food(this, snake.GetPositions());
        }

        //Update snake every Tick 
        private void Tick(object myobject, EventArgs myEventArgs)
        {
                snake.move_Snake();
                Refresh();
        }
        //draws Snake and Food
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Load Snake Positions
            Position[] positions = snake.GetPositions();
            //Draw Snake
            for (int i = 0; i < positions.Length; i++)
            {
                Color color = ColorTranslator.FromHtml(util.ColorGradientBlue[i * (util.ColorGradientBlue.Length / positions.Length)]);
                Brush brush = new SolidBrush(color);

                e.Graphics.FillRectangle(   brush,
                                            positions[i].Item1 * Tile_Size,
                                            positions[i].Item2 * Tile_Size,
                                            Tile_Size,
                                            Tile_Size);
            }

            //Draw Food
            e.Graphics.FillRectangle(Brushes.Crimson,
                                        food.getPosition().Item1 * Tile_Size,
                                        food.getPosition().Item2 * Tile_Size,
                                        Tile_Size,
                                        Tile_Size);
        }
        //store Last Input 
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.W || e.KeyCode == Keys.Up) && (snake.GetlastDirection() != 'd'))
                LastInput = 'u';
            if ((e.KeyCode == Keys.A || e.KeyCode == Keys.Left) && (snake.GetlastDirection() != 'r'))
                LastInput = 'l';
            if ((e.KeyCode == Keys.S || e.KeyCode == Keys.Down) && (snake.GetlastDirection() != 'u'))
                LastInput = 'd';
            if ((e.KeyCode == Keys.D || e.KeyCode == Keys.Right) && (snake.GetlastDirection() != 'l'))
                LastInput = 'r';
        }
        //Closes Exe correctely 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}

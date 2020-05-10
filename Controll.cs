using System;
using System.Windows.Forms;
using Position = System.Tuple<int, int>;


namespace Snake
{
    class Controll
    {
        //Member Variables
        private Timer Game_Timer = new Timer();
        private int FPS = 120;
        private Form1 drawing;
        private Food food;
        private AI ai;
        private Snake snake;
        int starve = 0;
        private Random rand = new Random();
        public Controll(Form1 form1)
        {
            //set pointer to form1
            drawing = form1;

            //Set starting direction to right
            drawing.SetLastInput(Tuple.Create(1, 0));

            //Create Snake and food
            snake = new Snake(drawing.GetTileamount());
            ai = new AI(this, drawing.GetTileamount());
            food = new Food(drawing.GetTileamount(), snake.GetPositions(),rand);
            //Initilaze Timer
            Game_Timer.Interval = (int)(1000 / FPS);
            Game_Timer.Tick += new EventHandler(Tick);
            //start Timer
            Game_Timer.Enabled = true;
            Game_Timer.Start();
        }

        //----------------- GETTER -------------------
        public Position GetFoodPosition() { return food.getPosition(); }
        public Position[] GetSnakePosition() { return snake.GetPositions(); }
        public AI GetAI()
        {
            return ai;
        }
        public bool timerRunning() { return Game_Timer.Enabled; }
        //----------------- SETTER 
        public void startTimer() { Game_Timer.Enabled = true;  Game_Timer.Start(); }
        //---------------- FUNCTIONS ------------------
        public void Learning()
        {
            ai.Learning();
        }
        //creates new food
        private Food newFood(Snake snake)
        {
            return new Food(drawing.GetTileamount(), snake.GetPositions(),rand);
        }

        //returns true if snake hits foods position
        private bool Eaten(Food food, Snake snake)
        {
            if (snake.GetPositions()[0].Equals(food.getPosition()))
                return true;

            return false;
        }

        //starts new Game
        public void reset()
        {
            snake = new Snake(drawing.GetTileamount());
            food = new Food(drawing.GetTileamount(), snake.GetPositions(), rand);
            drawing.Refresh();
            drawing.SetLastInput(Tuple.Create(1, 0));
            starve = 0;
        }

        //Update snake every Tick 
        private void Tick(object myobject, EventArgs myEventArgs)
        {
            //if (!snake.move_Snake(ai.RandomInput(snake.GetPositions()[0], food.getPosition())))
            Position move = ai.GetInput(snake.GetPositions(), food.getPosition(),false);
            if (!snake.move_Snake(move))
            {
                Game_Timer.Stop();
                Game_Timer.Enabled = false;
            }
            //eat Food
            while (Eaten(food,snake))
            {
                //add Tile at Snakes End
                snake.addTile();
                //Generate new Food
                food = new Food(drawing.GetTileamount(), snake.GetPositions(), rand);
                starve = 0;
            }
            drawing.Text = snake.GetPositions().Length.ToString();
            drawing.Refresh();
            if (starve > 200) { Game_Timer.Stop(); Game_Timer.Enabled = false; }
            starve++;
        }

        public Position run_AI()
        {
            Snake snake = new Snake(drawing.GetTileamount());
            Food food = new Food(drawing.GetTileamount(), snake.GetPositions(), rand);
            int move_count = 0, starve = 0;

            while (true)
            {
                Position move = ai.GetInput(snake.GetPositions(), food.getPosition(),true);
                if (!snake.move_Snake(move))
                { 
                    return Tuple.Create(snake.GetPositions().Length - 3, move_count);
                }
                //eat Food
                while (Eaten(food,snake))
                {
                    //add Tile at Snakes End
                    snake.addTile();
                    //Generate new Food
                    food = new Food(drawing.GetTileamount(), snake.GetPositions(), rand);
                    //reset Hunger
                    starve = 0;
                }
                move_count++;
                starve++;

                //return after termination condition
                /*if (move_count > 200)
                    return Tuple.Create(snake.GetPositions().Length - 3, move_count);*/
                if (starve > 150)
                    return Tuple.Create(snake.GetPositions().Length - 3, move_count);
                Application.DoEvents();
            }

            

        }
    }
}

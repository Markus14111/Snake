using System;
using System.Windows.Forms;
using Position = System.Tuple<int, int>;


namespace Snake
{
    class Controll
    {
        //Member Variables
        private Timer Game_Timer = new Timer();
        private Form1 drawing;
        private Snake snake;
        private Food food;
        private AI ai;

        public Controll(Form1 form1)
        {
            //set pointer to form1
            drawing = form1;

            //Set starting direction to right
            drawing.SetLastInput(Tuple.Create(1, 0));

            //Create Snake and food
            snake = new Snake(drawing.GetTileamount());
            food = new Food(drawing.GetTileamount(), snake.GetPositions());
            ai = new AI(this, drawing.GetTileamount());
            ai.Learning();
            //Initilaze Timer
            Game_Timer.Interval = (int)(1000 / 50);
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

        //starts new Game
        public void reset(int n)
        {
            Game_Timer.Stop();
            util.wait(50 * n);
            snake = new Snake(drawing.GetTileamount());
            food = new Food(drawing.GetTileamount(), snake.GetPositions());
            if (n == 1) { drawing.Refresh(); }
            util.wait(100 * n);
            drawing.SetLastInput(Tuple.Create(1, 0));
            Game_Timer.Start();
        }

        //creates new food
        private void newFood()
        {
            food = new Food(drawing.GetTileamount(), snake.GetPositions());
        }

        //returns true if snake hits foods position
        private bool Eaten()
        {
            if (snake.GetPositions()[0].Equals(food.getPosition()))
                return true;

            return false;
        }

        //Update snake every Tick 
        private void Tick(object myobject, EventArgs myEventArgs)
        {
            //if (!snake.move_Snake(ai.RandomInput(snake.GetPositions()[0], food.getPosition())))
            Position move = ai.GetInput(snake.GetPositions(), food.getPosition());
            if (!snake.move_Snake(move))
            {
                drawing.SetLastInput(Tuple.Create(1, 0));
                reset(1);
            }
            //eat Food
            if (Eaten())
            {
                //Generate new Food
                newFood();
                //add Tile at Snakes End
                snake.addTile();
            }
            drawing.Refresh();

        }

        public Position run_AI() {

            int Length = 0, move_count = 0, starve = 0;

            while (true)
            {
                Position move = ai.GetInput(snake.GetPositions(), food.getPosition());
                if (!snake.move_Snake(move))
                {
                    drawing.SetLastInput(Tuple.Create(1, 0));
                    reset(0);
                    Length = snake.GetPositions().Length - 3;

                    break;
                }
                //eat Food
                if (Eaten())
                {
                    //Generate new Food
                    newFood();
                    //add Tile at Snakes End
                    snake.addTile();
                    //reset Hunger
                    starve = 0;
                }

                move_count++;
                starve++;

                //return after termination condition
                if (move_count > 2000)
                    return Tuple.Create(Length, move_count);
                if (starve > 50)
                    return Tuple.Create(Length, move_count);

            }

            return Tuple.Create(Length, move_count);

        }
    }
}

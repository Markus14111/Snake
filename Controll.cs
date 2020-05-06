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
            snake = new Snake(drawing.GetAllowJump(), drawing.GetTileamount());
            food = new Food(drawing.GetTileamount(), snake.GetPositions());
            ai = new AI(drawing.GetAllowJump(), drawing.GetTileamount());

            //Initilaze Timer
            Game_Timer.Interval = (int)(1000 / 10);
            Game_Timer.Tick += new EventHandler(Tick);
            //start Timer
            Game_Timer.Enabled = true;
            Game_Timer.Start();
            Network test = new Network();
        }

        //----------------- GETTER -------------------
        public Position GetFoodPosition() { return food.getPosition(); }

        public Position[] GetSnakePosition() { return snake.GetPositions(); }

        //starts new Game
        public void reset()
        {
            Game_Timer.Stop();
            util.wait(1000);
            snake = new Snake(drawing.GetAllowJump(), drawing.GetTileamount());
            food = new Food(drawing.GetTileamount(), snake.GetPositions());
            ai = new AI(drawing.GetAllowJump(), drawing.GetTileamount());
            drawing.Refresh();
            util.wait(2000);
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
            if ((snake.GetPositions()[0].Item1 == food.getPosition().Item1) &&
                (snake.GetPositions()[0].Item2 == food.getPosition().Item2))
                return true;

            return false;
        }

        //Update snake every Tick 
        private void Tick(object myobject, EventArgs myEventArgs)
        {
            //if (!snake.move_Snake(ai.RandomInput(snake.GetPositions()[0], food.getPosition())))
            Position move = ai.GetInput(snake.GetPositions(), food.getPosition());
            Console.WriteLine(move);
            if (!snake.move_Snake(move))
            {
                drawing.SetLastInput(Tuple.Create(1, 0));
                reset();
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

        private void run_AI() {

            if (!snake.move_Snake(ai.GetInput(snake.GetPositions(), food.getPosition())))            
            {
                drawing.SetLastInput(Tuple.Create(1, 0));
                reset();
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

    }
}

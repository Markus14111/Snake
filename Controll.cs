using Snake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Controll(Form1 form1)
        {
            //set pointer to form1
            drawing = form1;

            //Create Snake and food
            snake = new Snake(drawing,this);
            food = new Food(drawing, snake.GetPositions());

            //Initilaze Timer
            Game_Timer.Interval = (int)(1000 / 10);
            Game_Timer.Tick += new EventHandler(Tick);
            //start Timer
            Game_Timer.Enabled = true;
            Game_Timer.Start();
        }

        //----------------- GETTER -------------------
        public Position GetFoodPosition()  { return food.getPosition(); }

        public Position[] GetSnakePosition() { return snake.GetPositions(); }

        //REPLACE LATER
        public char GetSnakeRotation() { return snake.GetlastDirection(); }

        //starts new Game
        public void reset()
        {
            Game_Timer.Stop();
            util.wait(1000);
            snake = new Snake(drawing,this);
            food = new Food(drawing, snake.GetPositions());
            drawing.Refresh();
            util.wait(2000);
            Game_Timer.Start();
        }

        //creates new food
        public void newFood() 
        { 
            food = new Food(drawing, snake.GetPositions()); 
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
            snake.move_Snake();
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

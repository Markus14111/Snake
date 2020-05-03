using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Position = System.Tuple<int, int>;

namespace Snake
{
    class Food
    {
        private Position position;
        private bool blocked = true;
        private Random random = new Random();



        public Food(Form1 form, Position[] blockedPositions)
        {
            while (blocked)
            {
                position = Tuple.Create(random.Next(0, form.GetTileamount()), random.Next(0, form.GetTileamount()));
                if (!is_blocked(position, blockedPositions)) { blocked = false; }
            }
        }

        public Position getPosition()
        {
            return position;
        }

        private bool is_blocked(Position position, Position[] blockedPosition)
        {
            foreach (Position snakeposition in blockedPosition)
            {
                if ((position.Item1 == snakeposition.Item1) && (position.Item2 == snakeposition.Item2)) 
                    return true;
            }

            return false;
        }

    }
}

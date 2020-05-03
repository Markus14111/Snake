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



        public Food(int Tileamount, Position[] blockedPositions)
        {
            while (blocked)
            {
                position = Tuple.Create(random.Next(0, Tileamount), random.Next(0, Tileamount));
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
                if (position.Equals(snakeposition))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

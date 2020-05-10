using System;
using Position = System.Tuple<int, int>;

namespace Snake
{
    class Food
    {
        private Position position;
        private bool blocked = true;



        public Food(int Tileamount, Position[] blockedPositions, Random random)
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

        public override string ToString()
        {
            return position.Item1.ToString() + "    " + position.Item2.ToString();
        }
    }
}

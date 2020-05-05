using System;
using System.Collections.Generic;
using System.Drawing.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Position = System.Tuple<int, int>;

namespace Snake
{
    class AI
    {

        Position[] Direction = { Tuple.Create(0, -1), Tuple.Create(1, 0), Tuple.Create(0, 1), Tuple.Create(-1, 0) };

        private int AllowJump, TileAmount;
        private int lastRotation;

        public AI(int Allowjump, int Tileamount)
        {
            AllowJump = Allowjump;
            TileAmount = Tileamount;
        }

        //returns direction of next move
        public Position RandomInput(Position position, Position food)
        {
            //take direction closest to food
            int closestFood = -1;
            int rotation = 0;
            for (int i = 0; i < 4; i++)
            {
                int fooddistance = FoodDistance(position, food, i);
                if (fooddistance > closestFood)
                {
                    closestFood = fooddistance;
                    rotation = i;
                }
            }
            //if snake wants to go backwards, make one turn right
            if ((rotation + 2) % 4 == lastRotation)
            {
                lastRotation = (rotation + 1) % 4;
                return Direction[lastRotation];
            }

            lastRotation = rotation;
            return Direction[rotation];
        }

        private int FoodDistance(Position position, Position food, int direction)
        {
            //assigns distance (negative value if food not in this direction)
            int distance = 0;
            switch (direction)
            {
                case 0:
                    distance = position.Item2 - food.Item2;
                    break;
                case 1:
                    distance = food.Item1 - position.Item1;
                    break;
                case 2:
                    distance = food.Item2 - position.Item2;
                    break;
                case 3:
                    distance = position.Item1 - food.Item1;
                    break;
            }
            //negative sign means no food found in this direction
            if (Math.Sign(distance) == -1)
                return -1;
            else
                return distance;
        }

        private int BorderDistance(Position position, int direction)
        {
            switch (direction)
            {
                case 0:
                    return position.Item2;
                case 1:
                    return TileAmount - position.Item1;
                case 2:
                    return TileAmount - position.Item2;
                case 3:
                    return position.Item1;
            }
            return 0;
        }
    }
}

/*

 ⣿⣿⣿⣿⣿⢻⣿⣿⣿⣿⣿⣿⣆⠻⡫⣢⠿⣿⣿⣿⣿⣿⣿⣿⣷⣜⢻⣿
 ⣿⣿⡏⣿⣿⣨⣝⠿⣿⣿⣿⣿⣿⢕⠸⣛⣩⣥⣄⣩⢝⣛⡿⠿⣿⣿⣆⢝
 ⣿⣿⢡⣸⣿⣏⣿⣿⣶⣯⣙⠫⢺⣿⣷⡈⣿⣿⣿⣿⡿⠿⢿⣟⣒⣋⣙⠊
 ⣿⡏⡿⣛⣍⢿⣮⣿⣿⣿⣿⣿⣿⣿⣶⣶⣶⣶⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿
 ⣿⢱⣾⣿⣿⣿⣝⡮⡻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠿⠛⣋⣻⣿⣿⣿⣿
 ⢿⢸⣿⣿⣿⣿⣿⣿⣷⣽⣿⣿⣿⣿⣿⣿⣿⡕⣡⣴⣶⣿⣿⣿⡟⣿⣿⣿
 ⣦⡸⣿⣿⣿⣿⣿⣿⡛⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⣿⣿⣿
 ⢛⠷⡹⣿⠋⣉⣠⣤⣶⣶⣿⣿⣿⣿⣿⣿⡿⠿⢿⣿⣿⣿⣿⣿⣷⢹⣿⣿
 ⣷⡝⣿⡞⣿⣿⣿⣿⣿⣿⣿⣿⡟⠋⠁⣠⣤⣤⣦⣽⣿⣿⣿⡿⠋⠘⣿⣿
 ⣿⣿⡹⣿⡼⣿⣿⣿⣿⣿⣿⣿⣧⡰⣿⣿⣿⣿⣿⣹⡿⠟⠉⡀⠄⠄⢿⣿
 ⣿⣿⣿⣽⣿⣼⣛⠿⠿⣿⣿⣿⣿⣿⣯⣿⠿⢟⣻⡽⢚⣤⡞⠄⠄⠄⢸⣿

 */

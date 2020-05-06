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

        static Position[] Direction = { Tuple.Create(0, -1), Tuple.Create(1, 0), Tuple.Create(0, 1), Tuple.Create(-1, 0), Tuple.Create(1,-1), Tuple.Create(1,1), Tuple.Create(-1, 1), Tuple.Create(-1, -1) };

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



        public double[] CreateInputs(Position[] pos, Position food_position)
        {
            Position Head = Tuple.Create(pos[0].Item1, pos[0].Item2);
            Position[] positions = new Position[pos.Length];
            for(int i = 0; i < positions.Length; i++)
            {
                positions[i] = Tuple.Create(pos[i].Item1, pos[i].Item2);
            }


            double[] Outputs = new double[24];

            //looks in 8 Direction
            int n = 0;
            for(int i = 0; i < Outputs.Length; i += 3)
            {
                //Wall
                Outputs[i] = DistanceToWall(Head, Direction[n]) / TileAmount;

                //Body
                if (DistanceToBody(positions, Direction[n]) == 0)
                    Outputs[i + 1] = 0;
                else
                    Outputs[i + 1] = DistanceToBody(positions, Direction[n]) / TileAmount;

                //Food
                if (DistanceToFood(Head, Direction[n], food_position) == 0)
                    Outputs[i + 2] = 0;
                else
                    Outputs[i + 2] = DistanceToFood(Head, Direction[n], food_position) / TileAmount;

                n++;
            }

            return Outputs;
        }

        private double DistanceToBody(Position[] positions, Position direction)
        {
            int n = 0;

            //While Head is in playing Area
            while (positions[0].Item1 >= 0 && positions[0].Item1 < TileAmount && positions[0].Item2 >= 0 && positions[0].Item2 < TileAmount)
            {
                n++;
                //update Head
                positions[0] = Tuple.Create(positions[0].Item1 + direction.Item1, positions[0].Item2 + direction.Item2);

                for (int i = 1; i < positions.Length; i++)
                {
                    //foound Bodypart
                    if (positions[0].Equals(positions[i]))
                        return n;
                }

            }

            return 0;
        }
        private double DistanceToWall(Position Head, Position direction)
        {
            int n = 0;

            //While Head is in playing Area
            while (Head.Item1 >= 0 && Head.Item1 < TileAmount && Head.Item2 >= 0 && Head.Item2 < TileAmount)
            {
                n++;
                Head = Tuple.Create(Head.Item1 + direction.Item1, Head.Item2 + direction.Item2);
            }
            
            return n;
        } 
        private double DistanceToFood(Position Head, Position direction, Position food_position)
        {
            int n = 0;

            //While Head is in playing Area
            while (Head.Item1 >= 0 && Head.Item1 < TileAmount && Head.Item2 >= 0 && Head.Item2 < TileAmount)
            {
                n++;
                Head = Tuple.Create(Head.Item1 + direction.Item1, Head.Item2 + direction.Item2);
                if (Head.Equals(food_position))
                    return n;

            }

            return 0;
        }
        private double Sigmoid(double n)
        {
            return 1 / (1 + Math.Exp(-n));
        }
    }
}

/*

 ⣿⣿⣿⣿⣿⢻⣿⣿⣿⣿⣿⣿⣆⠻⡫⣢⠿⣿⣿⣿⣿⣿⣿⣿⣷⣜⢻⣿
 ⣿⣿⡏⣿⣿⣨⣝⠿⣿⣿⣿⣿⣿⢕⠸⣛⣩⣥⣄⣩⢝⣛⡿⠿⣿⣿⣆⢝
 ⣿⣿⢡⣸⣿⣏⣿⣿⣶⣯⣙⠫⢺⣿⣷⡈⣿⣿⣿⣿⡿⠿⢿⣟⣒⣋⣙⠊
 ⣿⡏⡿⣛⣍⢿⣮⣿⣿⣿⣿⣿⣿⣿⣶⣶⣶⣶⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿
 ⣿⢱⣾⣿⣿⣿⣝⡮⡻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠿⠛⣋⣻⣿⣿⣿⣿
 ⢿⢸⣿⣿⣿⣿⣿⣿⣷⣽⣿⣿⣿⣿⣿⣿⣿⡕ü⣴⣶⣿⣿⣿⡟⣿⣿⣿
 ⣦⡸⣿⣿⣿⣿⣿⣿⡛⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⣿⣿⣿
 ⢛⠷⡹⣿⠋⣉ü⣤⣶⣶⣿⣿⣿⣿⣿⣿⡿⠿⢿⣿⣿⣿⣿⣿⣷⢹⣿⣿
 ⣷⡝⣿⡞⣿⣿⣿⣿⣿⣿⣿⣿⡟⠋ppp⣤⣦⣽⣿⣿⣿⡿⠋⠘⣿⣿
 ⣿⣿⡹⣿⡼⣿⣿⣿⣿⣿⣿⣿⣧⡰⣿⣿⣿⣿⣿⣹⡿⠟⠉⡀⠄⠄⢿⣿
 ⣿⣿⣿⣽⣿⣼⣛⠿⠿⣿⣿⣿⣿⣿⣯⣿⠿⢟⣻⡽⢚⣤⡞⠄⠄⠄⢸⣿

 */

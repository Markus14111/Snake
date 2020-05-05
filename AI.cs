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

        private int AllowJump, TileAmount;

        public AI(int Allowjump, int Tileamount)
        {
            AllowJump = Allowjump;
            TileAmount = Tileamount;
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(Distance(Tuple.Create(5, 8), i));
            }
        }

        public Position RandomInput()
        {
            Random rand = new Random();
            Position[] test = { Tuple.Create(0, 1), Tuple.Create(0, -1), Tuple.Create(1, 0), Tuple.Create(-1, 0) };
            return test[rand.Next(4)];
        }

        private int Distance(Position position, int direction)
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

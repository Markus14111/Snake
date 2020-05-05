using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Position = System.Tuple<int, int>;

namespace Snake
{
    class AI
    {
        public AI()
        {

        }

        public Position RandomInput()
        {
            Random rand = new Random();
            Position[] test = { Tuple.Create(0, 1), Tuple.Create(0, -1), Tuple.Create(1, 0), Tuple.Create(-1, 0) };
            return test[rand.Next(4)];
        }
    }
}

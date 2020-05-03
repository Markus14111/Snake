using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.Runtime.CompilerServices;
using Position = System.Tuple<int, int>;

namespace Snake
{
    class Snake
    {
        private Position lastdirection;
        private Position nextTile;
        private Position[] Body;

        private int AllowJump, TileAmount;

        public Snake(int allow, int tile)
        {
            AllowJump = allow;
            TileAmount = tile;

            //Generate starting position
            Body = new Position[3];
            Body[0] = Tuple.Create(3, TileAmount / 2);
            Body[1] = Tuple.Create(2, TileAmount / 2);
            Body[2] = Tuple.Create(1, TileAmount / 2);

            //starting direction
            lastdirection = new Position(1, 0);
        }

        //moves Snake
        public bool move_Snake(Position direction)
        {
            //catch 180 degree turns
            if (direction.Equals(Tuple.Create(-lastdirection.Item1, -lastdirection.Item2)))
                direction = lastdirection;

            Position[] temp = new Position[Body.Length];

            //update Head Tile
            temp[0] = Tuple.Create(Body[0].Item1 + direction.Item1, Body[0].Item2 + direction.Item2);

            //allow screen jumping after certain Length
            if (Body.Length >= AllowJump)
            {
                //x jump
                if (temp[0].Item1 < 0)
                    temp[0] = Tuple.Create(TileAmount - 1, temp[0].Item2);
                if (temp[0].Item1 > TileAmount - 1)
                    temp[0] = Tuple.Create(0, temp[0].Item2);
                //y jump
                if (temp[0].Item2 < 0)
                    temp[0] = Tuple.Create(temp[0].Item1, TileAmount - 1);
                if (temp[0].Item2 > TileAmount - 1)
                    temp[0] = Tuple.Create(temp[0].Item1, 0);
            }

            //shift rest of Body
            for (int i = 0; i < Body.Length - 1; i++)
            {
                //shift
                temp[i + 1] = Body[i];
            }

            nextTile = Body[Body.Length-1];

            //Update Snake
            Body = temp;
            
            //Check Collision of snakes Head
            if (Collision(Body[0]))
            {
                return false;
            }

            lastdirection = direction;

            return true;
        }

        public Position[] GetPositions()
        {
            return Body;
        }

        //adds Tile at the End of Snake
        public void addTile()
        {
            Position[] temp = new Position[Body.Length + 1];

            for (int i = 0; i < Body.Length; i++)
                temp[i] = Body[i];

            temp[Body.Length] = nextTile;

            Body = temp;
        }
        //returns true if Collision
        private bool Collision(Position Head)
        {
            //Border hit for Snakes Lentgh < 50
            if (Body.Length < AllowJump)
            {
                //Snake ot ouf Border
                if (Head.Item1 < 0 || Head.Item1 > TileAmount - 1)
                    return true;
                if (Head.Item2 < 0 || Head.Item2 > TileAmount - 1)
                    return true;
            }
                         
            //Snake hits Itself
            for (int i = 1; i < Body.Length; i++)
            {
                if ((Head.Item1 == Body[i].Item1) && (Head.Item2 == Body[i].Item2))
                    return true;
            }

            return false;
        }
    }
}

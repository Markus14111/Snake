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
        private Position direction, lastdirection;
        private Position[] Body = new Position[3];
        private Position nextTile;
        private Form1 drawing;
        //REPLACE SOON
        private Controll controll;

        public Snake(Form1 form, Controll cont)
        {
            drawing = form;
            controll = cont;

            //Generate starting position
            this.Body[0] = Tuple.Create(3, form.GetTileamount() / 2);
            this.Body[1] = Tuple.Create(2, form.GetTileamount() / 2);
            this.Body[2] = Tuple.Create(1, form.GetTileamount() / 2);

            //starting direction
            this.direction = Tuple.Create(1, 0);
            this.lastdirection = Tuple.Create(1, 0);

        }

        //moves Snake
        public void move_Snake()
        {
            direction = drawing.getLastInput();

            Position[] temp = new Position[Body.Length];

            //update Head Tile
            temp[0] = Tuple.Create(Body[0].Item1 + direction.Item1, Body[0].Item2 + direction.Item2);

            //allow screen jumping after certain Length
            if (Body.Length >= drawing.GetAllowJump())
            {
                //x jump
                if (temp[0].Item1 < 0)
                    temp[0] = Tuple.Create(drawing.GetTileamount() - 1, temp[0].Item2);
                if (temp[0].Item1 > drawing.GetTileamount() - 1)
                    temp[0] = Tuple.Create(0, temp[0].Item2);
                //y jump
                if (temp[0].Item2 < 0)
                    temp[0] = Tuple.Create(temp[0].Item1, drawing.GetTileamount() - 1);
                if (temp[0].Item2 > drawing.GetTileamount() - 1)
                    temp[0] = Tuple.Create(temp[0].Item1, 0);
            }

            //shift rest of Body
            for (int i = 0; i <= Body.Length - 1; i++)
            {
                //store possible next Tile
                if (i == Body.Length - 1)
                {
                    nextTile = Body[i];
                    continue;
                }

                //shift
                temp[i + 1] = Body[i];
            }

            //Update Snake
            Body = temp;
            
            //Check Collision of snakes Head
            if (Collision(Body[0]))
            {
                drawing.reset();
            }

            lastdirection = direction;

        }

        public Position[] GetPositions()
        {
            return Body;
        }
        public char GetlastDirection()
        {
            char d = '.';

            if (lastdirection.Item1 == 0 && lastdirection.Item2 == 1)
                d = 'd';
            if (lastdirection.Item1 == 0 && lastdirection.Item2 == -1)
                d = 'u';
            if (lastdirection.Item1 == 1 && lastdirection.Item2 == 0)
                d = 'r';
            if (lastdirection.Item1 == -1 && lastdirection.Item2 == 0)
                d = 'l';


            return d;
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
            if (Body.Length < drawing.GetAllowJump())
            {
                //Snake ot ouf Border
                if (Head.Item1 < 0 || Head.Item1 > drawing.GetTileamount() - 1)
                    return true;
                if (Head.Item2 < 0 || Head.Item2 > drawing.GetTileamount() - 1)
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

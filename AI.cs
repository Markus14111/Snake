using System;

using Position = System.Tuple<int, int>;
using Dataset = System.Tuple<double[,], double[,], double[,], double[], double[], double[]>;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Snake
{
    class AI
    {

        static Position[] Direction = { Tuple.Create(0, -1), Tuple.Create(1, 0), Tuple.Create(0, 1), Tuple.Create(-1, 0), Tuple.Create(1,-1), Tuple.Create(1,1), Tuple.Create(-1, 1), Tuple.Create(-1, -1) };

        private int TileAmount;
        private Network student;
        private Controll controller;

        public AI(Controll controller, int TileAmount)
        {
            this.controller = controller;
            this.TileAmount = TileAmount;
            student = new Network();
        }


        public int TeacherBot(Dataset dataset)
        {
            student.setValues(dataset);
            Position Values = controller.run_AI();

            int fitness = Values.Item1 + Values.Item2;

            return fitness;
        }

        public void Learning()
        {            
            Dataset[] Students = new Dataset[20];
            Position[] ValueIndexPair = new Position[20];
            Dataset[] Top3 = new Dataset[3];

            int cycles = 10;

            for (int i = 0; i < cycles; i++)
            {
                if (i == 1)
                {
                    //set Random
                    for (int j = 0; j < 20; j++)
                        Students[i] = Randomize();                       
                }
                else
                {
                    //call BuilderBot
                    Students = Builderbot(Top3);
                }
                
                //Run and Teach
                for (int j = 0; j < Students.Length; j++)
                    ValueIndexPair[j] = Tuple.Create(TeacherBot(Students[j]), j);

                //Sort Students
                Array.Sort(ValueIndexPair);

                //Take top 3
                Top3 = new Dataset[3];
                for (int j = 0; j < 3; j++)
                    Top3[j] = Students[ValueIndexPair[19 - j].Item2];

            }

        }

        private Dataset Randomize()
        {
            double[,] Weights0 = new double[18, 24];
            double[,] Weights1 = new double[18, 18];
            double[,] Weights2 = new double[4, 18];

            double[] Offset0 = new double[18];
            double[] Offset1 = new double[18];
            double[] Offset2 = new double[4];

            Random rand = new Random();
            for (int m = 0; m < Weights0.GetUpperBound(0) + 1; m++)
            {
                for (int n = 0; n < Weights0.GetUpperBound(1) + 1; n++)
                    Weights0[m, n] = (rand.Next(20) + 1) / 10;
            }
            for (int m = 0; m < Weights1.GetUpperBound(0) + 1; m++)
            {
                for (int n = 0; n < Weights1.GetUpperBound(1) + 1; n++)
                    Weights1[m, n] = (rand.Next(20) + 1) / 10;
            }
            for (int m = 0; m < Weights2.GetUpperBound(0) + 1; m++)
            {
                for (int n = 0; n < Weights2.GetUpperBound(1) + 1; n++)
                    Weights2[m, n] = (rand.Next(20) + 1) / 10;
            }
            for (int i = 0; i < Offset0.Length; i++)
                Offset0[i] = (rand.Next(20) + 1) / 10;
            for (int i = 0; i < Offset1.Length; i++)
                Offset1[i] = (rand.Next(20) + 1) / 10;
            for (int i = 0; i < Offset2.Length; i++)
                Offset2[i] = (rand.Next(20) + 1) / 10;

            return Tuple.Create(Weights0, Weights1, Weights2, Offset0, Offset1, Offset2);
        }

        public Position GetInput(Position[] pos, Position food_pos)
        {
            int result = student.run(CreateInputs(pos, food_pos));
            return Direction[result];
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
 ⣷⡝⣿⡞⣿⣿⣿⣿⣿⣿⣿⣿⡟⠋lll⣤⣦⣽⣿⣿⣿⡿⠋⠘⣿⣿
 ⣿⣿⡹⣿⡼⣿⣿⣿⣿⣿⣿⣿⣧⡰⣿⣿⣿⣿⣿⣹⡿⠟⠉⡀⠄⠄⢿⣿
 ⣿⣿⣿⣽⣿⣼⣛⠿⠿⣿⣿⣿⣿⣿⣯⣿⠿⢟⣻⡽⢚⣤⡞⠄⠄⠄⢸⣿

 */

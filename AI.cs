using System;

using Position = System.Tuple<int, int>;
using Dataset = System.Tuple<double[,], double[,], double[,], double[], double[], double[]>;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Snake
{
    class AI
    {

        static Position[] Direction = { Tuple.Create(0, -1), Tuple.Create(1, 0), Tuple.Create(0, 1), Tuple.Create(-1, 0), Tuple.Create(1, -1), Tuple.Create(1, 1), Tuple.Create(-1, 1), Tuple.Create(-1, -1) };

        private int TileAmount;
        private Network student;
        private Controll controller;
        private int MutationRate = 5;

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
                    Students = BuilderBot(Top3);
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
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = Tuple.Create(pos[i].Item1, pos[i].Item2);
            }


            double[] Outputs = new double[24];

            //looks in 8 Direction
            int n = 0;
            for (int i = 0; i < Outputs.Length; i += 3)
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

        private Dataset[] BuilderBot(Dataset[] winners)
        {
            //randomly crossbreed 2 winners
            Random rand = new Random();
            Dataset[] output = new Dataset[20];
            for (int i = 0; i < 20; i++)
            {
                Dataset Child = breed(winners[rand.Next(3)], winners[rand.Next(3)]);
                //apply mutation
                output[i] = mutation(Child);
            }
            return output;
        }

        private double[,] replaceLine(double[,] array, double[,] copyfrom, int line)
        {
            for (int i = 0; i < array.GetUpperBound(1) + 1; i++)
                array[i, line] = copyfrom[i, line];

            return array;
        }
        private Dataset breed(Dataset mother, Dataset father)
        {
            //crossbreeds the 2 neural networks
            //randomly pick one of every neuron from either parent
            Random rand = new Random();
            double[,] Weights0 = new double[18, 24];
            double[,] Weights1 = new double[18, 18];
            double[,] Weights2 = new double[4, 18];
            double[] Offset0 = new double[18];
            double[] Offset1 = new double[18];
            double[] Offset2 = new double[4];

            //weights 0, 1 and offset 0,1
            for (int i = 0; i < 18; i++)
            {
                //0 -> neuron from mother
                if (rand.Next(2) == 0)
                    Weights0 = replaceLine(Weights0,mother.Item1,i);
                else
                    Weights0 = replaceLine(Weights0, father.Item1, i);
                //0 -> neuron from mother
                if (rand.Next(2) == 0)
                    Weights1 = replaceLine(Weights1, mother.Item2, i);
                else
                    Weights1 = replaceLine(Weights1, father.Item2, i);
                //0 -> neuron from mother
                if (rand.Next(2) == 0)
                    Offset0[i] = mother.Item4[i];
                else
                    Offset0[i] = father.Item4[i];
                //0 -> neuron from mother
                if (rand.Next(2) == 0)
                    Offset1[i] = mother.Item5[i];
                else
                    Offset1[i] = father.Item5[i];
            }
            //weight and offset 2
            for (int i = 0; i < 4; i++)
            {
                //0 -> neuron from mother
                if (rand.Next(2) == 0)
                    Weights2 = replaceLine(Weights2, mother.Item3, i);
                else
                    Weights2 = replaceLine(Weights2, father.Item3, i);
                //0 -> neuron from mother
                if (rand.Next(2) == 0)
                    Offset2[i] = mother.Item6[i];
                else
                    Offset2[i] = father.Item6[i];
            }

            return Tuple.Create(Weights0,Weights1,Weights2,Offset0,Offset1,Offset2);
        }

        //mutates a single weight
        private double mutate(double weight)
        {
            Random rand = new Random();
            switch (rand.Next(3))
            {
                case 0:
                    return -weight;
                case 1:
                    return weight + (rand.Next(2000) / 1000) - 1;
                case 2:
                    return weight * (rand.Next(1000) / 1000 + 0.5);
                default:
                    return weight;
            }
        }
        private Dataset mutation(Dataset Child)
        {
            double[,] Weights0 = Child.Item1;
            double[,] Weights1 = Child.Item2;
            double[,] Weights2 = Child.Item3;
            double[] Offset0 = Child.Item4;
            double[] Offset1 = Child.Item5;
            double[] Offset2 = Child.Item6;

            Random rand = new Random();
            for (int i = 0; i < 18; i++)
            {
                //Weights0
                for (int j = 0; j < 24; j++)
                    if (rand.Next(100) < MutationRate)
                        Weights0[i, j] = mutate(Weights0[i, j]);
                //Weights1
                for (int j = 0; j < 18; j++)
                    if (rand.Next(100) < MutationRate)
                        Weights1[i, j] = mutate(Weights1[i, j]);
                //offset0
                if (rand.Next(100) < MutationRate)
                    Offset0[i] = mutate(Offset0[i]);
                //offset1
                if (rand.Next(100) < MutationRate)
                    Offset1[i] = mutate(Offset1[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                //Weights1
                for (int j = 0; j < 18; j++)
                    if (rand.Next(100) < MutationRate)
                        Weights2[i, j] = mutate(Weights2[i, j]);
                //offset1
                if (rand.Next(100) < MutationRate)
                    Offset2[i] = mutate(Offset2[i]);
            }
            return Child;
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

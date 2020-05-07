using System;

using Position = System.Tuple<int, int>;
using Dataset = System.Tuple<double[,], double[,], double[,], double[], double[], double[]>;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Snake
{
    class AI
    {

        static Position[] Direction = { Tuple.Create(0, -1), Tuple.Create(1, 0), Tuple.Create(0, 1), Tuple.Create(-1, 0), Tuple.Create(1, -1), Tuple.Create(1, 1), Tuple.Create(-1, 1), Tuple.Create(-1, -1) };

        private int TileAmount;
        private Network student;
        private Controll controller;
        private int MutationRate = 10;
        private int ClassSize = 5000;
        private int cycleSize = 10;
        private int GamesPerSnake = 5;
        private string[] path = new string[10];

        public AI(Controll controller, int TileAmount)
        {
            for (int i = 0; i < path.Length; i++)
                path[i] = ".\\Dataset" + i.ToString() + ".txt";

            this.controller = controller;
            this.TileAmount = TileAmount;
            student = new Network();
        }

        public int TeacherBot(Dataset dataset)
        {
            int fitness = 0;
            student.setValues(dataset);
            //n Games per Student
            for (int k = 0; k < GamesPerSnake; k++)
            {
                Position Values = controller.run_AI();
                fitness += Values.Item1 * 200 + Values.Item2;
            }
            return Convert.ToInt32(fitness / GamesPerSnake); ;
        }

        public void Learning()
        {            
            Dataset[] Students = new Dataset[ClassSize];
            Position[] ValueIndexPair = new Position[ClassSize];
            Dataset[] Top = new Dataset[10];
            int first = -1;
            int cycles = cycleSize;

            //if there is a file with saved AI load it
            if (File.Exists(path[0]))
            {
                Top = new Dataset[10];
                for (int i = 0; i < path.Length; i++)
                    Top[i] = ReadFromFile(i);
                Students = BuilderBot(Top);
                Console.WriteLine("Loaded DataSets");
            }
            else 
            {
                //randomize
                for (int i = 0; i < ClassSize; i++)
                    Students[i] = Randomize();
                Console.WriteLine("Random Seed Generated");

            }

            for (int i = 0; i < cycles; i++)
            {
                //Run and Teach
                for (int j = 0; j < Students.Length; j++)
                {
                    ValueIndexPair[j] = Tuple.Create(Tuple.Create(TeacherBot(Students[j]), j).Item1, j);
                }


                //Sort Students
                Array.Sort(ValueIndexPair);
                Console.WriteLine(ValueIndexPair[ClassSize-1].Item1);

                if (first == -1)
                    first = ValueIndexPair[ClassSize - 1].Item1;

                //Take top 3
                Top = new Dataset[10];
                for (int j = 0; j < 10; j++)
                    Top[j] = Students[ValueIndexPair[ClassSize - 1 - j].Item2];

                //call BuilderBot
                Students = BuilderBot(Top);

                //Console.WriteLine(i + 1);

            }

            student.setValues(Top[0]);
            
            if (TeacherBot(Top[0]) >= first)
            {
                for (int i = 0; i < path.Length; i++)
                    WriteToFile(Top[i], i);
                Console.WriteLine("New DataSet written");
            }                
            else
                Console.WriteLine("Failed Writing");
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
            Dataset[] output = new Dataset[ClassSize];
            for (int i = 10; i < ClassSize; i++)
            {
                Dataset Child = breed(winners[rand.Next(10)], winners[rand.Next(10)]);
                //apply mutation
                output[i] = mutation(Child);
            }
            for (int i = 0; i < 10; i++)
                output[i] = winners[i];
            return output;
        }

        private double[,] replaceLine(double[,] array, double[,] copyfrom, int line)
        {
            for (int i = 0; i < array.GetUpperBound(1) + 1; i++)
                array[line,i] = copyfrom[line,i];

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
            return Tuple.Create(Weights0, Weights1, Weights2, Offset0, Offset1, Offset2);
        }

        private Dataset ReadFromFile(int number)
        {
            string[] reading = File.ReadAllLines(path[number], Encoding.UTF8);
            double[,] Weights0 = new double[18, 24];
            double[,] Weights1 = new double[18, 18];
            double[,] Weights2 = new double[4, 18];
            double[] Offset0 = new double[18];
            double[] Offset1 = new double[18];
            double[] Offset2 = new double[4];
            int a = 0;
            for (int i = 0; i < 18; i++)
            {
                //Weights0
                for (int j = 0; j < 24; j++)
                { Weights0[i, j] = Convert.ToDouble(reading[a]); a++; }
                //Weights1
                for (int j = 0; j < 18; j++)
                { Weights1[i, j] = Convert.ToDouble(reading[a]); a++; }
                //weights2
                for (int j = 0; j < 4; j++)
                { Weights2[j, i] = Convert.ToDouble(reading[a]); a++; }
                //Offset0
                { Offset0[i] = Convert.ToDouble(reading[a]); a++; }
                //Offset1
                { Offset1[i] = Convert.ToDouble(reading[a]); a++; }
            }
            for (int i = 0; i < 4; i++)
            {
                { Offset2[i] = Convert.ToDouble(reading[a]); a++; }
            }
            return Tuple.Create(Weights0, Weights1, Weights2, Offset0, Offset1, Offset2);
        }

        private void WriteToFile(Dataset input,int number)
        {
            int length = input.Item1.Length + input.Item2.Length + input.Item3.Length +
                         input.Item4.Length + input.Item5.Length + input.Item6.Length + 1;
            string[] final = new string[length];
            int a = 0;
            for (int i = 0; i < 18; i++)
            {
                //Weights0
                for (int j = 0; j < 24; j++)
                { final[a] = input.Item1[i, j].ToString(); a++; }
                //Weights1
                for (int j = 0; j < 18; j++)
                { final[a] = input.Item2[i, j].ToString(); a++; }
                //weights2
                for (int j = 0; j < 4; j++)
                { final[a] = input.Item3[j, i].ToString(); a++; }
                //Offset0
                { final[a] = input.Item4[i].ToString(); a++; }
                //Offset1
                { final[a] = input.Item5[i].ToString(); a++; }
            }
            for (int i = 0; i < 4; i++)
            {
                { final[a] = input.Item6[i].ToString(); a++; }
            }
            
            //store current Date/Time on last row
            final[a] = DateTime.Now.ToString();

            File.WriteAllLines(path[number], final, Encoding.UTF8);
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

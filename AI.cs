using System;
using System.IO;
using System.Text;
using System.Runtime.Remoting.Lifetime;


using Position = System.Tuple<int, int>;
using ScorePair = System.Tuple<long, int>;
using Dataset = System.Tuple<double[,], double[,], double[,], double[], double[], double[]>;
using System.Windows.Forms;

namespace Snake
{
    class AI
    {

        static Position[] Direction = { Tuple.Create(0, -1), Tuple.Create(1, 0), Tuple.Create(0, 1), Tuple.Create(-1, 0), Tuple.Create(1, -1), Tuple.Create(1, 1), Tuple.Create(-1, 1), Tuple.Create(-1, -1) };

        private int TileAmount;
        private Network student,winner;
        private Controll controller;
        private Random rand = new Random();
        private int MutationRate = 5;
        private int ClassSize = 5000;
        private int cycleSize = 150;
        private int GamesPerSnake = 1;
        private string[] path = new string[10];

        public AI(Controll controller, int TileAmount)
        {
            for (int i = 0; i < path.Length; i++)
                path[i] = ".\\Dataset" + i.ToString() + ".txt";

            this.controller = controller;
            this.TileAmount = TileAmount;
            student = new Network();
            winner = new Network();
        }

        //calculates fitness of from given results
        private long calculateFitness(Position Values)
        {  
            long fitness = 0;
            int score = Values.Item1 + 2;
            int lifetime = Values.Item2;
            if (lifetime > 450)
                lifetime = 450;
            if (score < 10)
            {
                fitness = lifetime * lifetime * Convert.ToInt32(Math.Pow(2, score));
            }
            else
            {
                fitness = lifetime * lifetime;
                fitness *= Convert.ToInt32(Math.Pow(2, 10));
                fitness *= score - 9;
            }
            return fitness;
        }

        private long TeacherBot(Dataset dataset)
        {
            long fitness = 0;
            student.setValues(dataset);
            //n Games per Student
            for (int k = 0; k < GamesPerSnake; k++)
            {
                Position Values = controller.run_AI();
                fitness += calculateFitness(Values);
            }
            return Convert.ToInt64(fitness / GamesPerSnake); ;
        }

        public void Learning()
        {            
            Dataset[] Students = new Dataset[ClassSize];
            ScorePair[] ValueIndexPair = new ScorePair[ClassSize];
            int cycles = cycleSize;
            //randomize
            for (int i = 0; i < ClassSize; i++)
                Students[i] = Randomize();
            Console.WriteLine("Random Seed Generated");

            for (int i = 0; i < cycles; i++)
            {
                //Run and Teach
                for (int j = 0; j < Students.Length; j++)
                {
                    ValueIndexPair[j] = Tuple.Create(TeacherBot(Students[j]), j);
                }
                while(controller.timerRunning())
                {
                    Application.DoEvents();
                }


                //Sort Students
                Array.Sort(ValueIndexPair);

                winner.setValues(Students[ValueIndexPair[ClassSize-1].Item2]);                

                //call BuilderBot
                Students = BuilderBot(Students,ValueIndexPair);

                Console.WriteLine("Gen: " + (i + 1).ToString() + ", Fitness: " + ValueIndexPair[ClassSize - 1].Item1);
                controller.reset();
                controller.startTimer();
            }   
        }

        private Dataset Randomize()
        {
            double[,] Weights0 = new double[18, 25];
            double[,] Weights1 = new double[18, 18];
            double[,] Weights2 = new double[4, 18];

            double[] Offset0 = new double[18];
            double[] Offset1 = new double[18];
            double[] Offset2 = new double[4];

            Random rand = new Random();
            for (int m = 0; m < Weights0.GetUpperBound(0) + 1; m++)
            {
                for (int n = 0; n < Weights0.GetUpperBound(1) + 1; n++)
                    Weights0[m, n] = (rand.NextDouble() * 2) - 1;
            }
            for (int m = 0; m < Weights1.GetUpperBound(0) + 1; m++)
            {
                for (int n = 0; n < Weights1.GetUpperBound(1) + 1; n++)
                    Weights1[m, n] = (rand.NextDouble() * 2) - 1;
            }
            for (int m = 0; m < Weights2.GetUpperBound(0) + 1; m++)
            {      
                for (int n = 0; n < Weights2.GetUpperBound(1) + 1; n++)
                    Weights2[m, n] = (rand.NextDouble() * 2) - 1;
            }
            for (int i = 0; i < Offset0.Length; i++)
                Offset0[i] = (rand.NextDouble() * 2) - 1;
            for (int i = 0; i < Offset1.Length; i++)
                Offset1[i] = (rand.NextDouble() * 2) - 1;
            for (int i = 0; i < Offset2.Length; i++)
                Offset2[i] = (rand.NextDouble() * 2) - 1;

            return Tuple.Create(Weights0, Weights1, Weights2, Offset0, Offset1, Offset2);
        }

        public Position GetInput(Position[] pos, Position food_pos,bool learn)
        {
            int result = 0;
            if (learn)
                result = student.run(CreateInputs(pos, food_pos));
            else
                result = winner.run(CreateInputs(pos, food_pos));
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


            double[] Outputs = new double[25];

            //looks in 8 Direction
            int n = 0;
            for (int i = 0; i < Outputs.Length - 1; i += 3)
            {
                //Wall
                Outputs[i + 2] = DistanceToWall(Head, Direction[n]);

                //Body
                if (DistanceToBody(positions, Direction[n]) == 0)
                    Outputs[i] = 0;
                else
                    Outputs[i] = DistanceToBody(positions, Direction[n]);

                //Food [0;1]
                Outputs[i + 1] = DistanceToFood(Head, Direction[n], food_position);

                n++;
            }
            Outputs[24] = (pos.Length - 3) / 50;

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
                        return 1 / n;
                }

            }

            return 0;
        }
        private double DistanceToWall(Position Head, Position direction)
        {
            int n = 0;

            while (true)
            {
                n++;
                Head = Tuple.Create(Head.Item1 + direction.Item1, Head.Item2 + direction.Item2);

                if (Head.Item1 < 0 || Head.Item1 > (TileAmount - 1) || Head.Item2 < 0 || Head.Item2 > (TileAmount - 1))
                    return 1 / n;
            }

        }
        private double DistanceToFood(Position Head, Position direction, Position food_position)
        {
            int n = 0;
            //While Head is in playing Area
            while (Head.Item1 >= 0 && Head.Item1 < TileAmount && Head.Item2 >= 0 && Head.Item2 < TileAmount)
            {
                n++;
                Head = Tuple.Create(Head.Item1 + direction.Item1, Head.Item2 + direction.Item2);
                if (Head.Item1 == food_position.Item1 && Head.Item2 == food_position.Item2)
                    return 1;

            }

            return 0;
        }

        private long randomLong(long max, long min = 0)
        {
            //1 long has 8 bytes
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
        private long totalFitness(Dataset[] students, ScorePair[] indexValuePair)
        {
            long output = 0;
            foreach (ScorePair temp in indexValuePair)
                output += temp.Item1;
            return output;
        }
        private Dataset ChooseParent(Dataset[] students, ScorePair[] indexValuePair)
        {
            long required = randomLong(totalFitness(students, indexValuePair));
            long Fitness = 0;
            for (int i = 0; i < indexValuePair.Length; i++)
            {
                Fitness += indexValuePair[i].Item1;
                if (Fitness > required)
                    return students[indexValuePair[i].Item2];
            }
            return students[0];
            
        }
        private Dataset[] BuilderBot(Dataset[] winners, ScorePair[] indexValuePair)
        {
            //randomly crossbreed 2 winners
            Random rand = new Random();
            Dataset[] output = new Dataset[ClassSize];
            for (int i = 1; i < ClassSize; i++)
            {
                Dataset Child = breed(ChooseParent(winners, indexValuePair), ChooseParent(winners, indexValuePair));
                //apply mutation
                output[i] = mutation(Child);
            }
            output[0] = winners[indexValuePair[ClassSize - 1].Item2];
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
            double[,] Weights0 = new double[18, 25];
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

        private double gaussian ()
        {
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }
        //mutates a single weight
        private double mutate(double weight)
        {
            weight += gaussian(); 
            if (weight > 1) { weight = 1; }
            if (weight < 0) { weight = 0; }
            return weight;
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
                for (int j = 0; j < 25; j++)
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
            double[,] Weights0 = new double[18, 25];
            double[,] Weights1 = new double[18, 18];
            double[,] Weights2 = new double[4, 18];
            double[] Offset0 = new double[18];
            double[] Offset1 = new double[18];
            double[] Offset2 = new double[4];
            int a = 0;
            for (int i = 0; i < 18; i++)
            {
                //Weights0
                for (int j = 0; j < 25; j++)
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

        public void WriteToFile(Dataset input,int number)
        {
            int length = input.Item1.Length + input.Item2.Length + input.Item3.Length +
                         input.Item4.Length + input.Item5.Length + input.Item6.Length + 1;
            string[] final = new string[length];
            int a = 0;
            for (int i = 0; i < 18; i++)
            {
                //Weights0
                for (int j = 0; j < 25; j++)
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

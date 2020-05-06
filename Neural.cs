using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Snake
{
    class Network
    {
        private double[,] Weights0 = new double[18, 24];
        private double[,] Weights1 = new double[18, 18];
        private double[,] Weights2 = new double[4, 18];

        private double[] Offset0 = new double[18];
        private double[] Offset1 = new double[18];
        private double[] Offset2 = new double[4];

        public Network()
        {
            Randomize();
            Console.WriteLine(run(new double[24]));
        }

        //math functions missing in c#
        //dot product of weight matrix (mxn) and input vector (nx1)
        private double[] dot(double[,] weights, double[] input)
        {
            double[] output = new double[weights.GetUpperBound(0)+1];
            //make one sum per line of weights
            for (int m = 0; m < weights.GetUpperBound(0)+1; m++)
            {
                //sum of all elements of this line
                for (int n = 0; n < weights.GetUpperBound(1)+1; n++)
                {
                    output[m] += weights[m, n] * input[n];
                }
            }
            return output;
        }

        //add 2 vectors of same size together
        private double[] add(double[] in1, double[] in2)
        {
            double[] output = new double[in1.Length];
            for (int i = 0; i < in1.Length; i++)
                output[i] = in1[i] + in2[i];
            return output;
        }

        //applies sigmoid function to entire array
        private double[] Sigmoid(double[] input)
        {
            double[] output = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                output[i] = 1 / (1 + Math.Exp(-input[i]));
            return output;
        }

        public int run(double[] Layer0)
        {
            //input -> layer 1
            double[] Layer1 = dot(Weights0, Layer0);
            Layer1 = Sigmoid(add(Layer1, Offset0));
            //Layer1 -> Layer2
            double[] Layer2 = dot(Weights1, Layer1);
            Layer2 = Sigmoid(add(Layer2, Offset1));
            //Layer2 -> output
            double[] Layer3 = dot(Weights2, Layer2);
            Layer3 = Sigmoid(add(Layer3, Offset2));

            int highestNeurone = -1;
            double value = -1;
            for (int i = 0; i < Layer3.Length; i++)
            {
                if (Layer3[i] > value)
                { highestNeurone = i; value = Layer3[i]; }
            }
            return highestNeurone;
        }

        private void Randomize()
        {
            Random rand = new Random();
            for (int m = 0; m < Weights0.GetUpperBound(0)+1; m++)
            {
                for (int n = 0; n < Weights0.GetUpperBound(1) + 1; n++)
                    Weights0[m, n] = 2 / (rand.Next(20) + 1);
            }
            for (int m = 0; m < Weights1.GetUpperBound(0) + 1; m++)
            {
                for (int n = 0; n < Weights1.GetUpperBound(1) + 1; n++)
                    Weights1[m, n] = 2 / (rand.Next(20) + 1);
            }
            for (int m = 0; m < Weights2.GetUpperBound(0) + 1; m++)
            {
                for (int n = 0; n < Weights2.GetUpperBound(1) + 1; n++)
                    Weights2[m, n] = 2 / (rand.Next(20)+1);
            }
            for (int i = 0; i < Offset0.Length; i++)
                Offset0[i] = 2 / (rand.Next(20) + 1);
            for (int i = 0; i < Offset1.Length; i++)
                Offset1[i] = 2 / (rand.Next(20) + 1);
            for (int i = 0; i < Offset2.Length; i++)
                Offset2[i] = 2 / (rand.Next(20) + 1);
        }
    }
}

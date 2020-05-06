using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Snake
{
    class Network
    {
        private double[,] Weights0;
        private double[,] Weights1;
        private double[,] Weights2;

        private double[] Offset0;
        private double[] Offset1;
        private double[] Offset2;

        public Network()
        {
            Weights0 = new double[24, 18];
            double[] test = new double[3];
            double[] test1 = new double[3];
            double[] test2 = add(test1, test);            
        }

        //math functions missing in c#
        //dot product of weight matrix (mxn) and input vector (nx1)
        private double[] dot(double[] input, double[,] weights)
        {
            double[] output = new double[weights.GetUpperBound(1)];
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
                output[i] = 1 / (1 + Math.Exp(input[i]));
            return output;
        }
    }
}

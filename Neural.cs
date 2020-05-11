using System;
using Dataset = System.Tuple<double[,], double[,], double[,], double[], double[], double[]>;

namespace Snake
{
    class Network
    {
        private double[,] Weights0 = new double[18, 25];
        private double[,] Weights1 = new double[18, 18];
        private double[,] Weights2 = new double[4, 18];

        private double[] Offset0 = new double[18];
        private double[] Offset1 = new double[18];
        private double[] Offset2 = new double[4];

        public Network()
        {
        }

        //math functions missing in c#
        //dot product of weight matrix (mxn) and input vector (nx1)
        private double[] Dot(double[,] weights, double[] input)
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
        private double[] ReLu(double[] input)
        {
            double[] output = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
                output[i] = Math.Max(0, input[i]);          
            return output;
        }

        public int run(double[] Layer0)
        {
            //input -> layer 1
            double[] Layer1 = Dot(Weights0, Layer0);
            Layer1 = ReLu(add(Layer1, Offset0));
            //Layer1 -> Layer2
            double[] Layer2 = Dot(Weights1, Layer1);
            Layer2 = ReLu(add(Layer2, Offset1));
            //Layer2 -> output
            double[] Layer3 = Dot(Weights2, Layer2);
            Layer3 = ReLu(add(Layer3, Offset2));

            int highestNeurone = -1;
            double value = -1;
            for (int i = 0; i < Layer3.Length; i++)
            {
                if (Layer3[i] > value)
                { highestNeurone = i; value = Layer3[i]; }
            }
            return highestNeurone;
        }


        public void setValues(Dataset data)
        {
            Weights0 = data.Item1;
            Weights1 = data.Item2;
            Weights2 = data.Item3;
            Offset0 = data.Item4;
            Offset1 = data.Item5;
            Offset2 = data.Item6;
        }

        public Dataset getValues()
        {
            return Tuple.Create(Weights0, Weights1, Weights2, Offset0, Offset1, Offset2);
        }
    }
}

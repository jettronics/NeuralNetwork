using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Security.Cryptography;

public class Net
{
    public enum ActFctType { Sigmoid = 0, PLU, ReLu, LeReLu, Linear, SoftMax }; // Linear used for last layer in case of linear sum from previous layer
    public static readonly String[] ActFctTypeStr = { "Sigmoid", "PLU", "ReLu", "LeReLu", "Linear", "SoftMax" };

    public static double randomWeight()
    {

        int randValI = 0;
        do
        {
            randValI = new Random(Guid.NewGuid().GetHashCode()).Next(-50000, 50000);
            //randValI = new Random(Guid.NewGuid().GetHashCode()).Next(-70000, 70000);
        }
        while (Math.Abs(randValI) < 500);
        double randValD = ((double)randValI) / 100000.0;
        return randValD;
    }

    public Net()
    {
        weightsFileArg = "";
        tplgy = new List<int>();
        actFct = new List<ActFctType>();
        symmetricRange = true;
        
        //layers = new List<List<Neuron>>();

        //output = new List<double>();
        //inputMax = new List<double>();
        //inputMin = new List<double>();
        //inputScaled = new List<double>();
 
        //StreamReader linesRead = new StreamReader("");
    }

    ~Net()
    {
    }

    public void createNet(List<int> topology, List<ActFctType> actuatFct)
    {
        tplgy = topology;
        actFct = actuatFct;

        symmetricRange = true;

        int numLayers = topology.Count;
        layers = new List<List<Neuron>>();
        List<double> weights = new List<double>();

        output = new List<double>();
        inputMax = new List<double>();
        inputMin = new List<double>();
        inputScaled = new List<double>();
        for (int i = 0; i < topology.ElementAt(0); i++)
        {
            inputMax.Add(-1000000.0);
            inputMin.Add(1000000.0);
        }

        // Don't take output layer into account
        for (int j = 0; j < (actFct.Count-1); j++)
        {
            if ((actFct.ElementAt(j) == Net.ActFctType.ReLu) ||
                (actFct.ElementAt(j) == Net.ActFctType.LeReLu))
            {
                symmetricRange = false;
            }
        }

        int numEntries = 0;
        for (int layerNum = 0; layerNum < numLayers; layerNum++)
        {
            List<Neuron> Neurons = new List<Neuron>();
            layers.Add(Neurons);

            for (int neuronNum = 0; neuronNum < topology.ElementAt(layerNum); neuronNum++)
            {
                Neuron neuron; 

                double bias = 0.0;
                numEntries++;
                weights.Clear();

                if (layerNum == 0)
                {
                    neuron = new Neuron(Net.ActFctType.Linear);
                    
                    weights.Add(1.0);
                    numEntries++;
                }
                else
                {
                    neuron = new Neuron(actFct.ElementAt(layerNum - 1));

                    for (int i = 0; i < topology.ElementAt(layerNum - 1); i++)
                    {
                        //In a net with PLU don't initialize with 0 then gradients become 0
                        weights.Add(randomWeight());
                        numEntries++;
                    }
                }
                neuron.initWeights(weights);
                if ((layerNum == (numLayers - 1)) &&
                    (symmetricRange == false) &&
                    (actFct.ElementAt(layerNum - 1) == Net.ActFctType.Sigmoid))
                {
                    bias = -0.5;
                    neuron.setParamT(0.5);
                }
                neuron.initBias(bias);
                layers.Last().Add(neuron);
            }
        }
        
    }

    public void loadNet(String weightsFile)
    {
        weightsFileArg = weightsFile;
        tplgy.Clear();
        actFct.Clear();

        symmetricRange = true;

        layers = new List<List<Neuron>>();
        List<double> weights = new List<double>();

        output = new List<double>();
        inputMax = new List<double>();
        inputMin = new List<double>();
        inputScaled = new List<double>();
        
        StreamReader linesRead = new StreamReader(weightsFileArg);
        
        if ( File.Exists(weightsFileArg) == false )
        {
            return;
        }

        String line = "";
        while ((line != "\n\r") && (line != "\r\n"))
        {
            line = linesRead.ReadLine();
            int layerInputCnt = Convert.ToUInt16(line);
            tplgy.Add(layerInputCnt);
            line = linesRead.ReadLine();
            bool found = false;
            Net.ActFctType actFctLoc = ActFctType.Sigmoid;
            for (int j = 0; j < Net.ActFctTypeStr.Length; j++)
            {
                if (Net.ActFctTypeStr.ElementAt(j) == line)
                {
                    found = true;
                    actFctLoc = (Net.ActFctType)j;
                    break;
                }
            }
            if (found == true)
            {
                actFct.Add(actFctLoc);
            }
        }

        int numLayers = tplgy.Count;

        // Don't take output layer into account
        for (int j = 0; j < (actFct.Count - 1); j++)
        {
            if ((actFct.ElementAt(j) == Net.ActFctType.ReLu) ||
                (actFct.ElementAt(j) == Net.ActFctType.LeReLu))
            {
                symmetricRange = false;
            }
        }

        int numEntries = 0;
        for (int layerNum = 0; layerNum < numLayers; layerNum++)
        {
            List<Neuron> Neurons = new List<Neuron>();
            layers.Add(Neurons);

            for (int neuronNum = 0; neuronNum < tplgy.ElementAt(layerNum); neuronNum++)
            {
                Neuron neuron;

                double bias = 0.0;
                numEntries++;
                weights.Clear();
                
                if (layerNum == 0)
                {
                    neuron = new Neuron(Net.ActFctType.Linear);

                    weights.Add(1.0);
                    numEntries++;
                }
                else
                {
                    neuron = new Neuron(actFct.ElementAt(layerNum - 1));
                    
                    for (int i = 0; i < tplgy.ElementAt(layerNum - 1); i++)
                    {
                        line = linesRead.ReadLine();
                        double weight = Convert.ToDouble(line);
                        weights.Add(weight);
                        numEntries++;
                        //cout << "Weight " << numEntries << ": " << weight << endl;
                    }
                    
                    line = linesRead.ReadLine();
                    bias = Convert.ToDouble(line);
                    numEntries++;
                }
                //cout << "Bias " << numEntries << ": " << bias << endl;
                neuron.initWeights(weights);
                if ((layerNum == (numLayers - 1)) &&
                    (symmetricRange == false) &&
                    (actFct.ElementAt(layerNum - 1) == Net.ActFctType.Sigmoid))
                {
                    neuron.setParamT(0.5);
                }
                neuron.initBias(bias);
                layers.Last().Add(neuron);
            }
        }    
        linesRead.Close();
    }
 
    public void saveNet(String weightsFile)
    {
        int numLayers = tplgy.Count;

        weightsFileArg = weightsFile;

        File.Delete(weightsFileArg);

        StreamWriter linesWrite = new StreamWriter(weightsFileArg);

        String line;

        line = tplgy.ElementAt(0).ToString();
        linesWrite.WriteLine(line);

        for (int i = 1; i < numLayers; i++)
        {
            line = tplgy.ElementAt(i).ToString();
            linesWrite.WriteLine(line);
            int actInd = (int)actFct.ElementAt(i-1);
            line = Net.ActFctTypeStr.ElementAt(actInd);
            linesWrite.WriteLine(line);
        }

        linesWrite.WriteLine("\n\r");

        int numEntries = 0;
        for (int layerNum = 0; layerNum < numLayers; layerNum++)
        {
            for (int neuronNum = 0; neuronNum < tplgy.ElementAt(layerNum); neuronNum++)
            {
                List<double> wghts = new List<double>();
                wghts = layers[layerNum].ElementAt(neuronNum).getWeights();
                    
                for (int i = 0; i < wghts.Count; i++)
                {
                    line = wghts.ElementAt(i).ToString();
                    linesWrite.WriteLine(line);
                    numEntries++;
                }
                double bias = layers[layerNum].ElementAt(neuronNum).getBias();
                line = bias.ToString();
                linesWrite.WriteLine(line);
                numEntries++;
            }
        }

        linesWrite.Close();
    }
    
    public void feedForward(List<double> feedIn)
    {
        //assign the input values to the input neurons 
        //neurons acting as knots only
        for (int i = 0; i < feedIn.Count; i++)
        {
            //cout << "feedForward: feedIn " << i << ", val: " << feedIn->at(i) << endl;
            layers[0][i].setInput(feedIn.ElementAt(i));
        }

        for (int n = 0; n < layers[0].Count; n++)
        {
            // Calculate output of input layer
            layers[0][n].calcOutput(layers[0], 0);
        }

        for (int i = 1; i < layers.Count; i++)
        {
            //cout << "feedForward: layer " << i << endl;
            for (int n = 0; n < layers[i].Count; n++)
            {
                //cout << "feedForward: neuron " << n << endl;
                layers[i][n].calcOutput(layers[i - 1], i);
            }
        }

        for (int n = 0; n < layers.Last().Count; n++)
        {
            layers.Last()[n].calcSoftMaxOutput(layers.Last(), n);
        }
    }

    public double loss(List<double> targetOut)
    {
        //vector<Neuron> *outputLayer = &layers.back();
        double mse = 0.0;

        for (int n = 0; n < layers.Last().Count; n++)
        {
            if (layers.Last()[n].getActivationFct() == Net.ActFctType.SoftMax)
            {
                double output = layers.Last()[n].getOutput();
                if (Math.Abs(output) < 0.000000001)
                {
                    output = Math.Sign(output) * 0.000000001;
                }
                mse += ((-targetOut.ElementAt(n)) * Math.Log(output));
            }
            else
            {
                double delta = targetOut.ElementAt(n) - layers.Last()[n].getOutput();
                mse += ((delta * delta) / (double)layers.Last().Count);
            }
        }
        return mse;
    }

    public void batchGradientDescent(List<double> targetOut)
    {
        for (int n = 0; n < layers.Last().Count; n++)
        {
            layers.Last()[n].calcGradient(targetOut, n, Neuron.GradCalcMethod.SumUp);
            //layers.Last()[n].calcGradient(targetOut, n, Neuron.GradCalcMethod.Direct);
        }

        for (int layerNum = layers.Count - 2; layerNum > 0; layerNum--)
        {
            List<Neuron> act = layers[layerNum];
            List<Neuron> right = layers[layerNum + 1];

            //cout << "Layer Gradient calc: " << layerNum << endl;
            for (int n = 0; n < act.Count; n++)
            {
                act.ElementAt(n).calcGradient(right, n, Neuron.GradCalcMethod.SumUp);
                //act.ElementAt(n).calcGradient(right, n, Neuron.GradCalcMethod.Direct);
            }
        }
    }

    public void batchGradientAverage(List<double> targetOut)
    {
        for (int n = 0; n < layers.Last().Count; n++)
        {
            layers.Last()[n].calcGradient(targetOut, n, Neuron.GradCalcMethod.SumApply);
        }

        for (int layerNum = layers.Count - 2; layerNum > 0; layerNum--)
        {
            List<Neuron> act = layers[layerNum];
            List<Neuron> right = layers[layerNum + 1];

            //cout << "Layer Gradient calc: " << layerNum << endl;
            for (int n = 0; n < act.Count; n++)
            {
                act.ElementAt(n).calcGradient(right, n, Neuron.GradCalcMethod.SumApply);
            }
        }
    }

    public void updateWeights(double beta)
    {
        for (int layerNum = layers.Count - 1; layerNum > 0; layerNum--)
        {
            List<Neuron> act = layers[layerNum];
            List<Neuron> left = layers[layerNum - 1];

            //cout << "Layer Weights update: " << layerNum << endl;
            //Debug.Print("Weight Layer: " + layerNum);
            for (int n = 0; n < act.Count; n++)
            {
                act.ElementAt(n).updateWeights(left, beta, Neuron.GradCalcMethod.SumApply);
                //act.ElementAt(n).updateWeights(left, beta, Neuron.GradCalcMethod.Direct);
            }
        }
    }

    public void backProp(List<double> targetOut, double beta)
    {
        //cout << "Back Prop Squared Error: " << error << endl;

        for (int n = 0; n < layers.Last().Count; n++)
        {
            layers.Last()[n].calcGradient(targetOut, n, Neuron.GradCalcMethod.Direct);
        }

        for (int layerNum = layers.Count - 2; layerNum > 0; layerNum--)
        {
            List<Neuron> act = layers[layerNum];
            List<Neuron> right = layers[layerNum + 1];

            //cout << "Layer Gradient calc: " << layerNum << endl;
            for (int n = 0; n < act.Count; n++)
            {
                act.ElementAt(n).calcGradient(right, n, Neuron.GradCalcMethod.Direct);
            }
        }

        for (int layerNum = layers.Count - 1; layerNum > 0; layerNum--)
        {
            List<Neuron> act = layers[layerNum];
            List<Neuron> left = layers[layerNum - 1];

            //cout << "Layer Weights update: " << layerNum << endl;
            for (int n = 0; n < act.Count; n++)
            {
                act.ElementAt(n).updateWeights(left, beta, Neuron.GradCalcMethod.Direct);
            }

        }
    }
    //function to get result from the neural network
    public List<double> getOutput()
    {
        output.Clear();

        for (int n = 0; n < layers.Last().Count; n++)
        {
            output.Add(layers.Last()[n].getOutput());
        }
        return output;
    }
    // function to be called for all training data to get min and max values for all input channels 
    // Argument takes all input channels for one row of input data
    public void rangeInput(List<double> input)
    {
        if( (input.Count == inputMax.Count) &&
            (input.Count == inputMin.Count) ) 
        { 
            for (int n = 0; n < input.Count; n++) 
            { 
                if( input.ElementAt(n) > inputMax.ElementAt(n) ) 
                {
                    inputMax[n] = input.ElementAt(n);
                }
                
                if (input.ElementAt(n) < inputMin.ElementAt(n))
                {
                    inputMin[n] = input.ElementAt(n);
                }

            }
        }
        return;
    }
    // function to be called for every training or test data to scale one row of input data for all input channels 
    // Argument takes all input channels for one row of input data
    public ref List<double> scaleInput(List<double> input)
    {
        inputScaled.Clear();

        if (symmetricRange == true)
        {
            for (int n = 0; n < input.Count; n++)
            {
                double m = (Neuron.Param.MinMaxAbs - (-Neuron.Param.MinMaxAbs)) / (inputMax.ElementAt(n) - inputMin.ElementAt(n));
                double normed = (m * (input.ElementAt(n) - inputMin.ElementAt(n))) + (-Neuron.Param.MinMaxAbs);
                inputScaled.Add(normed);
            }
        }
        else
        {
            for (int n = 0; n < input.Count; n++)
            {
                double m = 1.0 / (inputMax.ElementAt(n) - inputMin.ElementAt(n));
                double normed = m * (input.ElementAt(n) - inputMin.ElementAt(n));
                inputScaled.Add(normed);
            }
        }
        return ref inputScaled;
    }

    public List<double> getMaxRanges() { return inputMax; }
    public List<double> getMinRanges() { return inputMin; }

    protected List<double> inputMax, inputMin, inputScaled;
    protected List<double> output;
    protected List<List<Neuron>> layers; //layers[layerNum][neuronNum]
    protected List<int> tplgy;
    protected List<ActFctType> actFct;
    protected String weightsFileArg;
    protected bool symmetricRange;
}

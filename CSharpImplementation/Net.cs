using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

public class Net
{
    public enum ActFctType { Logistic = 0, PieceWiseLinear, ReLu, LeakyReLu, Linear }; // Linear used for last layer in case of linear sum from previous layer

    public static double randomWeight()
    {
        int randValI = new Random(Guid.NewGuid().GetHashCode()).Next(0, 100000);
        double randValD = ((double)randValI) / 100000.0;
        return randValD;
    }

    public Net()
    {
        weightsFileArg = "";
        tplgy = new List<int>();
        layers = new List<List<Neuron>>();

        output = new List<double>();
        inputMax = new List<double>();
        inputMin = new List<double>();
        inputScaled = new List<double>();

        StreamReader linesRead = new StreamReader("");
    }

    public Net(List<int> topology, String weightsFile, List<ActFctType> actFct)
    {
        weightsFileArg = weightsFile;

        tplgy = topology;

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

        StreamWriter linesWrite = new StreamWriter(weightsFileArg);
        StreamReader linesRead = new StreamReader(weightsFileArg);
        bool emptyFile = true;
        if ( File.Exists(weightsFileArg) == true )
        {
            emptyFile = false;
            
        }
        
        int numEntries = 0;
        for (int layerNum = 0; layerNum < numLayers; layerNum++)
        {
            bool lastLayer = false;
            if (layerNum >= (numLayers - 1))
            {
                lastLayer = true;
            }
            List<Neuron> Neurons = new List<Neuron>();
            layers.Add(Neurons);

            for (int neuronNum = 0; neuronNum < topology.ElementAt(layerNum); neuronNum++)
            {
                Neuron neuron = new Neuron(0, actFct.ElementAt(neuronNum), lastLayer);
                if (layerNum == 0)
                {
                    layers.Last().Add(neuron);
                }
                else
                {
                    weights.Clear();
                    for (int i = 0; i < topology.ElementAt(layerNum - 1); i++)
                    {
                        double weight = 0.0;
                        if (emptyFile == true)
                        {
                            weight = randomWeight();
                            String line = weight.ToString();
                            linesWrite.WriteLine(line);
                        }
                        else
                        {
                            String? line = linesRead.ReadLine();
                            weight = Convert.ToDouble(line);
                        }
                        weights.Add(weight);
                        numEntries++;
                        //cout << "Weight " << numEntries << ": " << weight << endl;
                    }
                    double bias = 0.0;
                    if (emptyFile == true)
                    {
                        String line = bias.ToString();
                        linesWrite.WriteLine(line);
                    }
                    else
                    {
                        String? line = linesRead.ReadLine();
                        bias = Convert.ToDouble(line);
                    }
                    numEntries++;
                    //cout << "Bias " << numEntries << ": " << bias << endl;
                    neuron.initWeights(weights);
                    neuron.initBias(bias);
                    layers.Last().Add(neuron);
                }
            }
        }    
        if( emptyFile == true )
        {
            linesWrite.Close();
        }
        else
        {
            linesRead.Close();
        }
    }

    ~Net()
    {
        int numLayers = tplgy.Count;

        File.Delete(weightsFileArg);

        StreamWriter linesWrite = new StreamWriter(weightsFileArg);

        int numEntries = 0;
        for (int layerNum = 0; layerNum < numLayers; layerNum++)
        {
            for (int neuronNum = 0; neuronNum < tplgy.ElementAt(layerNum); neuronNum++)
            {
                if (layerNum != 0)
                {
                    List<double> wghts = new List<double>();
                    wghts = layers[layerNum].ElementAt(neuronNum).getWeights();
                    String line;
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
            layers[0][i].setOutput(feedIn.ElementAt(i));
        }

        for (int i = 1; i < layers.Count; i++)
        {
            //cout << "feedForward: layer " << i << endl;
            for (int n = 0; n < layers[i].Count; n++)
            {
                //cout << "feedForward: neuron " << n << endl;
                layers[i][n].calcOutput(layers[i - 1]);
            }
        }
    }

    public double loss(List<double> targetOut)
    {
        //vector<Neuron> *outputLayer = &layers.back();
        double mse = 0.0;

        for (int n = 0; n < layers.Last().Count; n++)
        {
            //cout << "Target val: " << targetOut->at(n) << ", Output val: " << layers.back()[n].getOutput() << endl;
            double delta = targetOut.ElementAt(n) - layers.Last()[n].getOutput();
            mse += (delta * delta);
        }
        //mse = Math.Sqrt(mse / ((double)layers.Last().Count));
        mse /= ((double)layers.Last().Count);

        return mse;
    }

    public void backProp(List<double> targetOut, double beta)
    {
        //cout << "Back Prop Squared Error: " << error << endl;

        for (int n = 0; n < layers.Last().Count; n++)
        {
            layers.Last()[n].calcGradients(targetOut.ElementAt(n));
        }

        for (int layerNum = layers.Count - 2; layerNum > 0; layerNum--)
        {
            List<Neuron> act = layers[layerNum];
            List<Neuron> right = layers[layerNum + 1];

            //cout << "Layer Gradient calc: " << layerNum << endl;
            for (int n = 0; n < act.Count; n++)
            {
                act.ElementAt(n).calcGradients(right);
            }
        }

        for (int layerNum = layers.Count - 1; layerNum > 0; layerNum--)
        {
            List<Neuron> act = layers[layerNum];
            List<Neuron> left = layers[layerNum - 1];

            //cout << "Layer Weights update: " << layerNum << endl;
            for (int n = 0; n < act.Count; n++)
            {
                act.ElementAt(n).updateWeights(left, beta);
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

    public List<double> scaleInput(List<double> input)
    {
        inputScaled.Clear();

        for (int n = 0; n < input.Count; n++)
        {
            double m = (Neuron.Logistic.MinMaxAbs - (-Neuron.Logistic.MinMaxAbs)) / (inputMax.ElementAt(n) - inputMin.ElementAt(n));
            double normed = (m * (input.ElementAt(n) - inputMin.ElementAt(n))) + (-Neuron.Logistic.MinMaxAbs);
            inputScaled.Add(normed);
        }
        return inputScaled;
    }

    protected List<double> inputMax, inputMin, inputScaled;
    protected List<double> output;
    protected List<List<Neuron>> layers; //layers[layerNum][neuronNum]
    protected List<int> tplgy;
    protected String weightsFileArg;
}

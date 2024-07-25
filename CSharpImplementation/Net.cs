using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.Intrinsics;

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
    }

    public Net(List<int> topology, String weightsFile, ActFctType actFct)
    {
        error = 0.0;
        weightsFileArg = weightsFile;

        tplgy = topology;

        int numLayers = topology.Count;
        List<double> weights = new List<double>();

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
            List<List<Neuron>> layers = new List<List<Neuron>>();
            layers.Add(Neurons);

            for (int neuronNum = 0; neuronNum < topology.ElementAt(layerNum); neuronNum++)
            {
                Neuron neuron = new Neuron(0, actFct, lastLayer);
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
                            String line = linesRead.ReadLine();
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
                        String line = linesRead.ReadLine();
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
    
    public void printTopology()
    {
        for (int layerNum = 0; layerNum < layers.Count; layerNum++)
        {
            Debug.WriteLine("Layer " + layerNum + "contains " + layers[layerNum].Count + " neurons");
        }
    }

    public void feedForward(ref List<double> feedIn)
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
    public void backProp(ref List<double> targetOut, double beta)
    {
        //vector<Neuron> *outputLayer = &layers.back();
        error = 0.0;

        for (int n = 0; n < layers.Last().Count; n++)
        {
            //cout << "Target val: " << targetOut->at(n) << ", Output val: " << layers.back()[n].getOutput() << endl;
            double delta = targetOut.ElementAt(n) - layers.Last()[n].getOutput();
            error += (delta * delta);
        }
        error = Math.Sqrt(error / ((double)layers.Last().Count));

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
    public void getResults(ref List<double> result)
    {
        result.Clear();

        for (int n = 0; n < layers.Last().Count; n++)
        {
            result.Add(layers.Last()[n].getOutput());
        }
        return;
    }

    public List<List<double>> input, output; // input data already being max/min adapted outside of net class
    protected List<List<Neuron>> layers; //layers[layerNum][neuronNum]
    protected double error;
    protected List<int> tplgy;
    protected String weightsFileArg;
}

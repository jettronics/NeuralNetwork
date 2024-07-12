using System;
using System.IO;
using System.Numerics;
using System.Runtime.Intrinsics;

public class Net
{
    public enum ActFctType { Logistic = 0, PieceWiseLinear, ReLu, LeakyReLu };

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

    }
    
    public void printTopology()
    {

    }

    public void feedForward(List<double> feedIn)
    {

    }
    public void backProp(List<double> targetOut, double beta)
    {

    }
    //function to get result from the neural network
    public void getResults(List<double> result)
    {

    }

    protected List<List<Neuron>> layers; //layers[layerNum][neuronNum]
    protected double error;
    protected List<int> tplgy;
    protected String weightsFileArg;
    //fstream weightsFile;
}

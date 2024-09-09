using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

public class Neuron
{
    public struct Param { public const double MinMaxAbs = 1.7; public const double Grad = 0.294;  public const double T = 0.5; };

	public Neuron()
	{
        bias = 0.0;
        net = 0.0;
        activation = 0.0;
        gradient = 0.0;
        actFctSelect = Net.ActFctType.Linear;
        lastLayerNeuron = false;

        weights = new List<double>();
    }

    public Neuron(int numConnections, Net.ActFctType actFct, bool lastNeurons)
    {
        bias = 0.0;
        net = 0.0;
        activation = 0.0;
        gradient = 0.0;
        actFctSelect = actFct;
        lastLayerNeuron = lastNeurons;

        weights = new List<double>();
        for (int i = 0; i < numConnections; i++)
        {
            weights.Add(Net.randomWeight());
        }
        
    }

    protected List<double> weights;
    protected double bias;
    protected double net;
    protected double activation;
    protected double gradient;
    protected Net.ActFctType actFctSelect;
    protected bool lastLayerNeuron;

    public void setOutput(double val) 
    { 
        activation = val; 
    }
    public double getOutput() 
    { 
        return activation; 
    }
    public void calcOutput(List<Neuron> layer)
    {
        double sum = 0.0;

        for (int n = 0; n < layer.Count; n++)
        {
            //cout << "calcOutput: " << layer->at(n).activation << ", weight: " << weights[n] << endl;
            sum += (layer.ElementAt(n).activation * weights[n]);
            //cout << "calcOutput: sum: " << sum << endl;
        }

        net = sum + bias;

        activation = transferFct(net);
        //cout << "calcOutput: " << activation << ", bias: " << bias << endl;
        return;
    }
    public void calcGradients(double target)
    {
        double delta = activation - target;
        gradient = delta * transferFctDeriv(net);
        //gradient = transferFctDeriv(delta);
        //cout << "Gradient: " << gradient << ", Delta: " << delta << endl;
        return;
    }
    public void calcGradients(List<Neuron> layer)
    {
        double outputSum = 0.0;

        for (int n = 0; n < layer.Count; n++)
        {
            //outputSum += (layer->at(n).weights[n] * layer->at(n).gradient);
            outputSum += (weights[n] * layer.ElementAt(n).gradient);
        }

        //gradient = transferFctDeriv(outputSum);
        gradient = outputSum * transferFctDeriv(net);
        //cout << "Gradient: " << gradient << ", Sum: " << outputSum << endl;
        return;
    }
    public void updateWeights(List<Neuron> layer, double beta)
    {
        //cout << "layer->size(): " << layer->size() << " , weights.size(): " << weights.size() << endl;
        for (int n = 0; n < layer.Count; n++)
        {
            // Left layer
            //Neuron *neuron = &layer->at(n);

            // Neuron weight same position as left neuron
            //weights[n] = weights[n] - (beta * gradient * neuron->getOutput());
            weights[n] = weights[n] - (beta * gradient * layer.ElementAt(n).activation);
        }
        //double b = bias;
        bias = bias - (beta * gradient);
        //cout << "bias new: " << bias << ", old: " << b << endl;

        return;
    }
    public void initWeights(List<double> wghts)
    {
        for (int i = 0; i < wghts.Count; i++)
        {
            weights.Add(wghts.ElementAt(i));
        }
    }
    public void initBias(double b)
    { 
        bias = b; 
    }
    public List<double> getWeights() 
    { 
        return weights; 
    }
    public double getBias() 
    { 
        return bias; 
    }

	protected double transferFct(double inp)
    {
        double ret = 0.0;

        if (actFctSelect == Net.ActFctType.Sigmoid)
        {
            ret = (1.0 / (1.0 + Math.Exp(-inp / Param.T)));
        }
        else
        if (actFctSelect == Net.ActFctType.PieceWiseLinear)
        {
            if (inp >= Param.MinMaxAbs)
            {
                ret = 1.0;
            }
            else
            if (inp <= (-Param.MinMaxAbs))
            {
                ret = 0.0;
            }
            else
            {
                ret = (Param.Grad * inp) + 0.5;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.ReLu)
        {
            if (inp < 0.0)
            {
                ret = 0.0;
            }
            else
            {
                ret = inp;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.LeakyReLu)
        {
            if (inp < 0.0)
            {
                ret = 0.01 * inp;
            }
            else
            {
                ret = inp;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.Linear)
        {
            ret = inp;
        }
        //cout << "transferFct: " << in << " -> " << sigmoid << endl;
        return ret;
    }
    protected double transferFctDeriv(double inp)
    {
        double ret = 0.0;

        if (actFctSelect == Net.ActFctType.Sigmoid)
        {
            ret = transferFct(inp) * (1 - transferFct(inp));
        }
        else
        if (actFctSelect == Net.ActFctType.PieceWiseLinear)
        {
            if (inp >= Param.MinMaxAbs)
            {
                ret = 0.0;
            }
            else
            if (inp <= (-Param.MinMaxAbs))
            {
                ret = 0.0;
            }
            else
            {
                ret = Param.Grad;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.ReLu)
        {
            //ReLU
            if (inp < 0.0)
            {
                ret = 0.0;
            }
            else
            {
                ret = 1.0;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.LeakyReLu)
        {
            if (inp < 0.0)
            {
                ret = 0.01;
            }
            else
            {
                ret = 1.0;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.Linear)
        {
            ret = 1.0;
        }

        return ret;
    }

}

using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

public class Neuron
{
    public struct Param { public const double MinMaxAbs = 1.0;  
                          public const double T = 1.0 /*0.2*/; 
                          public const double GradUnlim = 0.01;
                          public const double GradZero = 1.0;
    }; 

    protected List<double> weights;
    protected double bias;
    protected double net;
    protected double activation;
    protected double gradient;
    protected Net.ActFctType actFctSelect;
    //protected double gradZero;
    protected double input;
    protected double xUnlim;
    protected double bUnlim;

    public Neuron()
	{
        bias = 0.0;
        net = 0.0;
        activation = 0.0;
        gradient = 0.0;
        input = 0.0;
        actFctSelect = Net.ActFctType.Linear;

        weights = new List<double>();

        //gradZero = (1.0 / Param.T) * (fctSigmoid(0.0) * (1 - fctSigmoid(0.0)));
        xUnlim = Param.MinMaxAbs / Param.GradZero;
        bUnlim = Param.MinMaxAbs * (1.0 - (Param.GradUnlim / Param.GradZero));
    }

    public Neuron(Net.ActFctType actFct)
    {
        bias = 0.0;
        net = 0.0;
        activation = 0.0;
        gradient = 0.0;
        input = 0.0;
        actFctSelect = actFct;

        weights = new List<double>();

        //gradZero = (1.0 / Param.T) * (fctSigmoid(0.0) * (1 - fctSigmoid(0.0)));
        xUnlim = Param.MinMaxAbs / Param.GradZero;
        bUnlim = Param.MinMaxAbs * (1.0 - (Param.GradUnlim / Param.GradZero));
    }
    public void setInput(double val) 
    { 
        input = val; 
    }
    public double getOutput() 
    { 
        return activation; 
    }
    public void calcOutput(List<Neuron> layer, int actN, int actL)
    {
        double sum = 0.0;

        if (actFctSelect == Net.ActFctType.SoftMax)
        {
            for (int n = 0; n < layer.Count; n++)
            {
                //cout << "calcOutput: " << layer->at(n).activation << ", weight: " << weights[n] << endl;
                sum += Math.Exp(layer.ElementAt(n).activation);
                //cout << "calcOutput: sum: " << sum << endl;
            }

            net = Math.Exp(layer.ElementAt(actN).activation)/sum;
        }
        else
        {
            if (actL == 0)
            {
                sum = weights[0] * input;
            }
            else
            {
                for (int n = 0; n < layer.Count; n++)
                {
                    //cout << "calcOutput: " << layer->at(n).activation << ", weight: " << weights[n] << endl;
                    sum += (layer.ElementAt(n).activation * weights[n]);
                    //cout << "calcOutput: sum: " << sum << endl;
                }
            }
            net = sum + bias;
        }

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
    public void calcGradients(List<Neuron> layer, int act)
    {
        double outputSum = 0.0;

        for (int n = 0; n < layer.Count; n++)
        {
            if (layer.ElementAt(n).actFctSelect == Net.ActFctType.SoftMax)
            {
                if (act == n)
                {
                    // Weights in SoftMax Neuron always 1.0
                    outputSum = layer.ElementAt(n).gradient * (1.0 - layer.ElementAt(n).gradient);
                }
                else
                {
                    outputSum = -(layer.ElementAt(n).gradient * layer.ElementAt(act).gradient);
                }
            }
            else
            {
                //outputSum += (layer->at(n).weights[n] * layer->at(n).gradient);
                //outputSum += (weights[n] * layer.ElementAt(n).gradient);
                outputSum += (layer.ElementAt(n).weights[act] * layer.ElementAt(n).gradient);
            }
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
            if(actFctSelect == Net.ActFctType.SoftMax)
            {
                weights[n] = 1.0;
            }
            else
            {
                weights[n] = weights[n] - (beta * gradient * layer.ElementAt(n).activation);
            }
        }
        //double b = bias;
        if (actFctSelect == Net.ActFctType.SoftMax)
        {
            bias = 0.0;
        }
        else
        {
            bias = bias - (beta * gradient);
        }
            
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

    protected double fctSigmoid(double x)
    {
        double ret = 1.0 / (1.0 + Math.Exp(-x / Param.T));
        return ret;
    }

    protected double fctPLU(double x)
    {
        return (Math.Sign(x) * (Math.Min(Param.GradZero * Math.Abs(x), (Param.GradUnlim * Math.Abs(x)) + bUnlim)));
    }

	protected double transferFct(double inp)
    {
        double ret = 0.0;

        if (actFctSelect == Net.ActFctType.Sigmoid)
        {
            ret = fctSigmoid(inp);
        }
        else
        if (actFctSelect == Net.ActFctType.PLU)
        {
            ret = fctPLU(inp);
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
                ret = Param.GradZero * inp;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.LeakyReLu)
        {
            if (inp < 0.0)
            {
                ret = Param.GradUnlim * inp;
            }
            else
            {
                ret = Param.GradZero * inp;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.Linear)
        {
            ret = inp;
        }
        else
        if (actFctSelect == Net.ActFctType.SoftMax)
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
            ret = (1.0 / Param.T) * (transferFct(inp) * (1 - transferFct(inp)));
        }
        else
        if (actFctSelect == Net.ActFctType.PLU)
        {
            if (Math.Abs(inp) > xUnlim)
            {
                ret = Param.GradUnlim;
            }
            else
            {
                ret = Param.GradZero;
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
                ret = Param.GradZero;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.LeakyReLu)
        {
            if (inp < 0.0)
            {
                ret = Param.GradUnlim;
            }
            else
            {
                ret = Param.GradZero;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.Linear)
        {
            ret = 1.0;
        }
        else
        if (actFctSelect == Net.ActFctType.SoftMax)
        {
            ret = 1.0;
        }

        return ret;
    }

}

using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Diagnostics;

public class Neuron
{
    public struct Param { public const double MinMaxAbs = 1.0;  
                          public const double GradUnlim = 0.01;
                          public const double GradZero = 1.0;
    }; 

    protected List<double> weights;
    protected double bias;
    protected double net;
    protected double activation;
    protected double prediction;
    protected double gradient;
    protected Net.ActFctType actFctSelect;
    //protected double gradZero;
    protected double input;
    protected double xUnlim;
    protected double bUnlim;
    protected double T, O;
    protected bool init;

    public Neuron()
	{
        bias = 0.0;
        net = 0.0;
        activation = 0.0;
        prediction = 0.0;
        gradient = 0.0;
        input = 0.0;
        actFctSelect = Net.ActFctType.Linear;
        T = 1.0;
        O = 0.0;

        weights = new List<double>();

        //gradZero = (1.0 / Param.T) * (fctSigmoid(0.0) * (1 - fctSigmoid(0.0)));
        xUnlim = Param.MinMaxAbs / Param.GradZero;
        bUnlim = Param.MinMaxAbs * (1.0 - (Param.GradUnlim / Param.GradZero));

        init = true;
    }

    public Neuron(Net.ActFctType actFct)
    {
        bias = 0.0;
        net = 0.0;
        activation = 0.0;
        prediction = 0.0;
        gradient = 0.0;
        input = 0.0;
        actFctSelect = actFct;
        T = 1.0;
        O = 0.0;

        weights = new List<double>();

        //gradZero = (1.0 / Param.T) * (fctSigmoid(0.0) * (1 - fctSigmoid(0.0)));
        xUnlim = Param.MinMaxAbs / Param.GradZero;
        bUnlim = Param.MinMaxAbs * (1.0 - (Param.GradUnlim / Param.GradZero));

        init = true;
    }

    public void setInput(double val) 
    { 
        input = val; 
    }

    public void setParams(double paramT, double paramO)
    {
        T = paramT;
        O = paramO;
    }

    public double getOutput() 
    { 
        return prediction; 
    }

    public Net.ActFctType getActivationFct()
    {
        return actFctSelect;
    }
    
    public void calcOutput(List<Neuron> layer, int actL)
    {
        double sum = 0.0;
                
        if (actL == 0)
        {
            sum = weights[0] * input;
        }
        else
        {
            for (int n = 0; n < layer.Count; n++)
            {
                //cout << "calcOutput: " << layer->at(n).activation << ", weight: " << weights[n] << endl;
                sum += (layer.ElementAt(n).prediction * weights[n]);
                //cout << "calcOutput: sum: " << sum << endl;
            }
        }
        net = sum + bias;
        
        activation = transferFct(net);
        if(actFctSelect != Net.ActFctType.SoftMax)
        {
            prediction = activation;
        }
        //cout << "calcOutput: " << activation << ", bias: " << bias << endl;
        return;
    }

    public void calcSoftMaxOutput(List<Neuron> layer, int actN)
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

            if(Math.Abs(sum) < 0.001)
            {
                sum = Math.Sign(sum) * 0.001;
            }

            prediction = Math.Exp(layer.ElementAt(actN).activation) / sum;
            if( (Double.IsNaN(prediction) || Double.IsInfinity(prediction)) == true )
            {
                Debug.Print("Prediction value NaN or Infinity: " + prediction);
            }
            if( prediction > 1.0 )
            {
                prediction = 1.0;
            }
        }

        return;
    }

    public void calcGradient(List<double> target, int act, int batchSize)
    {
        double delta = prediction - target.ElementAt(act);
        if (actFctSelect == Net.ActFctType.SoftMax)
        {
            double sumTarget = target.Sum();
            double deltaGrad = (-target.ElementAt(act)) + (prediction * sumTarget);
            //gradient += deltaGrad;
            if (init == true)
            {
                gradient = deltaGrad;
                init = false;
            }
            else
            {
                if (batchSize > 1)
                {
                    gradient = gradient + ((1.0 / (double)batchSize) * (deltaGrad - gradient));
                }
                else
                {
                    gradient = deltaGrad;
                }
            }
        }
        else
        {
            double deltaGrad = delta * transferFctDeriv(net);
            //gradient += deltaGrad;
            if (init == true)
            {
                gradient = deltaGrad;
                init = false;
            }
            else
            {
                if (batchSize > 1)
                {
                    gradient = gradient + ((1.0 / (double)batchSize) * (deltaGrad - gradient));
                }
                else
                {
                    gradient = deltaGrad;
                }
            }
        }
        
        
        //gradient = transferFctDeriv(delta);
        //cout << "Gradient: " << gradient << ", Delta: " << delta << endl;
        return;
    }

    public void calcGradient(List<Neuron> layer, int act, int batchSize)
    {
        double outputSum = 0.0;

        for (int n = 0; n < layer.Count; n++)
        {
            //outputSum += (layer->at(n).weights[n] * layer->at(n).gradient);
            //outputSum += (weights[n] * layer.ElementAt(n).gradient);
            outputSum += (layer.ElementAt(n).weights[act] * layer.ElementAt(n).gradient);
        }
                
        double deltaGrad = outputSum * transferFctDeriv(net);
        //gradient += deltaGrad;
        if (init == true)
        {
            gradient = deltaGrad;
            init = false;
        }
        else
        {
            if (batchSize > 1)
            {
                gradient = gradient + ((1.0 / (double)batchSize) * (deltaGrad - gradient));
            }
            else
            {
                gradient = deltaGrad;
            }
        }
        
        
        //cout << "Gradient: " << gradient << ", Sum: " << outputSum << endl;
        return;
    }

    public void updateWeights(List<Neuron> layer, double beta, double weightsLimit)
    {
        //cout << "layer->size(): " << layer->size() << " , weights.size(): " << weights.size() << endl;
        for (int n = 0; n < layer.Count; n++)
        {
            // Left layer
            //Neuron *neuron = &layer->at(n);

            // Neuron weight same position as left neuron
            //weights[n] = weights[n] - (beta * gradient * neuron->getOutput());
            double weight = weights[n] - (beta * gradient * layer.ElementAt(n).prediction);
            if(Math.Abs(weight) < weightsLimit)
            {
                weights[n] = weight;    
            }
            else
            {
                Debug.Print("Weight limited: " + n + ", val: " + weights[n] + ", grad: " + gradient + ", pred: " + layer.ElementAt(n).prediction);
            }
            //Debug.Print("Weight: " + n + ", val: " + weights[n] + ", grad: " + gradient + ", pred: " + layer.ElementAt(n).prediction);
        }
        //double b = bias;
        bias = bias - (beta * gradient);
        
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
        double ret = 1.0 / (1.0 + Math.Exp(-(x + O) / T));
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
            if (inp <= 0.0)
            {
                ret = 0.0;
            }
            else
            {
                ret = Param.GradZero * inp;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.LeReLu)
        {
            if (inp <= 0.0)
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
            ret = (1.0 / T) * (transferFct(inp) * (1 - transferFct(inp)));
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
            if (inp <= 0.0)
            {
                ret = 0.0;
            }
            else
            {
                ret = Param.GradZero;
            }
        }
        else
        if (actFctSelect == Net.ActFctType.LeReLu)
        {
            if (inp <= 0.0)
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

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static Net;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices;
using System.Data.Common;
using System.Globalization;
using System.Xml.Linq;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Forms.DataVisualization.Charting;

namespace Windows
{
    public partial class NeuralNetwork : Form
    {
        protected Net network;
        protected List<int> topology;
        protected List<ActFctType> actFct;
        protected Reader reader;
        protected int inputRowCnt;
        protected List<double> row;
        protected bool training;
        protected bool testing;
        protected int loopDataIdx;
        protected double learningRate;
        protected int numOutputColumns;
        protected int epochIdx, epochMax;
        protected List<double> refOutput;
        protected int scrollBarWheelTurns;
        protected bool lossChartMouseClick;

        public NeuralNetwork()
        {
            InitializeComponent();
            topology = new List<int>();
            actFct = new List<ActFctType>();
            inputRowCnt = 0;
            row = new List<double>();
            training = false;
            testing = false;
            loopDataIdx = 0;
            learningRate = 0.001;
            numOutputColumns = 1;
            epochIdx = 0;
            epochMax = 10;
            scrollBarWheelTurns = 0;
            lossChartMouseClick = false;

            /*
            The number of neurons in the input layer is equal to the number of features in the data and in very rare cases, 
            there will be one input layer for bias. Whereas the number of neurons in the output depends on whether the model 
            is used as a regressor or classifier. If the model is a regressor then the output layer will have only a single 
            neuron but in case the model is a classifier it will have a single neuron or multiple neurons depending on the 
            class label of the model.

            When it comes to the hidden layers, the main concerns are how many hidden layers and how many neurons are required? 
            There are many rule-of-thumb methods for determining the correct number of neurons to use in the hidden layers, 
            such as the following:

            The number of 1st layer hidden neurons should be 2/3 the size of the input layer, plus the size of the output layer.
            The number of 1st layer hidden neurons should be less than twice the size of the input layer.
            The number of 2nd layer and above hidden neurons should be between the size of the input layer and the size of the output layer.
            Moreover, the number of neurons and number of layers required for the hidden layer also depends upon training cases, 
            amount of outliers, the complexity of, data that is to be learned, and the type of activation functions used.

            Most of the problems can be solved by using a single hidden layer with the number of neurons equal to the mean of the 
            input and output layer. If less number of neurons is chosen it will lead to underfitting and high statistical bias. 
            Whereas if we choose too many neurons it may lead to overfitting, high variance, and increases the time it takes to 
            train the network.

            There's one additional rule of thumb that helps for supervised learning problems. 
            You can usually prevent over-fitting if you keep your number of neurons below:
            Nh=Ns/(α∗(Ni+No))

            Ni = number of input neurons.
            No = number of output neurons.
            Ns = number of samples in training data set.
            α = an arbitrary scaling factor usually 2-10.

            For classification problems using more than 1 neuron a softmax activation function should be preferred but 
            only if the values to be classified are part of one class e.g. not to be used for members which can be part of different courses.
            */
            /*
            topology.Add(10);
            actFct.Add(Net.ActFctType.Sigmoid);
            topology.Add(11);
            actFct.Add(Net.ActFctType.Sigmoid);
            topology.Add(7);
            actFct.Add(Net.ActFctType.Sigmoid);
            topology.Add(4);
            actFct.Add(Net.ActFctType.Sigmoid);
            */
            //actFct.Add(Net.ActFctType.SoftMax);
            /* 
            Softmax activation function explained:
            https://www.pinecone.io/learn/softmax-activation/
            */

            //String weightsFile = Path.GetFullPath("weights.txt");

            network = new Net();
            LossChart.MouseWheel += LossChart_MouseWheel;

        }

        private void LossChart_Load(object sender, EventArgs e)
        {
            
        }

        private void openTrainingDataFile_Click(object sender, EventArgs e)
        {
            openTrainingDataCSVFileDialog.ShowDialog(this);
        }

        private void csvTrainingDataFileOpened(object sender, CancelEventArgs e)
        {
            outputTextBox.AppendText("Training data opened\r\n");
            numOutputColumns = 1;
            reader = new Reader(openTrainingDataCSVFileDialog.FileName);
            reader.Convert();
            
            outputTextBox.AppendText("Number of Input Nodes: " + reader.getNumInputNodes() + "\r\n");
            outputTextBox.AppendText("Input Names:\r\n");
            String[] colNames = reader.getColumnNames();
            for (int i = 0; i < colNames.Length - 1; i++)
            {
                outputTextBox.AppendText("   " + colNames[i] + "\r\n");

            }
            outputTextBox.AppendText("Output Name:\r\n");
            outputTextBox.AppendText("   " + colNames.Last() + "\r\n");
            outputTextBox.AppendText("Number of Classifiers: " + reader.getNumClassifiers() + "\r\n");
            if (reader.getNumClassifiers() > 1)
            {
                for (int i = 0; i < reader.getNumClassifiers(); i++)
                {
                    outputTextBox.AppendText("   " + reader.getClassifiers()[i] + "\r\n");
                }
            }
            else
            {
                outputTextBox.AppendText("   " + "Range 0...1\r\n");
            }
            outputTextBox.AppendText("Total Data number: " + reader.getNumTotalData() + "\r\n");
            outputTextBox.AppendText("----------------------------------------------\r\n");
            outputTextBox.AppendText("Define in following text box the inputs, outputs, layers and activation functions of the neural network.\r\n");
            outputTextBox.AppendText("Syntax: n th line number of inputs, n+1 th line activation function\r\n");
            outputTextBox.AppendText("Selection of activation functions: " + string.Join(", ", Net.ActFctTypeStr) + "\r\n");
        }

        private void generateNeuralNet_Click(object sender, EventArgs e)
        {
            topology.Clear();
            actFct.Clear(); 
            String netParams = netPropertyTextBox.Text;
            String netParamsReplaced = netParams.Replace("\r\n", ",");
            String[] netParamsSplitted = netParamsReplaced.Split(',');
            int len = netParamsSplitted.Length >> 1;
            bool errorOccurred = false;
            for (int i = 0; i < len; i++)
            {
                int topologyVal = Convert.ToUInt16(netParamsSplitted[i * 2]);

                bool found = false;
                Net.ActFctType actFctLoc = ActFctType.Sigmoid;
                String actFctString = netParamsSplitted[(i * 2) + 1];
                for ( int j = 0; j < Net.ActFctTypeStr.Length; j++ )
                {
                    if (Net.ActFctTypeStr.ElementAt(j) == actFctString)
                    {
                        found = true;
                        actFctLoc = (Net.ActFctType)j;
                        actFctString = Net.ActFctTypeStr.ElementAt(j);
                        break;
                    }
                }

                if (found == true)
                {
                    topology.Add(topologyVal);
                    actFct.Add(actFctLoc);
                    outputTextBox.AppendText("Layer added with " + topologyVal + " inputs and Activation Function " + actFctString + "\r\n");
                }
                else
                {
                    errorOccurred = true;
                    outputTextBox.AppendText("Error adding Layer with " + topologyVal + " inputs and Activation Function " + actFctString + "\r\n");
                }
            }
            if (errorOccurred == false)
            {
                network.createNet(topology, actFct);
                calculateMinMaxRange();
                outputTextBox.AppendText("All layers created successfully and input ranges analyzed\r\n");
            }
        }

        private void saveNeuralNet_Click(object sender, EventArgs e)
        {
            String weightsFile = Path.GetFullPath("weights.txt");
            network.saveNet(weightsFile);
            outputTextBox.AppendText("Network saved under: " + weightsFile + "\r\n");
        }

        private void calculateMinMaxRange()
        {
            inputRowCnt = reader.getInputData().Count;
            
            if (topology.Count == 0 )
            {
                outputTextBox.AppendText("Create Network first!\r\n");
                return;
            }

            for (int i = 0; i < reader.getInputData()[0].Count; i++)
            {
                row.Clear();
                for(int j = 0; j < inputRowCnt;j++)
                {
                    row.Add(reader.getInputData()[j][i]);
                }
                network.rangeInput(row);
            }
            outputTextBox.AppendText("Max and Min Ranges: \r\n");
            String[] colNames = reader.getColumnNames();
            for (int i = 0; i < colNames.Length - 1; i++)
            {
                outputTextBox.AppendText(colNames[i] + " - Max: " + network.getMaxRanges()[i] + ", Min: " + network.getMinRanges()[i] + "\r\n");
            }
        }

        private void outputTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void netPropertyTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void trainNeuralNet_Click(object sender, EventArgs e)
        {
            if (topology.Count == 0)
            {
                outputTextBox.AppendText("Create Network first!\r\n");
                return;
            }

            if (numOutputColumns == 1)
            {
                if (topology.Last() != reader.getNumClassifiers())
                {
                    outputTextBox.AppendText("Net number of outputs doesn't match with the output classifiers!\r\n");
                    return;
                }
            }

            refOutput = new List<double>();
            for (int i = 0; i < reader.getNumClassifiers(); i++)
            {
                refOutput.Add(0.0);
            }

            Double.TryParse(learningRateTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out learningRate);
            outputTextBox.AppendText("Learning rate set to: " + learningRate + "\r\n");

            epochMax = Convert.ToUInt16(epochMaxTextBox.Text);
            outputTextBox.AppendText("Number of Epochs: " + epochMax + "\r\n");

            int limitData = Convert.ToUInt16(limitTrainDataTextBox.Text);
            outputTextBox.AppendText("Limit Train Data to : " + limitData + "%\r\n");

            reader.LimitData(limitData);
            outputTextBox.AppendText("Number of taining data limited randomly to: " + reader.getInputTrainData()[0].Count + "\r\n");

            loopDataIdx = 0;

            scrollBarWheelTurns = 0;
            lossChartMouseClick = false;
            LossChart.Series["Loss"].Points.Clear();
                                                
            training = true;
            testing = false;
        }
        private void testNeuralNet_Click(object sender, EventArgs e)
        {
            if (topology.Count == 0)
            {
                outputTextBox.AppendText("Create Network first!\r\n");
                return;
            }

            if (numOutputColumns == 1)
            {
                if (topology.Last() != reader.getNumClassifiers())
                {
                    outputTextBox.AppendText("Net number of outputs doesn't match with the output classifiers!\r\n");
                    return;
                }
            }

            refOutput = new List<double>();
            for (int i = 0; i < reader.getNumClassifiers(); i++)
            {
                refOutput.Add(0.0);
            }

            int limitData = Convert.ToUInt16(limitTrainDataTextBox.Text);
            outputTextBox.AppendText("Limit Test Data to : " + limitData + "%\r\n");

            reader.TestData(limitData);
            outputTextBox.AppendText("Number of testing data limited randomly to: " + reader.getInputTestData()[0].Count + "\r\n");

            loopDataIdx = 0;

            scrollBarWheelTurns = 0;
            lossChartMouseClick = false;
            LossChart.Series["Loss"].Points.Clear();
            
            training = false;
            testing = true;
        }

        private void LossChart_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            scrollBarWheelTurns += e.Delta;

            if (scrollBarWheelTurns <= 0)
            {
                xAxis.ScaleView.ZoomReset();
            }
            else
            if (scrollBarWheelTurns > 0)
            {
                if (LossChart.Series["Loss"].Points.Count > 10)
                {
                    int xMinZoomPos = scrollBarWheelTurns >> 2;

                    if (xMinZoomPos >= LossChart.Series["Loss"].Points.Count)
                    {
                        xMinZoomPos = LossChart.Series["Loss"].Points.Count - 10;
                    }
                    xAxis.ScaleView.Zoom(xMinZoomPos, LossChart.Series["Loss"].Points.Count);
                }
            }
            
        }
        private void LossChart_MouseClick(object sender, MouseEventArgs e)
        {
            
        }
        private void LossChart_MouseDown(object sender, MouseEventArgs e)
        {
            lossChartMouseClick = true;
        }
        private void LossChart_MouseUp(object sender, MouseEventArgs e)
        {
            lossChartMouseClick = false;
        }

        private void Timer_Loop(object sender, EventArgs e)
        {
            if (training == true)
            {
                if (epochIdx < epochMax)
                {
                    if (loopDataIdx < reader.getInputTrainData()[0].Count)
                    {
                        row.Clear();
                        for (int j = 0; j < inputRowCnt; j++)
                        {
                            row.Add(reader.getInputTrainData()[j][loopDataIdx]);
                        }
                        List<double> refScaled = network.scaleInput(row);
                        network.feedForward(refScaled);

                        if (reader.getNumClassifiers() > 1)
                        {
                            for (int i = 0; i < refOutput.Count; i++)
                            {
                                if (i == (int)reader.getOutputTrainData(loopDataIdx))
                                {
                                    refOutput[i] = 1.0;
                                }
                                else
                                {
                                    refOutput[i] = 0.0;
                                }
                            }
                        }
                        else
                        {
                            refOutput[0] = reader.getOutputTrainData(loopDataIdx);
                        }
                        network.backProp(refOutput, learningRate);
                        
                        double losses = network.loss(refOutput);
                        LossChart.Series["Loss"].Points.Add(losses);
                        
                        var chartArea = LossChart.ChartAreas[LossChart.Series["Loss"].ChartArea];

                        if (lossChartMouseClick == false)
                        {
                            chartArea.AxisX.Maximum = LossChart.Series["Loss"].Points.Count;
                            chartArea.AxisX.ScaleView.Scroll(LossChart.Series["Loss"].Points.Count);
                        }

                        loopDataIdx++;
                    }
                    else
                    {
                        epochIdx++;
                        loopDataIdx = 0;
                    }
                }
                else 
                { 
                    training = false;
                    outputTextBox.AppendText("Training finished\r\n");
                }
            }
            else
            if (testing == true)
            {
                if (loopDataIdx < reader.getInputTestData()[0].Count)
                {
                    row.Clear();
                    for (int j = 0; j < inputRowCnt; j++)
                    {
                        row.Add(reader.getInputTestData()[j][loopDataIdx]);
                    }
                    List<double> refScaled = network.scaleInput(row);
                    network.feedForward(refScaled);

                    if (reader.getNumClassifiers() > 1)
                    {
                        for (int i = 0; i < refOutput.Count; i++)
                        {
                            if (i == (int)reader.getOutputTestData(loopDataIdx))
                            {
                                refOutput[i] = 1.0;
                            }
                            else
                            {
                                refOutput[i] = 0.0;
                            }
                        }
                    }
                    else
                    {
                        refOutput[0] = reader.getOutputTestData(loopDataIdx);
                    }

                    double losses = network.loss(refOutput);
                    LossChart.Series["Loss"].Points.Add(losses);

                    var chartArea = LossChart.ChartAreas[LossChart.Series["Loss"].ChartArea];

                    if (lossChartMouseClick == false)
                    {
                        chartArea.AxisX.Maximum = LossChart.Series["Loss"].Points.Count;
                        chartArea.AxisX.ScaleView.Scroll(LossChart.Series["Loss"].Points.Count);
                    }

                    loopDataIdx++;
                }
                else
                {
                    testing = false;
                    outputTextBox.AppendText("Testing finished\r\n");
                }
            }
        }
        private void stopTraining_Click(object sender, EventArgs e)
        {
            training = false;
            outputTextBox.AppendText("Training aborted by user\r\n");
        }
    }
}

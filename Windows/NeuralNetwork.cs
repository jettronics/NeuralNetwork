using System;
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
using System.Windows.Forms.VisualStyles;


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
        protected int totalTrainingData;
        protected int totalTestingData;
        protected Random random;
        protected int batchSize;
        protected List<int> trainingDataIndices;
        protected int batchCounter;

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
            learningRate = 0.1;
            numOutputColumns = 1;
            epochIdx = 0;
            epochMax = 100000;
            scrollBarWheelTurns = 0;
            lossChartMouseClick = false;
            refOutput = new List<double>();
            totalTrainingData = 0;
            totalTestingData = 0;
            batchSize = 1;
            trainingDataIndices = new List<int>();
            batchCounter = 0;

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
            AnalysisChart.MouseWheel += LossChart_MouseWheel;
            epochMaxTextBox.Text = epochMax.ToString();
            learningRateTextBox.Text = learningRate.ToString().Replace(",",".");
            batchSizeTextBox.Text = batchSize.ToString();
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
            for (int i = 1; i < AnalysisChart.Series.Count; i++)
            {
                AnalysisChart.Series.RemoveAt(i);
            }
            refOutput.Clear();
            if (reader.getNumClassifiers() > 1)
            {
                for (int i = 0; i < reader.getNumClassifiers(); i++)
                {
                    outputTextBox.AppendText("   " + reader.getClassifiers()[i] + "\r\n");
                    refOutput.Add(0.0);
                    Series yValSeriesRef = new Series();
                    yValSeriesRef.ChartType = SeriesChartType.Point;
                    yValSeriesRef.IsVisibleInLegend = false;
                    yValSeriesRef.Name = reader.getClassifiers()[i] + " ref";
                    AnalysisChart.Series.Add(yValSeriesRef);
                    Series yValSeriesNet = new Series();
                    yValSeriesNet.ChartType = SeriesChartType.Line;
                    yValSeriesNet.IsVisibleInLegend = false;
                    yValSeriesNet.Name = reader.getClassifiers()[i] + " net";
                    AnalysisChart.Series.Add(yValSeriesNet);

                }
            }
            else
            {
                outputTextBox.AppendText("   " + "Range 0...1\r\n");
                refOutput.Add(0.0);
                Series yValSeriesRef = new Series();
                yValSeriesRef.ChartType = SeriesChartType.Point;
                yValSeriesRef.Name = "ref";
                AnalysisChart.Series.Add(yValSeriesRef);
                Series yValSeriesNet = new Series();
                yValSeriesNet.ChartType = SeriesChartType.Line;
                yValSeriesNet.Name = "net";
                AnalysisChart.Series.Add(yValSeriesNet);
            }
            outputTextBox.AppendText("Total Data number: " + reader.getNumTotalData() + "\r\n");
            outputTextBox.AppendText("----------------------------------------------\r\n");
            outputTextBox.AppendText("Define in following text box the inputs, outputs, layers and activation functions of the neural network.\r\n");
            outputTextBox.AppendText("Syntax: 1st (Input Layer), 2nd (First hidden Layer), 4th, 6th, ... line - number of inputs, 3rd, 5th, ... line - activation function\r\n");

            outputTextBox.AppendText("Selection of activation functions: " + string.Join(", ", Net.ActFctTypeStr) + "\r\n");
        }

        private void generateNeuralNet_Click(object sender, EventArgs e)
        {
            topology.Clear();
            actFct.Clear(); 
            String netParams = netPropertyTextBox.Text;
            String netParamsReplaced = netParams.Replace("\r\n", ",");
            String[] netParamsSplitted = netParamsReplaced.Split(',');
            int len = netParamsSplitted.Length;
            bool errorOccurred = false;
            
            for (int i = 0; i < len; i++)
            {
                bool found = false;
                int topologyVal = 0;
                bool canConvert = int.TryParse(netParamsSplitted[i], out topologyVal);
                if( canConvert == true )
                {
                    topologyVal = Convert.ToUInt16(netParamsSplitted[i]);
                    topology.Add(topologyVal);
                    outputTextBox.AppendText("Topology added with " + topologyVal + "\r\n");
                }
                else
                {
                    Net.ActFctType actFctLoc = ActFctType.Sigmoid;
                    String actFctString = netParamsSplitted[i];
                    if (actFctString != "")
                    {
                        for (int j = 0; j < Net.ActFctTypeStr.Length; j++)
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
                            actFct.Add(actFctLoc);
                            outputTextBox.AppendText("Activation Function added " + actFctString + "\r\n");
                        }
                        else
                        {
                            errorOccurred = true;
                            outputTextBox.AppendText("Error adding " + netParamsSplitted[i] + "\r\n");
                        }
                    }
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

            Double.TryParse(learningRateTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out learningRate);
            outputTextBox.AppendText("Learning rate set to: " + learningRate + "\r\n");

            epochMax = Convert.ToInt32(epochMaxTextBox.Text);
            outputTextBox.AppendText("Number of Epochs: " + epochMax + "\r\n");

            double limitData = Convert.ToDouble(limitTrainDataTextBox.Text, CultureInfo.InvariantCulture);
            if (limitData > reader.getNumTotalData())
            {
                totalTrainingData = reader.getNumTotalData();
            }
            else
            {
                totalTrainingData = (int)limitData;
            }
            outputTextBox.AppendText("Number of Training Data: " + totalTrainingData + "\r\n");

            //reader.LimitData(limitData);
            //outputTextBox.AppendText("Number of taining data limited randomly to: " + reader.getInputTrainData()[0].Count + "\r\n");
            
            batchSize = Convert.ToUInt16(batchSizeTextBox.Text);

            random = new Random(Guid.NewGuid().GetHashCode());

            trainingDataIndices.Clear();
            for (int i = 0; i < totalTrainingData; i++)
            {
                trainingDataIndices.Add(random.Next(totalTrainingData));
            }
            
            loopDataIdx = 0;
            epochIdx = 0;

            scrollBarWheelTurns = 0;
            lossChartMouseClick = false;
            AnalysisChart.Series[0].Points.Clear();
            AnalysisChart.Series[0].IsVisibleInLegend = true;
            for (int i = 1; i < AnalysisChart.Series.Count; i++)
            {
                AnalysisChart.Series[i].Points.Clear();
                AnalysisChart.Series[i].IsVisibleInLegend = false;
            }

            var chartArea = AnalysisChart.ChartAreas[AnalysisChart.Series[0].ChartArea];
            chartArea.AxisX.Maximum = AnalysisChart.Series[0].Points.Count;
            chartArea.AxisX.ScaleView.Scroll(AnalysisChart.Series[0].Points.Count);
            chartArea.AxisX.ScaleView.ZoomReset();
            chartArea.AxisY2.Maximum = Double.NaN;

            training = true;
            testing = false;
            batchCounter = 0;
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

            double limitData = Convert.ToDouble(limitTestDataTextBox.Text, CultureInfo.InvariantCulture);
            if (limitData > reader.getNumTotalData())
            {
                totalTestingData = reader.getNumTotalData();
            }
            else
            {
                totalTestingData = (int)limitData;
            }
            outputTextBox.AppendText("Number of Testing Data: " + totalTestingData + "\r\n");

            //reader.TestData(limitData);
            //outputTextBox.AppendText("Number of testing data limited randomly to: " + reader.getInputTestData()[0].Count + "\r\n");
            
            random = new Random(Guid.NewGuid().GetHashCode());

            loopDataIdx = 0;

            scrollBarWheelTurns = 0;
            lossChartMouseClick = false;
            AnalysisChart.Series[0].Points.Clear();
            AnalysisChart.Series[0].IsVisibleInLegend = false;
            for (int i = 1; i < AnalysisChart.Series.Count; i++)
            {
                AnalysisChart.Series[i].Points.Clear();
                AnalysisChart.Series[i].IsVisibleInLegend = true;
            }
            
            Random r = new Random();
            for (int i = 0; i < reader.getNumClassifiers(); i++)
            {
                AnalysisChart.Series[(2 * i) + 1].Color = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
                AnalysisChart.Series[(2 * i) + 2].Color = AnalysisChart.Series[(2 * i) + 1].Color;
            }

            var chartArea = AnalysisChart.ChartAreas[AnalysisChart.Series[0].ChartArea];
            chartArea.AxisX.Maximum = AnalysisChart.Series[0].Points.Count;
            chartArea.AxisX.ScaleView.Scroll(AnalysisChart.Series[0].Points.Count);
            chartArea.AxisX.ScaleView.ZoomReset();
            chartArea.AxisY.Maximum = Double.NaN;
            chartArea.AxisY.Minimum = -0.1;

            training = false;
            testing = true;
        }

        private void Timer_Loop(object sender, EventArgs e)
        {
            if (training == true)
            {
                int diagramUpdateCnt = 0;
                while (diagramUpdateCnt < 500)
                {
                    if (epochIdx < epochMax)
                    {
                        //int batch = reader.getInputTrainData()[0].Count;

                        while (loopDataIdx < batchSize)
                        {
                            int nextIdx = 0;
                            if (batchSize <= 1)
                            {
                                int randIdx = random.Next(trainingDataIndices.Count());
                                nextIdx = trainingDataIndices.ElementAt(randIdx);
                            }
                            else
                            {
                                nextIdx = trainingDataIndices.ElementAt(batchCounter);
                                batchCounter++;
                                if (batchCounter >= trainingDataIndices.Count())
                                {
                                    batchCounter = 0;
                                }
                            }

                            row.Clear();
                            for (int j = 0; j < inputRowCnt; j++)
                            {
                                row.Add(reader.getInputData()[j][nextIdx]);
                            }
                            List<double> refScaled = network.scaleInput(row);
                            network.feedForward(refScaled);

                            if (reader.getNumClassifiers() > 1)
                            {
                                for (int i = 0; i < refOutput.Count; i++)
                                {
                                    if (i == (int)reader.getOutputData(nextIdx))
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
                                refOutput[0] = reader.getOutputData(nextIdx);
                            }

                            if (batchSize <= 1)
                            {
                                network.backProp(refOutput, learningRate);
                            }
                            else
                            {
                                network.batchGradientDescent(refOutput);
                            }

                            double losses = network.loss(refOutput);
                            AnalysisChart.Series[0].Points.Add(losses);

                            var chartArea = AnalysisChart.ChartAreas[AnalysisChart.Series[0].ChartArea];

                            if (lossChartMouseClick == false)
                            {
                                chartArea.AxisX.Maximum = AnalysisChart.Series[0].Points.Count;
                                chartArea.AxisX.ScaleView.Scroll(AnalysisChart.Series[0].Points.Count);
                            }

                            loopDataIdx++;
                            diagramUpdateCnt++;
                            epochIdx++;
                        }

                        if (batchSize > 1)
                        {
                            // refOutput not used internally
                            //network.batchGradientAverage(refOutput);
                            network.updateWeights(learningRate);
                        }
                                                
                        loopDataIdx = 0;
                    }
                    else
                    {
                        training = false;
                        outputTextBox.AppendText("Training finished\r\n");
                        diagramUpdateCnt = 500;
                    }
                }
            }
            else
            if (testing == true)
            {
                if (loopDataIdx < totalTestingData)
                {
                    int randIdx = random.Next(reader.getNumTotalData());

                    row.Clear();
                    for (int j = 0; j < inputRowCnt; j++)
                    {
                        row.Add(reader.getInputData()[j][randIdx]);
                    }
                    List<double> refScaled = network.scaleInput(row);
                    network.feedForward(refScaled);

                    if (reader.getNumClassifiers() > 1)
                    {
                        for (int i = 0; i < refOutput.Count; i++)
                        {
                            if (i == (int)reader.getOutputData(randIdx))
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
                        refOutput[0] = reader.getOutputData(randIdx);
                    }

                    
                    List<double> netOutput = network.getOutput();
                    netOutput = netOutput.Select(q => Math.Round(q, 3)).ToList();
                    for (int i = 0; i < netOutput.Count; i++)
                    {
                        AnalysisChart.Series[(2*i)+1].Points.Add(refOutput[i]);
                        AnalysisChart.Series[(2*i)+2].Points.Add(netOutput[i]);
                    }
                    
                    var chartArea = AnalysisChart.ChartAreas[AnalysisChart.Series[1].ChartArea];

                    if (lossChartMouseClick == false)
                    {
                        chartArea.AxisX.Maximum = AnalysisChart.Series[1].Points.Count;
                        chartArea.AxisX.ScaleView.Scroll(AnalysisChart.Series[1].Points.Count);
                    }

                    double loss = Math.Round(network.loss(refOutput),3);

                    String[] rowStrA = Array.ConvertAll(row.ToArray(), x => x.ToString());
                    String rowStr = String.Join(", ", rowStrA);
                    String[] netStrA = Array.ConvertAll(netOutput.ToArray(), x => x.ToString());
                    String netStr = String.Join(", ", netStrA);
                    String[] refStrA = Array.ConvertAll(refOutput.ToArray(), x => x.ToString());
                    String refStr = String.Join(", ", refStrA);
                    outputTextBox.AppendText((randIdx+2) + ": " + rowStr + " -> " + "is: " + netStr + " - shall: " + refStr + " - loss: " + loss + "\r\n");

                    loopDataIdx++;
                }
                else
                {
                    testing = false;
                    outputTextBox.AppendText("Testing finished\r\n");
                }
            }
        }

        private void stopTesting_Click(object sender, EventArgs e)
        {
            testing = false;
            outputTextBox.AppendText("Testing aborted by user\r\n");
        }

        private void methodGradientDescent_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void stopTraining_Click(object sender, EventArgs e)
        {
            training = false;
            outputTextBox.AppendText("Training aborted by user\r\n");
        }

        private void LossChart_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY2;

            if (e.Delta >= 0)
            {
                scrollBarWheelTurns += 1;
            }
            else
            {
                scrollBarWheelTurns -= 1;
            }

            if (scrollBarWheelTurns <= 0)
            {
                xAxis.ScaleView.ZoomReset();
                yAxis.Maximum = Double.NaN;
                scrollBarWheelTurns = 0;
            }
            else
            if (scrollBarWheelTurns > 0)
            {
                if (scrollBarWheelTurns >= 10)
                {
                    scrollBarWheelTurns = 10;
                }

                int seriesIndex = 0;
                if (AnalysisChart.Series[1].Points.Count > AnalysisChart.Series[0].Points.Count)
                {
                    seriesIndex = 1;
                }

                if (AnalysisChart.Series[seriesIndex].Points.Count > 0)
                {
                    int xMinZoomPos = (int)((((double)scrollBarWheelTurns) / 10.0) * ((double)AnalysisChart.Series[seriesIndex].Points.Count));

                    if (xMinZoomPos >= AnalysisChart.Series[seriesIndex].Points.Count)
                    {
                        xMinZoomPos = AnalysisChart.Series[seriesIndex].Points.Count;
                    }
                    xAxis.ScaleView.Zoom(xMinZoomPos, AnalysisChart.Series[seriesIndex].Points.Count);

                    if (seriesIndex == 0)
                    {
                        double maxVal = 0.0;
                        for (int i = xMinZoomPos; i < AnalysisChart.Series[seriesIndex].Points.Count; i++)
                        {
                            if (AnalysisChart.Series[seriesIndex].Points.ElementAt(i).YValues[0] > maxVal)
                            {
                                maxVal = AnalysisChart.Series[seriesIndex].Points.ElementAt(i).YValues[0];
                            }
                        }
                        yAxis.Maximum = maxVal;
                    }
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
    }
}

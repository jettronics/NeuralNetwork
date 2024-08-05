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

namespace Windows
{
    public partial class NeuralNetwork : Form
    {
        protected Net network;
        protected List<int> topology;
        protected List<ActFctType> actFct;
        protected Reader reader;

        public NeuralNetwork()
        {
            InitializeComponent();
            topology = new List<int>();
            actFct = new List<ActFctType>();
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

            For classification problems using more than 1 neuron a softmax activation function should be preferred
            */
            topology.Add(10);
            actFct.Add(Net.ActFctType.Logistic);
            topology.Add(11);
            actFct.Add(Net.ActFctType.Logistic);
            topology.Add(7);
            actFct.Add(Net.ActFctType.Logistic);
            topology.Add(4);
            actFct.Add(Net.ActFctType.Logistic); 
            //actFct.Add(Net.ActFctType.SoftMax);
            /* 
            Softmax activation function explained:
            https://www.pinecone.io/learn/softmax-activation/
            */

            String weightsFile = Path.GetFullPath("weights.txt");

            network = new Net(topology, weightsFile, actFct);

            
        }

        private void LossChart_Click(object sender, EventArgs e)
        {
            
        }

        private void LossChart_Load(object sender, EventArgs e)
        {
            
        }

        private void Timer_Loop(object sender, EventArgs e)
        {
            LossChart.Series["Loss"].Points.Add(10.0);
        }

        private void openTrainingDataFile_Click(object sender, EventArgs e)
        {
            openTrainingDataCSVFileDialog.ShowDialog(this);
        }

        private void csvTrainingDataFileOpened(object sender, CancelEventArgs e)
        {
            outputTextBox.AppendText("Training data opened\r\n");
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
        }
    }
}

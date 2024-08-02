using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows
{
    public partial class NeuralNetwork : Form
    {
        public NeuralNetwork()
        {
            InitializeComponent();
        }

        private void LossChart_Click(object sender, EventArgs e)
        {
            
        }

        private void LossChart_Load(object sender, EventArgs e)
        {
            
        }

        public void LossChar_AddData( double loss )
        {
            LossChart.Series["Loss"].Points.Add(loss);
        }
    }
}

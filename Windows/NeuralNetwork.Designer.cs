namespace Windows
{
    partial class NeuralNetwork
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea12 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend12 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title12 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.LossChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openTrainingDataCSVFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.netPropertyTextBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.LossChart)).BeginInit();
            this.SuspendLayout();
            // 
            // LossChart
            // 
            chartArea12.AxisX.MajorGrid.Enabled = false;
            chartArea12.AxisX2.MajorGrid.Enabled = false;
            chartArea12.AxisY.MajorGrid.Enabled = false;
            chartArea12.AxisY2.MajorGrid.Enabled = false;
            chartArea12.Name = "ChartArea1";
            this.LossChart.ChartAreas.Add(chartArea12);
            legend12.Name = "Legend1";
            this.LossChart.Legends.Add(legend12);
            this.LossChart.Location = new System.Drawing.Point(41, 31);
            this.LossChart.Name = "LossChart";
            series12.ChartArea = "ChartArea1";
            series12.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series12.IsVisibleInLegend = false;
            series12.Legend = "Legend1";
            series12.Name = "Loss";
            series12.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.LossChart.Series.Add(series12);
            this.LossChart.Size = new System.Drawing.Size(300, 300);
            this.LossChart.TabIndex = 0;
            this.LossChart.Text = "Loss";
            title12.Name = "Loss";
            title12.Text = "Loss";
            this.LossChart.Titles.Add(title12);
            this.LossChart.Click += new System.EventHandler(this.LossChart_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer_Loop);
            // 
            // openTrainingDataCSVFileDialog
            // 
            this.openTrainingDataCSVFileDialog.Filter = "|*.csv";
            this.openTrainingDataCSVFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.csvTrainingDataFileOpened);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(528, 75);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(175, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Open Training Data CSV File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.openTrainingDataFile_Click);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(410, 104);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(293, 156);
            this.outputTextBox.TabIndex = 2;
            // 
            // netPropertyTextBox
            // 
            this.netPropertyTextBox.Location = new System.Drawing.Point(410, 266);
            this.netPropertyTextBox.Multiline = true;
            this.netPropertyTextBox.Name = "netPropertyTextBox";
            this.netPropertyTextBox.Size = new System.Drawing.Size(160, 104);
            this.netPropertyTextBox.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(576, 347);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(127, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Generate Neural Net";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.generateNeuralNet_Click);
            // 
            // NeuralNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 509);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.netPropertyTextBox);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LossChart);
            this.Name = "NeuralNetwork";
            this.Text = "Neural Network";
            this.Load += new System.EventHandler(this.LossChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LossChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart LossChart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.OpenFileDialog openTrainingDataCSVFileDialog;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.TextBox netPropertyTextBox;
        private System.Windows.Forms.Button button2;
    }
}


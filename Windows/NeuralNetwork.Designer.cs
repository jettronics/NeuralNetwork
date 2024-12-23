﻿namespace Windows
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.AnalysisChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openTrainingDataCSVFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.netPropertyTextBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.learningRateTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.epochMaxTextBox = new System.Windows.Forms.TextBox();
            this.Epochs = new System.Windows.Forms.Label();
            this.limitTrainDataTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.batchSizeTextBox = new System.Windows.Forms.TextBox();
            this.limitTestDataTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.weightsLimitTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.AnalysisChart)).BeginInit();
            this.SuspendLayout();
            // 
            // AnalysisChart
            // 
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX2.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY2.MajorGrid.Enabled = false;
            chartArea1.Name = "ChartArea1";
            this.AnalysisChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.AnalysisChart.Legends.Add(legend1);
            this.AnalysisChart.Location = new System.Drawing.Point(321, 40);
            this.AnalysisChart.Name = "AnalysisChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.IsVisibleInLegend = false;
            series1.Legend = "Legend1";
            series1.Name = "Loss";
            series1.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.AnalysisChart.Series.Add(series1);
            this.AnalysisChart.Size = new System.Drawing.Size(545, 458);
            this.AnalysisChart.TabIndex = 0;
            this.AnalysisChart.Text = "Analysis";
            title1.Name = "Analysis";
            title1.Text = "Analysis";
            this.AnalysisChart.Titles.Add(title1);
            this.AnalysisChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LossChart_MouseClick);
            this.AnalysisChart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LossChart_MouseDown);
            this.AnalysisChart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LossChart_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.Timer_Loop);
            // 
            // openTrainingDataCSVFileDialog
            // 
            this.openTrainingDataCSVFileDialog.Filter = "|*.csv";
            this.openTrainingDataCSVFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.csvTrainingDataFileOpened);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(293, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Analyze Training Data CSV File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.openTrainingDataFile_Click);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(12, 504);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(854, 132);
            this.outputTextBox.TabIndex = 2;
            this.outputTextBox.TextChanged += new System.EventHandler(this.outputTextBox_TextChanged);
            // 
            // netPropertyTextBox
            // 
            this.netPropertyTextBox.Location = new System.Drawing.Point(12, 72);
            this.netPropertyTextBox.Multiline = true;
            this.netPropertyTextBox.Name = "netPropertyTextBox";
            this.netPropertyTextBox.Size = new System.Drawing.Size(160, 104);
            this.netPropertyTextBox.TabIndex = 3;
            this.netPropertyTextBox.TextChanged += new System.EventHandler(this.netPropertyTextBox_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(178, 72);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(127, 104);
            this.button2.TabIndex = 4;
            this.button2.Text = "Generate Neural Net";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.generateNeuralNet_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 417);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(293, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Test Neural Net";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.testNeuralNet_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 475);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(293, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Save Neural Net";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.saveNeuralNet_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(12, 359);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(293, 23);
            this.button6.TabIndex = 8;
            this.button6.Text = "Train Neural Net";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.trainNeuralNet_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(12, 388);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(293, 23);
            this.button7.TabIndex = 9;
            this.button7.Text = "Stop Training";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.stopTraining_Click);
            // 
            // learningRateTextBox
            // 
            this.learningRateTextBox.Location = new System.Drawing.Point(178, 182);
            this.learningRateTextBox.Name = "learningRateTextBox";
            this.learningRateTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.learningRateTextBox.Size = new System.Drawing.Size(127, 20);
            this.learningRateTextBox.TabIndex = 10;
            this.learningRateTextBox.Text = "0.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(100, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Learning rate:";
            // 
            // epochMaxTextBox
            // 
            this.epochMaxTextBox.Location = new System.Drawing.Point(178, 209);
            this.epochMaxTextBox.Name = "epochMaxTextBox";
            this.epochMaxTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.epochMaxTextBox.Size = new System.Drawing.Size(127, 20);
            this.epochMaxTextBox.TabIndex = 12;
            this.epochMaxTextBox.Text = "100000";
            // 
            // Epochs
            // 
            this.Epochs.AutoSize = true;
            this.Epochs.Location = new System.Drawing.Point(74, 209);
            this.Epochs.Name = "Epochs";
            this.Epochs.Size = new System.Drawing.Size(98, 13);
            this.Epochs.TabIndex = 13;
            this.Epochs.Text = "Number of Epochs:";
            // 
            // limitTrainDataTextBox
            // 
            this.limitTrainDataTextBox.Location = new System.Drawing.Point(178, 235);
            this.limitTrainDataTextBox.Name = "limitTrainDataTextBox";
            this.limitTrainDataTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.limitTrainDataTextBox.Size = new System.Drawing.Size(127, 20);
            this.limitTrainDataTextBox.TabIndex = 15;
            this.limitTrainDataTextBox.Text = "100";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 235);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Number of Training Data:";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(12, 446);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(293, 23);
            this.button5.TabIndex = 17;
            this.button5.Text = "Stop Testing";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.stopTesting_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(113, 287);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Batch size:";
            // 
            // batchSizeTextBox
            // 
            this.batchSizeTextBox.Location = new System.Drawing.Point(178, 287);
            this.batchSizeTextBox.Name = "batchSizeTextBox";
            this.batchSizeTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.batchSizeTextBox.Size = new System.Drawing.Size(127, 20);
            this.batchSizeTextBox.TabIndex = 20;
            this.batchSizeTextBox.Text = "1";
            // 
            // limitTestDataTextBox
            // 
            this.limitTestDataTextBox.Location = new System.Drawing.Point(178, 261);
            this.limitTestDataTextBox.Name = "limitTestDataTextBox";
            this.limitTestDataTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.limitTestDataTextBox.Size = new System.Drawing.Size(127, 20);
            this.limitTestDataTextBox.TabIndex = 21;
            this.limitTestDataTextBox.Text = "10";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(49, 261);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Number of Testing Data:";
            // 
            // weightsLimitTextBox
            // 
            this.weightsLimitTextBox.Location = new System.Drawing.Point(178, 313);
            this.weightsLimitTextBox.Name = "weightsLimitTextBox";
            this.weightsLimitTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.weightsLimitTextBox.Size = new System.Drawing.Size(127, 20);
            this.weightsLimitTextBox.TabIndex = 23;
            this.weightsLimitTextBox.Text = "4.0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(103, 313);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Weights limit:";
            // 
            // NeuralNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 648);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.weightsLimitTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.limitTestDataTextBox);
            this.Controls.Add(this.batchSizeTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.limitTrainDataTextBox);
            this.Controls.Add(this.Epochs);
            this.Controls.Add(this.epochMaxTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.learningRateTextBox);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.AnalysisChart);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.netPropertyTextBox);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.button1);
            this.Name = "NeuralNetwork";
            this.Text = "Neural Network";
            this.Load += new System.EventHandler(this.LossChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AnalysisChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart AnalysisChart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.OpenFileDialog openTrainingDataCSVFileDialog;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.TextBox netPropertyTextBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox learningRateTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox epochMaxTextBox;
        private System.Windows.Forms.Label Epochs;
        private System.Windows.Forms.TextBox limitTrainDataTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox batchSizeTextBox;
        private System.Windows.Forms.TextBox limitTestDataTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox weightsLimitTextBox;
        private System.Windows.Forms.Label label5;
    }
}


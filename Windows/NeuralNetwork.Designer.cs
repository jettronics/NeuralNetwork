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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.LossChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.openCSVFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.LossChart)).BeginInit();
            this.SuspendLayout();
            // 
            // LossChart
            // 
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX2.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY2.MajorGrid.Enabled = false;
            chartArea1.Name = "ChartArea1";
            this.LossChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.LossChart.Legends.Add(legend1);
            this.LossChart.Location = new System.Drawing.Point(41, 31);
            this.LossChart.Name = "LossChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.IsVisibleInLegend = false;
            series1.Legend = "Legend1";
            series1.Name = "Loss";
            series1.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.LossChart.Series.Add(series1);
            this.LossChart.Size = new System.Drawing.Size(300, 300);
            this.LossChart.TabIndex = 0;
            this.LossChart.Text = "Loss";
            title1.Name = "Loss";
            title1.Text = "Loss";
            this.LossChart.Titles.Add(title1);
            this.LossChart.Click += new System.EventHandler(this.LossChart_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer_Loop);
            // 
            // openCSVFileDialog
            // 
            this.openCSVFileDialog.FileName = "openCSVFileDialog";
            this.openCSVFileDialog.Filter = "|*.csv";
            this.openCSVFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.csvFileOpened);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(507, 131);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Open Input CSV File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.openFile_Click);
            // 
            // NeuralNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LossChart);
            this.Name = "NeuralNetwork";
            this.Text = "Neural Network";
            this.Load += new System.EventHandler(this.LossChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LossChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart LossChart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.OpenFileDialog openCSVFileDialog;
        private System.Windows.Forms.Button button1;
    }
}


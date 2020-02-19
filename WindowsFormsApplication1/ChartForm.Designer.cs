namespace WindowsFormsApplication1
{
    partial class ChartForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_SetGateFolder = new System.Windows.Forms.Button();
            this.checkBox_FinalGate = new System.Windows.Forms.CheckBox();
            this.checkBoxGate1 = new System.Windows.Forms.CheckBox();
            this.checkBoxGate3 = new System.Windows.Forms.CheckBox();
            this.checkBoxGate2 = new System.Windows.Forms.CheckBox();
            this.radioButton_BASO = new System.Windows.Forms.RadioButton();
            this.radioButton_EOS = new System.Windows.Forms.RadioButton();
            this.radioButton_3diff = new System.Windows.Forms.RadioButton();
            this.btnGating = new System.Windows.Forms.Button();
            this.btnSeparation = new System.Windows.Forms.Button();
            this.loadFile = new System.Windows.Forms.Button();
            this.RefeshButton = new System.Windows.Forms.Button();
            this.btnLoadClusters = new System.Windows.Forms.Button();
            this.btnPlotOnly = new System.Windows.Forms.Button();
            this.btnLoadPolygons = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox_PrefilterData = new System.Windows.Forms.CheckBox();
            this.DataFile = new System.Windows.Forms.OpenFileDialog();
            this.button_ProcessList = new System.Windows.Forms.Button();
            this.FileNameBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_FixedGating = new System.Windows.Forms.CheckBox();
            this.checkBox_DynamicGating = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chartData = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnPlotKde = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_SetGateFolder);
            this.groupBox1.Controls.Add(this.checkBox_FinalGate);
            this.groupBox1.Controls.Add(this.checkBoxGate1);
            this.groupBox1.Controls.Add(this.checkBoxGate3);
            this.groupBox1.Controls.Add(this.checkBoxGate2);
            this.groupBox1.Controls.Add(this.radioButton_BASO);
            this.groupBox1.Controls.Add(this.radioButton_EOS);
            this.groupBox1.Controls.Add(this.radioButton_3diff);
            this.groupBox1.Controls.Add(this.btnGating);
            this.groupBox1.Location = new System.Drawing.Point(727, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(220, 153);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Process WBC";
            // 
            // button_SetGateFolder
            // 
            this.button_SetGateFolder.Location = new System.Drawing.Point(34, 16);
            this.button_SetGateFolder.Margin = new System.Windows.Forms.Padding(2);
            this.button_SetGateFolder.Name = "button_SetGateFolder";
            this.button_SetGateFolder.Size = new System.Drawing.Size(155, 22);
            this.button_SetGateFolder.TabIndex = 30;
            this.button_SetGateFolder.Text = "Set Fixed Gating Folder";
            this.button_SetGateFolder.UseVisualStyleBackColor = true;
            this.button_SetGateFolder.Click += new System.EventHandler(this.button_SetGateFolder_Click);
            // 
            // checkBox_FinalGate
            // 
            this.checkBox_FinalGate.AutoSize = true;
            this.checkBox_FinalGate.Checked = true;
            this.checkBox_FinalGate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_FinalGate.Location = new System.Drawing.Point(132, 126);
            this.checkBox_FinalGate.Margin = new System.Windows.Forms.Padding(1);
            this.checkBox_FinalGate.Name = "checkBox_FinalGate";
            this.checkBox_FinalGate.Size = new System.Drawing.Size(48, 17);
            this.checkBox_FinalGate.TabIndex = 20;
            this.checkBox_FinalGate.Text = "Final";
            this.checkBox_FinalGate.UseVisualStyleBackColor = true;
            // 
            // checkBoxGate1
            // 
            this.checkBoxGate1.AutoSize = true;
            this.checkBoxGate1.Checked = true;
            this.checkBoxGate1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGate1.Location = new System.Drawing.Point(131, 55);
            this.checkBoxGate1.Margin = new System.Windows.Forms.Padding(1);
            this.checkBoxGate1.Name = "checkBoxGate1";
            this.checkBoxGate1.Size = new System.Drawing.Size(55, 17);
            this.checkBoxGate1.TabIndex = 18;
            this.checkBoxGate1.Text = "Gate1";
            this.checkBoxGate1.UseVisualStyleBackColor = true;
            this.checkBoxGate1.CheckedChanged += new System.EventHandler(this.checkBoxGate1_CheckedChanged);
            // 
            // checkBoxGate3
            // 
            this.checkBoxGate3.AutoSize = true;
            this.checkBoxGate3.Location = new System.Drawing.Point(131, 102);
            this.checkBoxGate3.Margin = new System.Windows.Forms.Padding(1);
            this.checkBoxGate3.Name = "checkBoxGate3";
            this.checkBoxGate3.Size = new System.Drawing.Size(55, 17);
            this.checkBoxGate3.TabIndex = 17;
            this.checkBoxGate3.Text = "Gate3";
            this.checkBoxGate3.UseVisualStyleBackColor = true;
            // 
            // checkBoxGate2
            // 
            this.checkBoxGate2.AutoSize = true;
            this.checkBoxGate2.Location = new System.Drawing.Point(131, 79);
            this.checkBoxGate2.Margin = new System.Windows.Forms.Padding(1);
            this.checkBoxGate2.Name = "checkBoxGate2";
            this.checkBoxGate2.Size = new System.Drawing.Size(55, 17);
            this.checkBoxGate2.TabIndex = 16;
            this.checkBoxGate2.Text = "Gate2";
            this.checkBoxGate2.UseVisualStyleBackColor = true;
            // 
            // radioButton_BASO
            // 
            this.radioButton_BASO.AutoSize = true;
            this.radioButton_BASO.Location = new System.Drawing.Point(37, 91);
            this.radioButton_BASO.Name = "radioButton_BASO";
            this.radioButton_BASO.Size = new System.Drawing.Size(54, 17);
            this.radioButton_BASO.TabIndex = 15;
            this.radioButton_BASO.Text = "BASO";
            this.radioButton_BASO.UseVisualStyleBackColor = true;
            this.radioButton_BASO.CheckedChanged += new System.EventHandler(this.radioButton_BASO_CheckedChanged);
            // 
            // radioButton_EOS
            // 
            this.radioButton_EOS.AutoSize = true;
            this.radioButton_EOS.Location = new System.Drawing.Point(37, 67);
            this.radioButton_EOS.Name = "radioButton_EOS";
            this.radioButton_EOS.Size = new System.Drawing.Size(47, 17);
            this.radioButton_EOS.TabIndex = 14;
            this.radioButton_EOS.Text = "EOS";
            this.radioButton_EOS.UseVisualStyleBackColor = true;
            this.radioButton_EOS.CheckedChanged += new System.EventHandler(this.radioButton_EOS_CheckedChanged);
            // 
            // radioButton_3diff
            // 
            this.radioButton_3diff.AutoSize = true;
            this.radioButton_3diff.Checked = true;
            this.radioButton_3diff.Location = new System.Drawing.Point(37, 44);
            this.radioButton_3diff.Name = "radioButton_3diff";
            this.radioButton_3diff.Size = new System.Drawing.Size(48, 17);
            this.radioButton_3diff.TabIndex = 13;
            this.radioButton_3diff.TabStop = true;
            this.radioButton_3diff.Text = "3-diff";
            this.radioButton_3diff.UseVisualStyleBackColor = true;
            this.radioButton_3diff.CheckedChanged += new System.EventHandler(this.radioButton_3diff_CheckedChanged);
            // 
            // btnGating
            // 
            this.btnGating.Location = new System.Drawing.Point(35, 118);
            this.btnGating.Margin = new System.Windows.Forms.Padding(1);
            this.btnGating.Name = "btnGating";
            this.btnGating.Size = new System.Drawing.Size(63, 28);
            this.btnGating.TabIndex = 6;
            this.btnGating.Text = "Gate File";
            this.btnGating.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnGating.UseVisualStyleBackColor = true;
            this.btnGating.Click += new System.EventHandler(this.btnGating_Click);
            // 
            // btnSeparation
            // 
            this.btnSeparation.Location = new System.Drawing.Point(557, 55);
            this.btnSeparation.Margin = new System.Windows.Forms.Padding(1);
            this.btnSeparation.Name = "btnSeparation";
            this.btnSeparation.Size = new System.Drawing.Size(87, 30);
            this.btnSeparation.TabIndex = 12;
            this.btnSeparation.Text = "Load + Cluster";
            this.btnSeparation.UseVisualStyleBackColor = true;
            this.btnSeparation.Click += new System.EventHandler(this.btnSeparation_Click);
            // 
            // loadFile
            // 
            this.loadFile.Location = new System.Drawing.Point(183, 26);
            this.loadFile.Margin = new System.Windows.Forms.Padding(2);
            this.loadFile.Name = "loadFile";
            this.loadFile.Size = new System.Drawing.Size(81, 27);
            this.loadFile.TabIndex = 10;
            this.loadFile.Text = "Load File";
            this.loadFile.UseVisualStyleBackColor = true;
            this.loadFile.Click += new System.EventHandler(this.Button1_Click_1);
            // 
            // RefeshButton
            // 
            this.RefeshButton.Location = new System.Drawing.Point(395, 71);
            this.RefeshButton.Margin = new System.Windows.Forms.Padding(2);
            this.RefeshButton.Name = "RefeshButton";
            this.RefeshButton.Size = new System.Drawing.Size(81, 27);
            this.RefeshButton.TabIndex = 3;
            this.RefeshButton.Text = "Refresh";
            this.RefeshButton.UseVisualStyleBackColor = true;
            this.RefeshButton.Click += new System.EventHandler(this.RebuildButton_Click);
            // 
            // btnLoadClusters
            // 
            this.btnLoadClusters.Location = new System.Drawing.Point(395, 26);
            this.btnLoadClusters.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadClusters.Name = "btnLoadClusters";
            this.btnLoadClusters.Size = new System.Drawing.Size(81, 27);
            this.btnLoadClusters.TabIndex = 5;
            this.btnLoadClusters.Text = "Load Clusters";
            this.btnLoadClusters.UseVisualStyleBackColor = true;
            this.btnLoadClusters.Click += new System.EventHandler(this.btnLoadClusters_Click);
            // 
            // btnPlotOnly
            // 
            this.btnPlotOnly.Location = new System.Drawing.Point(290, 71);
            this.btnPlotOnly.Margin = new System.Windows.Forms.Padding(1);
            this.btnPlotOnly.Name = "btnPlotOnly";
            this.btnPlotOnly.Size = new System.Drawing.Size(81, 27);
            this.btnPlotOnly.TabIndex = 0;
            this.btnPlotOnly.Text = "Plot only";
            this.btnPlotOnly.UseVisualStyleBackColor = true;
            this.btnPlotOnly.Click += new System.EventHandler(this.BtnPlotOnly_Click);
            // 
            // btnLoadPolygons
            // 
            this.btnLoadPolygons.Location = new System.Drawing.Point(290, 26);
            this.btnLoadPolygons.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadPolygons.Name = "btnLoadPolygons";
            this.btnLoadPolygons.Size = new System.Drawing.Size(81, 27);
            this.btnLoadPolygons.TabIndex = 4;
            this.btnLoadPolygons.Text = "Load Gate";
            this.btnLoadPolygons.UseVisualStyleBackColor = true;
            this.btnLoadPolygons.Click += new System.EventHandler(this.btnLoadPolygons_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(5, 22);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(107, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(5, 52);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(107, 21);
            this.comboBox2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(119, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "X-axis";
            this.label1.Click += new System.EventHandler(this.Label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(119, 60);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Y-axis";
            // 
            // checkBox_PrefilterData
            // 
            this.checkBox_PrefilterData.AutoSize = true;
            this.checkBox_PrefilterData.Checked = true;
            this.checkBox_PrefilterData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_PrefilterData.Location = new System.Drawing.Point(58, 82);
            this.checkBox_PrefilterData.Name = "checkBox_PrefilterData";
            this.checkBox_PrefilterData.Size = new System.Drawing.Size(60, 17);
            this.checkBox_PrefilterData.TabIndex = 11;
            this.checkBox_PrefilterData.Text = "prefilter";
            this.checkBox_PrefilterData.UseVisualStyleBackColor = true;
            this.checkBox_PrefilterData.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // DataFile
            // 
            this.DataFile.FileName = "openFileDialog1";
            // 
            // button_ProcessList
            // 
            this.button_ProcessList.Location = new System.Drawing.Point(557, 12);
            this.button_ProcessList.Margin = new System.Windows.Forms.Padding(2);
            this.button_ProcessList.Name = "button_ProcessList";
            this.button_ProcessList.Size = new System.Drawing.Size(86, 31);
            this.button_ProcessList.TabIndex = 17;
            this.button_ProcessList.Text = "Process List";
            this.button_ProcessList.UseVisualStyleBackColor = true;
            // 
            // FileNameBox
            // 
            this.FileNameBox.Location = new System.Drawing.Point(14, 145);
            this.FileNameBox.Margin = new System.Windows.Forms.Padding(2);
            this.FileNameBox.Name = "FileNameBox";
            this.FileNameBox.Size = new System.Drawing.Size(681, 20);
            this.FileNameBox.TabIndex = 21;
            this.FileNameBox.TextChanged += new System.EventHandler(this.FileNameBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(313, 125);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 15);
            this.label4.TabIndex = 24;
            this.label4.Text = "Loaded File Name";
            // 
            // checkBox_FixedGating
            // 
            this.checkBox_FixedGating.AutoSize = true;
            this.checkBox_FixedGating.Checked = true;
            this.checkBox_FixedGating.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_FixedGating.Location = new System.Drawing.Point(576, 113);
            this.checkBox_FixedGating.Name = "checkBox_FixedGating";
            this.checkBox_FixedGating.Size = new System.Drawing.Size(51, 17);
            this.checkBox_FixedGating.TabIndex = 26;
            this.checkBox_FixedGating.Text = "Fixed";
            this.checkBox_FixedGating.UseVisualStyleBackColor = true;
            this.checkBox_FixedGating.CheckedChanged += new System.EventHandler(this.checkBox_FixedGating_CheckedChanged);
            // 
            // checkBox_DynamicGating
            // 
            this.checkBox_DynamicGating.AutoSize = true;
            this.checkBox_DynamicGating.Location = new System.Drawing.Point(576, 91);
            this.checkBox_DynamicGating.Name = "checkBox_DynamicGating";
            this.checkBox_DynamicGating.Size = new System.Drawing.Size(67, 17);
            this.checkBox_DynamicGating.TabIndex = 27;
            this.checkBox_DynamicGating.Text = "Dynamic";
            this.checkBox_DynamicGating.UseVisualStyleBackColor = true;
            this.checkBox_DynamicGating.CheckedChanged += new System.EventHandler(this.checkBox_DynamicGating_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnPlotKde);
            this.groupBox2.Controls.Add(this.checkBox_PrefilterData);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.loadFile);
            this.groupBox2.Controls.Add(this.RefeshButton);
            this.groupBox2.Controls.Add(this.btnLoadClusters);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnPlotOnly);
            this.groupBox2.Controls.Add(this.comboBox2);
            this.groupBox2.Controls.Add(this.btnLoadPolygons);
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Location = new System.Drawing.Point(14, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(497, 109);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Explore FCS Data";
            // 
            // chartData
            // 
            this.chartData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartData.BackImageAlignment = System.Windows.Forms.DataVisualization.Charting.ChartImageAlignmentStyle.Center;
            chartArea2.AxisX.Title = "X";
            chartArea2.AxisY.Title = "Y";
            chartArea2.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea2.Name = "ChartArea1";
            this.chartData.ChartAreas.Add(chartArea2);
            legend2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend2.IsTextAutoFit = false;
            legend2.Name = "Legend1";
            this.chartData.Legends.Add(legend2);
            this.chartData.Location = new System.Drawing.Point(12, 186);
            this.chartData.Name = "chartData";
            this.chartData.Size = new System.Drawing.Size(1324, 610);
            this.chartData.TabIndex = 0;
            this.chartData.Text = "chart2";
            // 
            // btnPlotKde
            // 
            this.btnPlotKde.Location = new System.Drawing.Point(183, 71);
            this.btnPlotKde.Margin = new System.Windows.Forms.Padding(1);
            this.btnPlotKde.Name = "btnPlotKde";
            this.btnPlotKde.Size = new System.Drawing.Size(81, 27);
            this.btnPlotKde.TabIndex = 12;
            this.btnPlotKde.Text = "Plot KDE";
            this.btnPlotKde.UseVisualStyleBackColor = true;
            this.btnPlotKde.Click += new System.EventHandler(this.btnPlotKde_Click);
            // 
            // ChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1348, 808);
            this.Controls.Add(this.chartData);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBox_DynamicGating);
            this.Controls.Add(this.checkBox_FixedGating);
            this.Controls.Add(this.button_ProcessList);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FileNameBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSeparation);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ChartForm";
            this.Text = "Processing FCS files";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ChartForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button RefeshButton;
        private System.Windows.Forms.Button btnLoadPolygons;
        private System.Windows.Forms.Button btnLoadClusters;
        private System.Windows.Forms.Button btnGating;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPlotOnly;
        private System.Windows.Forms.Button loadFile;
        private System.Windows.Forms.CheckBox checkBox_PrefilterData;
        private System.Windows.Forms.Button btnSeparation;
        private System.Windows.Forms.OpenFileDialog DataFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_3diff;
        private System.Windows.Forms.RadioButton radioButton_BASO;
        private System.Windows.Forms.RadioButton radioButton_EOS;
        private System.Windows.Forms.Button button_ProcessList;
        private System.Windows.Forms.TextBox FileNameBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBoxGate3;
        private System.Windows.Forms.CheckBox checkBoxGate2;
        private System.Windows.Forms.CheckBox checkBoxGate1;
        private System.Windows.Forms.CheckBox checkBox_FixedGating;
        private System.Windows.Forms.CheckBox checkBox_DynamicGating;
        private System.Windows.Forms.CheckBox checkBox_FinalGate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_SetGateFolder;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartData;
        private System.Windows.Forms.Button btnPlotKde;
    }
}


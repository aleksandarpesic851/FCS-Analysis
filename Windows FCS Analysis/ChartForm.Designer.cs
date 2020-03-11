namespace Windows_FCS_Analysis
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartForm));
            this.tabRBC = new System.Windows.Forms.TabPage();
            this.tabWBC = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnFinalImage = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioOldName = new System.Windows.Forms.RadioButton();
            this.radioNewName = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnFixed = new System.Windows.Forms.RadioButton();
            this.btnDynamic = new System.Windows.Forms.RadioButton();
            this.button_ProcessList = new System.Windows.Forms.Button();
            this.btnSeparation = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnPlotKde = new System.Windows.Forms.Button();
            this.checkBox_PrefilterData = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.loadFile = new System.Windows.Forms.Button();
            this.RefeshButton = new System.Windows.Forms.Button();
            this.btnLoadClusters = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPlotOnly = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.btnLoadPolygons = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
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
            this.FileNameBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chartData = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.DataFile = new System.Windows.Forms.OpenFileDialog();
            this.radioAmlName = new System.Windows.Forms.RadioButton();
            this.tabWBC.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData)).BeginInit();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabRBC
            // 
            this.tabRBC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabRBC.Location = new System.Drawing.Point(4, 43);
            this.tabRBC.Name = "tabRBC";
            this.tabRBC.Padding = new System.Windows.Forms.Padding(3);
            this.tabRBC.Size = new System.Drawing.Size(1589, 917);
            this.tabRBC.TabIndex = 1;
            this.tabRBC.Text = "RBC";
            this.tabRBC.UseVisualStyleBackColor = true;
            // 
            // tabWBC
            // 
            this.tabWBC.Controls.Add(this.groupBox4);
            this.tabWBC.Controls.Add(this.chartData);
            this.tabWBC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabWBC.Location = new System.Drawing.Point(4, 43);
            this.tabWBC.Name = "tabWBC";
            this.tabWBC.Padding = new System.Windows.Forms.Padding(3);
            this.tabWBC.Size = new System.Drawing.Size(1589, 917);
            this.tabWBC.TabIndex = 0;
            this.tabWBC.Text = "WBC";
            this.tabWBC.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Controls.Add(this.groupBox3);
            this.groupBox4.Controls.Add(this.button_ProcessList);
            this.groupBox4.Controls.Add(this.btnSeparation);
            this.groupBox4.Controls.Add(this.groupBox2);
            this.groupBox4.Controls.Add(this.groupBox1);
            this.groupBox4.Controls.Add(this.FileNameBox);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(135, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(1312, 209);
            this.groupBox4.TabIndex = 31;
            this.groupBox4.TabStop = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnFinalImage);
            this.groupBox6.Location = new System.Drawing.Point(1112, 14);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(178, 187);
            this.groupBox6.TabIndex = 35;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Report";
            // 
            // btnFinalImage
            // 
            this.btnFinalImage.Enabled = false;
            this.btnFinalImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFinalImage.Location = new System.Drawing.Point(19, 52);
            this.btnFinalImage.Margin = new System.Windows.Forms.Padding(2);
            this.btnFinalImage.Name = "btnFinalImage";
            this.btnFinalImage.Size = new System.Drawing.Size(138, 40);
            this.btnFinalImage.TabIndex = 31;
            this.btnFinalImage.Text = "Save Final Gate As Image";
            this.btnFinalImage.UseVisualStyleBackColor = true;
            this.btnFinalImage.Click += new System.EventHandler(this.btnFinalImage_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioAmlName);
            this.groupBox5.Controls.Add(this.radioOldName);
            this.groupBox5.Controls.Add(this.radioNewName);
            this.groupBox5.Location = new System.Drawing.Point(718, 89);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(125, 83);
            this.groupBox5.TabIndex = 34;
            this.groupBox5.TabStop = false;
            // 
            // radioOldName
            // 
            this.radioOldName.AutoSize = true;
            this.radioOldName.Checked = true;
            this.radioOldName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioOldName.Location = new System.Drawing.Point(12, 35);
            this.radioOldName.Name = "radioOldName";
            this.radioOldName.Size = new System.Drawing.Size(89, 19);
            this.radioOldName.TabIndex = 30;
            this.radioOldName.Text = "Old Name";
            this.radioOldName.UseVisualStyleBackColor = true;
            this.radioOldName.CheckedChanged += new System.EventHandler(this.radioOldName_CheckedChanged);
            // 
            // radioNewName
            // 
            this.radioNewName.AutoSize = true;
            this.radioNewName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioNewName.Location = new System.Drawing.Point(12, 13);
            this.radioNewName.Name = "radioNewName";
            this.radioNewName.Size = new System.Drawing.Size(95, 19);
            this.radioNewName.TabIndex = 29;
            this.radioNewName.Text = "New Name";
            this.radioNewName.UseVisualStyleBackColor = true;
            this.radioNewName.CheckedChanged += new System.EventHandler(this.radioNewName_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnFixed);
            this.groupBox3.Controls.Add(this.btnDynamic);
            this.groupBox3.Location = new System.Drawing.Point(614, 90);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(96, 83);
            this.groupBox3.TabIndex = 33;
            this.groupBox3.TabStop = false;
            // 
            // btnFixed
            // 
            this.btnFixed.AutoSize = true;
            this.btnFixed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFixed.Location = new System.Drawing.Point(8, 49);
            this.btnFixed.Name = "btnFixed";
            this.btnFixed.Size = new System.Drawing.Size(60, 19);
            this.btnFixed.TabIndex = 32;
            this.btnFixed.Text = "Fixed";
            this.btnFixed.UseVisualStyleBackColor = true;
            this.btnFixed.CheckedChanged += new System.EventHandler(this.btnFixed_CheckedChanged);
            // 
            // btnDynamic
            // 
            this.btnDynamic.AutoSize = true;
            this.btnDynamic.Checked = true;
            this.btnDynamic.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDynamic.Location = new System.Drawing.Point(8, 21);
            this.btnDynamic.Name = "btnDynamic";
            this.btnDynamic.Size = new System.Drawing.Size(80, 19);
            this.btnDynamic.TabIndex = 31;
            this.btnDynamic.TabStop = true;
            this.btnDynamic.Text = "Dynamic";
            this.btnDynamic.UseVisualStyleBackColor = true;
            this.btnDynamic.CheckedChanged += new System.EventHandler(this.btnDynamic_CheckedChanged);
            // 
            // button_ProcessList
            // 
            this.button_ProcessList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ProcessList.Location = new System.Drawing.Point(614, 13);
            this.button_ProcessList.Margin = new System.Windows.Forms.Padding(2);
            this.button_ProcessList.Name = "button_ProcessList";
            this.button_ProcessList.Size = new System.Drawing.Size(228, 35);
            this.button_ProcessList.TabIndex = 17;
            this.button_ProcessList.Text = "Process List";
            this.button_ProcessList.UseVisualStyleBackColor = true;
            this.button_ProcessList.Click += new System.EventHandler(this.button_ProcessList_Click);
            // 
            // btnSeparation
            // 
            this.btnSeparation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeparation.Location = new System.Drawing.Point(614, 52);
            this.btnSeparation.Margin = new System.Windows.Forms.Padding(1);
            this.btnSeparation.Name = "btnSeparation";
            this.btnSeparation.Size = new System.Drawing.Size(228, 35);
            this.btnSeparation.TabIndex = 12;
            this.btnSeparation.Text = "Load + Cluster";
            this.btnSeparation.UseVisualStyleBackColor = true;
            this.btnSeparation.Click += new System.EventHandler(this.btnSeparation_Click);
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
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(11, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(569, 129);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Explore FCS Data";
            // 
            // btnPlotKde
            // 
            this.btnPlotKde.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlotKde.Location = new System.Drawing.Point(209, 69);
            this.btnPlotKde.Margin = new System.Windows.Forms.Padding(1);
            this.btnPlotKde.Name = "btnPlotKde";
            this.btnPlotKde.Size = new System.Drawing.Size(112, 34);
            this.btnPlotKde.TabIndex = 12;
            this.btnPlotKde.Text = "Plot KDE";
            this.btnPlotKde.UseVisualStyleBackColor = true;
            this.btnPlotKde.Click += new System.EventHandler(this.btnPlotKde_Click);
            // 
            // checkBox_PrefilterData
            // 
            this.checkBox_PrefilterData.AutoSize = true;
            this.checkBox_PrefilterData.Checked = true;
            this.checkBox_PrefilterData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_PrefilterData.Location = new System.Drawing.Point(72, 93);
            this.checkBox_PrefilterData.Name = "checkBox_PrefilterData";
            this.checkBox_PrefilterData.Size = new System.Drawing.Size(76, 19);
            this.checkBox_PrefilterData.TabIndex = 11;
            this.checkBox_PrefilterData.Text = "prefilter";
            this.checkBox_PrefilterData.UseVisualStyleBackColor = true;
            this.checkBox_PrefilterData.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Y-axis";
            // 
            // loadFile
            // 
            this.loadFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadFile.Location = new System.Drawing.Point(209, 24);
            this.loadFile.Margin = new System.Windows.Forms.Padding(2);
            this.loadFile.Name = "loadFile";
            this.loadFile.Size = new System.Drawing.Size(112, 34);
            this.loadFile.TabIndex = 10;
            this.loadFile.Text = "Load File";
            this.loadFile.UseVisualStyleBackColor = true;
            this.loadFile.Click += new System.EventHandler(this.Button1_Click_1);
            // 
            // RefeshButton
            // 
            this.RefeshButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RefeshButton.Location = new System.Drawing.Point(433, 69);
            this.RefeshButton.Margin = new System.Windows.Forms.Padding(2);
            this.RefeshButton.Name = "RefeshButton";
            this.RefeshButton.Size = new System.Drawing.Size(112, 34);
            this.RefeshButton.TabIndex = 3;
            this.RefeshButton.Text = "Refresh";
            this.RefeshButton.UseVisualStyleBackColor = true;
            this.RefeshButton.Click += new System.EventHandler(this.RebuildButton_Click);
            // 
            // btnLoadClusters
            // 
            this.btnLoadClusters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadClusters.Location = new System.Drawing.Point(433, 24);
            this.btnLoadClusters.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadClusters.Name = "btnLoadClusters";
            this.btnLoadClusters.Size = new System.Drawing.Size(112, 34);
            this.btnLoadClusters.TabIndex = 5;
            this.btnLoadClusters.Text = "Load Clusters";
            this.btnLoadClusters.UseVisualStyleBackColor = true;
            this.btnLoadClusters.Click += new System.EventHandler(this.btnLoadClusters_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "X-axis";
            this.label1.Click += new System.EventHandler(this.Label1_Click);
            // 
            // btnPlotOnly
            // 
            this.btnPlotOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlotOnly.Location = new System.Drawing.Point(321, 69);
            this.btnPlotOnly.Margin = new System.Windows.Forms.Padding(1);
            this.btnPlotOnly.Name = "btnPlotOnly";
            this.btnPlotOnly.Size = new System.Drawing.Size(112, 34);
            this.btnPlotOnly.TabIndex = 0;
            this.btnPlotOnly.Text = "Plot only";
            this.btnPlotOnly.UseVisualStyleBackColor = true;
            this.btnPlotOnly.Click += new System.EventHandler(this.BtnPlotOnly_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(19, 58);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(124, 23);
            this.comboBox2.TabIndex = 2;
            // 
            // btnLoadPolygons
            // 
            this.btnLoadPolygons.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadPolygons.Location = new System.Drawing.Point(321, 24);
            this.btnLoadPolygons.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadPolygons.Name = "btnLoadPolygons";
            this.btnLoadPolygons.Size = new System.Drawing.Size(112, 34);
            this.btnLoadPolygons.TabIndex = 4;
            this.btnLoadPolygons.Text = "Load Gate";
            this.btnLoadPolygons.UseVisualStyleBackColor = true;
            this.btnLoadPolygons.Click += new System.EventHandler(this.btnLoadPolygons_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(19, 28);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(124, 23);
            this.comboBox1.TabIndex = 1;
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
            this.groupBox1.Location = new System.Drawing.Point(879, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(220, 187);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Process WBC";
            // 
            // button_SetGateFolder
            // 
            this.button_SetGateFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SetGateFolder.Location = new System.Drawing.Point(12, 20);
            this.button_SetGateFolder.Margin = new System.Windows.Forms.Padding(2);
            this.button_SetGateFolder.Name = "button_SetGateFolder";
            this.button_SetGateFolder.Size = new System.Drawing.Size(194, 40);
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
            this.checkBox_FinalGate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_FinalGate.Location = new System.Drawing.Point(135, 148);
            this.checkBox_FinalGate.Margin = new System.Windows.Forms.Padding(1);
            this.checkBox_FinalGate.Name = "checkBox_FinalGate";
            this.checkBox_FinalGate.Size = new System.Drawing.Size(58, 19);
            this.checkBox_FinalGate.TabIndex = 20;
            this.checkBox_FinalGate.Text = "Final";
            this.checkBox_FinalGate.UseVisualStyleBackColor = true;
            // 
            // checkBoxGate1
            // 
            this.checkBoxGate1.AutoSize = true;
            this.checkBoxGate1.Checked = true;
            this.checkBoxGate1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGate1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxGate1.Location = new System.Drawing.Point(134, 77);
            this.checkBoxGate1.Margin = new System.Windows.Forms.Padding(1);
            this.checkBoxGate1.Name = "checkBoxGate1";
            this.checkBoxGate1.Size = new System.Drawing.Size(64, 19);
            this.checkBoxGate1.TabIndex = 18;
            this.checkBoxGate1.Text = "Gate1";
            this.checkBoxGate1.UseVisualStyleBackColor = true;
            // 
            // checkBoxGate3
            // 
            this.checkBoxGate3.AutoSize = true;
            this.checkBoxGate3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxGate3.Location = new System.Drawing.Point(134, 124);
            this.checkBoxGate3.Margin = new System.Windows.Forms.Padding(1);
            this.checkBoxGate3.Name = "checkBoxGate3";
            this.checkBoxGate3.Size = new System.Drawing.Size(64, 19);
            this.checkBoxGate3.TabIndex = 17;
            this.checkBoxGate3.Text = "Gate3";
            this.checkBoxGate3.UseVisualStyleBackColor = true;
            // 
            // checkBoxGate2
            // 
            this.checkBoxGate2.AutoSize = true;
            this.checkBoxGate2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxGate2.Location = new System.Drawing.Point(134, 101);
            this.checkBoxGate2.Margin = new System.Windows.Forms.Padding(1);
            this.checkBoxGate2.Name = "checkBoxGate2";
            this.checkBoxGate2.Size = new System.Drawing.Size(64, 19);
            this.checkBoxGate2.TabIndex = 16;
            this.checkBoxGate2.Text = "Gate2";
            this.checkBoxGate2.UseVisualStyleBackColor = true;
            // 
            // radioButton_BASO
            // 
            this.radioButton_BASO.AutoSize = true;
            this.radioButton_BASO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton_BASO.Location = new System.Drawing.Point(40, 115);
            this.radioButton_BASO.Name = "radioButton_BASO";
            this.radioButton_BASO.Size = new System.Drawing.Size(61, 19);
            this.radioButton_BASO.TabIndex = 15;
            this.radioButton_BASO.Text = "BASO";
            this.radioButton_BASO.UseVisualStyleBackColor = true;
            this.radioButton_BASO.CheckedChanged += new System.EventHandler(this.radioButton_BASO_CheckedChanged);
            // 
            // radioButton_EOS
            // 
            this.radioButton_EOS.AutoSize = true;
            this.radioButton_EOS.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton_EOS.Location = new System.Drawing.Point(40, 91);
            this.radioButton_EOS.Name = "radioButton_EOS";
            this.radioButton_EOS.Size = new System.Drawing.Size(53, 19);
            this.radioButton_EOS.TabIndex = 14;
            this.radioButton_EOS.Text = "EOS";
            this.radioButton_EOS.UseVisualStyleBackColor = true;
            this.radioButton_EOS.CheckedChanged += new System.EventHandler(this.radioButton_EOS_CheckedChanged);
            // 
            // radioButton_3diff
            // 
            this.radioButton_3diff.AutoSize = true;
            this.radioButton_3diff.Checked = true;
            this.radioButton_3diff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton_3diff.Location = new System.Drawing.Point(40, 68);
            this.radioButton_3diff.Name = "radioButton_3diff";
            this.radioButton_3diff.Size = new System.Drawing.Size(58, 19);
            this.radioButton_3diff.TabIndex = 13;
            this.radioButton_3diff.TabStop = true;
            this.radioButton_3diff.Text = "3-diff";
            this.radioButton_3diff.UseVisualStyleBackColor = true;
            this.radioButton_3diff.CheckedChanged += new System.EventHandler(this.radioButton_3diff_CheckedChanged);
            // 
            // btnGating
            // 
            this.btnGating.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGating.Location = new System.Drawing.Point(38, 143);
            this.btnGating.Margin = new System.Windows.Forms.Padding(1);
            this.btnGating.Name = "btnGating";
            this.btnGating.Size = new System.Drawing.Size(69, 31);
            this.btnGating.TabIndex = 6;
            this.btnGating.Text = "Gate File";
            this.btnGating.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnGating.UseVisualStyleBackColor = true;
            this.btnGating.Click += new System.EventHandler(this.btnGating_Click);
            // 
            // FileNameBox
            // 
            this.FileNameBox.Location = new System.Drawing.Point(11, 181);
            this.FileNameBox.Margin = new System.Windows.Forms.Padding(2);
            this.FileNameBox.Name = "FileNameBox";
            this.FileNameBox.Size = new System.Drawing.Size(853, 21);
            this.FileNameBox.TabIndex = 21;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(392, 161);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 15);
            this.label4.TabIndex = 24;
            this.label4.Text = "Loaded File Name";
            // 
            // chartData
            // 
            this.chartData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartData.BackImageAlignment = System.Windows.Forms.DataVisualization.Charting.ChartImageAlignmentStyle.Center;
            this.chartData.BorderlineColor = System.Drawing.Color.DarkGray;
            this.chartData.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.AxisX.Title = "X";
            chartArea1.AxisY.Title = "Y";
            chartArea1.Name = "ChartArea1";
            this.chartData.ChartAreas.Add(chartArea1);
            legend1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            legend1.IsTextAutoFit = false;
            legend1.Name = "Legend1";
            this.chartData.Legends.Add(legend1);
            this.chartData.Location = new System.Drawing.Point(11, 222);
            this.chartData.Name = "chartData";
            this.chartData.Size = new System.Drawing.Size(1564, 685);
            this.chartData.TabIndex = 0;
            this.chartData.Text = "chart2";
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title1.Name = "Title1";
            title1.Text = "WBC Analysis Result";
            this.chartData.Titles.Add(title1);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabWBC);
            this.tabControl.Controls.Add(this.tabRBC);
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(20);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(10, 10);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1597, 964);
            this.tabControl.TabIndex = 29;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 43);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1589, 917);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Mie Scatter";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // DataFile
            // 
            this.DataFile.FileName = "openFileDialog1";
            // 
            // radioAmlName
            // 
            this.radioAmlName.AutoSize = true;
            this.radioAmlName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioAmlName.Location = new System.Drawing.Point(12, 58);
            this.radioAmlName.Name = "radioAmlName";
            this.radioAmlName.Size = new System.Drawing.Size(95, 19);
            this.radioAmlName.TabIndex = 31;
            this.radioAmlName.Text = "AML Name";
            this.radioAmlName.UseVisualStyleBackColor = true;
            this.radioAmlName.CheckedChanged += new System.EventHandler(this.radioAmlName_CheckedChanged);
            // 
            // ChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1597, 964);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ChartForm";
            this.Text = "FCS Analytics";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ChartForm_Load);
            this.tabWBC.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartData)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabRBC;
        private System.Windows.Forms.TabPage tabWBC;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioOldName;
        private System.Windows.Forms.RadioButton radioNewName;
        private System.Windows.Forms.Button button_ProcessList;
        private System.Windows.Forms.Button btnSeparation;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnPlotKde;
        private System.Windows.Forms.CheckBox checkBox_PrefilterData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button loadFile;
        private System.Windows.Forms.Button RefeshButton;
        private System.Windows.Forms.Button btnLoadClusters;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPlotOnly;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button btnLoadPolygons;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_SetGateFolder;
        private System.Windows.Forms.CheckBox checkBox_FinalGate;
        private System.Windows.Forms.CheckBox checkBoxGate1;
        private System.Windows.Forms.CheckBox checkBoxGate3;
        private System.Windows.Forms.CheckBox checkBoxGate2;
        private System.Windows.Forms.RadioButton radioButton_BASO;
        private System.Windows.Forms.RadioButton radioButton_EOS;
        private System.Windows.Forms.RadioButton radioButton_3diff;
        private System.Windows.Forms.Button btnGating;
        private System.Windows.Forms.TextBox FileNameBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartData;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.OpenFileDialog DataFile;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton btnFixed;
        private System.Windows.Forms.RadioButton btnDynamic;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnFinalImage;
        private System.Windows.Forms.RadioButton radioAmlName;
    }
}


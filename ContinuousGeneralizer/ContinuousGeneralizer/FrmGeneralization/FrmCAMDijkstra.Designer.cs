namespace ContinuousGeneralizer.FrmMorphing
{
    partial class FrmCAMDijkstra
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnReduce = new System.Windows.Forms.Button();
            this.btnSaveInterpolation = new System.Windows.Forms.Button();
            this.pbScale = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnMultiResults = new System.Windows.Forms.Button();
            this.txtProportion = new System.Windows.Forms.TextBox();
            this.btnInputedScale = new System.Windows.Forms.Button();
            this.btn050 = new System.Windows.Forms.Button();
            this.btn025 = new System.Windows.Forms.Button();
            this.btn010 = new System.Windows.Forms.Button();
            this.btn020 = new System.Windows.Forms.Button();
            this.btn100 = new System.Windows.Forms.Button();
            this.btn030 = new System.Windows.Forms.Button();
            this.btn075 = new System.Windows.Forms.Button();
            this.btn040 = new System.Windows.Forms.Button();
            this.btn000 = new System.Windows.Forms.Button();
            this.btn060 = new System.Windows.Forms.Button();
            this.btn080 = new System.Windows.Forms.Button();
            this.btn070 = new System.Windows.Forms.Button();
            this.btn090 = new System.Windows.Forms.Button();
            this.txtSmallerScale = new System.Windows.Forms.TextBox();
            this.txtLargerScale = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtEvaluation = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.chkSmallest = new System.Windows.Forms.CheckBox();
            this.btnResultFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cboSmallerScaleLayer = new System.Windows.Forms.ComboBox();
            this.lblLayer = new System.Windows.Forms.Label();
            this.cboLargerScaleLayer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboShapeConstraint = new System.Windows.Forms.ComboBox();
            this.txtNodes = new System.Windows.Forms.TextBox();
            this.btnRunSpecified = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnRunILP_Extend = new System.Windows.Forms.Button();
            this.btnRunILP = new System.Windows.Forms.Button();
            this.btnRunILPSpecified = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkOutput = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnGreedy = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAdd.Location = new System.Drawing.Point(245, 179);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(35, 29);
            this.btnAdd.TabIndex = 80;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnReduce
            // 
            this.btnReduce.Font = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReduce.Location = new System.Drawing.Point(6, 179);
            this.btnReduce.Name = "btnReduce";
            this.btnReduce.Size = new System.Drawing.Size(35, 29);
            this.btnReduce.TabIndex = 79;
            this.btnReduce.Text = "-";
            this.btnReduce.UseVisualStyleBackColor = true;
            this.btnReduce.Click += new System.EventHandler(this.btnReduce_Click);
            // 
            // btnSaveInterpolation
            // 
            this.btnSaveInterpolation.Location = new System.Drawing.Point(8, 147);
            this.btnSaveInterpolation.Name = "btnSaveInterpolation";
            this.btnSaveInterpolation.Size = new System.Drawing.Size(162, 25);
            this.btnSaveInterpolation.TabIndex = 70;
            this.btnSaveInterpolation.Text = "Save the interpolated result";
            this.btnSaveInterpolation.UseVisualStyleBackColor = true;
            this.btnSaveInterpolation.Click += new System.EventHandler(this.btnSaveInterpolation_Click);
            // 
            // pbScale
            // 
            this.pbScale.Location = new System.Drawing.Point(62, 179);
            this.pbScale.Name = "pbScale";
            this.pbScale.Size = new System.Drawing.Size(162, 29);
            this.pbScale.TabIndex = 71;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnMultiResults);
            this.groupBox2.Controls.Add(this.btnAdd);
            this.groupBox2.Controls.Add(this.btnReduce);
            this.groupBox2.Controls.Add(this.pbScale);
            this.groupBox2.Controls.Add(this.btnSaveInterpolation);
            this.groupBox2.Controls.Add(this.txtProportion);
            this.groupBox2.Controls.Add(this.btnInputedScale);
            this.groupBox2.Controls.Add(this.btn050);
            this.groupBox2.Controls.Add(this.btn025);
            this.groupBox2.Controls.Add(this.btn010);
            this.groupBox2.Controls.Add(this.btn020);
            this.groupBox2.Controls.Add(this.btn100);
            this.groupBox2.Controls.Add(this.btn030);
            this.groupBox2.Controls.Add(this.btn075);
            this.groupBox2.Controls.Add(this.btn040);
            this.groupBox2.Controls.Add(this.btn000);
            this.groupBox2.Controls.Add(this.btn060);
            this.groupBox2.Controls.Add(this.btn080);
            this.groupBox2.Controls.Add(this.btn070);
            this.groupBox2.Controls.Add(this.btn090);
            this.groupBox2.Location = new System.Drawing.Point(11, 558);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(288, 221);
            this.groupBox2.TabIndex = 74;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Display";
            // 
            // btnMultiResults
            // 
            this.btnMultiResults.Location = new System.Drawing.Point(174, 85);
            this.btnMultiResults.Name = "btnMultiResults";
            this.btnMultiResults.Size = new System.Drawing.Size(106, 25);
            this.btnMultiResults.TabIndex = 81;
            this.btnMultiResults.Text = "MultiResults";
            this.btnMultiResults.UseVisualStyleBackColor = true;
            this.btnMultiResults.Click += new System.EventHandler(this.btnMultiResults_Click);
            // 
            // txtProportion
            // 
            this.txtProportion.Location = new System.Drawing.Point(116, 116);
            this.txtProportion.Name = "txtProportion";
            this.txtProportion.Size = new System.Drawing.Size(108, 20);
            this.txtProportion.TabIndex = 68;
            this.txtProportion.Text = "0.55";
            this.txtProportion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnInputedScale
            // 
            this.btnInputedScale.Location = new System.Drawing.Point(6, 116);
            this.btnInputedScale.Name = "btnInputedScale";
            this.btnInputedScale.Size = new System.Drawing.Size(104, 25);
            this.btnInputedScale.TabIndex = 67;
            this.btnInputedScale.Text = "Output with t";
            this.btnInputedScale.UseVisualStyleBackColor = true;
            this.btnInputedScale.Click += new System.EventHandler(this.btnInputedScale_Click);
            // 
            // btn050
            // 
            this.btn050.Location = new System.Drawing.Point(230, 22);
            this.btn050.Name = "btn050";
            this.btn050.Size = new System.Drawing.Size(50, 25);
            this.btn050.TabIndex = 63;
            this.btn050.Text = "t=0.5";
            this.btn050.UseVisualStyleBackColor = true;
            this.btn050.Click += new System.EventHandler(this.btn050_Click);
            // 
            // btn025
            // 
            this.btn025.Location = new System.Drawing.Point(62, 85);
            this.btn025.Name = "btn025";
            this.btn025.Size = new System.Drawing.Size(50, 25);
            this.btn025.TabIndex = 65;
            this.btn025.Text = "t=0.25";
            this.btn025.UseVisualStyleBackColor = true;
            this.btn025.Click += new System.EventHandler(this.btn025_Click);
            // 
            // btn010
            // 
            this.btn010.Location = new System.Drawing.Point(6, 22);
            this.btn010.Name = "btn010";
            this.btn010.Size = new System.Drawing.Size(50, 25);
            this.btn010.TabIndex = 51;
            this.btn010.Text = "t=0.1";
            this.btn010.UseVisualStyleBackColor = true;
            this.btn010.Click += new System.EventHandler(this.btn010_Click);
            // 
            // btn020
            // 
            this.btn020.Location = new System.Drawing.Point(62, 22);
            this.btn020.Name = "btn020";
            this.btn020.Size = new System.Drawing.Size(50, 25);
            this.btn020.TabIndex = 52;
            this.btn020.Text = "t=0.2";
            this.btn020.UseVisualStyleBackColor = true;
            this.btn020.Click += new System.EventHandler(this.btn020_Click);
            // 
            // btn100
            // 
            this.btn100.Location = new System.Drawing.Point(230, 53);
            this.btn100.Name = "btn100";
            this.btn100.Size = new System.Drawing.Size(50, 25);
            this.btn100.TabIndex = 62;
            this.btn100.Text = "t=1";
            this.btn100.UseVisualStyleBackColor = true;
            this.btn100.Click += new System.EventHandler(this.btn100_Click);
            // 
            // btn030
            // 
            this.btn030.Location = new System.Drawing.Point(118, 22);
            this.btn030.Name = "btn030";
            this.btn030.Size = new System.Drawing.Size(50, 25);
            this.btn030.TabIndex = 53;
            this.btn030.Text = "t=0.3";
            this.btn030.UseVisualStyleBackColor = true;
            this.btn030.Click += new System.EventHandler(this.btn030_Click);
            // 
            // btn075
            // 
            this.btn075.Location = new System.Drawing.Point(118, 85);
            this.btn075.Name = "btn075";
            this.btn075.Size = new System.Drawing.Size(50, 25);
            this.btn075.TabIndex = 61;
            this.btn075.Text = "t=0.75";
            this.btn075.UseVisualStyleBackColor = true;
            this.btn075.Click += new System.EventHandler(this.btn075_Click);
            // 
            // btn040
            // 
            this.btn040.Location = new System.Drawing.Point(174, 22);
            this.btn040.Name = "btn040";
            this.btn040.Size = new System.Drawing.Size(50, 25);
            this.btn040.TabIndex = 54;
            this.btn040.Text = "t=0.4";
            this.btn040.UseVisualStyleBackColor = true;
            this.btn040.Click += new System.EventHandler(this.btn040_Click);
            // 
            // btn000
            // 
            this.btn000.Location = new System.Drawing.Point(6, 85);
            this.btn000.Name = "btn000";
            this.btn000.Size = new System.Drawing.Size(50, 25);
            this.btn000.TabIndex = 60;
            this.btn000.Text = "t=0.0";
            this.btn000.UseVisualStyleBackColor = true;
            this.btn000.Click += new System.EventHandler(this.btn000_Click);
            // 
            // btn060
            // 
            this.btn060.Location = new System.Drawing.Point(6, 53);
            this.btn060.Name = "btn060";
            this.btn060.Size = new System.Drawing.Size(50, 25);
            this.btn060.TabIndex = 56;
            this.btn060.Text = "t=0.6";
            this.btn060.UseVisualStyleBackColor = true;
            this.btn060.Click += new System.EventHandler(this.btn060_Click);
            // 
            // btn080
            // 
            this.btn080.Location = new System.Drawing.Point(118, 53);
            this.btn080.Name = "btn080";
            this.btn080.Size = new System.Drawing.Size(50, 25);
            this.btn080.TabIndex = 59;
            this.btn080.Text = "t=0.8";
            this.btn080.UseVisualStyleBackColor = true;
            this.btn080.Click += new System.EventHandler(this.btn080_Click);
            // 
            // btn070
            // 
            this.btn070.Location = new System.Drawing.Point(62, 53);
            this.btn070.Name = "btn070";
            this.btn070.Size = new System.Drawing.Size(50, 25);
            this.btn070.TabIndex = 57;
            this.btn070.Text = "t=0.7";
            this.btn070.UseVisualStyleBackColor = true;
            this.btn070.Click += new System.EventHandler(this.btn070_Click);
            // 
            // btn090
            // 
            this.btn090.Location = new System.Drawing.Point(174, 53);
            this.btn090.Name = "btn090";
            this.btn090.Size = new System.Drawing.Size(50, 25);
            this.btn090.TabIndex = 58;
            this.btn090.Text = "t=0.9";
            this.btn090.UseVisualStyleBackColor = true;
            this.btn090.Click += new System.EventHandler(this.btn090_Click);
            // 
            // txtSmallerScale
            // 
            this.txtSmallerScale.Location = new System.Drawing.Point(168, 133);
            this.txtSmallerScale.Name = "txtSmallerScale";
            this.txtSmallerScale.Size = new System.Drawing.Size(114, 20);
            this.txtSmallerScale.TabIndex = 88;
            this.txtSmallerScale.Text = "250000";
            this.txtSmallerScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtLargerScale
            // 
            this.txtLargerScale.Location = new System.Drawing.Point(168, 107);
            this.txtLargerScale.Name = "txtLargerScale";
            this.txtLargerScale.Size = new System.Drawing.Size(114, 20);
            this.txtLargerScale.TabIndex = 87;
            this.txtLargerScale.Text = "50000";
            this.txtLargerScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(147, 136);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 86;
            this.label6.Text = "1/";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(147, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 84;
            this.label5.Text = "1/";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 13);
            this.label4.TabIndex = 82;
            this.label4.Text = "Scale of less detailed layer:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtEvaluation);
            this.groupBox3.Location = new System.Drawing.Point(12, 785);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(288, 53);
            this.groupBox3.TabIndex = 75;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Evaluation";
            // 
            // txtEvaluation
            // 
            this.txtEvaluation.Location = new System.Drawing.Point(6, 19);
            this.txtEvaluation.Name = "txtEvaluation";
            this.txtEvaluation.ReadOnly = true;
            this.txtEvaluation.Size = new System.Drawing.Size(114, 20);
            this.txtEvaluation.TabIndex = 59;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 13);
            this.label3.TabIndex = 81;
            this.label3.Text = "Scale of more detailed layer:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRunAll);
            this.groupBox1.Controls.Add(this.chkSmallest);
            this.groupBox1.Controls.Add(this.btnResultFolder);
            this.groupBox1.Controls.Add(this.txtSmallerScale);
            this.groupBox1.Controls.Add(this.txtLargerScale);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboSmallerScaleLayer);
            this.groupBox1.Controls.Add(this.lblLayer);
            this.groupBox1.Controls.Add(this.cboLargerScaleLayer);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 193);
            this.groupBox1.TabIndex = 73;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // btnRunAll
            // 
            this.btnRunAll.Location = new System.Drawing.Point(6, 159);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(64, 25);
            this.btnRunAll.TabIndex = 102;
            this.btnRunAll.Text = "Run All";
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.btnRunAll_Click);
            // 
            // chkSmallest
            // 
            this.chkSmallest.AutoSize = true;
            this.chkSmallest.Checked = true;
            this.chkSmallest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSmallest.Location = new System.Drawing.Point(148, 77);
            this.chkSmallest.Name = "chkSmallest";
            this.chkSmallest.Size = new System.Drawing.Size(134, 17);
            this.chkSmallest.TabIndex = 98;
            this.chkSmallest.Text = "Involve Smallest Patch";
            this.chkSmallest.UseVisualStyleBackColor = true;
            // 
            // btnResultFolder
            // 
            this.btnResultFolder.Location = new System.Drawing.Point(188, 159);
            this.btnResultFolder.Name = "btnResultFolder";
            this.btnResultFolder.Size = new System.Drawing.Size(92, 25);
            this.btnResultFolder.TabIndex = 97;
            this.btnResultFolder.Text = "Result Folder";
            this.btnResultFolder.UseVisualStyleBackColor = true;
            this.btnResultFolder.Click += new System.EventHandler(this.btnResultFolder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 75;
            this.label2.Text = "Less detailed layer:";
            // 
            // cboSmallerScaleLayer
            // 
            this.cboSmallerScaleLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSmallerScaleLayer.FormattingEnabled = true;
            this.cboSmallerScaleLayer.Location = new System.Drawing.Point(122, 50);
            this.cboSmallerScaleLayer.Name = "cboSmallerScaleLayer";
            this.cboSmallerScaleLayer.Size = new System.Drawing.Size(160, 21);
            this.cboSmallerScaleLayer.TabIndex = 76;
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(6, 25);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(99, 13);
            this.lblLayer.TabIndex = 18;
            this.lblLayer.Text = "More detailed layer:";
            // 
            // cboLargerScaleLayer
            // 
            this.cboLargerScaleLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLargerScaleLayer.FormattingEnabled = true;
            this.cboLargerScaleLayer.Location = new System.Drawing.Point(122, 22);
            this.cboLargerScaleLayer.Name = "cboLargerScaleLayer";
            this.cboLargerScaleLayer.Size = new System.Drawing.Size(160, 21);
            this.cboLargerScaleLayer.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 95;
            this.label1.Text = "Shape Constraint:";
            // 
            // cboShapeConstraint
            // 
            this.cboShapeConstraint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShapeConstraint.FormattingEnabled = true;
            this.cboShapeConstraint.Items.AddRange(new object[] {
            "NonShape",
            "MinimizeInteriorBoundaries",
            "MaximizeMinComp_EdgeNumber",
            "MaximizeMinComp_Combine",
            "MaximizeAvgComp_EdgeNumber",
            "MaximizeAvgComp_Combine"});
            this.cboShapeConstraint.Location = new System.Drawing.Point(122, 19);
            this.cboShapeConstraint.Name = "cboShapeConstraint";
            this.cboShapeConstraint.Size = new System.Drawing.Size(160, 21);
            this.cboShapeConstraint.TabIndex = 96;
            // 
            // txtNodes
            // 
            this.txtNodes.Location = new System.Drawing.Point(191, 48);
            this.txtNodes.Name = "txtNodes";
            this.txtNodes.Size = new System.Drawing.Size(77, 20);
            this.txtNodes.TabIndex = 99;
            this.txtNodes.Text = "200000";
            this.txtNodes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnRunSpecified
            // 
            this.btnRunSpecified.Location = new System.Drawing.Point(6, 50);
            this.btnRunSpecified.Name = "btnRunSpecified";
            this.btnRunSpecified.Size = new System.Drawing.Size(106, 25);
            this.btnRunSpecified.TabIndex = 98;
            this.btnRunSpecified.Text = "Run Specified";
            this.btnRunSpecified.UseVisualStyleBackColor = true;
            this.btnRunSpecified.Click += new System.EventHandler(this.btnRunSpecified_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(6, 19);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(64, 25);
            this.btnRun.TabIndex = 71;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnRunILP_Extend);
            this.groupBox4.Controls.Add(this.btnRunILP);
            this.groupBox4.Controls.Add(this.btnRunILPSpecified);
            this.groupBox4.Location = new System.Drawing.Point(12, 457);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(288, 56);
            this.groupBox4.TabIndex = 76;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ILP";
            // 
            // btnRunILP_Extend
            // 
            this.btnRunILP_Extend.Location = new System.Drawing.Point(188, 19);
            this.btnRunILP_Extend.Name = "btnRunILP_Extend";
            this.btnRunILP_Extend.Size = new System.Drawing.Size(64, 25);
            this.btnRunILP_Extend.TabIndex = 101;
            this.btnRunILP_Extend.Text = "Run_E";
            this.btnRunILP_Extend.UseVisualStyleBackColor = true;
            this.btnRunILP_Extend.Click += new System.EventHandler(this.btnRunILP_Extend_Click);
            // 
            // btnRunILP
            // 
            this.btnRunILP.Location = new System.Drawing.Point(6, 19);
            this.btnRunILP.Name = "btnRunILP";
            this.btnRunILP.Size = new System.Drawing.Size(64, 25);
            this.btnRunILP.TabIndex = 99;
            this.btnRunILP.Text = "Run";
            this.btnRunILP.UseVisualStyleBackColor = true;
            this.btnRunILP.Click += new System.EventHandler(this.btnRunILP_Click);
            // 
            // btnRunILPSpecified
            // 
            this.btnRunILPSpecified.Location = new System.Drawing.Point(76, 19);
            this.btnRunILPSpecified.Name = "btnRunILPSpecified";
            this.btnRunILPSpecified.Size = new System.Drawing.Size(106, 25);
            this.btnRunILPSpecified.TabIndex = 100;
            this.btnRunILPSpecified.Text = "Run Specified";
            this.btnRunILPSpecified.UseVisualStyleBackColor = true;
            this.btnRunILPSpecified.Click += new System.EventHandler(this.btnRunILPSpecified_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkOutput);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.textBox1);
            this.groupBox5.Controls.Add(this.txtNodes);
            this.groupBox5.Controls.Add(this.btnRun);
            this.groupBox5.Controls.Add(this.btnRunSpecified);
            this.groupBox5.Location = new System.Drawing.Point(9, 134);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(274, 100);
            this.groupBox5.TabIndex = 77;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "A Star";
            // 
            // chkOutput
            // 
            this.chkOutput.AutoSize = true;
            this.chkOutput.Location = new System.Drawing.Point(56, 77);
            this.chkOutput.Name = "chkOutput";
            this.chkOutput.Size = new System.Drawing.Size(87, 17);
            this.chkOutput.TabIndex = 102;
            this.chkOutput.Text = "Output Maps";
            this.chkOutput.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(157, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 13);
            this.label7.TabIndex = 101;
            this.label7.Text = "3GB";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox1.Location = new System.Drawing.Point(191, 74);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(77, 20);
            this.textBox1.TabIndex = 100;
            this.textBox1.Text = "2000000000";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnGreedy);
            this.groupBox6.Controls.Add(this.button3);
            this.groupBox6.Location = new System.Drawing.Point(7, 46);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(274, 82);
            this.groupBox6.TabIndex = 78;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Greedy";
            // 
            // btnGreedy
            // 
            this.btnGreedy.Location = new System.Drawing.Point(6, 19);
            this.btnGreedy.Name = "btnGreedy";
            this.btnGreedy.Size = new System.Drawing.Size(64, 25);
            this.btnGreedy.TabIndex = 99;
            this.btnGreedy.Text = "Run";
            this.btnGreedy.UseVisualStyleBackColor = true;
            this.btnGreedy.Click += new System.EventHandler(this.btnGreedy_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(76, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(106, 25);
            this.button3.TabIndex = 100;
            this.button3.Text = "Run Specified";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.groupBox5);
            this.groupBox7.Controls.Add(this.groupBox6);
            this.groupBox7.Controls.Add(this.cboShapeConstraint);
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Location = new System.Drawing.Point(12, 211);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(287, 240);
            this.groupBox7.TabIndex = 79;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Greedy and AStar";
            // 
            // FrmCAMDijkstra
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 853);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmCAMDijkstra";
            this.Text = "FrmCAMDijkstra";
            this.Load += new System.EventHandler(this.FrmCAMDijkstra_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnReduce;
        private System.Windows.Forms.Button btnSaveInterpolation;
        private System.Windows.Forms.ProgressBar pbScale;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtProportion;
        private System.Windows.Forms.Button btnInputedScale;
        private System.Windows.Forms.Button btn050;
        private System.Windows.Forms.Button btn025;
        private System.Windows.Forms.Button btn010;
        private System.Windows.Forms.Button btn020;
        private System.Windows.Forms.Button btn100;
        private System.Windows.Forms.Button btn030;
        private System.Windows.Forms.Button btn075;
        private System.Windows.Forms.Button btn040;
        private System.Windows.Forms.Button btn000;
        private System.Windows.Forms.Button btn060;
        private System.Windows.Forms.Button btn080;
        private System.Windows.Forms.Button btn070;
        private System.Windows.Forms.Button btn090;
        private System.Windows.Forms.TextBox txtSmallerScale;
        private System.Windows.Forms.TextBox txtLargerScale;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboSmallerScaleLayer;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.ComboBox cboLargerScaleLayer;
        private System.Windows.Forms.TextBox txtEvaluation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboShapeConstraint;
        private System.Windows.Forms.Button btnResultFolder;
        private System.Windows.Forms.Button btnMultiResults;
        private System.Windows.Forms.Button btnRunSpecified;
        private System.Windows.Forms.TextBox txtNodes;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnRunILP;
        private System.Windows.Forms.Button btnRunILPSpecified;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox chkSmallest;
        private System.Windows.Forms.Button btnRunILP_Extend;
        private System.Windows.Forms.Button btnRunAll;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox chkOutput;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnGreedy;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox7;
    }
}
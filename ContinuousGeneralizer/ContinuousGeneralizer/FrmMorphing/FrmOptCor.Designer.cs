namespace ContinuousGeneralizer.FrmMorphing
{
    partial class FrmOptCor
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRun = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboIntMaxBackKforJ = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkCoincidentPoints = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboEvaluationMethod = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboStandardVector = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboMorphingMethod = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIncrease = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMulti = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboSmallerScaleLayer = new System.Windows.Forms.ComboBox();
            this.lblLayer = new System.Windows.Forms.Label();
            this.cboLargerScaleLayer = new System.Windows.Forms.ComboBox();
            this.lblLengthBound = new System.Windows.Forms.Label();
            this.txtMaxBackK = new System.Windows.Forms.TextBox();
            this.txtEvaluation = new System.Windows.Forms.TextBox();
            this.txtProportion = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnstatisticEX = new System.Windows.Forms.Button();
            this.btnStatistic = new System.Windows.Forms.Button();
            this.btnTranslation = new System.Windows.Forms.Button();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.btnIntegral = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnReduce = new System.Windows.Forms.Button();
            this.pbScale = new System.Windows.Forms.ProgressBar();
            this.btnSaveInterpolation = new System.Windows.Forms.Button();
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
            this.label8 = new System.Windows.Forms.Label();
            this.txtAttributeOfKnown = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(8, 405);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(64, 25);
            this.btnRun.TabIndex = 71;
            this.btnRun.Text = "确  定";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtAttributeOfKnown);
            this.groupBox1.Controls.Add(this.cboIntMaxBackKforJ);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.chkCoincidentPoints);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cboEvaluationMethod);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cboStandardVector);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cboMorphingMethod);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtIncrease);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtMulti);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboSmallerScaleLayer);
            this.groupBox1.Controls.Add(this.lblLayer);
            this.groupBox1.Controls.Add(this.cboLargerScaleLayer);
            this.groupBox1.Controls.Add(this.lblLengthBound);
            this.groupBox1.Controls.Add(this.txtMaxBackK);
            this.groupBox1.Controls.Add(this.btnRun);
            this.groupBox1.Location = new System.Drawing.Point(12, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(360, 436);
            this.groupBox1.TabIndex = 64;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "参数输入";
            // 
            // cboIntMaxBackKforJ
            // 
            this.cboIntMaxBackKforJ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIntMaxBackKforJ.FormattingEnabled = true;
            this.cboIntMaxBackKforJ.Items.AddRange(new object[] {
            "1",
            "2",
            "5",
            "equal to I"});
            this.cboIntMaxBackKforJ.Location = new System.Drawing.Point(227, 229);
            this.cboIntMaxBackKforJ.Name = "cboIntMaxBackKforJ";
            this.cboIntMaxBackKforJ.Size = new System.Drawing.Size(125, 21);
            this.cboIntMaxBackKforJ.TabIndex = 102;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 232);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(140, 13);
            this.label7.TabIndex = 101;
            this.label7.Text = "look-back parameter K for J:";
            // 
            // chkCoincidentPoints
            // 
            this.chkCoincidentPoints.AutoSize = true;
            this.chkCoincidentPoints.Checked = true;
            this.chkCoincidentPoints.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCoincidentPoints.Location = new System.Drawing.Point(225, 155);
            this.chkCoincidentPoints.Name = "chkCoincidentPoints";
            this.chkCoincidentPoints.Size = new System.Drawing.Size(129, 17);
            this.chkCoincidentPoints.TabIndex = 99;
            this.chkCoincidentPoints.Text = "Use Coincident points";
            this.chkCoincidentPoints.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 13);
            this.label6.TabIndex = 97;
            this.label6.Text = "Evaluation Method:";
            // 
            // cboEvaluationMethod
            // 
            this.cboEvaluationMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEvaluationMethod.FormattingEnabled = true;
            this.cboEvaluationMethod.Items.AddRange(new object[] {
            "DeflectionCLIA (Integral)",
            "Translation",
            "Deflection (Integral)"});
            this.cboEvaluationMethod.Location = new System.Drawing.Point(122, 128);
            this.cboEvaluationMethod.Name = "cboEvaluationMethod";
            this.cboEvaluationMethod.Size = new System.Drawing.Size(232, 21);
            this.cboEvaluationMethod.TabIndex = 98;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 95;
            this.label5.Text = "Standard Vector:";
            // 
            // cboStandardVector
            // 
            this.cboStandardVector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStandardVector.FormattingEnabled = true;
            this.cboStandardVector.Items.AddRange(new object[] {
            "Zero",
            "Starts of the polylines"});
            this.cboStandardVector.Location = new System.Drawing.Point(122, 101);
            this.cboStandardVector.Name = "cboStandardVector";
            this.cboStandardVector.Size = new System.Drawing.Size(232, 21);
            this.cboStandardVector.TabIndex = 96;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 93;
            this.label4.Text = "Morphing Method:";
            // 
            // cboMorphingMethod
            // 
            this.cboMorphingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMorphingMethod.FormattingEnabled = true;
            this.cboMorphingMethod.Items.AddRange(new object[] {
            "OptCor",
            "OptCorSimplified",
            "OptCorMM",
            "OptCorMMSimplified"});
            this.cboMorphingMethod.Location = new System.Drawing.Point(122, 74);
            this.cboMorphingMethod.Name = "cboMorphingMethod";
            this.cboMorphingMethod.Size = new System.Drawing.Size(232, 21);
            this.cboMorphingMethod.TabIndex = 94;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 291);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 92;
            this.label3.Text = "Increase:";
            // 
            // txtIncrease
            // 
            this.txtIncrease.Location = new System.Drawing.Point(227, 288);
            this.txtIncrease.Name = "txtIncrease";
            this.txtIncrease.Size = new System.Drawing.Size(127, 20);
            this.txtIncrease.TabIndex = 91;
            this.txtIncrease.Text = "1";
            this.txtIncrease.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 265);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 90;
            this.label1.Text = "Multi:";
            // 
            // txtMulti
            // 
            this.txtMulti.Location = new System.Drawing.Point(227, 262);
            this.txtMulti.Name = "txtMulti";
            this.txtMulti.Size = new System.Drawing.Size(127, 20);
            this.txtMulti.TabIndex = 89;
            this.txtMulti.Text = "1";
            this.txtMulti.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 87;
            this.label2.Text = "小比例尺图层：";
            // 
            // cboSmallerScaleLayer
            // 
            this.cboSmallerScaleLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSmallerScaleLayer.FormattingEnabled = true;
            this.cboSmallerScaleLayer.Location = new System.Drawing.Point(122, 47);
            this.cboSmallerScaleLayer.Name = "cboSmallerScaleLayer";
            this.cboSmallerScaleLayer.Size = new System.Drawing.Size(232, 21);
            this.cboSmallerScaleLayer.TabIndex = 88;
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(6, 22);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(91, 13);
            this.lblLayer.TabIndex = 85;
            this.lblLayer.Text = "大比例尺图层：";
            // 
            // cboLargerScaleLayer
            // 
            this.cboLargerScaleLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLargerScaleLayer.FormattingEnabled = true;
            this.cboLargerScaleLayer.Location = new System.Drawing.Point(122, 19);
            this.cboLargerScaleLayer.Name = "cboLargerScaleLayer";
            this.cboLargerScaleLayer.Size = new System.Drawing.Size(232, 21);
            this.cboLargerScaleLayer.TabIndex = 86;
            // 
            // lblLengthBound
            // 
            this.lblLengthBound.AutoSize = true;
            this.lblLengthBound.Location = new System.Drawing.Point(6, 207);
            this.lblLengthBound.Name = "lblLengthBound";
            this.lblLengthBound.Size = new System.Drawing.Size(138, 13);
            this.lblLengthBound.TabIndex = 78;
            this.lblLengthBound.Text = "look-back parameter K for I:";
            // 
            // txtMaxBackK
            // 
            this.txtMaxBackK.Location = new System.Drawing.Point(227, 204);
            this.txtMaxBackK.Name = "txtMaxBackK";
            this.txtMaxBackK.Size = new System.Drawing.Size(127, 20);
            this.txtMaxBackK.TabIndex = 77;
            this.txtMaxBackK.Text = "203";
            this.txtMaxBackK.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtEvaluation
            // 
            this.txtEvaluation.Location = new System.Drawing.Point(6, 76);
            this.txtEvaluation.Name = "txtEvaluation";
            this.txtEvaluation.ReadOnly = true;
            this.txtEvaluation.Size = new System.Drawing.Size(114, 20);
            this.txtEvaluation.TabIndex = 58;
            // 
            // txtProportion
            // 
            this.txtProportion.Location = new System.Drawing.Point(116, 116);
            this.txtProportion.Name = "txtProportion";
            this.txtProportion.Size = new System.Drawing.Size(108, 20);
            this.txtProportion.TabIndex = 68;
            this.txtProportion.Text = "1.5";
            this.txtProportion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnstatisticEX);
            this.groupBox3.Controls.Add(this.btnStatistic);
            this.groupBox3.Controls.Add(this.btnTranslation);
            this.groupBox3.Controls.Add(this.btnExportToExcel);
            this.groupBox3.Controls.Add(this.txtEvaluation);
            this.groupBox3.Controls.Add(this.btnIntegral);
            this.groupBox3.Location = new System.Drawing.Point(12, 684);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(360, 105);
            this.groupBox3.TabIndex = 66;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "评 价";
            // 
            // btnstatisticEX
            // 
            this.btnstatisticEX.Location = new System.Drawing.Point(185, 81);
            this.btnstatisticEX.Name = "btnstatisticEX";
            this.btnstatisticEX.Size = new System.Drawing.Size(95, 24);
            this.btnstatisticEX.TabIndex = 63;
            this.btnstatisticEX.Text = "statisticEX";
            this.btnstatisticEX.UseVisualStyleBackColor = true;
            this.btnstatisticEX.Click += new System.EventHandler(this.btnstatisticEX_Click);
            // 
            // btnStatistic
            // 
            this.btnStatistic.Location = new System.Drawing.Point(185, 55);
            this.btnStatistic.Name = "btnStatistic";
            this.btnStatistic.Size = new System.Drawing.Size(95, 24);
            this.btnStatistic.TabIndex = 62;
            this.btnStatistic.Text = "statistic";
            this.btnStatistic.UseVisualStyleBackColor = true;
            this.btnStatistic.Click += new System.EventHandler(this.btnStatistic_Click);
            // 
            // btnTranslation
            // 
            this.btnTranslation.Location = new System.Drawing.Point(77, 22);
            this.btnTranslation.Name = "btnTranslation";
            this.btnTranslation.Size = new System.Drawing.Size(85, 27);
            this.btnTranslation.TabIndex = 60;
            this.btnTranslation.Text = "Translation";
            this.btnTranslation.UseVisualStyleBackColor = true;
            this.btnTranslation.Click += new System.EventHandler(this.btnTranslation_Click);
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Location = new System.Drawing.Point(185, 22);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(95, 27);
            this.btnExportToExcel.TabIndex = 59;
            this.btnExportToExcel.Text = "导出结果细节";
            this.btnExportToExcel.UseVisualStyleBackColor = true;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // btnIntegral
            // 
            this.btnIntegral.Location = new System.Drawing.Point(6, 22);
            this.btnIntegral.Name = "btnIntegral";
            this.btnIntegral.Size = new System.Drawing.Size(65, 27);
            this.btnIntegral.TabIndex = 57;
            this.btnIntegral.Text = "Integral";
            this.btnIntegral.UseVisualStyleBackColor = true;
            this.btnIntegral.Click += new System.EventHandler(this.btnIntegral_Click);
            // 
            // groupBox2
            // 
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
            this.groupBox2.Location = new System.Drawing.Point(12, 457);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(360, 221);
            this.groupBox2.TabIndex = 65;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "显 示";
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
            // pbScale
            // 
            this.pbScale.Location = new System.Drawing.Point(62, 179);
            this.pbScale.Name = "pbScale";
            this.pbScale.Size = new System.Drawing.Size(162, 29);
            this.pbScale.TabIndex = 71;
            // 
            // btnSaveInterpolation
            // 
            this.btnSaveInterpolation.Location = new System.Drawing.Point(8, 147);
            this.btnSaveInterpolation.Name = "btnSaveInterpolation";
            this.btnSaveInterpolation.Size = new System.Drawing.Size(162, 25);
            this.btnSaveInterpolation.TabIndex = 70;
            this.btnSaveInterpolation.Text = "保存当前内插曲线";
            this.btnSaveInterpolation.UseVisualStyleBackColor = true;
            this.btnSaveInterpolation.Click += new System.EventHandler(this.btnSaveInterpolation_Click);
            // 
            // btnInputedScale
            // 
            this.btnInputedScale.Location = new System.Drawing.Point(6, 116);
            this.btnInputedScale.Name = "btnInputedScale";
            this.btnInputedScale.Size = new System.Drawing.Size(104, 25);
            this.btnInputedScale.TabIndex = 67;
            this.btnInputedScale.Text = "按输入比例";
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
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 185);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(183, 13);
            this.label8.TabIndex = 104;
            this.label8.Text = "Attribute of Known Correspondences:";
            // 
            // txtAttributeOfKnown
            // 
            this.txtAttributeOfKnown.Location = new System.Drawing.Point(227, 178);
            this.txtAttributeOfKnown.Name = "txtAttributeOfKnown";
            this.txtAttributeOfKnown.Size = new System.Drawing.Size(127, 20);
            this.txtAttributeOfKnown.TabIndex = 103;
            this.txtAttributeOfKnown.Text = "5M-30M-Sg";
            this.txtAttributeOfKnown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // FrmOptCor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 801);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "FrmOptCor";
            this.Text = "FrmOptCor";
            this.Load += new System.EventHandler(this.FrmOptCor_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtEvaluation;
        private System.Windows.Forms.TextBox txtProportion;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnTranslation;
        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.Button btnIntegral;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSaveInterpolation;
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
        private System.Windows.Forms.Button btnReduce;
        private System.Windows.Forms.ProgressBar pbScale;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblLengthBound;
        private System.Windows.Forms.TextBox txtMaxBackK;
        private System.Windows.Forms.Button btnStatistic;
        private System.Windows.Forms.Button btnstatisticEX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboSmallerScaleLayer;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.ComboBox cboLargerScaleLayer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMulti;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIncrease;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboMorphingMethod;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboStandardVector;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboEvaluationMethod;
        private System.Windows.Forms.CheckBox chkCoincidentPoints;
        private System.Windows.Forms.ComboBox cboIntMaxBackKforJ;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAttributeOfKnown;
    }
}
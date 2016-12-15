namespace ContinuousGeneralizer.FrmMorphing
{
    partial class FrmRIBS
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
            this.pbScale = new System.Windows.Forms.ProgressBar();
            this.btnRun = new System.Windows.Forms.Button();
            this.txtProportion = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtEvaluation = new System.Windows.Forms.TextBox();
            this.btnTranslation = new System.Windows.Forms.Button();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.btnIntegral = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.cboLargerScaleLayer = new System.Windows.Forms.ComboBox();
            this.btnReduce = new System.Windows.Forms.Button();
            this.cboSmallerScaleLayer = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSaveInterpolation = new System.Windows.Forms.Button();
            this.btnInputedScale = new System.Windows.Forms.Button();
            this.btn050 = new System.Windows.Forms.Button();
            this.btn025 = new System.Windows.Forms.Button();
            this.btn010 = new System.Windows.Forms.Button();
            this.btn020 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btn100 = new System.Windows.Forms.Button();
            this.btn030 = new System.Windows.Forms.Button();
            this.btn075 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn040 = new System.Windows.Forms.Button();
            this.lblLayer = new System.Windows.Forms.Label();
            this.btn000 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn060 = new System.Windows.Forms.Button();
            this.btn080 = new System.Windows.Forms.Button();
            this.btn070 = new System.Windows.Forms.Button();
            this.btn090 = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbScale
            // 
            this.pbScale.Location = new System.Drawing.Point(62, 165);
            this.pbScale.Name = "pbScale";
            this.pbScale.Size = new System.Drawing.Size(162, 27);
            this.pbScale.TabIndex = 82;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(8, 187);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(64, 23);
            this.btnRun.TabIndex = 71;
            this.btnRun.Text = "确  定";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // txtProportion
            // 
            this.txtProportion.Location = new System.Drawing.Point(116, 109);
            this.txtProportion.Name = "txtProportion";
            this.txtProportion.Size = new System.Drawing.Size(108, 21);
            this.txtProportion.TabIndex = 81;
            this.txtProportion.Text = "1.5";
            this.txtProportion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtEvaluation);
            this.groupBox3.Controls.Add(this.btnTranslation);
            this.groupBox3.Controls.Add(this.btnExportToExcel);
            this.groupBox3.Controls.Add(this.btnIntegral);
            this.groupBox3.Location = new System.Drawing.Point(18, 463);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(288, 97);
            this.groupBox3.TabIndex = 72;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "评 价";
            // 
            // txtEvaluation
            // 
            this.txtEvaluation.Location = new System.Drawing.Point(8, 67);
            this.txtEvaluation.Name = "txtEvaluation";
            this.txtEvaluation.ReadOnly = true;
            this.txtEvaluation.Size = new System.Drawing.Size(114, 21);
            this.txtEvaluation.TabIndex = 61;
            // 
            // btnTranslation
            // 
            this.btnTranslation.Location = new System.Drawing.Point(77, 20);
            this.btnTranslation.Name = "btnTranslation";
            this.btnTranslation.Size = new System.Drawing.Size(85, 25);
            this.btnTranslation.TabIndex = 60;
            this.btnTranslation.Text = "Translation";
            this.btnTranslation.UseVisualStyleBackColor = true;
            this.btnTranslation.Click += new System.EventHandler(this.btnTranslation_Click);
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Location = new System.Drawing.Point(185, 67);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(95, 25);
            this.btnExportToExcel.TabIndex = 59;
            this.btnExportToExcel.Text = "导出结果细节";
            this.btnExportToExcel.UseVisualStyleBackColor = true;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // btnIntegral
            // 
            this.btnIntegral.Location = new System.Drawing.Point(6, 20);
            this.btnIntegral.Name = "btnIntegral";
            this.btnIntegral.Size = new System.Drawing.Size(65, 25);
            this.btnIntegral.TabIndex = 57;
            this.btnIntegral.Text = "Integral";
            this.btnIntegral.UseVisualStyleBackColor = true;
            this.btnIntegral.Click += new System.EventHandler(this.btnIntegral_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAdd.Location = new System.Drawing.Point(245, 165);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(35, 27);
            this.btnAdd.TabIndex = 80;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // cboLargerScaleLayer
            // 
            this.cboLargerScaleLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLargerScaleLayer.FormattingEnabled = true;
            this.cboLargerScaleLayer.Location = new System.Drawing.Point(122, 20);
            this.cboLargerScaleLayer.Name = "cboLargerScaleLayer";
            this.cboLargerScaleLayer.Size = new System.Drawing.Size(160, 20);
            this.cboLargerScaleLayer.TabIndex = 78;
            // 
            // btnReduce
            // 
            this.btnReduce.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReduce.Location = new System.Drawing.Point(6, 165);
            this.btnReduce.Name = "btnReduce";
            this.btnReduce.Size = new System.Drawing.Size(35, 27);
            this.btnReduce.TabIndex = 79;
            this.btnReduce.Text = "-";
            this.btnReduce.UseVisualStyleBackColor = true;
            this.btnReduce.Click += new System.EventHandler(this.btnReduce_Click);
            // 
            // cboSmallerScaleLayer
            // 
            this.cboSmallerScaleLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSmallerScaleLayer.FormattingEnabled = true;
            this.cboSmallerScaleLayer.Location = new System.Drawing.Point(122, 46);
            this.cboSmallerScaleLayer.Name = "cboSmallerScaleLayer";
            this.cboSmallerScaleLayer.Size = new System.Drawing.Size(160, 20);
            this.cboSmallerScaleLayer.TabIndex = 80;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 77;
            this.label3.Text = "大比例尺图层：";
            // 
            // btnSaveInterpolation
            // 
            this.btnSaveInterpolation.Location = new System.Drawing.Point(8, 136);
            this.btnSaveInterpolation.Name = "btnSaveInterpolation";
            this.btnSaveInterpolation.Size = new System.Drawing.Size(162, 23);
            this.btnSaveInterpolation.TabIndex = 70;
            this.btnSaveInterpolation.Text = "保存当前内插曲线";
            this.btnSaveInterpolation.UseVisualStyleBackColor = true;
            this.btnSaveInterpolation.Click += new System.EventHandler(this.btnSaveInterpolation_Click);
            // 
            // btnInputedScale
            // 
            this.btnInputedScale.Location = new System.Drawing.Point(6, 107);
            this.btnInputedScale.Name = "btnInputedScale";
            this.btnInputedScale.Size = new System.Drawing.Size(104, 23);
            this.btnInputedScale.TabIndex = 67;
            this.btnInputedScale.Text = "按输入比例";
            this.btnInputedScale.UseVisualStyleBackColor = true;
            this.btnInputedScale.Click += new System.EventHandler(this.btnInputedScale_Click);
            // 
            // btn050
            // 
            this.btn050.Location = new System.Drawing.Point(230, 20);
            this.btn050.Name = "btn050";
            this.btn050.Size = new System.Drawing.Size(50, 23);
            this.btn050.TabIndex = 63;
            this.btn050.Text = "t=0.5";
            this.btn050.UseVisualStyleBackColor = true;
            this.btn050.Click += new System.EventHandler(this.btn050_Click);
            // 
            // btn025
            // 
            this.btn025.Location = new System.Drawing.Point(62, 78);
            this.btn025.Name = "btn025";
            this.btn025.Size = new System.Drawing.Size(50, 23);
            this.btn025.TabIndex = 65;
            this.btn025.Text = "t=0.25";
            this.btn025.UseVisualStyleBackColor = true;
            this.btn025.Click += new System.EventHandler(this.btn025_Click);
            // 
            // btn010
            // 
            this.btn010.Location = new System.Drawing.Point(6, 20);
            this.btn010.Name = "btn010";
            this.btn010.Size = new System.Drawing.Size(50, 23);
            this.btn010.TabIndex = 51;
            this.btn010.Text = "t=0.1";
            this.btn010.UseVisualStyleBackColor = true;
            this.btn010.Click += new System.EventHandler(this.btn010_Click);
            // 
            // btn020
            // 
            this.btn020.Location = new System.Drawing.Point(62, 20);
            this.btn020.Name = "btn020";
            this.btn020.Size = new System.Drawing.Size(50, 23);
            this.btn020.TabIndex = 52;
            this.btn020.Text = "t=0.2";
            this.btn020.UseVisualStyleBackColor = true;
            this.btn020.Click += new System.EventHandler(this.btn020_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 79;
            this.label1.Text = "小比例尺图层：";
            // 
            // btn100
            // 
            this.btn100.Location = new System.Drawing.Point(230, 49);
            this.btn100.Name = "btn100";
            this.btn100.Size = new System.Drawing.Size(50, 23);
            this.btn100.TabIndex = 62;
            this.btn100.Text = "t=1";
            this.btn100.UseVisualStyleBackColor = true;
            this.btn100.Click += new System.EventHandler(this.btn100_Click);
            // 
            // btn030
            // 
            this.btn030.Location = new System.Drawing.Point(118, 20);
            this.btn030.Name = "btn030";
            this.btn030.Size = new System.Drawing.Size(50, 23);
            this.btn030.TabIndex = 53;
            this.btn030.Text = "t=0.3";
            this.btn030.UseVisualStyleBackColor = true;
            this.btn030.Click += new System.EventHandler(this.btn030_Click);
            // 
            // btn075
            // 
            this.btn075.Location = new System.Drawing.Point(118, 78);
            this.btn075.Name = "btn075";
            this.btn075.Size = new System.Drawing.Size(50, 23);
            this.btn075.TabIndex = 61;
            this.btn075.Text = "t=0.75";
            this.btn075.UseVisualStyleBackColor = true;
            this.btn075.Click += new System.EventHandler(this.btn075_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboSmallerScaleLayer);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cboLargerScaleLayer);
            this.groupBox1.Controls.Add(this.btnRun);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 216);
            this.groupBox1.TabIndex = 71;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "参数输入";
            // 
            // btn040
            // 
            this.btn040.Location = new System.Drawing.Point(174, 20);
            this.btn040.Name = "btn040";
            this.btn040.Size = new System.Drawing.Size(50, 23);
            this.btn040.TabIndex = 54;
            this.btn040.Text = "t=0.4";
            this.btn040.UseVisualStyleBackColor = true;
            this.btn040.Click += new System.EventHandler(this.btn040_Click);
            // 
            // lblLayer
            // 
            this.lblLayer.Location = new System.Drawing.Point(6, 6);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(100, 23);
            this.lblLayer.TabIndex = 68;
            // 
            // btn000
            // 
            this.btn000.Location = new System.Drawing.Point(6, 78);
            this.btn000.Name = "btn000";
            this.btn000.Size = new System.Drawing.Size(50, 23);
            this.btn000.TabIndex = 60;
            this.btn000.Text = "t=0.0";
            this.btn000.UseVisualStyleBackColor = true;
            this.btn000.Click += new System.EventHandler(this.btn000_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pbScale);
            this.groupBox2.Controls.Add(this.txtProportion);
            this.groupBox2.Controls.Add(this.btnAdd);
            this.groupBox2.Controls.Add(this.btnReduce);
            this.groupBox2.Controls.Add(this.btnSaveInterpolation);
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
            this.groupBox2.Location = new System.Drawing.Point(12, 234);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(288, 204);
            this.groupBox2.TabIndex = 70;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "显 示";
            // 
            // btn060
            // 
            this.btn060.Location = new System.Drawing.Point(6, 49);
            this.btn060.Name = "btn060";
            this.btn060.Size = new System.Drawing.Size(50, 23);
            this.btn060.TabIndex = 56;
            this.btn060.Text = "t=0.6";
            this.btn060.UseVisualStyleBackColor = true;
            this.btn060.Click += new System.EventHandler(this.btn060_Click);
            // 
            // btn080
            // 
            this.btn080.Location = new System.Drawing.Point(118, 49);
            this.btn080.Name = "btn080";
            this.btn080.Size = new System.Drawing.Size(50, 23);
            this.btn080.TabIndex = 59;
            this.btn080.Text = "t=0.8";
            this.btn080.UseVisualStyleBackColor = true;
            this.btn080.Click += new System.EventHandler(this.btn080_Click);
            // 
            // btn070
            // 
            this.btn070.Location = new System.Drawing.Point(62, 49);
            this.btn070.Name = "btn070";
            this.btn070.Size = new System.Drawing.Size(50, 23);
            this.btn070.TabIndex = 57;
            this.btn070.Text = "t=0.7";
            this.btn070.UseVisualStyleBackColor = true;
            this.btn070.Click += new System.EventHandler(this.btn070_Click);
            // 
            // btn090
            // 
            this.btn090.Location = new System.Drawing.Point(174, 49);
            this.btn090.Name = "btn090";
            this.btn090.Size = new System.Drawing.Size(50, 23);
            this.btn090.TabIndex = 58;
            this.btn090.Text = "t=0.9";
            this.btn090.UseVisualStyleBackColor = true;
            this.btn090.Click += new System.EventHandler(this.btn090_Click);
            // 
            // FrmRIBS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 566);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblLayer);
            this.Controls.Add(this.groupBox2);
            this.Name = "FrmRIBS";
            this.Text = "FrmRIBS";
            this.Load += new System.EventHandler(this.FrmRIBS_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbScale;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox txtProportion;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtEvaluation;
        private System.Windows.Forms.Button btnTranslation;
        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.Button btnIntegral;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox cboLargerScaleLayer;
        private System.Windows.Forms.Button btnReduce;
        private System.Windows.Forms.ComboBox cboSmallerScaleLayer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSaveInterpolation;
        private System.Windows.Forms.Button btnInputedScale;
        private System.Windows.Forms.Button btn050;
        private System.Windows.Forms.Button btn025;
        private System.Windows.Forms.Button btn010;
        private System.Windows.Forms.Button btn020;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn100;
        private System.Windows.Forms.Button btn030;
        private System.Windows.Forms.Button btn075;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn040;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.Button btn000;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn060;
        private System.Windows.Forms.Button btn080;
        private System.Windows.Forms.Button btn070;
        private System.Windows.Forms.Button btn090;
    }
}
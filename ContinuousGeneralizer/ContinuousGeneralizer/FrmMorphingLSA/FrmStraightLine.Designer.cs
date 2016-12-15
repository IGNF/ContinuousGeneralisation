namespace ContinuousGeneralizer.FrmMorphingLSA
{
    partial class FrmStraightLine
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
            this.btnStop = new System.Windows.Forms.Button();
            this.gpbDisplay = new System.Windows.Forms.GroupBox();
            this.btnConvergence = new System.Windows.Forms.Button();
            this.btnSaveLengthandAngles = new System.Windows.Forms.Button();
            this.btnSaveTrajectory = new System.Windows.Forms.Button();
            this.pbScale = new System.Windows.Forms.ProgressBar();
            this.txtProportion = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnReduce = new System.Windows.Forms.Button();
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
            this.btnRun = new System.Windows.Forms.Button();
            this.timerAdd = new System.Windows.Forms.Timer(this.components);
            this.btnReadData = new System.Windows.Forms.Button();
            this.timerReduce = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.btnInputResults = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtInterpolatedNum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIterationNum = new System.Windows.Forms.TextBox();
            this.btnInputReverse = new System.Windows.Forms.Button();
            this.gpbDisplay.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(264, 179);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(63, 29);
            this.btnStop.TabIndex = 84;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // gpbDisplay
            // 
            this.gpbDisplay.Controls.Add(this.btnConvergence);
            this.gpbDisplay.Controls.Add(this.btnSaveLengthandAngles);
            this.gpbDisplay.Controls.Add(this.btnStop);
            this.gpbDisplay.Controls.Add(this.btnSaveTrajectory);
            this.gpbDisplay.Controls.Add(this.pbScale);
            this.gpbDisplay.Controls.Add(this.txtProportion);
            this.gpbDisplay.Controls.Add(this.btnAdd);
            this.gpbDisplay.Controls.Add(this.btnReduce);
            this.gpbDisplay.Controls.Add(this.btnSaveInterpolation);
            this.gpbDisplay.Controls.Add(this.btnInputedScale);
            this.gpbDisplay.Controls.Add(this.btn050);
            this.gpbDisplay.Controls.Add(this.btn025);
            this.gpbDisplay.Controls.Add(this.btn010);
            this.gpbDisplay.Controls.Add(this.btn020);
            this.gpbDisplay.Controls.Add(this.btn100);
            this.gpbDisplay.Controls.Add(this.btn030);
            this.gpbDisplay.Controls.Add(this.btn075);
            this.gpbDisplay.Controls.Add(this.btn040);
            this.gpbDisplay.Controls.Add(this.btn000);
            this.gpbDisplay.Controls.Add(this.btn060);
            this.gpbDisplay.Controls.Add(this.btn080);
            this.gpbDisplay.Controls.Add(this.btn070);
            this.gpbDisplay.Controls.Add(this.btn090);
            this.gpbDisplay.Location = new System.Drawing.Point(12, 167);
            this.gpbDisplay.Name = "gpbDisplay";
            this.gpbDisplay.Size = new System.Drawing.Size(333, 283);
            this.gpbDisplay.TabIndex = 75;
            this.gpbDisplay.TabStop = false;
            this.gpbDisplay.Text = "Display";
            // 
            // btnConvergence
            // 
            this.btnConvergence.Location = new System.Drawing.Point(230, 116);
            this.btnConvergence.Name = "btnConvergence";
            this.btnConvergence.Size = new System.Drawing.Size(85, 25);
            this.btnConvergence.TabIndex = 86;
            this.btnConvergence.Text = "Convergence";
            this.btnConvergence.UseVisualStyleBackColor = true;
            this.btnConvergence.Visible = false;
            this.btnConvergence.Click += new System.EventHandler(this.btnConvergence_Click);
            // 
            // btnSaveLengthandAngles
            // 
            this.btnSaveLengthandAngles.Location = new System.Drawing.Point(165, 214);
            this.btnSaveLengthandAngles.Name = "btnSaveLengthandAngles";
            this.btnSaveLengthandAngles.Size = new System.Drawing.Size(162, 25);
            this.btnSaveLengthandAngles.TabIndex = 85;
            this.btnSaveLengthandAngles.Text = "Save Lenths and Angles";
            this.btnSaveLengthandAngles.UseVisualStyleBackColor = true;
            this.btnSaveLengthandAngles.Click += new System.EventHandler(this.btnSaveLengthandAngles_Click);
            // 
            // btnSaveTrajectory
            // 
            this.btnSaveTrajectory.Location = new System.Drawing.Point(6, 214);
            this.btnSaveTrajectory.Name = "btnSaveTrajectory";
            this.btnSaveTrajectory.Size = new System.Drawing.Size(153, 25);
            this.btnSaveTrajectory.TabIndex = 83;
            this.btnSaveTrajectory.Text = "Save Trajectory";
            this.btnSaveTrajectory.UseVisualStyleBackColor = true;
            this.btnSaveTrajectory.Click += new System.EventHandler(this.btnSaveTrajectory_Click);
            // 
            // pbScale
            // 
            this.pbScale.Location = new System.Drawing.Point(47, 179);
            this.pbScale.Name = "pbScale";
            this.pbScale.Size = new System.Drawing.Size(170, 29);
            this.pbScale.TabIndex = 82;
            // 
            // txtProportion
            // 
            this.txtProportion.Location = new System.Drawing.Point(116, 118);
            this.txtProportion.Name = "txtProportion";
            this.txtProportion.Size = new System.Drawing.Size(108, 20);
            this.txtProportion.TabIndex = 81;
            this.txtProportion.Text = "0.5";
            this.txtProportion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAdd.Location = new System.Drawing.Point(223, 179);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(35, 29);
            this.btnAdd.TabIndex = 80;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnReduce
            // 
            this.btnReduce.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
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
            this.btnSaveInterpolation.Location = new System.Drawing.Point(6, 148);
            this.btnSaveInterpolation.Name = "btnSaveInterpolation";
            this.btnSaveInterpolation.Size = new System.Drawing.Size(162, 25);
            this.btnSaveInterpolation.TabIndex = 70;
            this.btnSaveInterpolation.Text = "Save the interpolated line";
            this.btnSaveInterpolation.UseVisualStyleBackColor = true;
            this.btnSaveInterpolation.Click += new System.EventHandler(this.btnSaveInterpolation_Click);
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
            this.btn050.Location = new System.Drawing.Point(258, 22);
            this.btn050.Name = "btn050";
            this.btn050.Size = new System.Drawing.Size(57, 25);
            this.btn050.TabIndex = 63;
            this.btn050.Text = "t=0.5";
            this.btn050.UseVisualStyleBackColor = true;
            this.btn050.Click += new System.EventHandler(this.btn050_Click);
            // 
            // btn025
            // 
            this.btn025.Location = new System.Drawing.Point(69, 87);
            this.btn025.Name = "btn025";
            this.btn025.Size = new System.Drawing.Size(57, 25);
            this.btn025.TabIndex = 65;
            this.btn025.Text = "t=0.25";
            this.btn025.UseVisualStyleBackColor = true;
            this.btn025.Click += new System.EventHandler(this.btn025_Click);
            // 
            // btn010
            // 
            this.btn010.Location = new System.Drawing.Point(6, 22);
            this.btn010.Name = "btn010";
            this.btn010.Size = new System.Drawing.Size(57, 25);
            this.btn010.TabIndex = 51;
            this.btn010.Text = "t=0.1";
            this.btn010.UseVisualStyleBackColor = true;
            this.btn010.Click += new System.EventHandler(this.btn010_Click);
            // 
            // btn020
            // 
            this.btn020.Location = new System.Drawing.Point(69, 22);
            this.btn020.Name = "btn020";
            this.btn020.Size = new System.Drawing.Size(57, 25);
            this.btn020.TabIndex = 52;
            this.btn020.Text = "t=0.2";
            this.btn020.UseVisualStyleBackColor = true;
            this.btn020.Click += new System.EventHandler(this.btn020_Click);
            // 
            // btn100
            // 
            this.btn100.Location = new System.Drawing.Point(258, 53);
            this.btn100.Name = "btn100";
            this.btn100.Size = new System.Drawing.Size(57, 25);
            this.btn100.TabIndex = 62;
            this.btn100.Text = "t=1";
            this.btn100.UseVisualStyleBackColor = true;
            this.btn100.Click += new System.EventHandler(this.btn100_Click);
            // 
            // btn030
            // 
            this.btn030.Location = new System.Drawing.Point(132, 22);
            this.btn030.Name = "btn030";
            this.btn030.Size = new System.Drawing.Size(57, 25);
            this.btn030.TabIndex = 53;
            this.btn030.Text = "t=0.3";
            this.btn030.UseVisualStyleBackColor = true;
            this.btn030.Click += new System.EventHandler(this.btn030_Click);
            // 
            // btn075
            // 
            this.btn075.Location = new System.Drawing.Point(132, 85);
            this.btn075.Name = "btn075";
            this.btn075.Size = new System.Drawing.Size(57, 25);
            this.btn075.TabIndex = 61;
            this.btn075.Text = "t=0.75";
            this.btn075.UseVisualStyleBackColor = true;
            this.btn075.Click += new System.EventHandler(this.btn075_Click);
            // 
            // btn040
            // 
            this.btn040.Location = new System.Drawing.Point(195, 22);
            this.btn040.Name = "btn040";
            this.btn040.Size = new System.Drawing.Size(57, 25);
            this.btn040.TabIndex = 54;
            this.btn040.Text = "t=0.4";
            this.btn040.UseVisualStyleBackColor = true;
            this.btn040.Click += new System.EventHandler(this.btn040_Click);
            // 
            // btn000
            // 
            this.btn000.Location = new System.Drawing.Point(6, 85);
            this.btn000.Name = "btn000";
            this.btn000.Size = new System.Drawing.Size(57, 25);
            this.btn000.TabIndex = 60;
            this.btn000.Text = "t=0.0";
            this.btn000.UseVisualStyleBackColor = true;
            this.btn000.Click += new System.EventHandler(this.btn000_Click);
            // 
            // btn060
            // 
            this.btn060.Location = new System.Drawing.Point(6, 53);
            this.btn060.Name = "btn060";
            this.btn060.Size = new System.Drawing.Size(57, 25);
            this.btn060.TabIndex = 56;
            this.btn060.Text = "t=0.6";
            this.btn060.UseVisualStyleBackColor = true;
            this.btn060.Click += new System.EventHandler(this.btn060_Click);
            // 
            // btn080
            // 
            this.btn080.Location = new System.Drawing.Point(132, 53);
            this.btn080.Name = "btn080";
            this.btn080.Size = new System.Drawing.Size(57, 25);
            this.btn080.TabIndex = 59;
            this.btn080.Text = "t=0.8";
            this.btn080.UseVisualStyleBackColor = true;
            this.btn080.Click += new System.EventHandler(this.btn080_Click);
            // 
            // btn070
            // 
            this.btn070.Location = new System.Drawing.Point(69, 53);
            this.btn070.Name = "btn070";
            this.btn070.Size = new System.Drawing.Size(57, 25);
            this.btn070.TabIndex = 57;
            this.btn070.Text = "t=0.7";
            this.btn070.UseVisualStyleBackColor = true;
            this.btn070.Click += new System.EventHandler(this.btn070_Click);
            // 
            // btn090
            // 
            this.btn090.Location = new System.Drawing.Point(195, 53);
            this.btn090.Name = "btn090";
            this.btn090.Size = new System.Drawing.Size(57, 25);
            this.btn090.TabIndex = 58;
            this.btn090.Text = "t=0.9";
            this.btn090.UseVisualStyleBackColor = true;
            this.btn090.Click += new System.EventHandler(this.btn090_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 43);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(104, 25);
            this.btnRun.TabIndex = 77;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // timerAdd
            // 
            this.timerAdd.Interval = 50;
            this.timerAdd.Tick += new System.EventHandler(this.timerAdd_Tick);
            // 
            // btnReadData
            // 
            this.btnReadData.Location = new System.Drawing.Point(12, 12);
            this.btnReadData.Name = "btnReadData";
            this.btnReadData.Size = new System.Drawing.Size(104, 25);
            this.btnReadData.TabIndex = 76;
            this.btnReadData.Text = "Input";
            this.btnReadData.UseVisualStyleBackColor = true;
            this.btnReadData.Click += new System.EventHandler(this.btnReadData_Click);
            // 
            // timerReduce
            // 
            this.timerReduce.Interval = 50;
            this.timerReduce.Tick += new System.EventHandler(this.timerReduce_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(232, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 91;
            this.label2.Text = "Layers:";
            this.label2.Visible = false;
            // 
            // cboLayer
            // 
            this.cboLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(167, 139);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(160, 21);
            this.cboLayer.TabIndex = 90;
            this.cboLayer.Visible = false;
            // 
            // btnInputResults
            // 
            this.btnInputResults.Location = new System.Drawing.Point(12, 136);
            this.btnInputResults.Name = "btnInputResults";
            this.btnInputResults.Size = new System.Drawing.Size(104, 25);
            this.btnInputResults.TabIndex = 87;
            this.btnInputResults.Text = "Input Results";
            this.btnInputResults.UseVisualStyleBackColor = true;
            this.btnInputResults.Visible = false;
            this.btnInputResults.Click += new System.EventHandler(this.btnInputResults_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 13);
            this.label1.TabIndex = 89;
            this.label1.Text = "the number of inerpolated features:";
            this.label1.Visible = false;
            // 
            // txtInterpolatedNum
            // 
            this.txtInterpolatedNum.Location = new System.Drawing.Point(204, 95);
            this.txtInterpolatedNum.Name = "txtInterpolatedNum";
            this.txtInterpolatedNum.Size = new System.Drawing.Size(123, 20);
            this.txtInterpolatedNum.TabIndex = 88;
            this.txtInterpolatedNum.Text = "3";
            this.txtInterpolatedNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtInterpolatedNum.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 13);
            this.label3.TabIndex = 93;
            this.label3.Text = "the number of iteration steps";
            // 
            // txtIterationNum
            // 
            this.txtIterationNum.Location = new System.Drawing.Point(204, 73);
            this.txtIterationNum.Name = "txtIterationNum";
            this.txtIterationNum.Size = new System.Drawing.Size(123, 20);
            this.txtIterationNum.TabIndex = 92;
            this.txtIterationNum.Text = "500";
            this.txtIterationNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnInputReverse
            // 
            this.btnInputReverse.Location = new System.Drawing.Point(122, 12);
            this.btnInputReverse.Name = "btnInputReverse";
            this.btnInputReverse.Size = new System.Drawing.Size(104, 25);
            this.btnInputReverse.TabIndex = 94;
            this.btnInputReverse.Text = "Input Reverse";
            this.btnInputReverse.UseVisualStyleBackColor = true;
            this.btnInputReverse.Click += new System.EventHandler(this.btnInputReverse_Click);
            // 
            // FrmStraightLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 462);
            this.Controls.Add(this.btnInputReverse);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtIterationNum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboLayer);
            this.Controls.Add(this.btnInputResults);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtInterpolatedNum);
            this.Controls.Add(this.gpbDisplay);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnReadData);
            this.Name = "FrmStraightLine";
            this.Text = "FrmStraightLine";
            this.Load += new System.EventHandler(this.FrmStraightLine_Load);
            this.gpbDisplay.ResumeLayout(false);
            this.gpbDisplay.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Button btnStop;
        protected System.Windows.Forms.GroupBox gpbDisplay;
        protected System.Windows.Forms.Button btnSaveTrajectory;
        protected System.Windows.Forms.ProgressBar pbScale;
        protected System.Windows.Forms.TextBox txtProportion;
        protected System.Windows.Forms.Button btnAdd;
        protected System.Windows.Forms.Button btnReduce;
        protected System.Windows.Forms.Button btnSaveInterpolation;
        protected System.Windows.Forms.Button btnInputedScale;
        protected System.Windows.Forms.Button btn050;
        protected System.Windows.Forms.Button btn025;
        protected System.Windows.Forms.Button btn010;
        protected System.Windows.Forms.Button btn020;
        protected System.Windows.Forms.Button btn100;
        protected System.Windows.Forms.Button btn030;
        protected System.Windows.Forms.Button btn075;
        protected System.Windows.Forms.Button btn040;
        protected System.Windows.Forms.Button btn000;
        protected System.Windows.Forms.Button btn060;
        protected System.Windows.Forms.Button btn080;
        protected System.Windows.Forms.Button btn070;
        protected System.Windows.Forms.Button btn090;
        protected System.Windows.Forms.Button btnRun;
        protected System.Windows.Forms.Timer timerAdd;
        protected System.Windows.Forms.Button btnReadData;
        protected System.Windows.Forms.Timer timerReduce;
        protected System.Windows.Forms.Button btnSaveLengthandAngles;
        protected System.Windows.Forms.ComboBox cboLayer;
        protected System.Windows.Forms.TextBox txtInterpolatedNum;
        protected System.Windows.Forms.Button btnInputResults;
        protected System.Windows.Forms.Label label2;
        protected System.Windows.Forms.Label label1;
        protected System.Windows.Forms.Label label3;
        protected System.Windows.Forms.TextBox txtIterationNum;
        protected System.Windows.Forms.Button btnConvergence;
        protected System.Windows.Forms.Button btnInputReverse;
    }
}
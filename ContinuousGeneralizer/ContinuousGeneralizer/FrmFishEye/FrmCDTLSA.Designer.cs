namespace ContinuousGeneralizer.FrmFishEye
{
    partial class FrmCDTLSA
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtZ = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtR = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCenterY = new System.Windows.Forms.TextBox();
            this.lblLengthBound = new System.Windows.Forms.Label();
            this.txtCenterX = new System.Windows.Forms.TextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.lblLayer = new System.Windows.Forms.Label();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtZ);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtR);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtCenterY);
            this.groupBox1.Controls.Add(this.lblLengthBound);
            this.groupBox1.Controls.Add(this.txtCenterX);
            this.groupBox1.Controls.Add(this.btnRun);
            this.groupBox1.Controls.Add(this.lblLayer);
            this.groupBox1.Controls.Add(this.cboLayer);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 239);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "参数输入";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 92;
            this.label3.Text = "放大系数 Z：";
            // 
            // txtZ
            // 
            this.txtZ.Location = new System.Drawing.Point(122, 135);
            this.txtZ.Name = "txtZ";
            this.txtZ.Size = new System.Drawing.Size(160, 20);
            this.txtZ.TabIndex = 91;
            this.txtZ.Text = "2";
            this.txtZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 90;
            this.label2.Text = "半径 R：";
            // 
            // txtR
            // 
            this.txtR.Location = new System.Drawing.Point(122, 109);
            this.txtR.Name = "txtR";
            this.txtR.Size = new System.Drawing.Size(160, 20);
            this.txtR.TabIndex = 89;
            this.txtR.Text = "150";
            this.txtR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 88;
            this.label1.Text = "中心坐标 Y：";
            // 
            // txtCenterY
            // 
            this.txtCenterY.Location = new System.Drawing.Point(122, 81);
            this.txtCenterY.Name = "txtCenterY";
            this.txtCenterY.Size = new System.Drawing.Size(160, 20);
            this.txtCenterY.TabIndex = 87;
            this.txtCenterY.Text = "15";
            this.txtCenterY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblLengthBound
            // 
            this.lblLengthBound.AutoSize = true;
            this.lblLengthBound.Location = new System.Drawing.Point(6, 55);
            this.lblLengthBound.Name = "lblLengthBound";
            this.lblLengthBound.Size = new System.Drawing.Size(77, 13);
            this.lblLengthBound.TabIndex = 86;
            this.lblLengthBound.Text = "中心坐标 X：";
            // 
            // txtCenterX
            // 
            this.txtCenterX.Location = new System.Drawing.Point(122, 55);
            this.txtCenterX.Name = "txtCenterX";
            this.txtCenterX.Size = new System.Drawing.Size(160, 20);
            this.txtCenterX.TabIndex = 85;
            this.txtCenterX.Text = "15";
            this.txtCenterX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(9, 208);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(64, 25);
            this.btnRun.TabIndex = 50;
            this.btnRun.Text = "确  定";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(6, 25);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(67, 13);
            this.lblLayer.TabIndex = 18;
            this.lblLayer.Text = "操作图层：";
            // 
            // cboLayer
            // 
            this.cboLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(122, 22);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(160, 21);
            this.cboLayer.TabIndex = 19;
            // 
            // FrmCDTLSA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 453);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmCDTLSA";
            this.Text = "FrmCDTLSA";
            this.Load += new System.EventHandler(this.FrmCDTLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtR;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCenterY;
        private System.Windows.Forms.Label lblLengthBound;
        private System.Windows.Forms.TextBox txtCenterX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtZ;
    }
}
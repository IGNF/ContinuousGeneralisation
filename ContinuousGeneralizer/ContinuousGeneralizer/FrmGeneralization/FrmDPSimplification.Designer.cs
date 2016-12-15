namespace ContinuousGeneralizer.FrmGeneralization
{
    partial class FrmDPSimplification
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
            this.lblLayer = new System.Windows.Forms.Label();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.btnDivideByDP = new System.Windows.Forms.Button();
            this.txtParameter = new System.Windows.Forms.TextBox();
            this.rdoRemainNum = new System.Windows.Forms.RadioButton();
            this.rdoRemainRatio = new System.Windows.Forms.RadioButton();
            this.rdoDistance = new System.Windows.Forms.RadioButton();
            this.btnSimplify = new System.Windows.Forms.Button();
            this.btnDPMorph = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtProportion = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdoDeleteNum = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblLayer);
            this.groupBox1.Controls.Add(this.cboLayer);
            this.groupBox1.Controls.Add(this.btnDivideByDP);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 129);
            this.groupBox1.TabIndex = 62;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(6, 25);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(36, 13);
            this.lblLayer.TabIndex = 18;
            this.lblLayer.Text = "Layer:";
            // 
            // cboLayer
            // 
            this.cboLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(102, 22);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(160, 21);
            this.cboLayer.TabIndex = 19;
            // 
            // btnDivideByDP
            // 
            this.btnDivideByDP.Location = new System.Drawing.Point(184, 49);
            this.btnDivideByDP.Name = "btnDivideByDP";
            this.btnDivideByDP.Size = new System.Drawing.Size(78, 25);
            this.btnDivideByDP.TabIndex = 65;
            this.btnDivideByDP.Text = "DivideByDP";
            this.btnDivideByDP.UseVisualStyleBackColor = true;
            this.btnDivideByDP.Click += new System.EventHandler(this.btnDivideByDP_Click);
            // 
            // txtParameter
            // 
            this.txtParameter.Location = new System.Drawing.Point(124, 42);
            this.txtParameter.Name = "txtParameter";
            this.txtParameter.Size = new System.Drawing.Size(130, 20);
            this.txtParameter.TabIndex = 83;
            this.txtParameter.Text = "510546";
            this.txtParameter.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // rdoRemainNum
            // 
            this.rdoRemainNum.AutoSize = true;
            this.rdoRemainNum.Location = new System.Drawing.Point(171, 19);
            this.rdoRemainNum.Name = "rdoRemainNum";
            this.rdoRemainNum.Size = new System.Drawing.Size(83, 17);
            this.rdoRemainNum.TabIndex = 82;
            this.rdoRemainNum.Text = "RemainNum";
            this.rdoRemainNum.UseVisualStyleBackColor = true;
            // 
            // rdoRemainRatio
            // 
            this.rdoRemainRatio.AutoSize = true;
            this.rdoRemainRatio.Location = new System.Drawing.Point(79, 19);
            this.rdoRemainRatio.Name = "rdoRemainRatio";
            this.rdoRemainRatio.Size = new System.Drawing.Size(86, 17);
            this.rdoRemainRatio.TabIndex = 81;
            this.rdoRemainRatio.Text = "RemainRatio";
            this.rdoRemainRatio.UseVisualStyleBackColor = true;
            // 
            // rdoDistance
            // 
            this.rdoDistance.AutoSize = true;
            this.rdoDistance.Location = new System.Drawing.Point(6, 19);
            this.rdoDistance.Name = "rdoDistance";
            this.rdoDistance.Size = new System.Drawing.Size(67, 17);
            this.rdoDistance.TabIndex = 63;
            this.rdoDistance.Text = "Distance";
            this.rdoDistance.UseVisualStyleBackColor = true;
            // 
            // btnSimplify
            // 
            this.btnSimplify.Location = new System.Drawing.Point(35, 44);
            this.btnSimplify.Name = "btnSimplify";
            this.btnSimplify.Size = new System.Drawing.Size(64, 25);
            this.btnSimplify.TabIndex = 61;
            this.btnSimplify.Text = "Simplify";
            this.btnSimplify.UseVisualStyleBackColor = true;
            this.btnSimplify.Click += new System.EventHandler(this.btnSimplify_Click);
            // 
            // btnDPMorph
            // 
            this.btnDPMorph.Location = new System.Drawing.Point(6, 44);
            this.btnDPMorph.Name = "btnDPMorph";
            this.btnDPMorph.Size = new System.Drawing.Size(64, 25);
            this.btnDPMorph.TabIndex = 63;
            this.btnDPMorph.Text = "DPMorph";
            this.btnDPMorph.UseVisualStyleBackColor = true;
            this.btnDPMorph.Click += new System.EventHandler(this.btnDPMorph_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtProportion);
            this.groupBox2.Controls.Add(this.btnDPMorph);
            this.groupBox2.Location = new System.Drawing.Point(141, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(121, 93);
            this.groupBox2.TabIndex = 64;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "DPMorph";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 66;
            this.label1.Text = "t:";
            // 
            // txtProportion
            // 
            this.txtProportion.Location = new System.Drawing.Point(30, 19);
            this.txtProportion.Name = "txtProportion";
            this.txtProportion.Size = new System.Drawing.Size(64, 20);
            this.txtProportion.TabIndex = 84;
            this.txtProportion.Text = "0.5";
            this.txtProportion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdoDeleteNum);
            this.groupBox3.Controls.Add(this.txtParameter);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.rdoRemainNum);
            this.groupBox3.Controls.Add(this.rdoRemainRatio);
            this.groupBox3.Controls.Add(this.rdoDistance);
            this.groupBox3.Location = new System.Drawing.Point(12, 147);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(268, 189);
            this.groupBox3.TabIndex = 66;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnSimplify);
            this.groupBox4.Location = new System.Drawing.Point(6, 90);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(129, 93);
            this.groupBox4.TabIndex = 65;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Simplify";
            // 
            // rdoDeleteNum
            // 
            this.rdoDeleteNum.AutoSize = true;
            this.rdoDeleteNum.Checked = true;
            this.rdoDeleteNum.Location = new System.Drawing.Point(6, 42);
            this.rdoDeleteNum.Name = "rdoDeleteNum";
            this.rdoDeleteNum.Size = new System.Drawing.Size(78, 17);
            this.rdoDeleteNum.TabIndex = 84;
            this.rdoDeleteNum.TabStop = true;
            this.rdoDeleteNum.Text = "DeleteNum";
            this.rdoDeleteNum.UseVisualStyleBackColor = true;
            // 
            // FrmDPSimplification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 348);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmDPSimplification";
            this.Text = "FrmDPSimplification";
            this.Load += new System.EventHandler(this.FrmDPSimplification_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.Button btnSimplify;
        private System.Windows.Forms.RadioButton rdoDistance;
        private System.Windows.Forms.RadioButton rdoRemainNum;
        private System.Windows.Forms.RadioButton rdoRemainRatio;
        private System.Windows.Forms.TextBox txtParameter;
        private System.Windows.Forms.Button btnDPMorph;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtProportion;
        private System.Windows.Forms.Button btnDivideByDP;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rdoDeleteNum;
    }
}
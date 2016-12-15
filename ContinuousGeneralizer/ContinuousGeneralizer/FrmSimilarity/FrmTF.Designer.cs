namespace ContinuousGeneralizer.FrmSimilarity
{
    partial class FrmTF
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
            this.label1 = new System.Windows.Forms.Label();
            this.cboSmallerScaleLayer = new System.Windows.Forms.ComboBox();
            this.btnReadData = new System.Windows.Forms.Button();
            this.lblLayer = new System.Windows.Forms.Label();
            this.cboLargerScaleLayer = new System.Windows.Forms.ComboBox();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRunData = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 83;
            this.label1.Text = "Smaller-scale layer";
            // 
            // cboSmallerScaleLayer
            // 
            this.cboSmallerScaleLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSmallerScaleLayer.FormattingEnabled = true;
            this.cboSmallerScaleLayer.Location = new System.Drawing.Point(122, 47);
            this.cboSmallerScaleLayer.Name = "cboSmallerScaleLayer";
            this.cboSmallerScaleLayer.Size = new System.Drawing.Size(160, 21);
            this.cboSmallerScaleLayer.TabIndex = 84;
            // 
            // btnReadData
            // 
            this.btnReadData.Location = new System.Drawing.Point(6, 34);
            this.btnReadData.Name = "btnReadData";
            this.btnReadData.Size = new System.Drawing.Size(78, 31);
            this.btnReadData.TabIndex = 0;
            this.btnReadData.Text = "ReadData";
            this.btnReadData.UseVisualStyleBackColor = true;
            this.btnReadData.Click += new System.EventHandler(this.btnReadData_Click);
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(6, 22);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(90, 13);
            this.lblLayer.TabIndex = 81;
            this.lblLayer.Text = "Larger-scale layer";
            // 
            // cboLargerScaleLayer
            // 
            this.cboLargerScaleLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLargerScaleLayer.FormattingEnabled = true;
            this.cboLargerScaleLayer.Location = new System.Drawing.Point(122, 19);
            this.cboLargerScaleLayer.Name = "cboLargerScaleLayer";
            this.cboLargerScaleLayer.Size = new System.Drawing.Size(160, 21);
            this.cboLargerScaleLayer.TabIndex = 82;
            // 
            // btnRunAll
            // 
            this.btnRunAll.Location = new System.Drawing.Point(6, 107);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(64, 25);
            this.btnRunAll.TabIndex = 50;
            this.btnRunAll.Text = "Run";
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnReadData);
            this.groupBox1.Controls.Add(this.btnRunData);
            this.groupBox1.Location = new System.Drawing.Point(12, 166);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 94);
            this.groupBox1.TabIndex = 85;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Corresponding relationship has been built";
            // 
            // btnRunData
            // 
            this.btnRunData.Location = new System.Drawing.Point(90, 34);
            this.btnRunData.Name = "btnRunData";
            this.btnRunData.Size = new System.Drawing.Size(79, 33);
            this.btnRunData.TabIndex = 1;
            this.btnRunData.Text = "RunData";
            this.btnRunData.UseVisualStyleBackColor = true;
            this.btnRunData.Click += new System.EventHandler(this.btnRunData_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.cboSmallerScaleLayer);
            this.groupBox3.Controls.Add(this.lblLayer);
            this.groupBox3.Controls.Add(this.cboLargerScaleLayer);
            this.groupBox3.Controls.Add(this.btnRunAll);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(288, 148);
            this.groupBox3.TabIndex = 87;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Parameter";
            // 
            // FrmTF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 272);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "FrmTF";
            this.Text = "FrmTF";
            this.Load += new System.EventHandler(this.FrmTF_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboSmallerScaleLayer;
        private System.Windows.Forms.Button btnReadData;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.ComboBox cboLargerScaleLayer;
        private System.Windows.Forms.Button btnRunAll;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRunData;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}
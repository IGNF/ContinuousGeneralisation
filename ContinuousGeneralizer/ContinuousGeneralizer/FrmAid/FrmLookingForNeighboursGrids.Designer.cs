namespace ContinuousGeneralizer.FrmAid
{
    partial class FrmLookingForNeighboursGrids
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
            this.btnRun = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtThreshold = new System.Windows.Forms.TextBox();
            this.lblLayer = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.btnRunMulti = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtGridSize = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 251);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(64, 25);
            this.btnRun.TabIndex = 66;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 80;
            this.label3.Text = "Threshold:";
            // 
            // txtThreshold
            // 
            this.txtThreshold.Location = new System.Drawing.Point(132, 90);
            this.txtThreshold.Name = "txtThreshold";
            this.txtThreshold.Size = new System.Drawing.Size(130, 20);
            this.txtThreshold.TabIndex = 79;
            this.txtThreshold.Text = "0.003";
            this.txtThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtGridSize);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtThreshold);
            this.groupBox1.Controls.Add(this.lblLayer);
            this.groupBox1.Controls.Add(this.cboLayer);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 200);
            this.groupBox1.TabIndex = 67;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameter";
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
            // btnRunMulti
            // 
            this.btnRunMulti.Location = new System.Drawing.Point(82, 251);
            this.btnRunMulti.Name = "btnRunMulti";
            this.btnRunMulti.Size = new System.Drawing.Size(64, 25);
            this.btnRunMulti.TabIndex = 68;
            this.btnRunMulti.Text = "Run Multi";
            this.btnRunMulti.UseVisualStyleBackColor = true;
            this.btnRunMulti.Click += new System.EventHandler(this.btnRunMulti_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 82;
            this.label1.Text = "Grid Size:";
            // 
            // txtGridSize
            // 
            this.txtGridSize.Location = new System.Drawing.Point(132, 116);
            this.txtGridSize.Name = "txtGridSize";
            this.txtGridSize.Size = new System.Drawing.Size(130, 20);
            this.txtGridSize.TabIndex = 81;
            this.txtGridSize.Text = "0.003";
            this.txtGridSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // FrmLookingForNeighboursGrids
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 288);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRunMulti);
            this.Name = "FrmLookingForNeighboursGrids";
            this.Text = "FrmLookingForNeighboursGrids";
            this.Load += new System.EventHandler(this.FrmLookingForNeighboursGrids_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtThreshold;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.Button btnRunMulti;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGridSize;
    }
}
namespace ContinuousGeneralizer.FrmAid
{
    partial class FrmEnlargeLayer
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
            this.txtEnlargementFactor = new System.Windows.Forms.TextBox();
            this.lblLayer = new System.Windows.Forms.Label();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtEnlargementFactor);
            this.groupBox1.Controls.Add(this.lblLayer);
            this.groupBox1.Controls.Add(this.cboLayer);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 200);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 80;
            this.label3.Text = "Enlargement Factor:";
            // 
            // txtEnlargementFactor
            // 
            this.txtEnlargementFactor.Location = new System.Drawing.Point(132, 52);
            this.txtEnlargementFactor.Name = "txtEnlargementFactor";
            this.txtEnlargementFactor.Size = new System.Drawing.Size(130, 20);
            this.txtEnlargementFactor.TabIndex = 79;
            this.txtEnlargementFactor.Text = "1000";
            this.txtEnlargementFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 251);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(64, 25);
            this.btnRun.TabIndex = 59;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // FrmEnlargeLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 288);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRun);
            this.Name = "FrmEnlargeLayer";
            this.Text = "FrmEnlargeLayer";
            this.Load += new System.EventHandler(this.FrmEnlargeLayer_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtEnlargementFactor;
    }
}
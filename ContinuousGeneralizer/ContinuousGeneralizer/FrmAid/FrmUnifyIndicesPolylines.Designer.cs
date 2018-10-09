namespace ContinuousGeneralizer.FrmAid
{
    partial class FrmUnifyIndicesPolylines
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
            this.cboBuffer = new System.Windows.Forms.ComboBox();
            this.lblLengthBound = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboSmallerScaleLayer = new System.Windows.Forms.ComboBox();
            this.lblLayer = new System.Windows.Forms.Label();
            this.cboLargerScaleLayer = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOverlapRatio = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 250);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(64, 25);
            this.btnRun.TabIndex = 63;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // cboBuffer
            // 
            this.cboBuffer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBuffer.FormattingEnabled = true;
            this.cboBuffer.Items.AddRange(new object[] {
            "LSMidLength",
            "SSMidLength",
            "dblVerySmall"});
            this.cboBuffer.Location = new System.Drawing.Point(111, 74);
            this.cboBuffer.Name = "cboBuffer";
            this.cboBuffer.Size = new System.Drawing.Size(151, 21);
            this.cboBuffer.TabIndex = 83;
            // 
            // lblLengthBound
            // 
            this.lblLengthBound.AutoSize = true;
            this.lblLengthBound.Location = new System.Drawing.Point(6, 74);
            this.lblLengthBound.Name = "lblLengthBound";
            this.lblLengthBound.Size = new System.Drawing.Size(61, 13);
            this.lblLengthBound.TabIndex = 82;
            this.lblLengthBound.Text = "Buffer Size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 79;
            this.label2.Text = "Less detailed layer:";
            // 
            // cboSmallerScaleLayer
            // 
            this.cboSmallerScaleLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSmallerScaleLayer.FormattingEnabled = true;
            this.cboSmallerScaleLayer.Location = new System.Drawing.Point(111, 44);
            this.cboSmallerScaleLayer.Name = "cboSmallerScaleLayer";
            this.cboSmallerScaleLayer.Size = new System.Drawing.Size(151, 21);
            this.cboSmallerScaleLayer.TabIndex = 80;
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(6, 19);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(99, 13);
            this.lblLayer.TabIndex = 77;
            this.lblLayer.Text = "More detailed layer:";
            // 
            // cboLargerScaleLayer
            // 
            this.cboLargerScaleLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLargerScaleLayer.FormattingEnabled = true;
            this.cboLargerScaleLayer.Location = new System.Drawing.Point(111, 19);
            this.cboLargerScaleLayer.Name = "cboLargerScaleLayer";
            this.cboLargerScaleLayer.Size = new System.Drawing.Size(151, 21);
            this.cboLargerScaleLayer.TabIndex = 78;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtOverlapRatio);
            this.groupBox1.Controls.Add(this.cboBuffer);
            this.groupBox1.Controls.Add(this.lblLengthBound);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboSmallerScaleLayer);
            this.groupBox1.Controls.Add(this.lblLayer);
            this.groupBox1.Controls.Add(this.cboLargerScaleLayer);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 200);
            this.groupBox1.TabIndex = 64;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 113;
            this.label3.Text = "Overlap Ratio:";
            // 
            // txtOverlapRatio
            // 
            this.txtOverlapRatio.Location = new System.Drawing.Point(111, 101);
            this.txtOverlapRatio.Name = "txtOverlapRatio";
            this.txtOverlapRatio.Size = new System.Drawing.Size(151, 20);
            this.txtOverlapRatio.TabIndex = 112;
            this.txtOverlapRatio.Text = "0.5";
            this.txtOverlapRatio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // FrmUnifyIndicesPolylines
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 288);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmUnifyIndicesPolylines";
            this.Text = "FrmUnifyIndicesPolylines";
            this.Load += new System.EventHandler(this.FrmUnifyIndicesPolylines_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.ComboBox cboBuffer;
        private System.Windows.Forms.Label lblLengthBound;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboSmallerScaleLayer;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.ComboBox cboLargerScaleLayer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOverlapRatio;
    }
}
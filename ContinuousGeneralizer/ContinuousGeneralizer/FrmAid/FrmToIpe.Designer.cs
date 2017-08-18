namespace ContinuousGeneralizer.FrmAid
{
    partial class FrmToIpe
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
            this.txtIpeMaxY = new System.Windows.Forms.TextBox();
            this.txtIpeMinY = new System.Windows.Forms.TextBox();
            this.txtIpeMinX = new System.Windows.Forms.TextBox();
            this.txtIpeMaxX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkOverrideWidth = new System.Windows.Forms.CheckBox();
            this.chkGroup = new System.Windows.Forms.CheckBox();
            this.chkSaveIntoSameFile = new System.Windows.Forms.CheckBox();
            this.cboSize = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblLayer = new System.Windows.Forms.Label();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.tltCboSize = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtIpeMaxY
            // 
            this.txtIpeMaxY.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.txtIpeMaxY.Location = new System.Drawing.Point(181, 103);
            this.txtIpeMaxY.Name = "txtIpeMaxY";
            this.txtIpeMaxY.Size = new System.Drawing.Size(90, 20);
            this.txtIpeMaxY.TabIndex = 106;
            this.txtIpeMaxY.Text = "128";
            this.txtIpeMaxY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtIpeMinY
            // 
            this.txtIpeMinY.Location = new System.Drawing.Point(181, 77);
            this.txtIpeMinY.Name = "txtIpeMinY";
            this.txtIpeMinY.Size = new System.Drawing.Size(90, 20);
            this.txtIpeMinY.TabIndex = 105;
            this.txtIpeMinY.Text = "0";
            this.txtIpeMinY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtIpeMinX
            // 
            this.txtIpeMinX.Location = new System.Drawing.Point(45, 77);
            this.txtIpeMinX.Name = "txtIpeMinX";
            this.txtIpeMinX.Size = new System.Drawing.Size(90, 20);
            this.txtIpeMinX.TabIndex = 104;
            this.txtIpeMinX.Text = "0";
            this.txtIpeMinX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtIpeMaxX
            // 
            this.txtIpeMaxX.Location = new System.Drawing.Point(45, 103);
            this.txtIpeMaxX.Name = "txtIpeMaxX";
            this.txtIpeMaxX.Size = new System.Drawing.Size(90, 20);
            this.txtIpeMaxX.TabIndex = 103;
            this.txtIpeMaxX.Text = "128";
            this.txtIpeMaxX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 107;
            this.label2.Text = "Envelope in Ipe:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 108;
            this.label1.Text = "MinX:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(141, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 109;
            this.label3.Text = "MinY:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 110;
            this.label4.Text = "MaxX:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(141, 106);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 111;
            this.label5.Text = "MaxY:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkOverrideWidth);
            this.groupBox1.Controls.Add(this.chkGroup);
            this.groupBox1.Controls.Add(this.chkSaveIntoSameFile);
            this.groupBox1.Controls.Add(this.cboSize);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.lblLayer);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboLayer);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtIpeMaxX);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtIpeMinX);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtIpeMinY);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtIpeMaxY);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(286, 324);
            this.groupBox1.TabIndex = 112;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameter";
            // 
            // chkOverrideWidth
            // 
            this.chkOverrideWidth.AutoSize = true;
            this.chkOverrideWidth.Checked = true;
            this.chkOverrideWidth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOverrideWidth.Location = new System.Drawing.Point(9, 173);
            this.chkOverrideWidth.Name = "chkOverrideWidth";
            this.chkOverrideWidth.Size = new System.Drawing.Size(147, 17);
            this.chkOverrideWidth.TabIndex = 128;
            this.chkOverrideWidth.Text = "Override boundary width?";
            this.chkOverrideWidth.UseVisualStyleBackColor = true;
            // 
            // chkGroup
            // 
            this.chkGroup.AutoSize = true;
            this.chkGroup.Checked = true;
            this.chkGroup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGroup.Location = new System.Drawing.Point(9, 257);
            this.chkGroup.Name = "chkGroup";
            this.chkGroup.Size = new System.Drawing.Size(61, 17);
            this.chkGroup.TabIndex = 127;
            this.chkGroup.Text = "Group?";
            this.chkGroup.UseVisualStyleBackColor = true;
            // 
            // chkSaveIntoSameFile
            // 
            this.chkSaveIntoSameFile.AutoSize = true;
            this.chkSaveIntoSameFile.Checked = true;
            this.chkSaveIntoSameFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveIntoSameFile.Location = new System.Drawing.Point(9, 234);
            this.chkSaveIntoSameFile.Name = "chkSaveIntoSameFile";
            this.chkSaveIntoSameFile.Size = new System.Drawing.Size(139, 17);
            this.chkSaveIntoSameFile.TabIndex = 126;
            this.chkSaveIntoSameFile.Text = "Save into the same file?";
            this.chkSaveIntoSameFile.UseVisualStyleBackColor = true;
            // 
            // cboSize
            // 
            this.cboSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSize.FormattingEnabled = true;
            this.cboSize.Items.AddRange(new object[] {
            "0.01",
            "0.05",
            "0.5",
            "0.8",
            "1.2",
            "2"});
            this.cboSize.Location = new System.Drawing.Point(42, 196);
            this.cboSize.Name = "cboSize";
            this.cboSize.Size = new System.Drawing.Size(160, 21);
            this.cboSize.TabIndex = 125;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 199);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 13);
            this.label10.TabIndex = 124;
            this.label10.Text = "Size:";
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(6, 22);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(96, 13);
            this.lblLayer.TabIndex = 114;
            this.lblLayer.Text = "Envelope of Layer:";
            // 
            // cboLayer
            // 
            this.cboLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(108, 19);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(160, 21);
            this.cboLayer.TabIndex = 115;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 342);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(64, 25);
            this.btnRun.TabIndex = 113;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(116, 346);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(179, 16);
            this.label11.TabIndex = 128;
            this.label11.Text = "Only Export Visuable Layers!";
            // 
            // tltCboSize
            // 
            this.tltCboSize.AutoPopDelay = 20000;
            this.tltCboSize.InitialDelay = 500;
            this.tltCboSize.ReshowDelay = 100;
            // 
            // FrmToIpe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 379);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmToIpe";
            this.Text = "FrmToIpe";
            this.Load += new System.EventHandler(this.FrmToIpe_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtIpeMaxY;
        private System.Windows.Forms.TextBox txtIpeMinY;
        private System.Windows.Forms.TextBox txtIpeMinX;
        private System.Windows.Forms.TextBox txtIpeMaxX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.CheckBox chkSaveIntoSameFile;
        private System.Windows.Forms.ComboBox cboSize;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkGroup;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkOverrideWidth;
        private System.Windows.Forms.ToolTip tltCboSize;
    }
}
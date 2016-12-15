namespace ContinuousGeneralizer.RoadNetwork
{
    partial class FrmRightAngelDPS
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TxtAngelThreshold = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Cancel = new System.Windows.Forms.Button();
            this.Choose = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TxtProportion = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtDistanceThreshold = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(56, 313);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(63, 23);
            this.btnRun.TabIndex = 0;
            this.btnRun.Text = "确定";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "操作图层:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TxtAngelThreshold);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.Cancel);
            this.groupBox1.Controls.Add(this.Choose);
            this.groupBox1.Controls.Add(this.txtPath);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.TxtProportion);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.TxtDistanceThreshold);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboLayer);
            this.groupBox1.Controls.Add(this.btnRun);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 353);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "参数输入";
            // 
            // TxtAngelThreshold
            // 
            this.TxtAngelThreshold.Location = new System.Drawing.Point(125, 139);
            this.TxtAngelThreshold.Name = "TxtAngelThreshold";
            this.TxtAngelThreshold.Size = new System.Drawing.Size(145, 21);
            this.TxtAngelThreshold.TabIndex = 15;
            this.TxtAngelThreshold.Text = "5";
            this.TxtAngelThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "AngelThreshold:";
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(160, 313);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(60, 23);
            this.Cancel.TabIndex = 13;
            this.Cancel.Text = "取  消";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Choose
            // 
            this.Choose.Location = new System.Drawing.Point(213, 203);
            this.Choose.Name = "Choose";
            this.Choose.Size = new System.Drawing.Size(57, 23);
            this.Choose.TabIndex = 12;
            this.Choose.Text = "选  择";
            this.Choose.UseVisualStyleBackColor = true;
            this.Choose.Click += new System.EventHandler(this.Choose_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(24, 252);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(246, 21);
            this.txtPath.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "文件存放路径：";
            // 
            // TxtProportion
            // 
            this.TxtProportion.Location = new System.Drawing.Point(125, 101);
            this.TxtProportion.Name = "TxtProportion";
            this.TxtProportion.Size = new System.Drawing.Size(145, 21);
            this.TxtProportion.TabIndex = 6;
            this.TxtProportion.Text = "0.5";
            this.TxtProportion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Proportion:";
            // 
            // TxtDistanceThreshold
            // 
            this.TxtDistanceThreshold.Location = new System.Drawing.Point(125, 64);
            this.TxtDistanceThreshold.Name = "TxtDistanceThreshold";
            this.TxtDistanceThreshold.Size = new System.Drawing.Size(145, 21);
            this.TxtDistanceThreshold.TabIndex = 4;
            this.TxtDistanceThreshold.Text = "150";
            this.TxtDistanceThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "DistanceThreshold:";
            // 
            // cboLayer
            // 
            this.cboLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(125, 25);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(148, 20);
            this.cboLayer.TabIndex = 2;
            // 
            // FrmRightAngelDpSimplification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 392);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmRightAngelDPS";
            this.Text = "FrmRightAngelDPS";
            this.Load += new System.EventHandler(this.FrmRightAngelDPS_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtDistanceThreshold;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtProportion;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button Choose;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TextBox TxtAngelThreshold;
        private System.Windows.Forms.Label label5;
    }
}
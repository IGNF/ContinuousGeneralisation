namespace ContinuousGeneralizer.FrmAid
{
    partial class FrmBezierDetectPoint
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
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.lblLayer = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblLengthBound = new System.Windows.Forms.Label();
            this.txtError = new System.Windows.Forms.TextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboLayer
            // 
            this.cboLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(102, 20);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(160, 20);
            this.cboLayer.TabIndex = 19;
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(6, 23);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(65, 12);
            this.lblLayer.TabIndex = 18;
            this.lblLayer.Text = "操作图层：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblLengthBound);
            this.groupBox1.Controls.Add(this.txtError);
            this.groupBox1.Controls.Add(this.lblLayer);
            this.groupBox1.Controls.Add(this.cboLayer);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 185);
            this.groupBox1.TabIndex = 58;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "参数输入";
            // 
            // lblLengthBound
            // 
            this.lblLengthBound.AutoSize = true;
            this.lblLengthBound.Location = new System.Drawing.Point(6, 51);
            this.lblLengthBound.Name = "lblLengthBound";
            this.lblLengthBound.Size = new System.Drawing.Size(47, 12);
            this.lblLengthBound.TabIndex = 74;
            this.lblLengthBound.Text = "Error：";
            // 
            // txtError
            // 
            this.txtError.Location = new System.Drawing.Point(102, 48);
            this.txtError.Name = "txtError";
            this.txtError.Size = new System.Drawing.Size(160, 21);
            this.txtError.TabIndex = 73;
            this.txtError.Text = "2";
            this.txtError.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 231);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(64, 23);
            this.btnRun.TabIndex = 57;
            this.btnRun.Text = "确  定";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // FrmBezierDetectPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRun);
            this.Name = "FrmBezierDetectPoint";
            this.Text = "FrmBezierDetectPoint";
            this.Load += new System.EventHandler(this.FrmBezierDetectPoint_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblLengthBound;
        private System.Windows.Forms.TextBox txtError;
    }
}
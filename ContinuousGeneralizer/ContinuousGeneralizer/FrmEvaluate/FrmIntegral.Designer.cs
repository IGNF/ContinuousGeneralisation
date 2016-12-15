namespace ContinuousGeneralizer.FrmEvaluate
{
    partial class FrmIntegral
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIntegral = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtIntegral);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 185);
            this.groupBox1.TabIndex = 56;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "参数输入";
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 231);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(64, 23);
            this.btnRun.TabIndex = 50;
            this.btnRun.Text = "确  定";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Integral:";
            // 
            // txtIntegral
            // 
            this.txtIntegral.Location = new System.Drawing.Point(71, 14);
            this.txtIntegral.Name = "txtIntegral";
            this.txtIntegral.ReadOnly = true;
            this.txtIntegral.Size = new System.Drawing.Size(191, 21);
            this.txtIntegral.TabIndex = 1;
            // 
            // FrmIntegral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmIntegral";
            this.Text = "FrmIntegral";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtIntegral;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRun;

    }
}
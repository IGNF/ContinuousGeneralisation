namespace ContinuousGeneralizer.FrmSimilarity
{
    partial class FrmSimAngle3
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
            this.btnReadData = new System.Windows.Forms.Button();
            this.btnRunData = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblLengthBound = new System.Windows.Forms.Label();
            this.txtRadius = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboLayer = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cboRoMiRo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboMirrorimage = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboRotate = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboSmallerScaleLayer = new System.Windows.Forms.ComboBox();
            this.lblLayer = new System.Windows.Forms.Label();
            this.cboLargerScaleLayer = new System.Windows.Forms.ComboBox();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(90, 110);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 33);
            this.button1.TabIndex = 2;
            this.button1.Text = "RunCircle";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnRunCircle_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnReadData);
            this.groupBox1.Controls.Add(this.btnRunData);
            this.groupBox1.Location = new System.Drawing.Point(12, 352);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 94);
            this.groupBox1.TabIndex = 71;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "已建立对应关系";
            // 
            // lblLengthBound
            // 
            this.lblLengthBound.AutoSize = true;
            this.lblLengthBound.Location = new System.Drawing.Point(6, 65);
            this.lblLengthBound.Name = "lblLengthBound";
            this.lblLengthBound.Size = new System.Drawing.Size(47, 13);
            this.lblLengthBound.TabIndex = 82;
            this.lblLengthBound.Text = "radius：";
            // 
            // txtRadius
            // 
            this.txtRadius.Location = new System.Drawing.Point(155, 62);
            this.txtRadius.Name = "txtRadius";
            this.txtRadius.Size = new System.Drawing.Size(127, 20);
            this.txtRadius.TabIndex = 81;
            this.txtRadius.Text = "50";
            this.txtRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 79;
            this.label2.Text = "非圆图层：";
            // 
            // cboLayer
            // 
            this.cboLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLayer.FormattingEnabled = true;
            this.cboLayer.Location = new System.Drawing.Point(122, 35);
            this.cboLayer.Name = "cboLayer";
            this.cboLayer.Size = new System.Drawing.Size(160, 21);
            this.cboLayer.TabIndex = 80;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtRadius);
            this.groupBox2.Controls.Add(this.lblLengthBound);
            this.groupBox2.Controls.Add(this.cboLayer);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Location = new System.Drawing.Point(12, 452);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(288, 149);
            this.groupBox2.TabIndex = 83;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "其中有对象为圆(以(0,0)为中心)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.cboRoMiRo);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.cboMirrorimage);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.cboRotate);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.cboSmallerScaleLayer);
            this.groupBox3.Controls.Add(this.lblLayer);
            this.groupBox3.Controls.Add(this.cboLargerScaleLayer);
            this.groupBox3.Controls.Add(this.btnRunAll);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(288, 334);
            this.groupBox3.TabIndex = 84;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "参数输入";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 89;
            this.label5.Text = "旋转镜像旋转？";
            // 
            // cboRoMiRo
            // 
            this.cboRoMiRo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRoMiRo.FormattingEnabled = true;
            this.cboRoMiRo.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cboRoMiRo.Location = new System.Drawing.Point(122, 128);
            this.cboRoMiRo.Name = "cboRoMiRo";
            this.cboRoMiRo.Size = new System.Drawing.Size(160, 21);
            this.cboRoMiRo.TabIndex = 90;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 87;
            this.label4.Text = "镜像？";
            // 
            // cboMirrorimage
            // 
            this.cboMirrorimage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMirrorimage.FormattingEnabled = true;
            this.cboMirrorimage.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cboMirrorimage.Location = new System.Drawing.Point(122, 101);
            this.cboMirrorimage.Name = "cboMirrorimage";
            this.cboMirrorimage.Size = new System.Drawing.Size(160, 21);
            this.cboMirrorimage.TabIndex = 88;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 85;
            this.label3.Text = "旋转？";
            // 
            // cboRotate
            // 
            this.cboRotate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRotate.FormattingEnabled = true;
            this.cboRotate.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cboRotate.Location = new System.Drawing.Point(122, 74);
            this.cboRotate.Name = "cboRotate";
            this.cboRotate.Size = new System.Drawing.Size(160, 21);
            this.cboRotate.TabIndex = 86;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 83;
            this.label1.Text = "小比例尺图层：";
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
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.Location = new System.Drawing.Point(6, 22);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(91, 13);
            this.lblLayer.TabIndex = 81;
            this.lblLayer.Text = "大比例尺图层：";
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
            this.btnRunAll.Location = new System.Drawing.Point(6, 201);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(64, 25);
            this.btnRunAll.TabIndex = 50;
            this.btnRunAll.Text = "RunAll";
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.btnRunAll_Click);
            // 
            // FrmSimAngle3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 613);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmSimAngle3";
            this.Text = "FrmSimAngle3";
            this.Load += new System.EventHandler(this.FrmSimAngle3_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnReadData;
        private System.Windows.Forms.Button btnRunData;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblLengthBound;
        private System.Windows.Forms.TextBox txtRadius;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboLayer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboSmallerScaleLayer;
        private System.Windows.Forms.Label lblLayer;
        private System.Windows.Forms.ComboBox cboLargerScaleLayer;
        private System.Windows.Forms.Button btnRunAll;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboMirrorimage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboRotate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboRoMiRo;
    }
}
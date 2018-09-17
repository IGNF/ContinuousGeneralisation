namespace ContinuousGeneralizer.FrmAid
{
    partial class FrmCalGeo_Ipe
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
            this.rtbInput = new System.Windows.Forms.RichTextBox();
            this.btnLength = new System.Windows.Forms.Button();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.txtCentroidX = new System.Windows.Forms.TextBox();
            this.btnCentroid = new System.Windows.Forms.Button();
            this.txtCentroidY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtArea = new System.Windows.Forms.TextBox();
            this.btnArea = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCentroidXY = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // rtbInput
            // 
            this.rtbInput.Location = new System.Drawing.Point(12, 12);
            this.rtbInput.Name = "rtbInput";
            this.rtbInput.Size = new System.Drawing.Size(306, 287);
            this.rtbInput.TabIndex = 0;
            this.rtbInput.Text = "232.132 606.883 m\n229.086 617.152 l\n235.751 625.862 l\n247.101 629.623 l\n261.096 6" +
    "24.733 l\n274.836 626.582 l";
            // 
            // btnLength
            // 
            this.btnLength.Location = new System.Drawing.Point(14, 305);
            this.btnLength.Name = "btnLength";
            this.btnLength.Size = new System.Drawing.Size(87, 27);
            this.btnLength.TabIndex = 1;
            this.btnLength.Text = "Length";
            this.btnLength.UseVisualStyleBackColor = true;
            this.btnLength.Click += new System.EventHandler(this.btnLength_Click);
            // 
            // txtLength
            // 
            this.txtLength.Location = new System.Drawing.Point(187, 309);
            this.txtLength.Name = "txtLength";
            this.txtLength.Size = new System.Drawing.Size(130, 20);
            this.txtLength.TabIndex = 2;
            // 
            // txtCentroidX
            // 
            this.txtCentroidX.Location = new System.Drawing.Point(187, 412);
            this.txtCentroidX.Name = "txtCentroidX";
            this.txtCentroidX.Size = new System.Drawing.Size(130, 20);
            this.txtCentroidX.TabIndex = 4;
            // 
            // btnCentroid
            // 
            this.btnCentroid.Location = new System.Drawing.Point(14, 391);
            this.btnCentroid.Name = "btnCentroid";
            this.btnCentroid.Size = new System.Drawing.Size(87, 27);
            this.btnCentroid.TabIndex = 3;
            this.btnCentroid.Text = "Centroid";
            this.btnCentroid.UseVisualStyleBackColor = true;
            this.btnCentroid.Click += new System.EventHandler(this.btnCentroid_Click);
            // 
            // txtCentroidY
            // 
            this.txtCentroidY.Location = new System.Drawing.Point(188, 438);
            this.txtCentroidY.Name = "txtCentroidY";
            this.txtCentroidY.Size = new System.Drawing.Size(130, 20);
            this.txtCentroidY.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(165, 415);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(165, 441);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Y:";
            // 
            // txtArea
            // 
            this.txtArea.Location = new System.Drawing.Point(187, 342);
            this.txtArea.Name = "txtArea";
            this.txtArea.Size = new System.Drawing.Size(130, 20);
            this.txtArea.TabIndex = 9;
            // 
            // btnArea
            // 
            this.btnArea.Location = new System.Drawing.Point(14, 338);
            this.btnArea.Name = "btnArea";
            this.btnArea.Size = new System.Drawing.Size(87, 27);
            this.btnArea.TabIndex = 8;
            this.btnArea.Text = "Area";
            this.btnArea.UseVisualStyleBackColor = true;
            this.btnArea.Click += new System.EventHandler(this.btnArea_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(107, 389);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "X Y:";
            // 
            // txtCentroidXY
            // 
            this.txtCentroidXY.Location = new System.Drawing.Point(140, 386);
            this.txtCentroidXY.Name = "txtCentroidXY";
            this.txtCentroidXY.Size = new System.Drawing.Size(177, 20);
            this.txtCentroidXY.TabIndex = 10;
            // 
            // FrmCalGeo_Ipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 462);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCentroidXY);
            this.Controls.Add(this.txtArea);
            this.Controls.Add(this.btnArea);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCentroidY);
            this.Controls.Add(this.txtCentroidX);
            this.Controls.Add(this.btnCentroid);
            this.Controls.Add(this.txtLength);
            this.Controls.Add(this.btnLength);
            this.Controls.Add(this.rtbInput);
            this.Name = "FrmCalGeo_Ipe";
            this.Text = "s";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbInput;
        private System.Windows.Forms.Button btnLength;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.TextBox txtCentroidX;
        private System.Windows.Forms.Button btnCentroid;
        private System.Windows.Forms.TextBox txtCentroidY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtArea;
        private System.Windows.Forms.Button btnArea;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCentroidXY;
    }
}